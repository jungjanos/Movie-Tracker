using Ch9.Models;

using System;

namespace Ch9.Infrastructure.Extensions
{
    public static class PersonsMovieCreditsModelExtensions
    {
        public static void SortMoviesByYearDesc(this PersonsMovieCreditsModel movieCredits)
        {
            Array.Sort(movieCredits.MoviesAsActor, new MovieYearDescComparer());
            Array.Sort(movieCredits.MoviesAsCrewMember, new MovieYearDescComparer());
        }
    }
}
