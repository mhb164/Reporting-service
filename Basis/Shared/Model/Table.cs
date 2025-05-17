namespace Shared.Model;

public sealed class Table
{
    public Table() { /* Method intentionally left empty.*/ }

    public Table(string? name, string? title, IEnumerable<string>? columns, params List<string>[] rows)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} is required!", nameof(name));

        Name = name;
        Title = string.IsNullOrWhiteSpace(title) ? Name : title;
        Columns = columns?.ToList() ?? new List<string>();
        Rows = rows.ToList();
    }

    public string? Name { get; set; }
    public string? Title { get; set; }
    public List<string>? Columns { get; set; }
    public List<List<string>>? Rows { get; set; }

    public Table AddRow(IEnumerable<string> row)
    {
        if (Rows is null) 
            Rows = new List<List<string>>();

        Rows.Add(row.ToList());
        return this;
    }
}
