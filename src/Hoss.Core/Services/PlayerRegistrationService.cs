﻿// 🃏 The HossGame 🃏
// <copyright file="PlayerRegistrationService.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏
// --------------------------------------------------------------------------------------------------------------------

namespace Hoss.Core.Services;

#region

using Hoss.Core.PlayerAggregate;
using Hoss.SharedKernel.Interfaces;

#endregion

public abstract class PlayerRegistrationService
{
    private readonly IRepository<Profile> repository;
    private readonly IEventStore<Profile> store;

    protected PlayerRegistrationService(IRepository<Profile> repository, IEventStore<Profile> store)
    {
        this.repository = repository;
        this.store = store;
    }

    public async Task RegisterAsync(Profile profile)
    {
        var playerIsRegistered = await this.PlayerIsRegistered();
        if (playerIsRegistered)
        {
            return;
        }

        var playerNameNotIsUnique = await this.UserNameIsNotUniqueAsync();
        if (playerNameNotIsUnique)
        {
#pragma warning disable S3626 // Jump statements should not be redundant
            return;
#pragma warning restore S3626 // Jump statements should not be redundant
        }

        await this.store.PushEventsAsync(profile.Events);
    }

    private async Task<bool> UserNameIsNotUniqueAsync()
    {
        var specification = new PlayerWithNameSpec();
        return await this.repository.AnyAsync(specification);
    }

    private async Task<bool> PlayerIsRegistered()
    {
        var specification = new PlayerWithEmailSpec();
        return await this.repository.AnyAsync(specification);
    }
}