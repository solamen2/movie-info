namespace MovieInfoBackend.DataModels;

// The whole oscars.csv file, parsed by OscarsHttpClient.GetModelFromResponse
public record OscarsResponseDataModel
{
    public required List<OscarsRowDataModel> Rows { get; init; }

    public override string ToString()
    {
        return $"Rows: {Rows.Count}\n*****\n{string.Join("\n\n", Rows)}";
    }
}
