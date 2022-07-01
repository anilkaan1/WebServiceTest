namespace TCPIPListenerWorker
{
    public class TCPIPListenerBGS : BackgroundService
    {
        private readonly ILogger<TCPIPListenerBGS> _logger;

        public TCPIPListenerBGS(ILogger<TCPIPListenerBGS> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}