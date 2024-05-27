using POEMgr.EmailAgent.ProcessingCores;

namespace POEMgr.EmailAgent.Workers
{
    public class POESendHttpWorker : BackgroundService
    {
        private readonly ILogger<POESendHttpWorker> _logger;
        private readonly POEEmailProcessingCore _core;

        public POESendHttpWorker(ILogger<POESendHttpWorker> logger, POEEmailProcessingCore core)
        {
            this._logger = logger;
            this._core = core;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("{time} Start to send http request.", DateTimeOffset.Now);

                try
                {
                    await _core.SendHttpRequest();
                }
                catch (Exception ex)
                {
                    _logger.LogError("{time} Fail to send http request.", DateTimeOffset.Now);
                    _logger.LogError(ex, "{Message}", ex.Message);
                }

                _logger.LogInformation("{time} Finish sending http request.", DateTimeOffset.Now);


                await Task.Delay(TimeSpan.FromSeconds(500), stoppingToken);
            }
        }
    }
}
