﻿using Ch9.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface IMovieDetailsService
    {
        Task<List<IStaffMemberRole>> FetchMovieCredits(int movieId, int retryCount, int delayMilliseconds, bool fromCache);
        Task<PersonsDetailsModel> FetchPersonsDetails(int personId, int retryCount, int delayMilliseconds);
        Task LoadMovieGallery(MovieDetailModel movieToPopulate, int retryCount, int delayMilliseconds, bool fromCache);
        Task LoadVideoThumbnailCollection(MovieDetailModel movieToPopulate, int retryCount, int delayMilliseconds, bool fromCache);
        Task<AccountMovieStatesDto> PopulateMovieWithDetailsAndFetchStates(MovieDetailModel movieToPopulate, int retryCount, int delayMilliseconds);
    }
}