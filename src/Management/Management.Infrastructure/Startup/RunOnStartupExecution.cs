using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.Infrastructure.Startup;
public sealed class RunOnStartupExecution : IRunOnStartupExecution
{
    private readonly ILogger<RunOnStartupExecution> _logger;
    private readonly IEnumerable<IRunOnStartup> _runOnStartups;

    public RunOnStartupExecution(IEnumerable<IRunOnStartup> runOnStartups, ILogger<RunOnStartupExecution> logger)
    {
        _runOnStartups = runOnStartups;
        _logger = logger;
    }

    public async Task RunAll()
    {
        try
        {
            var executableTasks = _runOnStartups.Select(x => x.Run()).ToArray();
            await Task.WhenAll(executableTasks);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to run startup tasks. Message {message}", e.Message);
            throw;
        }
    }
}
