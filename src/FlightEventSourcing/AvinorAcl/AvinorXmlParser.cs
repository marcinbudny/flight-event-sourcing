using System.Xml.Linq;

namespace FlightEventSourcing.AvinorAcl;

public static class AvinorXmlParser
{
    public static AirportFlights ParseAllFlightsXml(string xml)
    {
        // another way to parse the XML document would be to use XmlSerializer and classes generated from XSD
        // however that way the parsing process is less flexible and requires extra mapping from XSD generated
        // code into the embedding
        var doc = XDocument.Load(new StringReader(xml));

        // TODO: add XML validation (e.g. validate against schema) and report issues
        
        var thisAirportCode = doc.Root!.Attribute("name")!.Value; 
        
        var flightsElem = doc.Root!.Element("flights")!;
        var lastUpdatedAt = DateTimeOffset.Parse(flightsElem.Attribute("lastUpdate")!.Value);

        var flights = flightsElem
            .Elements("flight")
            .Select(ParseFlightXml)
            .ToArray();

        return new AirportFlights(lastUpdatedAt, flights);

        FlightEmbedding ParseFlightXml(XElement e)
        {
            string? statusCode = null;
            DateTimeOffset? statusTime = null;
            var statusElem = e.Element("status");
            if (statusElem != null)
            {
                statusCode = statusElem.Attribute("code")!.Value;
                if (statusElem.Attribute("time") != null)
                    statusTime = DateTimeOffset.Parse(statusElem.Attribute("time")!.Value);
            }

            return new FlightEmbedding
            {
                UniqueId = e.Attribute("uniqueID")!.Value,
                FlightId = e.Element("flight_id")!.Value,
                FlightZone = e.Element("dom_int")!.Value,
                ArrivalDeparture = e.Element("arr_dep")!.Value,
                ScheduleTime = DateTimeOffset.Parse(e.Element("schedule_time")!.Value),
                ThisAirport = thisAirportCode,
                OtherAirport = e.Element("airport")!.Value,
                Airline = e.Element("airline")!.Value,
                StatusCode = statusCode,
                StatusTime = statusTime,
                Gate = e.Element("gate")?.Value,
                CheckIn = e.Element("check_in")?.Value,
                Belt = e.Element("belt")?.Value
            };
        }
    }
}