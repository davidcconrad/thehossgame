﻿// 🃏 The HossGame 🃏
// <copyright file="GuardExtensions.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏

namespace TheHossGame.SharedKernel;

using Ardalis.GuardClauses;

/// <summary>
/// Custom common guard clauses.
/// </summary>
public static class GuardExtensions
{
   /// <summary>
   /// Guards input from invalid length.
   /// </summary>
   /// <param name="guardClause">The guard static class.</param>
   /// <param name="input">The imput to validate.</param>
   /// <param name="parameterName">The parameter name.</param>
   /// <param name="minLength">The minimun lenght of the input.</param>
   /// <param name="maxLength">The max lenght of th input.</param>
   /// <returns>Whether the input has a valis length.</returns>
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed for extension method")]
   public static bool InvalidLength(this IGuardClause guardClause, string input, string parameterName, int minLength, int maxLength)
   {
      if (input?.Length > minLength && input?.Length <= maxLength)
      {
         return true;
      }

      throw new ArgumentException($"{nameof(parameterName)} \"{input}\" Length should be longer than {minLength} and shorter than {maxLength}", parameterName);
   }
}