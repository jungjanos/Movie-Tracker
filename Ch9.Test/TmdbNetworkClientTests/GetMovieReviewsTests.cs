﻿using Ch9.ApiClient;
using Ch9.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ch9.Test.TmdbNetworkClientTests
{
    // INTEGRATION TESTS
    // for the critical TmdbNetworkClient.GetMovieReviews(...) function accessing the TMDB WebAPI
    public class GetMovieReviewsTests
    {
        private readonly ITestOutputHelper _output;
        Dictionary<string, object> _settingsKeyValues;
        Settings _settings;
        TmdbNetworkClient _client;

        int _movieWithReviews = 297761;
        int _movieWithoutReviews = 60800; // Macskafogó
        int _invalidMovieId = 99999999;
        int _movieWithReviewsInDifferentLanguages = 299534; // Has english and spanish reviews
        public GetMovieReviewsTests(ITestOutputHelper output)
        {
            _output = output;

            _settingsKeyValues = new Dictionary<string, object>();
            _settingsKeyValues[nameof(Settings.ApiKey)] = "764d596e888359d26c0dc49deffecbb3";
            _settingsKeyValues[nameof(Settings.SessionId)] = "563636d0e4a0b41b775ba7703cc5c985f36cffaf"; // !!!! correct it !!!!!
            _settings = new Settings(_settingsKeyValues);
            _client = new TmdbNetworkClient(_settings);
        }

        [Fact]
        // happy path
        public async Task WhenCalledOnMovieWithReviews_ReturnsNonemptyCollection()
        {
            var result = await _client.GetMovieReviews(_movieWithReviews, language: null, page: null, retryCount: 0);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");

            GetReviewsModel reviews = JsonConvert.DeserializeObject<GetReviewsModel>(result.Json);
            PrintReviews(reviews);
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(reviews.Reviews.Length > 0);
        }


        [Fact]
        // happy path
        public async Task WhenCalledOnMovieWithoutReviews_ReturnsEmptyCollection()
        {
            var result = await _client.GetMovieReviews(_movieWithoutReviews, language: null, page: null, retryCount: 0);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");

            GetReviewsModel reviews = JsonConvert.DeserializeObject<GetReviewsModel>(result.Json);
            PrintReviews(reviews);
            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(reviews.Reviews.Length == 0);
        }
        [Fact]
        // failure path
        public async Task WhenCalledWithInvalidMovieId_Returns404()
        {
            var result = await _client.GetMovieReviews(_invalidMovieId, language: null, page: null, retryCount: 0);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        // happy path
        public async Task WhenCalledWithLanguageOption_RespectsDesiredLanguage()
        {
            var result = await _client.GetMovieReviews(_movieWithReviewsInDifferentLanguages, language: "es", page: null, retryCount: 0);

            _output.WriteLine($"Server returned {result.HttpStatusCode}");

            GetReviewsModel reviews = JsonConvert.DeserializeObject<GetReviewsModel>(result.Json);
            PrintReviews(reviews);

            Assert.True(result.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(reviews.Reviews.Length > 0);
            Assert.DoesNotContain(" movie ", result.Json);  // no english
            Assert.Contains("muchos", result.Json); //spanish

        }


        private void PrintReviews(GetReviewsModel reviewsModel)
        {
            _output.WriteLine($"Returned {reviewsModel.Reviews.Length} results");
            _output.WriteLine($"id: {reviewsModel.Id}");
            _output.WriteLine($"page: {reviewsModel.Page}");
            _output.WriteLine($"total pages: {reviewsModel.TotalPages}");
            _output.WriteLine($"total results: {reviewsModel.TotalResults}");

            if (reviewsModel.Reviews.Length == 0)
                return;
            _output.WriteLine("=======RREVIEWS=======");

            foreach (Review review in reviewsModel.Reviews)
            {
                _output.WriteLine($"account id: {review.Id}");
                _output.WriteLine($"author: {review.Author}");
                _output.WriteLine($"content: {review.Content}");
                _output.WriteLine($"url: {review.Url}");
                _output.WriteLine("----------------------");
            }
            _output.WriteLine("=====RREVIEWS END=====");
        }
    }
}