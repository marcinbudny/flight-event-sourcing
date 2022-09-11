using Dolittle.SDK;
using FlightEventSourcing.AvinorAcl;
using FlightEventSourcing.ReadModel;
using MongoDB.Driver;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// TODO: central logging, tracing, metrics
// TODO: health checks
// TODO: put Avinor poller and flight embedding in separate service to ensure API can be deployed as multiple replicas (poller should be a single replica)
// TODO: put the projection in separate service to ensure API can be deployed as multiple replicas (projection should be a single replica)
// TODO: auth

builder.Services
    .AddHttpClient<IAvinorClient, AvinorClient>(client =>
        client.BaseAddress = new Uri(builder.Configuration["Avinor:BaseUrl"]))
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)) // TODO: improve
    );

builder.Services.AddDolittle();

builder.Services
    .AddSingleton<FlightEmbeddingUpdater>()
    .AddSingleton<FlightPoller>()
    .AddSingleton<OnFlightsUpdated>(ctx => ctx.GetRequiredService<FlightEmbeddingUpdater>().Update)
    .AddHostedService(ctx =>
        new FlightPollerService(
            ctx.GetRequiredService<ILogger<FlightPollerService>>(),
            ctx.GetRequiredService<FlightPoller>(),
            builder.Configuration.GetValue<TimeSpan>("Avinor:PollingInterval"))
    );

var mongoClient = new MongoClient(MongoUrl.Create(builder.Configuration["Mongo:ConnectionString"]));
var database = mongoClient.GetDatabase(builder.Configuration["Mongo:Database"]);

builder.Services.AddSingleton(database);

var app = builder.Build();

app.UseFlightsQueryApi();

app.Run();