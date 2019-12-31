This project contains the components necessary to make RESTfull Http requests to the https://api.themoviedb.org/3/ WebApi server.

1, TmdbNetworkClient for Uncached RESTfull Http-request

Returns query results as TmdbResponseBase object (see bellow). 
Handles automatic retries and timeouts based on query configuration. 

HttpStatusCode indicates success or failure throug Http codes and the Json object (in case of success) contains the payload.

    public class TmdbResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Json { get; set; }
    }

All distinct WebApi queries use a separate result class derived from TmdbResponseBase. 
(they basically add semantic markup to indicate content type) 

I made this decission to enforce strong typing even at the data access level. 
The cost for this are several dozens of boilerplate classes. 



2, TmdbCachedSerachClient uses TmdbNetworkClient for locally cached searches where it makes sense

-No dependency allowed except Ch9.Data.Contracts (its interface assembly).
-Communicates through primitive types and JSON-format strings with the service layer
-JSON deserialization and data model object creation is responsibility of a service layer component



