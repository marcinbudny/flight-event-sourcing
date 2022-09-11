using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Copies.MongoDB;
using FlightEventSourcing.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static FlightEventSourcing.AvinorAcl.FlightStatus;

namespace FlightEventSourcing.ReadModel;

// provides a read model that for each flight lists all the flight time updates that were issued over the lifecycle

[Projection("b6f535be-dd33-43cb-ba23-178fbb167fb8")]
[CopyProjectionToMongoDB("flight_time_updates")]
public class AllFlightTimeUpdatesProjection
{
    public string? UniqueId { get; set; }
    public string? FlightId { get; set; }
    public string? ArrivalDeparture { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? ScheduleTime { get; set; }
    
    public string? ThisAirport { get; set; }
    public string? OtherAirport { get; set; }
    public string? Airline { get; set; }

    public List<DateTimeOffset?> TimeUpdates { get; set; } = new();
    
    [KeyFromProperty("UniqueId")]
    public void On(FlightRegistered e, ProjectionContext _)
    {
        UniqueId = e.UniqueId;
        FlightId = e.FlightId;
        ArrivalDeparture = e.ArrivalDeparture;
        ScheduleTime = e.ScheduleTime;
        ThisAirport = e.ThisAirport;
        OtherAirport = e.OtherAirport;
        Airline = e.Airline;
        
        if(e.StatusCode == NewTime)
            TimeUpdates.Add(e.StatusTime);
    }


    [KeyFromProperty("UniqueId")]
    public void On(FlightTimeChanged e, ProjectionContext _)
    {
         TimeUpdates.Add(e.NewTime); // not idempotent! TODO: fix e.g. by recording event time along flight time
    }
}