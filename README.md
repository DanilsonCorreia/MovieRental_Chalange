# MovieRental Exercise

This is a dummy representation of a movie rental system.
Can you help us fix some issues and implement missing features?

 * The app is throwing an error when we start, please help us. Also, tell us what caused the issue.
 
    The problem was on the Program.cs line 12 where was useded the metod AddSingeton to register the IRentalFeatures as a singeton but the RentalFeature depends on MovieRentalDbContext which is resgistered as scope so it cause a dependency injection problem.
    A singleton cannot depend on a scoped service because the scoped service would outlive its intended lifetime.
 
 * The rental class has a method to save, but it is not async, can you make it async and explain to us what is the difference?

    Now it works with threads and the thread is freed up to handle other requests while waiting for the database,
    Can handle many more concurrent requests with fewer threads,
    Threads can be reused while waiting for I/O operations

 * Please finish the method to filter rentals by customer name, and add the new endpoint.
    
    Changed the existing endpoint to work with async requests.
 
 * We noticed we do not have a table for customers, it is not good to have just the customer name in the rental.
   Can you help us add a new entity for this? Don't forget to change the customer name field to a foreign key, and fix your previous method!

    New entity created for Customers with all need information, and creation of the service, the interface and controller for managing and adding the customers on the Db. 
    Has changed the foreign key of customer for Id that is now used as identifier in the others methods and classes.
    
    
 * In the MovieFeatures class, there is a method to list all movies, tell us your opinion about it.

    The current implementation is fine for a small demo or prototype, but it won't scale well as your movie catalog grows, 
    to be more robost it should implement pagination, sorting, filtering capabilities, chacing in case of many request to it and no changes made, and made it async to work with threads.
    I think that that implementation could transform the methot more scaleble.
 
 * No exceptions are being caught in this api, how would you deal with these exceptions?
 
 


	## Challenge (Nice to have)
We need to implement a new feature in the system that supports automatic payment processing. Given the advancements in technology, it is essential to integrate multiple payment providers into our system.

Here are the specific instructions for this implementation:

* Payment Provider Classes:
    * In the "PaymentProvider" folder, you will find two classes that contain basic (dummy) implementations of payment providers. These can be used as a starting point for your work.
* RentalFeatures Class:
    * Within the RentalFeatures class, you are required to implement the payment processing functionality.
* Payment Provider Designation:
    * The specific payment provider to be used in a rental is specified in the Rental model under the attribute named "PaymentMethod".
* Extensibility:
    * The system should be designed to allow the addition of more payment providers in the future, ensuring flexibility and scalability.
* Payment Failure Handling:
    * If the payment method fails during the transaction, the system should prevent the creation of the rental record. In such cases, no rental should be saved to the database.
