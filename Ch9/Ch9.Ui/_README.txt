All UI related components

0, Dependency Injection (with Autofac):

Dependency graph is configured by a single call to DependencyResolver.ConfigureServices() method from the App() constructor. 
View models classes are the resolution roots of the application: view models are the only types which are manually resolved for the DI container by calling Resolve<T>(...). 
All other types in the dependency chains are resolved automatically. The resolution of view models is always initiated from their owner, the View object. 


1, Regarding ViewModel dependency to Xamarin.Forms:

Note that the ViewModel classes have no "real dependency" to Xamarin.Forms. The only dependency being the Xamarin.Forms.Command object for 
UI -> ViewModel command propagation. The internal implementation of Xamarin.Forms.Command is actually not depending on any Xamarin functionality, 
see: https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Command.cs

ViewModels can be tested without actually running any UI.

 