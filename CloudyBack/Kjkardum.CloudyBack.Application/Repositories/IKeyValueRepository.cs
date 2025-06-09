namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IKeyValueRepository
{
    public Task<string?> GetValueAsync(string key);
    public Task SetValueAsync(string key, string value);
}
