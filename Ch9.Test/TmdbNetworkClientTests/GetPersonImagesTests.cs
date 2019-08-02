using Ch9.ApiClient;
using Ch9.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetPresonImages(...) function accessing the TMDB WebAPI
    public class GetPersonImagesTests
    {
        private readonly ITestOutputHelper _output;
        readonly Dictionary<string, object> _settingsKeyValues;
        readonly Settings _settings;
        readonly TmdbNetworkClient _client;
        readonly int _actor = 11856;         
        readonly int _invalidActor = 9999999;

        public GetPersonImagesTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";

            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings, null);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnPerson_Return_200OK()
        {
            GetPersonImagesResult response = await _client.GetPersonImages(_actor, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnPerson_ReturnImageCollection()
        {
            GetPersonImagesResult response = await _client.GetPersonImages(_actor, retryCount: 0, delayMilliseconds: 1000);

            _output.WriteLine($"Server responded: {response.Json}");
            var images = JsonConvert.DeserializeObject<ImageDetailCollection>(response.Json).Profiles;

            PrintImageDetail(images);

            Assert.True(images.Length > 0);            
        }

        [Fact]
        // failure path
        public async Task WhenCalledOnPerson_Returns404()
        {
            GetPersonImagesResult response = await _client.GetPersonImages(_invalidActor, retryCount: 0, delayMilliseconds: 1000);
            _output.WriteLine($"Server responded: {response.HttpStatusCode}");

            Assert.True(response.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        private void PrintImageDetail(IList<ImageModel> images)
        {
            _output.WriteLine($"{images.Count} images");

            foreach(var image in images)
            {
                _output.WriteLine("----------------------------------");
                _output.WriteLine($"Filepath: \t\t {image.FilePath}");
                _output.WriteLine($"With: \t\t {image.Width}");
                _output.WriteLine($"Height: \t\t {image.Height}");
                _output.WriteLine($"Aspect: \t\t {image.AspectRatio}");
                _output.WriteLine($"Iso: \t\t {image.Iso}");
                _output.WriteLine($"Vote count: \t {image.VoteCount}");
                _output.WriteLine($"Vote avg: \t {image.VoteAverage}");
            }
        }
    }
}
