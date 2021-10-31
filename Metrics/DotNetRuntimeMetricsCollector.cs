using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prometheus.DotNetRuntime;

namespace JobeSharp.Metrics
{
    public class DotNetRuntimeMetricsCollector : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
	        using var collector = DotNetRuntimeStatsBuilder
                .Default()
                .StartCollecting();

	        await Task.Delay(-1, stoppingToken);
        }
    }
}