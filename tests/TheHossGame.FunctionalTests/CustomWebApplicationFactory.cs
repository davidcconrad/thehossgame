﻿// 🃏 The HossGame 🃏
// <copyright file="CustomWebApplicationFactory.cs" company="Reactive">
// Copyright (c) Reactive. All rights reserved.
// </copyright>
// 🃏 The HossGame 🃏

namespace TheHossGame.FunctionalTests;

using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheHossGame.Infrastructure;
using TheHossGame.Infrastructure.Data;
using TheHossGame.UnitTests;
using TheHossGame.Web;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
  where TStartup : class
{
  protected override IHost CreateHost(IHostBuilder builder)
  {
    Guard.Against.Null(builder);
    var host = builder.Build();
    host.Start();

    // Get service provider.
    var serviceProvider = host.Services;

    // Create a scope to obtain a reference to the database
    // context (AppDbContext).
    using (var scope = serviceProvider.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<AppDbContext>();

      var logger = scopedServices
          .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

      // Ensure the database is created.
      db.Database.EnsureCreated();

      try
      {
        SeedData.PopulateTestData(db);
      }
      catch (Exception ex)
      {
        logger.LogError($"An error occurred seeding the database with test messages. Error: {ex.Message}");
        throw;
      }
    }

    return host;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    Guard.Against.Null(builder);
    builder
        .ConfigureServices(services =>
        {
          // Remove the app's ApplicationDbContext registration.
          var descriptor = services.SingleOrDefault(
          d => d.ServiceType ==
              typeof(DbContextOptions<AppDbContext>));

          if (descriptor != null)
          {
            services.Remove(descriptor);
          }

          // This should be set for each individual test run
          string inMemoryCollectionName = Guid.NewGuid().ToString();

          // Add ApplicationDbContext using an in-memory database for testing.
          services.AddDbContext<AppDbContext>(options =>
      {
        options.UseInMemoryDatabase(inMemoryCollectionName);
      });

          services.AddScoped<IMediator, NoOpMediator>();
        });
  }
}
