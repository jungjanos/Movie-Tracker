All UI related components

0, Dependency Injection (with Autofac):

Dependency graph is configured by a single call to DependencyResolver.ConfigureServices() method from the App() constructor. 
View models classes are the resolution roots of the application: view models are the only types which are manually resolved for the DI container by calling Resolve<T>(...). 
All other types in the dependency chains are resolved automatically. The resolution of view models is always initiated from their owner, the View object. 


1, Regarding ViewModel dependency to Xamarin.Forms:

Note that the ViewModel classes have no "real dependency" to Xamarin.Forms. The only dependency being the Xamarin.Forms.Command object for 
UI -> ViewModel command propagation. The internal implementation of Xamarin.Forms.Command is actually not depending on any Xamarin functionality, 
see: https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Command.cs 
and could be easily replaced by a custom 50-line ICommand implamantation (e.g. for a later WPF implementation, currently not planned)

The main takeaway is: ViewModels can be tested without actually running any UI.


2, View-first approach, page navigation and IPageService:

I have decided to go with a View-first approach, that means that the View is always instantiated first and when the View is instantiated 
it resolves its ViewModel and all associated dependencies via DI and takes ownsership over the resulting ViewModel object. The VM objects lifetime 
is tied to the View. 

To be able to access page navigation from the ViewModels without any Xamarin dependencies, the ViewModel is injected with an IPageService object 
at DI-resolution time. The IPageService exposes the narrow set of Xamarin based services (mainly page navigation, alert-display and opening browser 
and device media player) without forcing any Xamarin dependency to the ViewModel object. 

Because the project has started much smaller (much fewer Views and needs for navigation), I have not outright started with a navigation framework like 
MVVM-cross or MVVM-Light as I considered them at that time an overkill (at that time I didnt even had DI in the project). Instead I fielded a custom 
IPageService-PageService based navigation. 

A future switch to a framework based navigation (like MVVM-Light) is a possibility. 


