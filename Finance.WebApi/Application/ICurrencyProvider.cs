namespace Finance.WebApi.Services;

public interface ICurrencyProvider
{
    public Task<IEnumerable<Currency>> GetAll(CancellationToken cancellationToken);
}