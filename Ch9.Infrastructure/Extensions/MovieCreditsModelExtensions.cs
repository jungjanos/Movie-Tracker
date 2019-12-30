using Ch9.Models;

using System.Collections.Generic;
using System.Linq;

namespace Ch9.Infrastructure.Extensions
{
    public static class MovieCreditsModelExtensions
    {
        /// <summary>
        /// Extracts from server response the Director, Writer and top billed Actors to display on the UI
        /// </summary>        
        /// <param name="numberOfActors">Number of actors to display after the director and writer</param>
        /// <returns>Collection to be bound to the UI</returns>
        public static List<IStaffMemberRole> ExtractStaffToDisplay(this MovieCreditsModel credits, int numberOfActors = 5)
        {
            List<IStaffMemberRole> staffMembers = new List<IStaffMemberRole>();

            var director = credits.Crew.FirstOrDefault(c => c.Role == "Director");
            var writer = credits.Crew.FirstOrDefault(c => (c.Role == "Writer") || (c.Department == "Writing"));

            var firstCreditedActors = credits.Cast.Take(numberOfActors);

            if (director != null)
                staffMembers.Add(director);

            foreach (var actor in firstCreditedActors)
                staffMembers.Add(actor);

            if (writer != null)
                staffMembers.Add(writer);

            return staffMembers;
        }
    }
}
