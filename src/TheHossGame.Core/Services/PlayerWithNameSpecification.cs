﻿// 🃏 The HossGame 🃏
// <copyright file="PlayerWithNameSpecification.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏

namespace TheHossGame.Core.Services;

using Ardalis.Specification;
using TheHossGame.Core.PlayerAggregate;

public class PlayerWithNameSpec : Specification<Player>
{
   private readonly Player player;

   public PlayerWithNameSpec(Player player)
   {
      this.player = player;
   }
}
