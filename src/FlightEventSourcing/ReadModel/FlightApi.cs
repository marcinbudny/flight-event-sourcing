using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FlightEventSourcing.ReadModel;

public static class FlightQueryApiExtensions
{
    // TODO: add swagger
    // TODO: add proper error handling and mapping to messages for the client
    // TODO: if Dolittle's concept of tenancy is used, then these results need to be filtered by tenant
    public static IEndpointConventionBuilder UseFlightsQueryApi(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/flights/recent/time-updates", 
            async ([FromServices] IMongoDatabase db, CancellationToken requestCancel) =>
        {
            var coll = db.GetCollection<AllFlightTimeUpdatesProjection>("flight_time_updates");

            var recent = DateTimeOffset.UtcNow.AddHours(-2); // let's assume this is what "recent" means

            // get the flights directly from mongo copy of the projection
            // there may be a way to query with filtering via Dolittle directly, but it's not documented
            // TODO: proper index needed in Mongo
            var flights = await coll
                .Find(f => f.ScheduleTime > recent)
                .Sort(Builders<AllFlightTimeUpdatesProjection>.Sort
                    .Ascending(f => f.ScheduleTime))
                .ToListAsync(requestCancel);

            // TODO: the internal read model format should be first mapped to external, versioned API contract before returning data
            return flights;
        });
    }
}