using Dolittle.SDK.Embeddings;
using FlightEventSourcing.Domain;
using static FlightEventSourcing.AvinorAcl.FlightStatus;

namespace FlightEventSourcing.AvinorAcl;

[Embedding("1c30c49a-acc9-4cff-8aa0-0771553658cd")]
public class FlightEmbedding
{
    public string? UniqueId { get; set; }
    public string? FlightId { get; set; }
    public string? FlightZone { get; set; }
    public string? ArrivalDeparture { get; set; }
    public DateTimeOffset? ScheduleTime { get; set; }
    public string? ThisAirport { get; set; }
    public string? OtherAirport { get; set; }
    public string? Airline { get; set; }
    public string? StatusCode { get; set; }
    public DateTimeOffset? StatusTime { get; set; }
    public string? Gate { get; set; }
    public string? CheckIn { get; set; }
    public string? Belt { get; set; }

    public object ResolveUpdateToEvents(FlightEmbedding updated, EmbeddingContext _)
    {
        if (UniqueId != updated.UniqueId)
        {
            return new FlightRegistered(
                updated.UniqueId!,
                updated.FlightId!,
                updated.FlightZone!,
                updated.ArrivalDeparture!,
                updated.ScheduleTime!.Value,
                updated.ThisAirport!,
                updated.OtherAirport!,
                updated.Airline!,
                updated.StatusCode,
                updated.StatusTime,
                updated.Gate,
                updated.CheckIn,
                updated.Belt
            );
        }

        if (updated.StatusCode != StatusCode || updated.StatusTime != StatusTime)
        {
            return updated.StatusCode switch
            {
                Arrived => new FlightArrived(UniqueId!, updated.StatusTime),
                Cancelled => new FlightCancelled(UniqueId!, updated.StatusTime),
                Departed => new FlightDeparted(UniqueId!, updated.StatusTime),
                
                // TODO: what does this status mean exactly? assume gate, belt or checkin change
                NewInfo => new FlightInfoUpdated(UniqueId!,
                    updated.Gate,
                    updated.CheckIn,
                    updated.Belt,
                    updated.StatusTime),
                
                NewTime => new FlightTimeChanged(UniqueId!, updated.StatusTime),
                Empty =>  new FlightStatusRemoved(UniqueId!, updated.StatusTime),
                _ => throw new NotImplementedException() // fail fast
            };
        }

        // gate, checkin and belt can be updated without status change
        if (updated.Belt != Belt || updated.CheckIn != CheckIn || updated.Gate != Gate)
        {
            return new FlightInfoUpdated(UniqueId!,
                updated.Gate,
                updated.CheckIn,
                updated.Belt,
                updated.StatusTime);
        }

        throw new NotImplementedException(); // fail fast
    }
    
    public object ResolveDeletionToEvents(EmbeddingContext _)
    {
        throw new NotImplementedException();
    }

    public void On(FlightRegistered e, EmbeddingProjectContext _)
    {
        UniqueId = e.UniqueId;
        FlightId = e.FlightId;
        FlightZone = e.FlightZone;
        ArrivalDeparture = e.ArrivalDeparture;
        ScheduleTime = e.ScheduleTime;
        ThisAirport = e.ThisAirport;
        OtherAirport = e.OtherAirport;
        Airline = e.Airline;
        StatusCode = e.StatusCode;
        StatusTime = e.StatusTime;
        Gate = e.Gate;
        CheckIn = e.CheckIn;
        Belt = e.Belt;
    }

    public void On(FlightArrived e, EmbeddingProjectContext _)
    {
        StatusCode = Arrived;
        StatusTime = e.ArrivedTime;
    }

    public void On(FlightCancelled e, EmbeddingProjectContext _)
    {
        StatusCode = Cancelled;
        StatusTime = e.CancelledTime;
    }

    public void On(FlightDeparted e, EmbeddingProjectContext _)
    {
        StatusCode = Departed;
        StatusTime = e.DepartedTime;
    }

    public void On(FlightInfoUpdated e, EmbeddingProjectContext _)
    {
        Gate = e.Gate;
        CheckIn = e.CheckIn;
        Belt = e.Belt;
        StatusCode = NewInfo;
        StatusTime = e.InfoTime;
    }

    public void On(FlightTimeChanged e, EmbeddingProjectContext _)
    {
        StatusCode = NewTime;
        StatusTime = e.NewTime;
    }
    
    public void On(FlightStatusRemoved e, EmbeddingProjectContext _)
    {
        StatusCode = Empty;
        StatusTime = e.RemovedTime;
    }
}