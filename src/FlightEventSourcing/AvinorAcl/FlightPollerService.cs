namespace FlightEventSourcing.AvinorAcl;

// TODO: consider using something ready-made like Hangfire

public class FlightPollerService : BackgroundService
{
    private readonly ILogger<FlightPollerService> _logger;
    private readonly FlightPoller _poller;
    private readonly TimeSpan _pollingInterval;

    public FlightPollerService(ILogger<FlightPollerService> logger, FlightPoller poller, TimeSpan pollingInterval)
    {
        _logger = logger;
        _poller = poller;
        _pollingInterval = pollingInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastStartedAt = DateTimeOffset.MinValue;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (DateTimeOffset.Now - lastStartedAt > _pollingInterval) 
                {
                    lastStartedAt = DateTimeOffset.Now;
                    await _poller.PollFlightsForAirports(Airports.All, stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // expected - the service is being stopped
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while polling flights");
                
                // TODO: consider what to do when the polling fails repeatedly
                // forcing service shutdown might be an option, in hope that after orchestrator like k8s
                // brings the service back up, it will once again be able to connect to either Avinor API 
                // or Dolittle runtime
            }
        }
    }
}