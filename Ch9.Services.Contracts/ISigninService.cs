using System.Threading.Tasks;

namespace Ch9.Services.Contracts
{
    public interface ISigninService
    {
        Task Signin(string userName, string password, int retryCount, int delayMilliseconds);
    }
}