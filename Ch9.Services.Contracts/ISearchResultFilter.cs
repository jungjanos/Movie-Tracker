using Ch9.Models;

using System.Collections.Generic;

namespace Ch9.Services.Contracts
{
    public interface ISearchResultFilter
    {
        IEnumerable<MovieDetailModel> FilterBySearchSettings(IEnumerable<MovieDetailModel> movies);
        IEnumerable<MovieDetailModel> FilterBySearchSettingsIncludeAdult(IEnumerable<MovieDetailModel> movies);
        IEnumerable<MovieDetailModel> FilterForAdultOnly(IEnumerable<MovieDetailModel> movies);
    }
}
