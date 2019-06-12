using Ch9.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ch9
{
    public class ReviewsPageViewModel
    {
        public MovieDetailModel Movie { get; private set; }
        public ObservableCollection<Review> Reviews { get; private set; }

        public ReviewsPageViewModel(MovieDetailModel movie, ObservableCollection<Review> reviews)
        {
            Movie = movie;
            Reviews = reviews;
        }
    }
}
