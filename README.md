# Distributions
## Google Play
Get [Movie Tracker from GooglePlay](https://play.google.com/store/apps/details?id=org.janos.jung) (no Youtube stream ripping)

![](/Documentation_images/gplay.png)

## Download APK

You can directly download the APK from the below links. 
Please not that it might be necessary to enable App installation from external sources on your phone. The app is perfectly safe and does not require any special permission.



# Abstract 

The aim of this project is to produce a searcher/tracker/detailer mobile application for movies. The app is based on [TMDb](https://www.themoviedb.org/)'s open [WebAPI](https://developers.themoviedb.org/3/getting-started/introduction) and implements the most important features the API exposes.

Platform specific code is kept to a minimum to provide a base for future code sharing with a possible iOS client: currently 98% of the app's code resides in the shared project. 

# Technology

* Xamarin.Forms as UI framework with MVVM pattern
* Fielding my own HTTPS based REST client implementation 
* XUnit for integration testing with API server and YouTube scraper
* Newtonsoft.Json
* FFImageLoading for image caching
* LazyCache for object caching
* YoutubeExplode for YouTube video scraping (only when not targeting Google Play)
* CardsView for swipe based gallery with cards animation
* Dotfuscator for assembly obfuscation (currently not thoroughly tested, **don't build with it, has known problems**)
* Extensive care has been taken to provide a smooth, non blocking UI experience based on the Task-async pattern

# Features

As of version nr. 60, the app provides the features listed below. Refer to the app-screen images at the bottom of the page for how the feature is integrated into the UI (subject to change with further versions).


### Searching, listing and getting recommendations:

* Search for movies based on keywords
* Configure search filters based on movies age, genre preferences, adult switch
* Top trending movies for the current week and day 
* Get the list of similar movies for the currently selected movie by tapping on the compass icon
* Get the list of recommended movies for the currently selected movie
* All result lists implement infinite scrolling  

### Get the movie's detailed info:  

* Tap a movie entry in a list to display its details (descriptions, length, score, countries, etc..)
* Tap on the people icon on the movie's detail page to have the movie's cast displayed in descending order 
* Tap on one person in the cast to open that person's detail page with swipeable gallery, biography and popularity index, list his movie participations separately as being actor or crew member 
* Tap on the web-link icon to open the movie's page on your favorite movie website 
* Tap on the text icon to have any reviews displayed on a separate page. Here you also can rate the movie (you need to be logged in)

### Gallery pictures and associated videos (trailers, behind the scenes etc):

* Swipe the gallery to view the movie's backdrop pictures, tap to view the picture in large
* Tap under the gallery to view trailer and other associated video clip thumbnails
* Tap a video thumbnail to watch the short video clip. Depending on the distribution platform the video is either played via the device default YouTube player (GooglePlay distribution) or (in case of an ad-hoc distribution) stream-scraped from Youtube and played with the apps built in player this circumvents any ads from YouTube. 
* In case YouTube scraping fails (e.g. due to future changes to YouTube page DOM),  the YouTube player is used as auto-failback.
* In case of YouTube-scraping the target video quality of the scraped YouTube stream can be controlled from the Settings

### Favorite-, Watchlist and custom lists management: 
(**needs an account on TMDb** and the user needs to sign in on the app)

* View your personal Watchlist (bucket list) and Favorite lists
* Display and edit your user defined lists, set one of them as active
* Watchlist, Favorite list and user defined lists are server side features synchronized to the device
* Add any movie to your personal Watchlist or Favorite list by tapping on the bookmark or heart icon on the movie's page
* Add any movie to your currently selected custom list by taping on the '+' sign 
* Visual real time feedback of the movie's status in relation to your Watchlist, Favorite list, currently active custom list and your signed-in-status when opening the movie's detail page (green, red, gray colors) 

### Login and Settings:

* When starting up the application, you can opt out from logging in with your TMDb account
* Display your login status on the Settings page
* Configure safe search (porn) 
* Set your preferred language which governs the content's display language (descriptions, genre tags, posters etc...)
* Filter out old movies
* Configure your favorite movie genre categories, not matching items will be filtered out where appropriate
* Configure to which supported movie site the "more detail weblink" will direct you when opening external infolinks to view movie or actor details. 
* Set your preferred YouTube stream quality (only applies to non GooglePlay distributions) for stream-scraping
* Configure HTTP/HTTPS option for fetching images (HTTPS is strongly recommended)


# Screenshots

## Searching and recommendations

| *keyword search* | *result filter settings* |
|:--:|:--:|
| ![](/Documentation_images/Search_for_keyword_SM.png) | ![](/Documentation_images/search_settings_SM.png ) |
 

| *trending movies* | *recommendations and similars* |
|:--:|:--:|
| ![](/Documentation_images/trending_page_SM.png) | ![](/Documentation_images/recommended_similars_SM.png) |

## Movie details

| *movie details* | *movie cast* |
|:--:|:--:|
| ![](/Documentation_images/movie_detail_page_SM.png) | ![](/Documentation_images/movie_cast_SM.png) |

| *actor's details* | *reviews* |
|:--:|:--:|
| ![](/Documentation_images/actors_page_SM.png ) | ![](/Documentation_images/reviews_page_SM.png) |

| *browse and play the trailers from Youtube* | *ad-free clips (not in GooglePlay version)* |
|:--:|:--:|
| ![](/Documentation_images/movie_trailer_thumbnails_SM.png) | ![](/Documentation_images/trailer_playback_SM.png) | 

## Warchlist, Favorites and custom lists

| *live state display* | *server synced lists* |
|:--:|:--:|
| ![](/Documentation_images/synchronized_movie_states_SM.png) | ![](/Documentation_images/personal_watchlist_SM.png) |

| *manage your custom lists* |  |
|:--:|:--:|
| ![](/Documentation_images/user_defined_lists_SM.png) |  |


## Login and settings

| *login options* | *settings page* |
|:--:|:--:|
| ![](/Documentation_images/login_page_SM.png) | ![](/Documentation_images/settings_SM.png) |
