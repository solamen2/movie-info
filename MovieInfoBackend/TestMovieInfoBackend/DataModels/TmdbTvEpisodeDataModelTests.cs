using MovieInfoBackend.DataModels;
using Xunit.Abstractions;

public class TmdbTvEpisodeDataModelTests
{
    private string tmdbHttpClientTvEpisodeCreditsResponse;
    private string tmdbHttpClientTvEpisodeResponse;

    public TmdbTvEpisodeDataModelTests(ITestOutputHelper output)
    {
        // Arrange

        string testTvEpisodeCreditsDataFilename = "TmdbHttpClientTvEpisodeCreditsResponse.json";
        string testTvEpisodeDataFilename = "TmdbHttpClientTvEpisodeResponse.json";

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeCreditsDataFilename}"))
        {
            tmdbHttpClientTvEpisodeCreditsResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeCreditsResponse))
        {
            throw new ArgumentException($"{testTvEpisodeCreditsDataFilename} is not valid test data.");
        }

        using (StreamReader sr = File.OpenText($"../../../TestData/{testTvEpisodeDataFilename}"))
        {
            tmdbHttpClientTvEpisodeResponse = sr.ReadToEnd();
        }
        if (String.IsNullOrWhiteSpace(tmdbHttpClientTvEpisodeResponse))
        {
            throw new ArgumentException($"{testTvEpisodeDataFilename} is not valid test data.");
        }
    }

    [Fact]
    public void GetModelFromResponse_ValidResponse_ReturnsValidModels()
    {
        // Act
        
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse = TmdbHttpClient.GetTvEpisodeModelFromResponse(tmdbHttpClientTvEpisodeResponse);
        
        // Assert

        // TV episode credits
        Assert.NotNull(tvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeCreditsResponse);
        Assert.NotEmpty(tvEpisodeCreditsResponse.Cast);
        TmdbTvEpisodeCreditsCastDataModel firstCreditsCast = tvEpisodeCreditsResponse.Cast[0];
        Assert.True(firstCreditsCast.Gender >= 0, "TV episode credits cast Gender must not be a negative number");
        Assert.True(firstCreditsCast.Id > 0, "TV episode credits cast Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCast.KnownForDepartment), "TV episode credits cast KnownForDepartment must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCast.Name), "TV episode credits cast Name must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCast.OriginalName), "TV episode credits cast OriginalName must not be empty");
        Assert.True(firstCreditsCast.Popularity >= 0.0, "TV episode credits cast Popularity must not be a negative decimal number");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCast.Character), "TV episode credits cast Character must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCast.CreditId), "TV episode credits cast CreditId must not be empty");
        Assert.True(firstCreditsCast.Order >= 0, "TV episode credits cast Order must not be a negative number");
        Assert.NotEmpty(tvEpisodeCreditsResponse.Crew);
        TmdbTvEpisodeCreditsCrewDataModel firstCreditsCrew = tvEpisodeCreditsResponse.Crew[0];
        Assert.True(firstCreditsCrew.Gender >= 0, "TV episode credits crew Gender must not be a negative number");
        Assert.True(firstCreditsCrew.Id > 0, "TV episode credits crew Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCrew.KnownForDepartment), "TV episode credits crew KnownForDepartment must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCrew.Name), "TV episode credits crew Name must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCrew.OriginalName), "TV episode credits crew OriginalName must not be empty");
        Assert.True(firstCreditsCrew.Popularity >= 0.0, "TV episode credits crew Popularity must not be a negative decimal number");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCrew.CreditId), "TV episode credits crew CreditId must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCrew.Department), "TV episode credits crew Department must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCreditsCrew.Job), "TV episode credits crew Job must not be empty");
        if (tvEpisodeCreditsResponse.GuestStars.Length > 0)
        {
            TmdbTvEpisodeCreditsGuestStarDataModel firstCreditsGuestStar = tvEpisodeCreditsResponse.GuestStars[0];
            Assert.False(String.IsNullOrWhiteSpace(firstCreditsGuestStar.Character), "TV episode credits guest star Character must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(firstCreditsGuestStar.CreditId), "TV episode credits guest star CreditId must not be empty");
            Assert.True(firstCreditsGuestStar.Order >= 0, "TV episode credits guest star Order must not be a negative number");
            Assert.True(firstCreditsGuestStar.Gender >= 0, "TV episode credits guest star Gender must not be a negative number");
            Assert.True(firstCreditsGuestStar.Id > 0, "TV episode credits guest star Id must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(firstCreditsGuestStar.KnownForDepartment), "TV episode credits guest star KnownForDepartment must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(firstCreditsGuestStar.Name), "TV episode credits guest star Name must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(firstCreditsGuestStar.OriginalName), "TV episode credits guest star OriginalName must not be empty");
            Assert.True(firstCreditsGuestStar.Popularity >= 0.0, "TV episode credits guest star Popularity must not be a negative decimal number");
        }
        Assert.True(tvEpisodeCreditsResponse.Id > 0, "TV episode credits Id must be a positive number");

        // TV episode

        Assert.NotNull(tvEpisodeResponse);
        Assert.NotEmpty(tvEpisodeResponse.Crew);
        TmdbTvEpisodeCrewDataModel firstCrew = tvEpisodeResponse.Crew[0];
        Assert.False(String.IsNullOrWhiteSpace(firstCrew.Department), "TV episode crew Department must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCrew.Job), "TV episode crew Job must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCrew.CreditId), "TV episode crew CreditId must not be empty");
        Assert.True(firstCrew.Gender >= 0, "TV episode crew Gender must not be a negative number");
        Assert.True(firstCrew.Id > 0, "TV episode crew Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(firstCrew.KnownForDepartment), "TV episode crew KnownForDepartment must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCrew.Name), "TV episode crew Name must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(firstCrew.OriginalName), "TV episode crew OriginalName must not be empty");
        Assert.True(firstCrew.Popularity >= 0.0, "TV episode crew Popularity must not be a negative decimal number");
        Assert.True(tvEpisodeResponse.EpisodeNumber > 0, "TV episode EpisodeNumber must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeResponse.EpisodeType), "TV episode EpisodeType must not be empty");
        if (tvEpisodeCreditsResponse.GuestStars.Length > 0)
        {
            TmdbTvEpisodeGuestStarDataModel firstGuestStar = tvEpisodeResponse.GuestStars[0];
            Assert.False(String.IsNullOrWhiteSpace(firstGuestStar.Character), "TV episode guest star Character must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(firstGuestStar.CreditId), "TV episode guest star CreditId must not be empty");
            Assert.True(firstGuestStar.Order >= 0, "TV episode guest star Order must not be a negative number");
            Assert.True(firstGuestStar.Gender >= 0, "TV episode guest star Gender must not be a negative number");
            Assert.True(firstGuestStar.Id > 0, "TV episode guest star Id must be a positive number");
            Assert.False(String.IsNullOrWhiteSpace(firstGuestStar.KnownForDepartment), "TV episode guest star KnownForDepartment must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(firstGuestStar.Name), "TV episode guest star Name must not be empty");
            Assert.False(String.IsNullOrWhiteSpace(firstGuestStar.OriginalName), "TV episode guest star OriginalName must not be empty");
            Assert.True(firstGuestStar.Popularity >= 0.0, "TV episode guest star Popularity must not be a negative decimal number");
        }
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeResponse.Name), "TV episode Name must not be empty");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeResponse.Overview), "TV episode Overview must not be empty");
        Assert.True(tvEpisodeResponse.Id > 0, "TV episode Id must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeResponse.ProductionCode), "TV episode ProductionCode must not be empty");
        Assert.True(tvEpisodeResponse.Runtime > 0, "TV episode Runtime must be a positive number");
        Assert.True(tvEpisodeResponse.SeasonNumber > 0, "TV episode SeasonNumber must be a positive number");
        Assert.False(String.IsNullOrWhiteSpace(tvEpisodeResponse.StillPath), "TV episode StillPath must not be empty");
        Assert.True(tvEpisodeResponse.VoteAverage >= 0.0, "TV episode VoteAverage must not be a negative decimal number");
        Assert.True(tvEpisodeResponse.VoteCount >= 0, "TV episode VoteCount must not be a negative number");
    }

    [Fact]
    public void TmdbTvEpisodeDataModel_ValidModelToString_ReturnsCorrectValue()
    {
        // Act
        
        TmdbTvEpisodeCreditsResponseDataModel? tvEpisodeCreditsResponse = TmdbHttpClient.GetTvEpisodeCreditsModelFromResponse(tmdbHttpClientTvEpisodeCreditsResponse);
        TmdbTvEpisodeResponseDataModel? tvEpisodeResponse = TmdbHttpClient.GetTvEpisodeModelFromResponse(tmdbHttpClientTvEpisodeResponse);
        
        // Assert
        
        Assert.NotNull(tvEpisodeCreditsResponse);
        Assert.NotNull(tvEpisodeResponse);
        Assert.Equal(
            "Cast:\n*****\nAdult: False\nGender: 1\nId: 11863\nKnownForDepartment: Acting\nName: Example Smith\nOriginalName: Example Smith\nPopularity: 2.7348\nProfilePath: /xKe52w4tpv61ohz9iz75wNdzcwZ.jpg\nCharacter: Example Character 1\nCreditId: 5253386d19c2957940053d95\nOrder: 0\n\nAdult: False\nGender: 2\nId: 71675\nKnownForDepartment: Acting\nName: Example Actor 2\nOriginalName: Example Actor 2\nPopularity: 1.0828\nProfilePath: /6O5sg97Fv07EzdWjpoq3lb8KnTM.jpg\nCharacter: Example Character 4\nCreditId: 5253386d19c2957940053db3\nOrder: 1\n\nAdult: False\nGender: 1\nId: 21595\nKnownForDepartment: Acting\nName: Example Actress 3\nOriginalName: Example Actress 3\nPopularity: 2.4503\nProfilePath: /bO16z8rAzZWdjCga8dcbJ2AFAh2.jpg\nCharacter: Example Character 5\nCreditId: 5253386d19c2957940053dbd\nOrder: 2\n\nAdult: False\nGender: 1\nId: 66745\nKnownForDepartment: Acting\nName: Emma Caulfield\nOriginalName: Emma Caulfield\nPopularity: 0.9354\nProfilePath: /tYQURD5pl4iGHjyA2yVwmmhaoPt.jpg\nCharacter: Anya\nCreditId: 52599e3519c295731c08d0c7\nOrder: 3\n\nAdult: False\nGender: 1\nId: 49961\nKnownForDepartment: Acting\nName: Michelle Trachtenberg\nOriginalName: Michelle Trachtenberg\nPopularity: 2.7178\nProfilePath: /8eb8ts6E7gM5SgEFn8VcXfGc39r.jpg\nCharacter: Dawn Summers\nCreditId: 5253386d19c2957940053e31\nOrder: 6\n\nAdult: False\nGender: 2\nId: 47297\nKnownForDepartment: Acting\nName: Example Actor 4\nOriginalName: Example Actor 4\nPopularity: 1.1273\nProfilePath: /oJnBB3g2IINnsLSr8B79bJ9ykkx.jpg\nCharacter: Example Character 4\nCreditId: 5253386d19c2957940053de5\nOrder: 10\n*****\nCrew:\n*****\nAdult: False\nGender: 2\nId: 12891\nKnownForDepartment: Writing\nName: Example Creator Martinez\nOriginalName: Example Creator Martinez\nPopularity: 1.4737\nProfilePath: /6PJwHV17KTuTRQaqrXBtVCwchcU.jpg\nCreditId: 5253387019c2957940053fa5\nDepartment: Writing\nJob: Writer\n\nAdult: False\nGender: 2\nId: 12891\nKnownForDepartment: Writing\nName: Example Creator Martinez\nOriginalName: Example Creator Martinez\nPopularity: 1.4737\nProfilePath: /6PJwHV17KTuTRQaqrXBtVCwchcU.jpg\nCreditId: 52599e2019c295731c08a50b\nDepartment: Directing\nJob: Director\n\nAdult: False\nGender: 2\nId: 1068803\nKnownForDepartment: Directing\nName: John Medlen\nOriginalName: John Medlen\nPopularity: 0.174\nProfilePath: /31mi98LXo5z6tFCxdaZLM7EHHF7.jpg\nCreditId: 5ea5273b66f2d2001a3f14da\nDepartment: Crew\nJob: Stunt Coordinator\n\nAdult: False\nGender: 2\nId: 1399874\nKnownForDepartment: Crew\nName: Scott Workman\nOriginalName: Scott Workman\nPopularity: 0.2469\nProfilePath: /dDjY5nmDBTenBSAQGQqqkwvZ9A.jpg\nCreditId: 5e80da9d498ef90015c9108f\nDepartment: Crew\nJob: Stunts\n\nAdult: False\nGender: 0\nId: 2285291\nKnownForDepartment: Crew\nName: Gregg Sargeant\nOriginalName: Gregg Sargeant\nPopularity: 0.5525\nProfilePath: \nCreditId: 5e874c1e15376c0015e78874\nDepartment: Crew\nJob: Stunts\n\nAdult: False\nGender: 2\nId: 65965\nKnownForDepartment: Acting\nName: Steve Tartalia\nOriginalName: Steve Tartalia\nPopularity: 0.1325\nProfilePath: \nCreditId: 5e89c60cd207f3001698af36\nDepartment: Crew\nJob: Stunt Double\n\nAdult: False\nGender: 0\nId: 62583\nKnownForDepartment: Camera\nName: Raymond Stella\nOriginalName: Raymond Stella\nPopularity: 0.4147\nProfilePath: \nCreditId: 5e878eaf3f7e1d00175ec805\nDepartment: Camera\nJob: Director of Photography\n\nAdult: False\nGender: 2\nId: 1217941\nKnownForDepartment: Acting\nName: Sam Ayers\nOriginalName: Sam Ayers\nPopularity: 0.2652\nProfilePath: /rnHDYhwMjCigDTGpwpfC1KYxEOq.jpg\nCreditId: 5ebfe66a28723c001f448439\nDepartment: Crew\nJob: Stunts\n\nAdult: False\nGender: 2\nId: 1545551\nKnownForDepartment: Crew\nName: Todd Schneider\nOriginalName: Todd Schneider\nPopularity: 0.2903\nProfilePath: /iYK1vXyekEebqkPYP9oSBsrugiP.jpg\nCreditId: 5edab0bb5f4b73001e243cb0\nDepartment: Crew\nJob: Stunts\n\nAdult: False\nGender: 1\nId: 1018965\nKnownForDepartment: Editing\nName: Lisa Lassek\nOriginalName: Lisa Lassek\nPopularity: 0.2038\nProfilePath: /mbtXqnNEiUA8ixsLTjuwo3XVBLa.jpg\nCreditId: 5f29b1c64e674200347949f4\nDepartment: Editing\nJob: Editor\n\nAdult: False\nGender: 2\nId: 24528\nKnownForDepartment: Acting\nName: Matthew R. Anderson\nOriginalName: Matthew R. Anderson\nPopularity: 0.7002\nProfilePath: /6NJI6IX1V07j9oFaVBpEpoiaa6u.jpg\nCreditId: 5f30f213f1b5710035e9e585\nDepartment: Crew\nJob: Stunts\n\nAdult: False\nGender: 2\nId: 20739\nKnownForDepartment: Directing\nName: Adam Shankman\nOriginalName: Adam Shankman\nPopularity: 0.733\nProfilePath: /6Eu0BNd8rMuXaIXkF7nPD7qYYW2.jpg\nCreditId: 63ebe00df92532007f1bb90a\nDepartment: Crew\nJob: Choreographer\n\nAdult: False\nGender: 2\nId: 1213071\nKnownForDepartment: Writing\nName: Douglas Petrie\nOriginalName: Douglas Petrie\nPopularity: 0.4306\nProfilePath: /jt79Llnx8MdrgfBV7Tjoy27t3Gj.jpg\nCreditId: 6596bf40ea37e007534c8ed9\nDepartment: Production\nJob: Producer\n\nAdult: False\nGender: 2\nId: 1699138\nKnownForDepartment: Production\nName: Brian Wankum\nOriginalName: Brian Wankum\nPopularity: 0.0933\nProfilePath: \nCreditId: 6596bf550e64af72c78c193e\nDepartment: Production\nJob: Associate Producer\n*****\nGuestStars:\n*****\nCharacter: Example Character 6\nCreditId: 5253386d19c2957940053dc7\nOrder: 5\nAdult: False\nGender: 2\nId: 34257\nKnownForDepartment: Acting\nName: Example Actor 3\nOriginalName: Example Actor 3\nPopularity: 5.9016\nProfilePath: /eRfRWnoipu1Tx84fcuOEdfR87qb.jpg\n\nCharacter: Tara Maclay\nCreditId: 5253386d19c2957940053eb7\nOrder: 14\nAdult: False\nGender: 1\nId: 35468\nKnownForDepartment: Acting\nName: Amber Benson\nOriginalName: Amber Benson\nPopularity: 0.8414\nProfilePath: /vEBdN1BhSOG2pCZPpCb6dgj6Wer.jpg\n\nCharacter: Mustard Man\nCreditId: 52599e5619c295731c0912aa\nOrder: 522\nAdult: False\nGender: 2\nId: 149520\nKnownForDepartment: Writing\nName: David Fury\nOriginalName: David Fury\nPopularity: 0.4174\nProfilePath: /4V371yAWCJ1s2cVCamX5eYjMNVc.jpg\n\nCharacter: Sweet\nCreditId: 52599e5619c295731c091360\nOrder: 625\nAdult: False\nGender: 2\nId: 15567\nKnownForDepartment: Acting\nName: Hinton Battle\nOriginalName: Hinton Battle\nPopularity: 0.1936\nProfilePath: /zdAzNmXQUXhc9cDTjGBQ0HEJK2V.jpg\n\nCharacter: Parking Ticket Woman\nCreditId: 571782cd9251412b050010a4\nOrder: 694\nAdult: False\nGender: 1\nId: 149495\nKnownForDepartment: Production\nName: Marti Noxon\nOriginalName: Marti Noxon\nPopularity: 0.6933\nProfilePath: /rrt5WkIi31DKr30vJWEPlRpkgHL.jpg\n\nCharacter: Henchman / Tap Dancing Victim\nCreditId: 5f30f2ad8ed03f0035eeb551\nOrder: 1123\nAdult: False\nGender: 2\nId: 149740\nKnownForDepartment: Acting\nName: Scot Zeller\nOriginalName: Scot Zeller\nPopularity: 0.2752\nProfilePath: /vYb6it59p5TADS6oapywXLRaEGl.jpg\n\nCharacter: Demon / Henchman\nCreditId: 5f30f2d87739410035eaaf92\nOrder: 1124\nAdult: False\nGender: 2\nId: 29216\nKnownForDepartment: Crew\nName: Zachary Woodlee\nOriginalName: Zachary Woodlee\nPopularity: 0.2066\nProfilePath: /1fmzKHfS928pYm4sqE4o3CNFw.jpg\n\nCharacter: Henchman\nCreditId: 5f30f31a8ed03f0035eebbc4\nOrder: 1125\nAdult: False\nGender: 0\nId: 1773681\nKnownForDepartment: Acting\nName: Timothy Anderson\nOriginalName: Timothy Anderson\nPopularity: 0.169\nProfilePath: \n\nCharacter: Henchman\nCreditId: 5f30f33e8ed03f0036efc6c8\nOrder: 1126\nAdult: False\nGender: 2\nId: 1282303\nKnownForDepartment: Acting\nName: Alejandro Estornel\nOriginalName: Alejandro Estornel\nPopularity: 0.0387\nProfilePath: /hNEicTJNU3iCGQaULDDlCkqdD9I.jpg\n\nCharacter: Young Man\nCreditId: 5f30f3867739410038e5c24f\nOrder: 1127\nAdult: False\nGender: 2\nId: 2737347\nKnownForDepartment: Acting\nName: Daniel Weaver\nOriginalName: Daniel Weaver\nPopularity: 0.0537\nProfilePath: \n\nCharacter: College Guy\nCreditId: 5f30f3d0559d22003779bdf3\nOrder: 1128\nAdult: False\nGender: 0\nId: 202663\nKnownForDepartment: Acting\nName: Hunter Cochran\nOriginalName: Hunter Cochran\nPopularity: 0.095\nProfilePath: \n\nCharacter: College Guy\nCreditId: 6596c16f5907de6a4563bef4\nOrder: 1301\nAdult: False\nGender: 0\nId: 4463868\nKnownForDepartment: Acting\nName: Matt Sims\nOriginalName: Matt Sims\nPopularity: 0.0143\nProfilePath: \n*****\nId: 949534"
                , tvEpisodeCreditsResponse.ToString());
        Assert.Equal(
            "AirDate: 2001-11-06\nCrew:\n*****\nDepartment: Writing\nJob: Writer\nCreditId: 5253387019c2957940053fa5\nAdult: False\nGender: 2\nId: 12891\nKnownForDepartment: Writing\nName: Example Creator Martinez\nOriginalName: Example Creator Martinez\nPopularity: 1.5599\nProfilePath: /6PJwHV17KTuTRQaqrXBtVCwchcU.jpg\n\nDepartment: Directing\nJob: Director\nCreditId: 52599e2019c295731c08a50b\nAdult: False\nGender: 2\nId: 12891\nKnownForDepartment: Writing\nName: Example Creator Martinez\nOriginalName: Example Creator Martinez\nPopularity: 1.5599\nProfilePath: /6PJwHV17KTuTRQaqrXBtVCwchcU.jpg\n\nDepartment: Crew\nJob: Stunt Coordinator\nCreditId: 5ea5273b66f2d2001a3f14da\nAdult: False\nGender: 2\nId: 1068803\nKnownForDepartment: Directing\nName: John Medlen\nOriginalName: John Medlen\nPopularity: 0.2056\nProfilePath: /31mi98LXo5z6tFCxdaZLM7EHHF7.jpg\n\nDepartment: Crew\nJob: Stunts\nCreditId: 5e80da9d498ef90015c9108f\nAdult: False\nGender: 2\nId: 1399874\nKnownForDepartment: Crew\nName: Scott Workman\nOriginalName: Scott Workman\nPopularity: 0.4956\nProfilePath: /dDjY5nmDBTenBSAQGQqqkwvZ9A.jpg\n\nDepartment: Crew\nJob: Stunts\nCreditId: 5e874c1e15376c0015e78874\nAdult: False\nGender: 0\nId: 2285291\nKnownForDepartment: Crew\nName: Gregg Sargeant\nOriginalName: Gregg Sargeant\nPopularity: 0.2319\nProfilePath: \n\nDepartment: Crew\nJob: Stunt Double\nCreditId: 5e89c60cd207f3001698af36\nAdult: False\nGender: 2\nId: 65965\nKnownForDepartment: Acting\nName: Steve Tartalia\nOriginalName: Steve Tartalia\nPopularity: 0.2093\nProfilePath: \n\nDepartment: Camera\nJob: Director of Photography\nCreditId: 5e878eaf3f7e1d00175ec805\nAdult: False\nGender: 0\nId: 62583\nKnownForDepartment: Camera\nName: Raymond Stella\nOriginalName: Raymond Stella\nPopularity: 0.3473\nProfilePath: \n\nDepartment: Crew\nJob: Stunts\nCreditId: 5ebfe66a28723c001f448439\nAdult: False\nGender: 2\nId: 1217941\nKnownForDepartment: Acting\nName: Sam Ayers\nOriginalName: Sam Ayers\nPopularity: 0.4247\nProfilePath: /rnHDYhwMjCigDTGpwpfC1KYxEOq.jpg\n\nDepartment: Crew\nJob: Stunts\nCreditId: 5edab0bb5f4b73001e243cb0\nAdult: False\nGender: 2\nId: 1545551\nKnownForDepartment: Crew\nName: Todd Schneider\nOriginalName: Todd Schneider\nPopularity: 1.157\nProfilePath: /iYK1vXyekEebqkPYP9oSBsrugiP.jpg\n\nDepartment: Editing\nJob: Editor\nCreditId: 5f29b1c64e674200347949f4\nAdult: False\nGender: 1\nId: 1018965\nKnownForDepartment: Editing\nName: Lisa Lassek\nOriginalName: Lisa Lassek\nPopularity: 0.1866\nProfilePath: /mbtXqnNEiUA8ixsLTjuwo3XVBLa.jpg\n\nDepartment: Crew\nJob: Stunts\nCreditId: 5f30f213f1b5710035e9e585\nAdult: False\nGender: 2\nId: 24528\nKnownForDepartment: Acting\nName: Matthew R. Anderson\nOriginalName: Matthew R. Anderson\nPopularity: 0.2177\nProfilePath: /6NJI6IX1V07j9oFaVBpEpoiaa6u.jpg\n\nDepartment: Crew\nJob: Choreographer\nCreditId: 63ebe00df92532007f1bb90a\nAdult: False\nGender: 2\nId: 20739\nKnownForDepartment: Directing\nName: Adam Shankman\nOriginalName: Adam Shankman\nPopularity: 0.6645\nProfilePath: /6Eu0BNd8rMuXaIXkF7nPD7qYYW2.jpg\n\nDepartment: Production\nJob: Producer\nCreditId: 6596bf40ea37e007534c8ed9\nAdult: False\nGender: 2\nId: 1213071\nKnownForDepartment: Writing\nName: Douglas Petrie\nOriginalName: Douglas Petrie\nPopularity: 0.5165\nProfilePath: /jt79Llnx8MdrgfBV7Tjoy27t3Gj.jpg\n\nDepartment: Production\nJob: Associate Producer\nCreditId: 6596bf550e64af72c78c193e\nAdult: False\nGender: 2\nId: 1699138\nKnownForDepartment: Production\nName: Brian Wankum\nOriginalName: Brian Wankum\nPopularity: 0.1226\nProfilePath: \n*****\nEpisodeNumber: 7\nEpisodeType: standard\nGuestStars:\n*****\nCharacter: Example Character 6\nCreditId: 5253386d19c2957940053dc7\nOrder: 5\nAdult: False\nGender: 2\nId: 34257\nKnownForDepartment: Acting\nName: Example Actor 3\nOriginalName: Example Actor 3\nPopularity: 1.729\nProfilePath: /eRfRWnoipu1Tx84fcuOEdfR87qb.jpg\n\nCharacter: Tara Maclay\nCreditId: 5253386d19c2957940053eb7\nOrder: 14\nAdult: False\nGender: 1\nId: 35468\nKnownForDepartment: Acting\nName: Amber Benson\nOriginalName: Amber Benson\nPopularity: 0.8875\nProfilePath: /vEBdN1BhSOG2pCZPpCb6dgj6Wer.jpg\n\nCharacter: Mustard Man\nCreditId: 52599e5619c295731c0912aa\nOrder: 522\nAdult: False\nGender: 2\nId: 149520\nKnownForDepartment: Writing\nName: David Fury\nOriginalName: David Fury\nPopularity: 0.4897\nProfilePath: /4V371yAWCJ1s2cVCamX5eYjMNVc.jpg\n\nCharacter: Sweet\nCreditId: 52599e5619c295731c091360\nOrder: 625\nAdult: False\nGender: 2\nId: 15567\nKnownForDepartment: Acting\nName: Hinton Battle\nOriginalName: Hinton Battle\nPopularity: 0.1856\nProfilePath: /zdAzNmXQUXhc9cDTjGBQ0HEJK2V.jpg\n\nCharacter: Parking Ticket Woman\nCreditId: 571782cd9251412b050010a4\nOrder: 694\nAdult: False\nGender: 1\nId: 149495\nKnownForDepartment: Production\nName: Marti Noxon\nOriginalName: Marti Noxon\nPopularity: 0.3648\nProfilePath: /rrt5WkIi31DKr30vJWEPlRpkgHL.jpg\n\nCharacter: Henchman / Tap Dancing Victim\nCreditId: 5f30f2ad8ed03f0035eeb551\nOrder: 1123\nAdult: False\nGender: 2\nId: 149740\nKnownForDepartment: Acting\nName: Scot Zeller\nOriginalName: Scot Zeller\nPopularity: 0.319\nProfilePath: /vYb6it59p5TADS6oapywXLRaEGl.jpg\n\nCharacter: Demon / Henchman\nCreditId: 5f30f2d87739410035eaaf92\nOrder: 1124\nAdult: False\nGender: 2\nId: 29216\nKnownForDepartment: Crew\nName: Zachary Woodlee\nOriginalName: Zachary Woodlee\nPopularity: 0.2807\nProfilePath: /1fmzKHfS928pYm4sqE4o3CNFw.jpg\n\nCharacter: Henchman\nCreditId: 5f30f31a8ed03f0035eebbc4\nOrder: 1125\nAdult: False\nGender: 0\nId: 1773681\nKnownForDepartment: Acting\nName: Timothy Anderson\nOriginalName: Timothy Anderson\nPopularity: 0.0938\nProfilePath: \n\nCharacter: Henchman\nCreditId: 5f30f33e8ed03f0036efc6c8\nOrder: 1126\nAdult: False\nGender: 2\nId: 1282303\nKnownForDepartment: Acting\nName: Alejandro Estornel\nOriginalName: Alejandro Estornel\nPopularity: 0.1177\nProfilePath: /hNEicTJNU3iCGQaULDDlCkqdD9I.jpg\n\nCharacter: Young Man\nCreditId: 5f30f3867739410038e5c24f\nOrder: 1127\nAdult: False\nGender: 2\nId: 2737347\nKnownForDepartment: Acting\nName: Daniel Weaver\nOriginalName: Daniel Weaver\nPopularity: 0.0493\nProfilePath: \n\nCharacter: College Guy\nCreditId: 5f30f3d0559d22003779bdf3\nOrder: 1128\nAdult: False\nGender: 0\nId: 202663\nKnownForDepartment: Acting\nName: Hunter Cochran\nOriginalName: Hunter Cochran\nPopularity: 0.1637\nProfilePath: \n\nCharacter: College Guy\nCreditId: 6596c16f5907de6a4563bef4\nOrder: 1301\nAdult: False\nGender: 0\nId: 4463868\nKnownForDepartment: Acting\nName: Matt Sims\nOriginalName: Matt Sims\nPopularity: 0.0143\nProfilePath: \n*****\nName: Example TV Episode\nOverview: This episode has a lot of interesting things in it, especially music.\nId: 949534\nProductionCode: 6ABB07\nRuntime: 50\nSeasonNumber: 6\nStillPath: /m6DAoR7I3UAeyjGA5ekLf5KQDfS.jpg\nVoteAverage: 8.778\nVoteCount: 45"
                , tvEpisodeResponse.ToString());
    }
}