using Ch9.Ui.Contracts.Models;

namespace Ch9.Services.Contracts
{
    public interface IWeblinkComposer
    {
        string Compose(MovieDetailModel movie);
        string Compose(PersonsDetailsModel person);
    }
}