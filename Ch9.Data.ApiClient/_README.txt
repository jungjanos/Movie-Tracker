This project contains the components necessary to make RESTfull Http requests to the TMDb WebApi accessible at https://api.themoviedb.org/3/.

-No dependency allowed except Ch9.Data.Contracts (its interface assembly).
-Communicates through primitive types and JSON-format strings with the service layer
-JSON deserialization and data model object creation is responsibility of a service layer component


Main components:

1, TmdbNetworkClient for Uncached RESTfull Http-request:

-Returns WebApi query results encapsulated into TmdbResponseBase objects (see bellow). 
-Handles retries and timeouts automatically based on query configuration. 
-Swallows network exceptions (retries and if all retries fail returns Http failure code)
-HttpStatusCode property indicates success or failure and the Json property contains the response's payload (in case of success).

    public class TmdbResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Json { get; set; }
    }

-All distinct WebApi queries use a separate result class derived from TmdbResponseBase. 
(they basically add semantic markup to indicate content type) 

I made this decission to enforce strong typing even at the data access level. 
The cost for this are several dozens of boilerplate classes. 


2, TmdbCachedSerachClient uses TmdbNetworkClient for locally cached searches where it makes sense
-Uses an object cache (LazyCache)




