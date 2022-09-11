using System.Collections.Concurrent;

namespace FlightEventSourcing.AvinorAcl;

public delegate Task OnFlightsUpdated(FlightEmbedding[] flight, CancellationToken cancel);

public class FlightPoller
{
    private readonly ILogger<FlightPoller> _logger;
    private readonly IAvinorClient _client;
    private readonly OnFlightsUpdated _onFlightsUpdated;

    public FlightPoller(ILogger<FlightPoller> logger, IAvinorClient client, OnFlightsUpdated onFlightsUpdated)
    {
        _logger = logger;
        _client = client;
        _onFlightsUpdated = onFlightsUpdated;
    }

    // TODO: move this cache to a database (like Redis)
    private static readonly ConcurrentDictionary<string, DateTimeOffset?> AirportLastUpdates = new();

    public async Task PollFlightsForAirports(IEnumerable<string> airportCodes, CancellationToken cancel)
    {
        // there is no need to synchronize this code
        // assume it is only called from the FlightPollerService which is a singleton and runs in a loop

        var tasks = airportCodes.Select(code => PollFlightsForAirport(code, cancel));
        await Task.WhenAll(tasks);
    }

    private async Task PollFlightsForAirport(string airportCode, CancellationToken cancel)
    {
        _logger.LogInformation("Polling flights for airport {AirportCode}", airportCode);
        
        AirportLastUpdates.TryGetValue(airportCode, out var lastUpdatedAt);

        var airportFlights = await _client.GetAllFlightsForAirport(airportCode, lastUpdatedAt, cancel);
        await _onFlightsUpdated(airportFlights.Flights, cancel);

        // update the timestamps only after successfully processing the flights
        AirportLastUpdates[airportCode] = airportFlights.LastUpdatedAt;
        
        _logger.LogInformation("Completed polling flights for airport {AirportCode}", airportCode);
    }
}