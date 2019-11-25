using System.Net;

namespace Ch9.Services.ApiCommunicationService
{
    class ApiCommunicationServiceResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
