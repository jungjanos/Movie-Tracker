using Ch9.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ch9.Utils
{
    public interface IPageService
    {
        Task PushAsync(MovieDetailModel movie);
    }

    public class PageService : IPageService
    {
        private readonly Page _currentPage;


        public PageService(Page current)
        {
            _currentPage = current;
        }

        public async Task PushAsync(MovieDetailModel movie)
        {
            await _currentPage.Navigation.PushAsync(new MovieDetailPage(movie));
        }
    }


}
