using Finance.CurrencyUpdater.Application.Currency;

namespace Finance.CurrencyUpdater;

public class TimelyCurrencyUpdater(ICurrencyUpdater currencyUpdater, ILogger<TimelyCurrencyUpdater> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogInformation("Currency Updater service has started.");
        while (!token.IsCancellationRequested)
        {
            try
            {
                await currencyUpdater.Update(token);
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddHours(3);
                var delay = nextRun - now;
                logger.LogInformation("Next currency update is scheduled at {NextRun}", nextRun);
                await Task.Delay(delay, token);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Currency Updater is gracefully shutdown.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during currency update.");
                await Task.Delay(TimeSpan.FromMinutes(1), token);
            }
        }
    }
}