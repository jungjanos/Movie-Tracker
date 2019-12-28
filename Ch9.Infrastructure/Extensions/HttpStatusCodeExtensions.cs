using System.Net;

namespace Ch9.Infrastructure.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessCode(this HttpStatusCode httpStatusCode)
        {
            return (200 <= (int)httpStatusCode && (int)httpStatusCode < 300);
        }

        public static bool Is500Code(this HttpStatusCode httpStatusCode)
        {
            return 500 == (int)httpStatusCode;
        }

        public static bool Is200Code(this HttpStatusCode httpStatusCode)
        {
            return 200 == (int)httpStatusCode;
        }
    }
}
