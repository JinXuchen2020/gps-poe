using POEMgr.EmailAgent.ProcessingCores;

namespace POEMgr.EmailAgent.Workers
{
    public class POESendTestMailWorker : BackgroundService
    {
        private readonly ILogger<POESendTestMailWorker> _logger;
        private readonly POEEmailProcessingCore _core;

        public POESendTestMailWorker(ILogger<POESendTestMailWorker> logger, POEEmailProcessingCore core)
        {
            this._logger = logger;
            this._core = core;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("{time} Start to send test email.", DateTimeOffset.Now);

                try
                {
                    await _core.SendMailTest();
                }
                catch (Exception ex)
                {
                    _logger.LogError("{time} Fail to send test email.", DateTimeOffset.Now);
                    _logger.LogError(ex, "{Message}", ex.Message);
                }

                _logger.LogInformation("{time} Finish sending test email.", DateTimeOffset.Now);


                await Task.Delay(TimeSpan.FromSeconds(500), stoppingToken);
            }
        }
    }
}