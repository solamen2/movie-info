using System.Collections.Frozen;

namespace MovieInfoBackend.DataModels;

public record TmdbConfigurationLanguagesResponseDataModel
{
    // NOTE: Cannot be created from response directly, since the top level of the response is an unnamed array

    public required TmdbConfigurationLanguageDataModel[] Languages { get; init; }

    public override string ToString()
    {
        return $"Languages:\n*****\n{string.Join("\n\n", Languages)}";
    }

    public class ConfigurationLanguagesDictionary {
        public FrozenDictionary<string, string> iso6391ToEnglishLanguageNameDictionary { get; }

        public ConfigurationLanguagesDictionary(FrozenDictionary<string, string> dict)
        {
            iso6391ToEnglishLanguageNameDictionary = dict;
        }
    }

    public ConfigurationLanguagesDictionary? GetConfigurationLanguagesDictionary()
    {   
        if (Languages == null || Languages.Length <= 0)
        {
            return null;
        }

        Dictionary<string, string> iso6391ToEnglishLanguageNameDictionary = new Dictionary<string, string>();

        foreach (TmdbConfigurationLanguageDataModel language in Languages)
        {
            iso6391ToEnglishLanguageNameDictionary[language.Iso6391] = language.EnglishName;
        }

        return new ConfigurationLanguagesDictionary(iso6391ToEnglishLanguageNameDictionary.ToFrozenDictionary());
    }
}