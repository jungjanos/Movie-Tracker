using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.ApiClient
{
    public static class WebApiPathConstants
    {
        public const string BASE_Address = "https://api.themoviedb.org";
        public const string BASE_Path = "/3";
        public const string CONFIG_Path = "/configuration";
        public const string GENRE_LIST_Path = "/genre/movie/list";
        public const string SEARCH_MOVIE_Path = "/search/movie";
        public const string SEARCH_QUERY_Key = "query";
        public const string TRENDING_Path = "/trending";
        public const string TRENDING_MOVIE_Selector = "/movie";
        public const string WEEK_Path = "/week";
        public const string DAY_Path = "/day";
        public const string IMAGE_DETAIL_Path = "/images";
        public const string IMAGE_ADDITIONAL_LANGUAGES = "include_image_language";

        public const string TRENDING_WEEK_Path = "/trending/movie/week";
        public const string TRENDING_DAY_Path = "/trending/movie/day";
        public const string MOVIE_DETAILS_Path = "/movie";
        public const string LANGUAGE_Key = "language";
        public const string PAGE_Key = "page";
        public const string INCLUDE_Adult_Key = "include_adult";
        public const string API_KEY_Key = "api_key";
        public const string RECOMMENDATIONS_Path = "/recommendations";
        public const string SIMILARS_Path = "/similar";
        public const string REQUEST_TOKEN_Path = "/authentication/token/new";
        public const string VALIDATE_REQUEST_TOKEN_W_LOGIN_Path = "/authentication/token/validate_with_login";
        public const string CREATE_SESSION_ID_Path = "/authentication/session/new";
        public const string DELETE_SESSION_Path = "/authentication/session";
        public const string ACCOUNT_DETAILS_Path = "/account";
        public const string SESSION_ID_Key = "session_id";
        public const string LISTS_path = "/lists";
        public const string LIST_path = "/list";
    }
}
