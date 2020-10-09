using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Ch9.Models;
using Ch9.Services.LocalSettings;
using Ch9.Data.ApiClient;
using Ch9.Data.Contracts;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetPresonsDetails(...) function accessing the TMDB WebAPI
    public class GetPersonsDetailsTests
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _actor = 11856; // Daniel Day-Lewis
        readonly int _invalidActor = 9999999;

        public GetPersonsDetailsTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";

            _settings = new Settings(_settingsKeyValues, null);
            _client = new TmdbNetworkClient(null, _settings.ApiKey);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnActor_ReturnsPersonsDetails()
        {
            GetPersonsDetailsResult response = await _client.GetPersonsDetails(personId: _actor, language: null, retryCount: 0, delayMilliseconds: 1000);
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);

            PersonsDetailsModel personsDetails = JsonConvert.DeserializeObject<PersonsDetailsModel>(response.Json);
            PrintPersonsDetails(personsDetails);

            Assert.Contains("Daniel Day-Lewis", personsDetails.Name, System.StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        // failure path
        public async Task WhenCalledOnInvalidPerson_Return404NotFound()
        {
            GetPersonsDetailsResult response = await _client.GetPersonsDetails(personId: _invalidActor, language: null, retryCount: 0, delayMilliseconds: 1000);
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        // NO, server currently does not return localized biographies
        // TmdbNetworkClient call should be initiated without language option set

        //[Fact]
        //// happy path
        //public async Task WhenCalledWithLanguageOption_ReturnsResultsInLanguage()
        //{
        //    string language = "de";

        //    GetPersonsDetailsResult response = await _client.GetPersonsDetails(personId: _actor2, language: language, retryCount: 0, delayMilliseconds: 1000);
        //    Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);

        //    GetPersonsDetailsModel personsDetails = JsonConvert.DeserializeObject<GetPersonsDetailsModel>(response.Json);
        //    PrintPersonsDetails(personsDetails);
        //}

        private void PrintPersonsDetails(PersonsDetailsModel person)
        {
            _output.WriteLine($"Id: {person.Id}");
            _output.WriteLine($"Name: {person.Name}");
            _output.WriteLine($"Biography: {person.Biography}");
            _output.WriteLine($"Known for: {person.KnownFor}");
            _output.WriteLine($"Birthday: {person.Birthday}");
            _output.WriteLine($"Place of birth: {person.PlaceOfBirth}");
            _output.WriteLine($"Death day: {person.DeathDay ?? ""}");

            _output.WriteLine($"Sex: {person.Sex}");
            _output.WriteLine($"Popularity: {person.Popularity}");
            _output.WriteLine($"Profile: {person.ProfilePath}");
            _output.WriteLine($"IMDB id: {person.ImdbId}");
            _output.WriteLine($"Adult: {person.Adult}");

            _output.WriteLine($"Nicknames");
            foreach (var nickName in person.AlsoKnownAs)
                _output.WriteLine(nickName);
        }
    }
}
