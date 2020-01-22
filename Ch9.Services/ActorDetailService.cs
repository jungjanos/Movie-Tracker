using Ch9.Models;
using Ch9.Services.Contracts;
using Ch9.Infrastructure.Extensions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services
{
    public class ActorDetailService : IActorDetailService
    {
        private readonly ISettings _settings;
        private readonly ITmdbApiService _tmdbApiService;
        private readonly IMovieDetailModelConfigurator _movieDetailModelConfigurator;
        private readonly IPersonDetailModelConfigurator _personDetailModelConfigurator;

        public ActorDetailService(
            ISettings settings,
            ITmdbApiService tmdbApiService,
            IMovieDetailModelConfigurator movieDetailModelConfigurator,
            IPersonDetailModelConfigurator personDetailModelConfigurator)
        {
            _settings = settings;
            _tmdbApiService = tmdbApiService;
            _movieDetailModelConfigurator = movieDetailModelConfigurator;
            _personDetailModelConfigurator = personDetailModelConfigurator;
        }

        public async Task<IList<ImageModel>> LoadPersonsImageCollection(int personId, int retryCount, int delayMilliseconds, bool fromCahe = true)
        {
            var response = await _tmdbApiService.TryGetPersonImages(personId, retryCount: retryCount, delayMilliseconds: delayMilliseconds, fromCache: fromCahe);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                ImageModel[] result = response.Images;
                _personDetailModelConfigurator.SetProfileGalleryImageSources(result);

                return result;
            }
            else
                throw new Exception($"Could not load persons gallery,  {nameof(LoadPersonsImageCollection)} responded :{response.HttpStatusCode}");
        }

        public async Task<PersonsMovieCreditsModel> LoadPersonsMovieCredits(int personId, int retryCount, int delayMilliseconds, bool fromCahe = true)
        {
            var response = await _tmdbApiService.TryGetPersonsMovieCredits(personId, _settings.SearchLanguage, retryCount, delayMilliseconds, fromCache: fromCahe);

            if (response.HttpStatusCode.IsSuccessCode())
            {
                var result = response.PersonsMovieCreditsModel;

                // Task.Run() decouples this time consuming operation from the calling UI thread
                await Task.Run(() =>
                {
                    _movieDetailModelConfigurator.SetImageSrc(result.MoviesAsActor);
                    _movieDetailModelConfigurator.SetImageSrc(result.MoviesAsCrewMember);
                    _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(result.MoviesAsActor);
                    _movieDetailModelConfigurator.SetGenreNamesFromGenreIds(result.MoviesAsCrewMember);
                    result.SortMoviesByYearDesc();
                });
                return result;
            }
            else
                throw new Exception($"Could not load persons movie participations,  {nameof(LoadPersonsMovieCredits)} responded :{response.HttpStatusCode}");
        }
    }
}
