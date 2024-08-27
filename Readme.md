
# Store database with cache

This C# application let's you store data of employees, customers, products and orders.
It has functionality of adding, editing, and deleting data for any of the objects mentioned before. Main database is SQL, but it also is using MongoDB for cache.

# Installation

1. Create an SQL database and insert the connection string at "...Store.Core\Repositories\MyDbContext.cs" line 20.
2. Open the project with Visual Studio, open console shell terminal at "Store.Core". Run these 3 commands to set up tables in your SQL database:
* dotnet tool install --global dotnet-ef
* dotnet ef migrations add InitialCreate
* dotnet ef database update
3. Create a MongoDB database and insert the connection string at "...Store.API\Program.cs" line 26.
4. Start Store.API and enjoy!

# Features


* Employee and customer data validation: email and phone number. Supported formats for phone number: +3706xxxxxxx and 3706xxxxxxx.
* Product data validation for price and stock.
* Order data validation for amount of products.
* Logging: most of methods in services are logged, so it will be easier to pin point what is going on with object data if needed. Log files can be found in "...Store.API\Logs".
* Orders can be edited fully and the product stock is always checked if some orders or users get deleted.
* This project laos includes tests for services, so you can check their functionality by running them in "Store.Tests".
* Cache is deleted every 2 minutes.

# Coded by Edvinas Pocius