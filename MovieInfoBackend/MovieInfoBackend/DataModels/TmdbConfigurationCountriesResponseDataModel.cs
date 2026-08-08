using System.Collections.Frozen;

namespace MovieInfoBackend.DataModels;

public record TmdbConfigurationCountriesResponseDataModel
{
    // NOTE: Cannot be created from response directly, since the top level of the response is an unnamed array

    public required TmdbConfigurationCountryDataModel[] Countries { get; init; }

    public override string ToString()
    {
        return $"Countries:\n*****\n{string.Join("\n\n", Countries)}";
    }
    
    public class ConfigurationCountriesDictionary {
        public FrozenDictionary<string, string> iso31661ToEnglishCountryNameDictionary { get; }

        public ConfigurationCountriesDictionary(FrozenDictionary<string, string> dict)
        {
            iso31661ToEnglishCountryNameDictionary = dict;
        }
    }

    public ConfigurationCountriesDictionary? GetConfigurationCountriesDictionary()
    {   
        if (Countries == null || Countries.Length <= 0)
        {
            return null;
        }

        Dictionary<string, string> iso31661ToEnglishCountryNameDictionary = new Dictionary<string, string>();

        foreach (TmdbConfigurationCountryDataModel country in Countries)
        {
            iso31661ToEnglishCountryNameDictionary[country.Iso31661] = country.EnglishName;
        }

        return new ConfigurationCountriesDictionary(iso31661ToEnglishCountryNameDictionary.ToFrozenDictionary());
    }
}