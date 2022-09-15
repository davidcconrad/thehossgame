﻿// ---
// 🃏 The HossGame 🃏
// <copyright file="IEventStore.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏
// ---

namespace TheHossGame.SharedKernel.Interfaces;

/// <summary>
/// Provides methods to retrieve aggregate roots from an eventstore.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
public interface IEventStore<TAggregate>
   where TAggregate : IAggregateRoot
{
   /// <summary>
   /// Gets an aggregate root by its Id.
   /// </summary>
   /// <returns>The aggregate root.</returns>
   /// <typeparam name="TId">The type of the aggregate.</typeparam>
   Task<TAggregate> GetAggregateAsync<TId>()
      where TId : ValueObject;

   /// <summary>
   /// Push events to and aggregate's event stream.
   /// </summary>
   /// <param name="events">The events to push.</param>
   /// <returns>A Task representing the asynchronous operation.</returns>
   Task PushEventsAsync(IEnumerable<DomainEventBase> events);
}