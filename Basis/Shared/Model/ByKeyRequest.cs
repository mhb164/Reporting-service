namespace Shared.Model;

public sealed class ByKeyRequest
{
    public ByKeyRequest() { /* Method intentionally left empty.*/ }
    public ByKeyRequest(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException($"{nameof(key)} is required!", nameof(key));

        Key = key;
    }

    public string? Key { get; set; }


    public ServiceResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
            return ServiceResult.BadRequest($"{nameof(Key)} is required!");

        return ServiceResult.Success();
    }
}
