using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.Models
{
    // Class is used to store the retreived publicly accessible movie lists of the user
    public class UserLists
    {
        public MovieListModel[] PublicMovieLists { get; set; }        
    }
}
