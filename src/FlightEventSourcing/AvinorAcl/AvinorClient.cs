using Microsoft.AspNetCore.Http.Extensions;
using static FlightEventSourcing.AvinorAcl.AvinorXmlParser;

namespace FlightEventSourcing.AvinorAcl;

public record AirportFlights(DateTimeOffset LastUpdatedAt, FlightEmbedding[] Flights);

public interface IAvinorClient
{
    Task<AirportFlights> GetAllFlightsForAirport(string airportCode, DateTimeOffset? updatedAfter,
        CancellationToken cancel);
}

public class AvinorClient : IAvinorClient
{
    private readonly HttpClient _client;

    public AvinorClient(HttpClient client) => _client = client;

    public async Task<AirportFlights> GetAllFlightsForAirport(string airportCode, DateTimeOffset? updatedAfter,
        CancellationToken cancel)
    {
        var allFlightsXml = await GetAllFlightsXml();
        return ParseAllFlightsXml(allFlightsXml);

        Task<string> GetAllFlightsXml()
        {
            var qb = new QueryBuilder { { "airport", airportCode } };
            if (updatedAfter != null)
                qb.Add("lastUpdate", updatedAfter.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            return _client.GetStringAsync("XmlFeed.asp" + qb, cancel);
        }
    }
}