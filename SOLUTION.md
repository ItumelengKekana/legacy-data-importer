# Design

This project uses Domain Driven Design in order to separate our business rules from our techinical implementation.

# Mediator Pattern

I opted to use the mediator pattern in this project in order to handle our business logic away from the controller by making use of the CQRS pattern.
The handlers and repositories will do most of the work alongside the validators while the controllers will have a single instance of Mediator injected instead if many services.

# Testing 
I used an in memory server for the unit tests