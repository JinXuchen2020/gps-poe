using POEMgr.EmailAgent.ProcessingCores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.EmailAgent.Workers
{
    public class POECheckStatusWorker : BackgroundService
    {
        private readonly ILogger<POECheckStatusWorker> _logger;
        private readonly POEEmailProcessingCore _core;

        public POECheckStatusWorker(ILogger<POECheckStatusWorker> logger, POEEmailProcessingCore core)
        {
            this._logger = logger;
            this._core = core;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("{time} Start to check POE status.", DateTimeOffset.Now);

                try
                {
                    await _core.StartCheck();
                }
                catch (Exception ex)
                {
                    _logger.LogError("{time} Fail to check POE status.", DateTimeOffset.Now);
                    _logger.LogError(ex, "{Message}", ex.Message);
                }

                _logger.LogInformation("{time} Finish checking POE status.", DateTimeOffset.Now);


                await Task.Delay(TimeSpan.FromSeconds(600), stoppingToken);
            }
        }
    }
}
