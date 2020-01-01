Manages the access and persistance of settings data local to the device.

Since a dependency to Xamarin.Forms is required, I decided to extract this component into its separate assembly to avoid 
the Ch9.Data.ApiClient (which manages Http-communication and has nothing to do with Xamarin) take a dependency to Xamarin.Forms 