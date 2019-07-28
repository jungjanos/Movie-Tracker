using Ch9.ApiClient;
using Ch9.Models;
using Ch9.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetMovieCredits(...) function accessing the TMDB WebAPI
    public class GetMovieCreditsTests
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _movie = 60800; // Macskafogó
        readonly int _invalidMovie = 9999999;

        public GetMovieCreditsTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";

            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnMovie_ReturnsCredits()
        {
            GetMovieCreditsResult result = await _client.GetMovieCredits(_movie, 0, 1000);

            MovieCreditsModel credits = JsonConvert.DeserializeObject<MovieCreditsModel>(result.Json);

            PrintCast(credits.Cast);
            PrintCrew(credits.Crew);

            Assert.True(result.HttpStatusCode.IsSuccessCode());
        }

        [Fact]
        // failure path
        public async Task WhenCalledOnInvalidMovie_Returns404()
        {
            GetMovieCreditsResult result = await _client.GetMovieCredits(_invalidMovie, 0, 1000);

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }


        private void PrintCast(IList<MovieCastModel> actors)
        {
            _output.WriteLine("============ CAST ============");
            _output.WriteLine($"{actors.Count.ToString()} items");
            _output.WriteLine("___");

            foreach (var actor in actors)
            {
                _output.WriteLine($"Cast id: {actor.CastId}");
                _output.WriteLine($"Character: {actor.Character}");
                _output.WriteLine($"Credit id: {actor.CreditId}");
                _output.WriteLine($"Sex: {actor.Gender}");
                _output.WriteLine($"Id: {actor.Id}");
                _output.WriteLine($"Name: {actor.Name}");
                _output.WriteLine($"Order: {actor.Order}");
                _output.WriteLine($"Profile path: {actor.ProfilePath}");
                //_output.WriteLine($"{}");

                _output.WriteLine("-------------");
            }
        }

        private void PrintCrew(IList<MovieCrewModel> crew)
        {
            _output.WriteLine("============ CREW ============");
            _output.WriteLine($"{crew.Count.ToString()} items");
            _output.WriteLine("___");

            foreach (var member in crew)
            {
                _output.WriteLine($"Credit id: {member.CreditId}");
                _output.WriteLine($"Department: {member.Department}");

                _output.WriteLine($"Sex: {member.Gender}");
                _output.WriteLine($"Id: {member.Id}");
                _output.WriteLine($"Job: {member.Job}");
                _output.WriteLine($"Name: {member.Name}");
                _output.WriteLine($"Profile path: {member.ProfilePath}");
                //_output.WriteLine($"{}");

                _output.WriteLine("-------------");
            }
        }

    }
}
