## Flight data event sourcing

An exercise: get data from Avinor flight data API, produce events, build a read model, expose it via API. 

Uses [Dolittle](https://dolittle.io) runtime.

### Running 

```bash
docker compose up -d
dotnet run --project src/FlightEventSourcing/FlightEventSourcing.csproj
```

### Call the API

```bash
curl http://localhost:5101/flights/recent/time-updates
```