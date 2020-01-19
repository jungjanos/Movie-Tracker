Assembly to hold the Model classes to be used by all UI (View and ViewModels) and Service layer objects. The Model objects are vertical in the layering, thus they in a certain aspect marry the presentation with the domain. I have decided against the introduction of a (Domain model object) <=> (Presentation layer object) mapping. Instead of an object-object mapping I only do a lightweight configuration of the model object in the service layer to be able to present it in the View. 

The reasons are:

-One of the main feature of the app is to enable fast infinite scrolling of results on the UI. I dont want to slow down weak phones with a further layer of object instantiation. 

-API seams to be pretty stable so I dont fear that the Domain structure (API response structure) gets changed (which would imply changes Model's I use)

-Even if the Domain model (API response structure) is broken on the server side, Json.NET still provides great flexibility when deserializing 


Model object (de)serialization and model object configuration (to be able to use in the Ui) are done in the Service layer. 

No dependencies allowed in this assembly. Please note that the Data access layer does not use any models, only primitives and JSON strings
