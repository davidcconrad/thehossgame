﻿// 🃏 The HossGame 🃏
// <copyright file="GamePlayer.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏
// --------------------------------------------------------------------------------------------------------------------

namespace Hoss.Core.GameAggregate.PlayerEntity;

#region



#endregion

public abstract class GamePlayer : EntityBase
{
    protected GamePlayer(GameId gameId, PlayerId playerId, Action<DomainEventBase> applier)
        : base(playerId, applier)
    {
        GameId = gameId;
        PlayerId = playerId;
    }

    public bool IsReady { get; protected set; }

    public PlayerId PlayerId { get; protected set; }

    public TeamId TeamId { get; protected set; }

    protected GameId GameId { get; }

    internal abstract void Join(TeamId teamId);

    internal abstract void Ready();
}