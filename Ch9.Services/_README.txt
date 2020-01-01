Project containing the Service components. 

1, ApiCommunicationService
Uses the Data.ApiClient component from the Data access layer to provide high level RESTfull Http communication to other services and ViewModels.

The task of this component is to deserialize the Json response (wraped into subclasses of TmdbResponseBase) from the Data access layer into Model objects to consume by this service's clients.

The resulting Model objects are wraped into the derivates of ApiCommunicationServiceResponseBase baseclass which also signals success status for the higher level components.
If the success status is HTTP-success (200-299) the client is free to take the enclosed Model object. Otherwise the Model is NULL. 

Since in some cases the WebApi server sends dynamically typed JSON responses, in some cases deserialization contains custom logic and error handling
this is hidden from the clients of this component. 

Example return type for the TryGetMovieCredits(...) request:

    public class TryGetMovieCreditsResponse : ApiCommunicationServiceResponseBase
    {
        public base.HttpStatusCode HttpStatusCode { get; private set; }
        public MovieCreditsModel MovieCreditsModel { get; private set; }

        public TryGetMovieCreditsResponse(HttpStatusCode statusCode, MovieCreditsModel movieCredits)
        {
            HttpStatusCode = statusCode;
            MovieCreditsModel = movieCredits;
        }
    }

2, LocalSettings
Provides the means to load/query/persist local settings on the device. The abstraction of settings is based on a KeyValuePair<string, string> dictionary
with the values deserialized into actual Model objects. This is highly flexible and also compatible with the underlying Xamarin.Forms AppDictionary used by the Data access layer.

This Service component uses Ch9.Data.LocalSettings in the Data access layer to avoid taking a direct dependency to Xamarin.Forms (which provides its Appdictionary)
for the Ch9.Service assembly.


3, UiModelConfigurationServices
Provide configuration services for freshly created Model objects based on server settings. UiModelConfigurationServices set up (where required) the properties of 
the model objects so that it can be used for data binding to the UI.

Example: WebApi service response provides Models with relative Urls for images. The configurator straightens the relative Urls to fully qualified Urls based on 
dynamic configuration (prepends server base Url). 

4, VideoService
Provides the means to: 
-query metadata information about available movie videos from Youtube (title, length, availability, available media streams)
-load above information into the model objects

YtExplodeVideoService builds on this component but is implemented in a separate assembly as it uses dirty (legally grey) means to parse/rip
Youtube streams. (the reason is that YtExplodeVideoService must be excluded from build when targeting GooglePlay)

The actual video playback is implemnted by a platform video player (located in Ch9.Android and Ch9.Ui). 

