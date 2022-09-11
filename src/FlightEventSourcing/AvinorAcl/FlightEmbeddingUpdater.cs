using Dolittle.SDK;
using Dolittle.SDK.Tenancy;

namespace FlightEventSourcing.AvinorAcl;

public class FlightEmbeddingUpdater
{
    private readonly ILogger<FlightEmbeddingUpdater> _logger;
    private readonly IDolittleClient _client;

    public FlightEmbeddingUpdater(ILogger<FlightEmbeddingUpdater> logger, IDolittleClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Update(FlightEmbedding[] flights, CancellationToken cancel)
    {
        _logger.LogInformation("Updating {EmbeddingCount} flight embeddings", flights.Length);
        
        var embeddings = _client.Embeddings.ForTenant(TenantId.Development);  // TODO: tenant should be retrieved from context

        var tasks = flights
            .Select(f => embeddings.Update(f.UniqueId!, f, cancel));

        await Task.WhenAll(tasks);

        _logger.LogInformation("Completed updating {EmbeddingCount} flight embeddings", flights.Length);
    }
}