using MovieInfoBackend.DataModels;
using MovieInfoBackend.ViewModels;
using Xunit.Abstractions;

namespace TestMovieInfoBackend.DataModels;

public class PersonViewModelTests
{
    private string suggestionHttpClientResponse1;
    private string tmdbHttpClientPersonResponse;
    private string tmdbHttpClientPersonMovieCreditsResponse;
    private string tmdbHttpClientPersonTvSeriesCreditsResponse;
    private string tmdbHttpClientPersonImagesResponse;

    public PersonViewModelTests(ITestOutputHelper output)
    {        
        // Arrange

        string testDataFilename1 = "MovieHttpClientResponse1.json";
        string testPersonDataFilename = "TmdbHttpClientPersonResponse.json";
        string testPersonMovieCreditsDataFilename = "TmdbHttpClientPersonMovieCreditsResponse.json";
        string testPersonTvSeriesCreditsDataFilename = "TmdbHttpClientPersonTvSeriesCreditsResponse.json";
        string testPersonImagesDataFilename = "TmdbHttpClientPersonImagesResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testDataFilename1}"))
        {
            suggestionHttpClientResponse1 = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(suggestionHttpClientResponse1))
        {
            throw new ArgumentException($"{testDataFilename1} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonDataFilename}"))
        {
            tmdbHttpClientPersonResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonResponse))
        {
            throw new ArgumentException($"{testPersonDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonMovieCreditsDataFilename}"))
        {
            tmdbHttpClientPersonMovieCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonMovieCreditsResponse))
        {
            throw new ArgumentException($"{testPersonMovieCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonTvSeriesCreditsDataFilename}"))
        {
            tmdbHttpClientPersonTvSeriesCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonTvSeriesCreditsResponse))
        {
            throw new ArgumentException($"{testPersonTvSeriesCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testPersonImagesDataFilename}"))
        {
            tmdbHttpClientPersonImagesResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientPersonImagesResponse))
        {
            throw new ArgumentException($"{testPersonImagesDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetViewModelFromDataModel_ValidDataModel_ReturnsValidViewModel()
    {
        // Arrange (continued)
        
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        SuggestionDataModel? suggestionPersonDataModel = suggestions1?[0];  // Person suggestion = index 0
        Assert.NotNull(suggestionPersonDataModel);
        SuggestionViewModel suggestionPersonViewModel = new(suggestionPersonDataModel);

        TmdbPersonResponseDataModel? personResponse = TmdbHttpClient.GetPersonModelFromResponse(tmdbHttpClientPersonResponse);
        Assert.NotNull(personResponse);

        TmdbPersonMovieCreditsResponseDataModel? personMovieCreditsResponse = TmdbHttpClient.GetPersonMovieCreditsModelFromResponse(tmdbHttpClientPersonMovieCreditsResponse);
        Assert.NotNull(personMovieCreditsResponse);

        TmdbPersonTvSeriesCreditsResponseDataModel? personTvSeriesCreditsResponse = TmdbHttpClient.GetPersonTvSeriesCreditsModelFromResponse(tmdbHttpClientPersonTvSeriesCreditsResponse);
        Assert.NotNull(personTvSeriesCreditsResponse);

        TmdbPersonImagesResponseDataModel? personImagesResponse = TmdbHttpClient.GetPersonImagesModelFromResponse(tmdbHttpClientPersonImagesResponse);
        Assert.NotNull(personImagesResponse);

        // Act

        PersonViewModel personViewModel = new(suggestionPersonViewModel, 
                                              personResponse,
                                              personMovieCreditsResponse,
                                              personTvSeriesCreditsResponse,
                                              personImagesResponse);
        
        // Assert
        
        Assert.NotNull(personViewModel);

        // No fields are significantly transformed by the view model currently, so none are checked here yet
        // Other fields are checked by data model tests or ToString() test below
    }

    [Fact]
    public void PersonViewModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Arrange (continued)
        
        MovieSuggestionsResponseDataModel? actual1 = MovieHttpClient.GetModelFromResponse(suggestionHttpClientResponse1);
        SuggestionDataModel[]? suggestions1 = actual1?.Suggestions;
        Assert.Equal(8, suggestions1?.Length);
        SuggestionDataModel? suggestionPersonDataModel = suggestions1?[0];  // Person suggestion = index 0
        Assert.NotNull(suggestionPersonDataModel);
        SuggestionViewModel suggestionPersonViewModel = new(suggestionPersonDataModel, default(Guid));

        TmdbPersonResponseDataModel? personResponse = TmdbHttpClient.GetPersonModelFromResponse(tmdbHttpClientPersonResponse);
        Assert.NotNull(personResponse);

        TmdbPersonMovieCreditsResponseDataModel? personMovieCreditsResponse = TmdbHttpClient.GetPersonMovieCreditsModelFromResponse(tmdbHttpClientPersonMovieCreditsResponse);
        Assert.NotNull(personMovieCreditsResponse);

        TmdbPersonTvSeriesCreditsResponseDataModel? personTvSeriesCreditsResponse = TmdbHttpClient.GetPersonTvSeriesCreditsModelFromResponse(tmdbHttpClientPersonTvSeriesCreditsResponse);
        Assert.NotNull(personTvSeriesCreditsResponse);

        TmdbPersonImagesResponseDataModel? personImagesResponse = TmdbHttpClient.GetPersonImagesModelFromResponse(tmdbHttpClientPersonImagesResponse);
        Assert.NotNull(personImagesResponse);

        // Act

        PersonViewModel personViewModel = new(suggestionPersonViewModel, 
                                              personResponse,
                                              personMovieCreditsResponse,
                                              personTvSeriesCreditsResponse,
                                              personImagesResponse,
                                              default(Guid));
        
        // Assert
        
        Assert.NotNull(personViewModel);
        
        Assert.Equal(
            "ID: 00000000-0000-0000-0000-000000000000\nImage:\n*****\nID: 00000000-0000-0000-0000-000000000000\nHeight: 2048\nImageURL: https://example.com/example1.jpg\nWidth: 1359\n*****\nImdbId: nm9000000\nName: Example Smith\nImdbRank: 3\nKnownForMovies: Actress, Example Film\nAlsoKnownAs: Example Smithee, Example Jones, Betsy Smith\nBiography: Example Smith (née Jones; born April 14, 1977) is an American actress, producer, and entrepreneur. She was in a lot of stuff but is especially known for Example Film.\nBirthday: 1977-04-14\nDeathday: \nGender: Female\nHomepage: \nTmdbId: 11863\nKnownForDepartment: Acting\nPlaceOfBirth: New York City, New York, USA\nProfilePath: /xKe52w4tpv61ohz9iz75wNdzcwZ.jpg\nMovieCastCredits:\n*****\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /yz0puYEAPgfBus2GtKU3LJpkSge.jpg\nTmdbId: 10093\nTitle: The Return\nOriginalTitle: The Return\nPopularity: 1.0656\nPosterPath: /ywBeNqMJKBowfwrv274blsVXCB5.jpg\nReleaseDate: 2006-11-10\nIsVideo: False\nCharacter: Joanna Mills\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /oaT5yMG61XetddVmymGrDVeoCg4.jpg\nTmdbId: 4723\nTitle: Southland Tales\nOriginalTitle: Southland Tales\nPopularity: 2.1026\nPosterPath: /7dbIDQ80z4bxiDlAvxRwc5TI44C.jpg\nReleaseDate: 2007-11-14\nIsVideo: False\nCharacter: Krysta Kapowski / Krysta Now\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /zBW4LZyYi0Moqz82BTkEQzR7h0z.jpg\nTmdbId: 5393\nTitle: Happily N'Ever After\nOriginalTitle: Happily N'Ever After\nPopularity: 1.6834\nPosterPath: /gOOlHRhdEoJbPNE1jpocNawjnc5.jpg\nReleaseDate: 2007-01-05\nIsVideo: False\nCharacter: Ella (voice)\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /gdQk5g4TC95957Xs4xGbcOBnG56.jpg\nTmdbId: 11551\nTitle: Small Soldiers\nOriginalTitle: Small Soldiers\nPopularity: 4.4363\nPosterPath: /2nuUjSzHsoYlRvTPmLo7m7gCQry.jpg\nReleaseDate: 1998-07-10\nIsVideo: False\nCharacter: Gwendy Doll (voice)\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /yap1xGljc2mED8Upk4NYy3f1r7F.jpg\nTmdbId: 27451\nTitle: Harvard Man\nOriginalTitle: Harvard Man\nPopularity: 1.6121\nPosterPath: /vxZG1UXh8gA3nYmrPNUWCHGGBFn.jpg\nReleaseDate: 2001-08-01\nIsVideo: False\nCharacter: Cindy Bandolini\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /bh1YOzQ5HiJZ0yZFQIdYhFhznBr.jpg\nTmdbId: 25968\nTitle: Veronika Decides to Die\nOriginalTitle: Veronika Decides to Die\nPopularity: 1.1189\nPosterPath: /oUo9CNSKz4ELnxAh4l3BPcXcxKq.jpg\nReleaseDate: 2009-05-16\nIsVideo: False\nCharacter: Veronika\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /5QDoHPqcKkCZ1WGAQpDUDm2dVhI.jpg\nTmdbId: 33788\nTitle: Possession\nOriginalTitle: Possession\nPopularity: 0.665\nPosterPath: /8L0ZVI4OAm6nagDx5QM0ufUsqG8.jpg\nReleaseDate: 2009-07-16\nIsVideo: False\nCharacter: Jessica\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /8DNw1DOVNse4CyRpPkQbfPvtcgm.jpg\nTmdbId: 13525\nTitle: Suburban Girl\nOriginalTitle: Suburban Girl\nPopularity: 1.6932\nPosterPath: /fkXyvlMzlU9yszj6Y2TyjbxLUkj.jpg\nReleaseDate: 2007-04-27\nIsVideo: False\nCharacter: Brett Eisenberg\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /meU637lfjE8tpxj0dxXRIkj0G8B.jpg\nTmdbId: 251260\nTitle: Over the Brooklyn Bridge\nOriginalTitle: Over the Brooklyn Bridge\nPopularity: 0.5686\nPosterPath: /aYO5Dxvf1L5zT0f17sfE5YMuZ49.jpg\nReleaseDate: 1984-03-02\nIsVideo: False\nCharacter: Phil's Daughter (uncredited)\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /650GtWNvw78YKebPTAgY3xxZWQs.jpg\nTmdbId: 13641\nTitle: The Air I Breathe\nOriginalTitle: The Air I Breathe\nPopularity: 1.1034\nPosterPath: /hx89b5sinoXmkMNyK1PUgwjWQX4.jpg\nReleaseDate: 2007-02-07\nIsVideo: False\nCharacter: Sorrow\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: \nTmdbId: 982890\nTitle: USIDent TV: Surveilling the Southland\nOriginalTitle: USIDent TV: Surveilling the Southland\nPopularity: 0.3555\nPosterPath: /iwPwjEeJJe0MjCt0sK693yozsNI.jpg\nReleaseDate: 2008-03-18\nIsVideo: False\nCharacter: Self\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /qIDYvZBLYLuWtKnSgq1okHU3Nt8.jpg\nTmdbId: 16172\nTitle: Simply Irresistible\nOriginalTitle: Simply Irresistible\nPopularity: 1.0316\nPosterPath: /zFsqqPQUz69wPaLBqQeiGbGiW7J.jpg\nReleaseDate: 1999-02-05\nIsVideo: False\nCharacter: Amanda Shelton\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /rkkg0lZyOVbggSLmiHyrTt6E9MW.jpg\nTmdbId: 79738\nTitle: Beverly Hills Family Robinson\nOriginalTitle: Beverly Hills Family Robinson\nPopularity: 0.3404\nPosterPath: /erZZMrIc8uMSXieoauWk7xL7K8c.jpg\nReleaseDate: 1997-01-25\nIsVideo: False\nCharacter: Jane Robinson\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /n7abxswaQqO2XUh5KcDZOYpAtnW.jpg\nTmdbId: 219182\nTitle: High Stakes\nOriginalTitle: High Stakes\nPopularity: 0.468\nPosterPath: /cF0uZcuW5lKZxXCqGwlzswnH9CN.jpg\nReleaseDate: 1989-10-06\nIsVideo: False\nCharacter: Karen Rose\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /yWRvAfZmbzk61REYod4WQACDhRj.jpg\nTmdbId: 762968\nTitle: Do Revenge\nOriginalTitle: Do Revenge\nPopularity: 2.8879\nPosterPath: /akIjKJDHcVN4bzifcEarKVPNpoa.jpg\nReleaseDate: 2022-09-14\nIsVideo: False\nCharacter: The Headmistress\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: \nTmdbId: 627512\nTitle: An Invasion of Privacy\nOriginalTitle: An Invasion of Privacy\nPopularity: 0.4134\nPosterPath: /rouZqKstu3oznJg735mKe2qLc6a.jpg\nReleaseDate: 1983-01-12\nIsVideo: False\nCharacter: Jennifer Bianchi\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /nF3At92CEaTHhPactTHgPRpMpnh.jpg\nTmdbId: 61487\nTitle: Mayor of the Sunset Strip\nOriginalTitle: Mayor of the Sunset Strip\nPopularity: 0.6447\nPosterPath: /1vRVeVhozMozvQSubTlcmQlHKK0.jpg\nReleaseDate: 2003-06-17\nIsVideo: False\nCharacter: Self\n\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /i8KndMn7MBdglOKCyzKUMoiXNEU.jpg\nTmdbId: 1541560\nTitle: Stop! That! Train!\nOriginalTitle: Stop! That! Train!\nPopularity: 5.3617\nPosterPath: /w90dGS6D2lVO4aO5rdQ8QECrUGY.jpg\nReleaseDate: 2026-06-12\nIsVideo: False\nCharacter: Famous Actress\n*****\nMovieCrewCredits:\n*****\nID: 00000000-0000-0000-0000-000000000000\nStillPath: /xLfBGBUnEkhwzn1lMRwZNxfregM.jpg\nTmdbId: 8080\nTitle: Suspect Zero\nOriginalTitle: Suspect Zero\nPopularity: 1.7198\nPosterPath: /sIIahTE5sfOJtieytf252kB9RcS.jpg\nReleaseDate: 2004-08-27\nIsVideo: False\nDepartment: Production\nJob: Executive Producer\n*****\nTvSeriesCastCredits:\n*****\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /lBmlLro9ZfY815ZXE5NKhYNxPRQ.jpg\nTmdbId: 95\nOriginalName: Example TV Series\nPopularity: 48.0462\nPosterPath: /y7fVZkyheCEQHDUEHwNmYENGfT2.jpg\nFirstAirDate: 1997-03-10\nName: Example TV Series\nCharacter: Example Character 3\nEpisodeCount: 144\nFirstCreditAirDate: 1997-03-10\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /fqAaKOgE0nWDBYgKy7DrQ5vcp7Q.jpg\nTmdbId: 996\nOriginalName: Spenser: For Hire\nPopularity: 6.9948\nPosterPath: /8n4O7Reyk5es5iyzQk7zfNvuMJ9.jpg\nFirstAirDate: 1985-09-20\nName: Spenser: For Hire\nCharacter: Emily\nEpisodeCount: 1\nFirstCreditAirDate: 1988-03-12\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /y3tN4mPq1L3PNCeOPilc5YncGus.jpg\nTmdbId: 6025\nOriginalName: Swans Crossing\nPopularity: 2.7598\nPosterPath: /fPPfiDtBYvvv69DyxRFugYiemyz.jpg\nFirstAirDate: 1992-06-29\nName: Swans Crossing\nCharacter: Sydney Orion Rutledge\nEpisodeCount: 65\nFirstCreditAirDate: 1992-06-29\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /3DYcyg9hOdoCg4zKNYBcCIN3Q5w.jpg\nTmdbId: 19255\nOriginalName: A Woman Named Jackie\nPopularity: 1.9152\nPosterPath: /uNaBmDyWbM3GBpXiXhsYZFEvUyq.jpg\nFirstAirDate: 1991-10-13\nName: A Woman Named Jackie\nCharacter: Teenage Jacqueline Bouvier\nEpisodeCount: 3\nFirstCreditAirDate: 1991-10-13\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: \nTmdbId: 22073\nOriginalName: Good Day Live\nPopularity: 17.2233\nPosterPath: \nFirstAirDate: 2001-09-17\nName: Good Day Live\nCharacter: Self\nEpisodeCount: 1\nFirstCreditAirDate: 2004-10-22\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /3EunXRxGrBDbu9FsXoD3UDJ0xsF.jpg\nTmdbId: 35341\nOriginalName: Ringer\nPopularity: 5.1195\nPosterPath: /t27TSKDVmGwrH8NQjyQuPvKSmo5.jpg\nFirstAirDate: 2011-09-13\nName: Ringer\nCharacter: Bridget Kelly / Siobhan Martin\nEpisodeCount: 22\nFirstCreditAirDate: 2011-09-13\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /7phlGHRupo38EnuwmkAHdNUqov3.jpg\nTmdbId: 58932\nOriginalName: The Crazy Ones\nPopularity: 4.0941\nPosterPath: /s2e7hTrdmNUaJDf0yDP5b4AHvrD.jpg\nFirstAirDate: 2013-09-26\nName: The Crazy Ones\nCharacter: Sydney Roberts\nEpisodeCount: 22\nFirstCreditAirDate: 2013-09-26\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /5UNcEz7Lc3OGilpPn1Tb9cd1xsc.jpg\nTmdbId: 1220\nOriginalName: The Graham Norton Show\nPopularity: 76.1198\nPosterPath: /vrbqaBXB8AALynQzpWz6JdCPEJS.jpg\nFirstAirDate: 2007-02-22\nName: The Graham Norton Show\nCharacter: Self\nEpisodeCount: 1\nFirstCreditAirDate: 2023-01-27\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /jvoRuN9yzMFaMRv8FuZKiKczE4E.jpg\nTmdbId: 134865\nOriginalName: Wolf Pack\nPopularity: 4.0874\nPosterPath: /rbCANmS1ogweUkIBghP03EHtdHB.jpg\nFirstAirDate: 2023-01-26\nName: Wolf Pack\nCharacter: Kristin Ramsey\nEpisodeCount: 8\nFirstCreditAirDate: 2023-01-26\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /lfgjDGLdVgyCSx4XUiclckeQf5R.jpg\nTmdbId: 3511\nOriginalName: God, the Devil and Bob\nPopularity: 2.2705\nPosterPath: /uSpUzhklRGKcBgeuDdAxsOuizCs.jpg\nFirstAirDate: 2000-03-09\nName: God, the Devil and Bob\nCharacter: That Actress on That Show (voice)\nEpisodeCount: 1\nFirstCreditAirDate: 2011-02-12\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /oN5YwipvAYAUXrif4cv1CQxW1t8.jpg\nTmdbId: 65574\nOriginalName: Those Who Can't\nPopularity: 1.2706\nPosterPath: /w0sO57eQ0VUsZKPoE7JQnVu9sUR.jpg\nFirstAirDate: 2016-02-11\nName: Those Who Can't\nCharacter: Gwen Stephanie\nEpisodeCount: 1\nFirstCreditAirDate: 2016-02-25\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: \nTmdbId: 80515\nOriginalName: The Reichen Show\nPopularity: 11.272\nPosterPath: /x9fDBKqmmHHUvcaal1Tzw6iqTW.jpg\nFirstAirDate: 2005-11-11\nName: The Reichen Show\nCharacter: Self\nEpisodeCount: 1\nFirstCreditAirDate: 2006-03-10\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /3tyLSzhTZRUJD1P28Zi2akckZez.jpg\nTmdbId: 208208\nOriginalName: Breaking Bear\nPopularity: 0.551\nPosterPath: /f5wjCX7c0uM0igS4wUoRF5NiiYm.jpg\nFirstAirDate: \nName: Breaking Bear\nCharacter: Blair (voice)\nEpisodeCount: 0\nFirstCreditAirDate: \n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /jG7JUDNSGesCynjeFClyrvDZw25.jpg\nTmdbId: 4268\nOriginalName: Crossbow\nPopularity: 4.1133\nPosterPath: /bdcX9YuakI4LJrEianMDTMQnCms.jpg\nFirstAirDate: 1987-08-30\nName: Crossbow\nCharacter: Sara Guidotti\nEpisodeCount: 1\nFirstCreditAirDate: 1988-11-19\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /uy56v1uid203h1ETexdaNARyid7.jpg\nTmdbId: 40302\nOriginalName: The Jonathan Ross Show\nPopularity: 14.2112\nPosterPath: /zglUOb3r2C867Rk6qhnmAn2kOSq.jpg\nFirstAirDate: 2011-09-03\nName: The Jonathan Ross Show\nCharacter: Self - Guest\nEpisodeCount: 1\nFirstCreditAirDate: 2026-03-21\n*****\nTvSeriesCrewCredits:\n*****\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /3EunXRxGrBDbu9FsXoD3UDJ0xsF.jpg\nTmdbId: 35341\nOriginalName: Ringer\nPopularity: 5.1195\nPosterPath: /t27TSKDVmGwrH8NQjyQuPvKSmo5.jpg\nFirstAirDate: 2011-09-13\nName: Ringer\nDepartment: Production\nEpisodeCount: 22\nFirstCreditAirDate: 2011-09-13\nJob: Co-Executive Producer\n\nID: 00000000-0000-0000-0000-000000000000\nBackdropPath: /jvoRuN9yzMFaMRv8FuZKiKczE4E.jpg\nTmdbId: 134865\nOriginalName: Wolf Pack\nPopularity: 4.0874\nPosterPath: /rbCANmS1ogweUkIBghP03EHtdHB.jpg\nFirstAirDate: 2023-01-26\nName: Wolf Pack\nDepartment: Production\nEpisodeCount: 3\nFirstCreditAirDate: 2023-01-26\nJob: Executive Producer\n*****\nProfileImages:\n*****\nID: 00000000-0000-0000-0000-000000000000\nHeight: 2813\nFilePath: /xKe52w4tpv61ohz9iz75wNdzcwZ.jpg\nWidth: 1875\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 900\nFilePath: /lQD0YNIfpoFlUnn5ThTNFXvp6tH.jpg\nWidth: 600\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 1500\nFilePath: /sCpCv2ldnrAQN9NVQOE6PYxG8TN.jpg\nWidth: 1000\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 1170\nFilePath: /mr74lbU9MLcYV4aQDsfHp7rnwS4.jpg\nWidth: 780\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 961\nFilePath: /wuctIW1mhtBjSBYFXPTKpKpxpZC.jpg\nWidth: 647\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 1500\nFilePath: /grlhyolOpwZAnpeFvuTC3SQAUij.jpg\nWidth: 1000\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 600\nFilePath: /ysKT3TAytCoVeMxD113b2UuCFL4.jpg\nWidth: 400\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 464\nFilePath: /a0KfVH8u7YonRWdST9cWzZxLRc4.jpg\nWidth: 309\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 1440\nFilePath: /eMS0yGFXrTOubvBWAMRIiHuh9FJ.jpg\nWidth: 960\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 484\nFilePath: /f8tlLjqe5g4gvQJq6ebW9ACbAW6.jpg\nWidth: 323\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 535\nFilePath: /bT75yilHEUCeQNVj6iXXjQgqOUw.jpg\nWidth: 357\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 827\nFilePath: /eiB4P1JaSq4PLh134XgMhDPObTL.jpg\nWidth: 552\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 471\nFilePath: /2p0CEzdnHXSs0g6yTdIqJoztfBT.jpg\nWidth: 314\n\nID: 00000000-0000-0000-0000-000000000000\nHeight: 1200\nFilePath: /8WlHVw7Hn5MubRk53dFkDJHQg6c.jpg\nWidth: 800"
                , personViewModel.ToString());
    }
}
