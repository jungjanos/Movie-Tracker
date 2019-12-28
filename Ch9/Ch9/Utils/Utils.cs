using Ch9.Ui.Contracts.Models;

using System.Net;

namespace Ch9.Utils
{
    public static class Utils
    {
        public static bool IsSuccessCode(this HttpStatusCode httpStatusCode)
        {
            return 200 <= (int)httpStatusCode && (int)httpStatusCode < 300;
        }

        public static decimal GetValue(this Rating rating) => (decimal)rating / 2;

    }
}
