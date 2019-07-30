using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;
using System.Linq;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetPresonsMovieCredits(...) function accessing the TMDB WebAPI
    public class GetPresonsMovieCreditsTests
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _movie = 7345; // There Will Be Blood 
        readonly int _actor = 11856; // Daniel Day-Lewis
        readonly int _director = 4762; // Paul Thomas Anderson
        readonly int _invalidActor = 9999999;

        public GetPresonsMovieCreditsTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";

            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnActor_ReturnsPersonsCredits()
        {
            GetPersonsMovieCreditsResult response = await _client.GetPersonsMovieCredits(personId: _actor, language: null, retryCount: 0, delayMilliseconds: 1000);
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);

            GetPersonsMovieCreditsModel personsCredits = JsonConvert.DeserializeObject<GetPersonsMovieCreditsModel>(response.Json);
            PrintActorsCredits(personsCredits.MoviesAsActor);

            Assert.True(personsCredits.MoviesAsActor.Length >= 32);
            Assert.True(personsCredits.MoviesAsActor.SingleOrDefault(m => m.Id == _movie) != null);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnDirector_ReturnsPersonsCredits()
        {
            GetPersonsMovieCreditsResult response = await _client.GetPersonsMovieCredits(personId: _director, language: null, retryCount: 0, delayMilliseconds: 1000);
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);

            GetPersonsMovieCreditsModel personsCredits = JsonConvert.DeserializeObject<GetPersonsMovieCreditsModel>(response.Json);
            PrintCrewMembersCredits(personsCredits.MoviesAsCrewMember);

            Assert.True(personsCredits.MoviesAsCrewMember.Length >= 1);
            Assert.True(personsCredits.MoviesAsCrewMember.Where(m => m.Id == _movie).Count() > 0);
        }

        [Fact]
        // failure path
        public async Task WhenCalledOnInvalidPerson_Return404NotFound()
        {
            GetPersonsMovieCreditsResult response = await _client.GetPersonsMovieCredits(personId: _invalidActor, language: null, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine(response.HttpStatusCode.ToString());            
            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithLanguageOption_ReturnsResultInSetLanguage()
        {
            string language = "hu";
            string hungarianTitle = "Vérző olaj";
            GetPersonsMovieCreditsResult response = await _client.GetPersonsMovieCredits(personId: _actor, language: language, retryCount: 0, delayMilliseconds: 1000);

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);

            GetPersonsMovieCreditsModel personsCredits = JsonConvert.DeserializeObject<GetPersonsMovieCreditsModel>(response.Json);
            PrintActorsCredits(personsCredits.MoviesAsActor);

            Assert.Contains(hungarianTitle, response.Json, System.StringComparison.OrdinalIgnoreCase);
        }


        private void PrintActorsCredits(IList<ActorsMovieCredit> actorsCredits)
        {
            _output.WriteLine("========== As actor ==========");
            _output.WriteLine($"Number of entries: {actorsCredits.Count}");
            _output.WriteLine("------");

            foreach (var credit in actorsCredits)
            {
                _output.WriteLine($"Movie Id: {credit.Id}");
                _output.WriteLine($"Title: {credit.Title}");
                _output.WriteLine($"Character: {credit.Character}");
                _output.WriteLine($"Credit id: {credit.CreditId}");

                _output.WriteLine("-------------------------");                
            }
        }

        private void PrintCrewMembersCredits(IList<CrewMembersMovieCredit> crewMembersCredits)
        {
            _output.WriteLine("========== As actor ==========");
            _output.WriteLine($"Number of entries: {crewMembersCredits.Count}");
            _output.WriteLine("------");

            foreach (var credit in crewMembersCredits)
            {
                _output.WriteLine($"Movie Id: {credit.Id}");
                _output.WriteLine($"Title: {credit.Title}");
                _output.WriteLine($"Department: {credit.Department}");
                _output.WriteLine($"Job: {credit.Job}");
                _output.WriteLine($"Credit id: {credit.CreditId}");

                _output.WriteLine("-------------------------");
            }
        }
    }
}
