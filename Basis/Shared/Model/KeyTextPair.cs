namespace Shared.Model;

public sealed class KeyTextPair
{
    public KeyTextPair() { /* Method intentionally left empty.*/ }

    public KeyTextPair(string? key, string? text)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException($"{nameof(key)} is required!", nameof(key));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException($"{nameof(text)} is required!", nameof(text));

        Key = key;
        Text = text;
    }

    public string? Key { get; set; }
    public string? Text { get; set; }
}