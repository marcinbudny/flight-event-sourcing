using Dolittle.SDK.Events;

namespace FlightEventSourcing.Domain;

// TODO: these events are "best guess" on how the domain looks like for the flights
// a proper Event Storming session should reveal actual process and allow to improve the model

// TODO: consider introducing event versions starting with v1

[EventType("5331006d-928d-46ed-a864-8fa133d20414")]
public record FlightRegistered(
    string UniqueId,
    string FlightId,
    string FlightZone,
    string ArrivalDeparture,
    DateTimeOffset ScheduleTime,
    string ThisAirport,
    string OtherAirport,
    string Airline,
    string? StatusCode,
    DateTimeOffset? StatusTime,
    string? Gate,
    string? CheckIn,
    string? Belt
);

[EventType("80408fcf-8828-4e46-8ed4-686dc146cd33")]
public record FlightDeparted(
    string UniqueId,
    DateTimeOffset? DepartedTime);

[EventType("cc7a3842-e261-47fe-9cfa-6a9beefa0c24")]
public record FlightArrived(
    string UniqueId,
    DateTimeOffset? ArrivedTime
);

[EventType("42224889-9eef-4cc9-bc7c-8403df7a805d")]
public record FlightTimeChanged(
    string UniqueId,
    DateTimeOffset? NewTime
);

[EventType("6ad25137-96ec-4b88-8571-85bc074bc38f")]
public record FlightInfoUpdated(
    string UniqueId,
    string? Gate,
    string? CheckIn,
    string? Belt,
    DateTimeOffset? InfoTime
);

[EventType("8acbaba0-cf16-440d-accd-92247bca8c5a")]
public record FlightCancelled(
    string UniqueId,
    DateTimeOffset? CancelledTime
);

[EventType("0380884d-88b1-4da3-b307-62ffaec5592f")]
public record FlightStatusRemoved(
    string UniqueId,
    DateTimeOffset? RemovedTime
);
