﻿// 🃏 The HossGame 🃏
// <copyright file="AGame.Behaviour.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏
// --------------------------------------------------------------------------------------------------------------------

namespace Hoss.Core.GameAggregate;

#region

using Hoss.Core.GameAggregate.PlayerEntity;
using Hoss.Core.GameAggregate.RoundEntity;
using Hoss.Core.GameAggregate.RoundEntity.BidEntity;
using Hoss.Core.GameAggregate.RoundEntity.DeckValueObjects;
using Hoss.Core.Interfaces;
using static Game.TeamId;
using static Hoss.Core.GameAggregate.RoundEntity.RoundEvents;

#endregion

/// <summary>
///    Behaviour side.
/// </summary>
public sealed partial class AGame
{
    private readonly IShufflingService shufflingService;

    public static AGame CreateForPlayer(PlayerId playerId, IShufflingService shufflingService)
    {
        AGame game = new(shufflingService);
        game.CreateNewGame(playerId);
        game.JoinPlayerToTeam(playerId, Team1);

        return game;
    }

    public void JoinPlayerToTeam(PlayerId playerId, TeamId teamId)
    {
        this.FindPlayer(playerId).Join(teamId);

        if (this.TeamsAreComplete())
        {
            this.Apply(new GameEvents.TeamsFormedEvent(this.Id));
        }
    }

    public void TeamPlayerReady(PlayerId playerId)
    {
        this.FindPlayer(playerId).Ready();

        if (this.PlayersNotReady())
        {
            return;
        }

        var shuffledDeck = ADeck.ShuffleNew(this.shufflingService);
        var round = Round.StartNew(this.Id, this.GetTeamPlayers(), shuffledDeck, this.Apply);

        this.Apply(new GameEvents.GameStartedEvent(this.Id, round.Id, round.RoundPlayers, round.Deals, round.Bids));
    }

    public void Bid(PlayerId playerId, BidValue value)
    {
        // TO DO: Hoss without help
        this.CurrentRoundBase.Bid(playerId, value);
    }

    public void RequestHoss(PlayerId playerId, Card card)
    {
        this.CurrentRoundBase.RequestHoss(playerId, card);
    }

    public void GiveHossCard(PlayerId playerId, Card card)
    {
        this.CurrentRoundBase.GiveHossCard(playerId, card);
    }

    public void SelectTrump(PlayerId currentPlayerId, Suit suit)
    {
        this.CurrentRoundBase.SelectTrump(currentPlayerId, suit);
    }

    public void PlayCard(PlayerId playerId, Card card)
    {
        this.CurrentRoundBase.PlayCard(playerId, card);
    }

    public void Finish()
    {
        this.Apply(new GameEvents.GameFinishedEvent(this.Id));
    }

    protected override void EnsureValidState()
    {
#pragma warning disable CS8524
        var valid = this.Stage switch
#pragma warning restore CS8524
        {
            GameState.Created => this.TeamValid(Team1) && this.TeamValid(Team2),
            GameState.TeamsFormed => this.TeamComplete(Team1) && this.TeamComplete(Team2),
            GameState.Started => this.FindGamePlayers().All(t => t.IsReady),
            GameState.Finished => this.Stage == GameState.Finished,
        };

        if (!valid)
        {
            throw new InvalidEntityStateException();
        }
    }

    protected override void When(DomainEventBase @event)
    {
        (@event switch
        {
            GameEvents.PlayerJoinedEvent e => (Action) (() => this.HandleJoin(e)),
            GameEvents.NewGameCreatedEvent => HandleGameCreated,
            GameEvents.TeamsFormedEvent => this.HandleTeamsFormedEvent,
            GameEvents.GameStartedEvent e => () => this.HandleGameStartedEvent(e),
            GameEvents.GameFinishedEvent => () => this.HandleGameFinishedEvent(),
            RoundPlayedEvent e => () => this.HandleRoundPlayedEvent(e),
            _ => () => { },
        }).Invoke();
    }

    private GameState HandleGameFinishedEvent()
    {
        return this.Stage = GameState.Finished;
    }

    private static void HandleGameCreated()
    {
    }

    private void CreateNewGame(PlayerId playerId)
    {
        this.Apply(new GameEvents.NewGameCreatedEvent(this.Id, playerId));
    }

    private bool TeamsAreComplete()
    {
        return this.TeamComplete(Team1) && this.TeamComplete(Team2);
    }

    private bool PlayersNotReady()
    {
        return !this.FindGamePlayers().All(p => p.IsReady) || !this.TeamComplete(Team1) || !this.TeamComplete(Team2);
    }

    private void HandleGameStartedEvent(GameEvents.GameStartedEvent @event)
    {
        var round = Round.FromStream(@event, this.Apply);
        this.rounds.Add(round);

        this.Stage = GameState.Started;
    }

    private void HandleRoundPlayedEvent(RoundPlayedEvent e)
    {
        this.Score = this.Score.AddRoundScore(e.RoundScore);
        if (this.Score >= MaxScore)
        {
            this.Apply(new GameEvents.GameFinishedEvent(this.Id));
            return;
        }

        this.rounds.Add(Round.StartNew(this.Id, new Queue<RoundPlayer>(this.GetTeamPlayers()),
            ADeck.ShuffleNew(this.shufflingService), this.Apply, this.rounds.Count));
    }

    private IEnumerable<RoundPlayer> GetTeamPlayers()
    {
        return this.FindGamePlayers().Select(g => new RoundPlayer(g.Id, g.TeamId));
    }

    private void HandleTeamsFormedEvent()
    {
        this.Stage = GameState.TeamsFormed;
    }

    private void HandleJoin(GameEvents.PlayerJoinedEvent e)
    {
        var teamPlayer = AGamePlayer.FromStream(e, this.Apply);
        this.gamePlayers.Add(teamPlayer);
    }

    private bool TeamValid(TeamId team1)
    {
        return this.FindGamePlayers(team1).Count <= 2;
    }

    private bool TeamComplete(TeamId team1)
    {
        return this.FindGamePlayers(team1).Count == 2;
    }
}