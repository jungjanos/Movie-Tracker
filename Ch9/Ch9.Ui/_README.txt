All UI related components


1, Regarding ViewModel dependency to Xamarin.Forms:

Note that the ViewModel classes have no "real dependency" to Xamarin.Forms. The only dependency being the Xamarin.Forms.Command object for 
UI -> ViewModel command propagation. The internal implementation of Xamarin.Forms.Command is actually not depending on any Xamarin functionality, 
see: https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Command.cs

ViewModels can be tested without actually running any UI.

2, 