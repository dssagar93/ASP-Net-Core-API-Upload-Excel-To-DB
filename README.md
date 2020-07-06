# ASP Net Core API :computer:

An ASP Net Core API project built to upload excel file to SQL database. It fetches dataset from the SQL DB based on conditional parameters pass to a stored proedure through an API.

EPPlus -  A nuget plugin to parse and manipulate excel worksheets.

## API Endpoints :satellite:

/UploadExcel -  It expects an excel file as an input with a particular format.

/BrandChannel -  It expects a raw incomplete sql query that is passed to a stored procedure to fetch conditional data. 

## Net Core features implemented :octocat:

* Services
* Dependency Injection
* User Defined Table Type
* Stored Procedures
* Excel Worksheet parser
* Async/await
* Helper Classes
* Interface
* SQL Connection
* Repository Pattern

## Stack 🔖
* C#
* Net Core
* Web API

## Restore Nuget Packages

You need to restore nuget packages that are required to run the project.

You can use Package Manager to restore - Visual Studio -> Tools -> NuGet Package Manager-> Manage NuGet Packages for Solution and download the required packages.

You can also you Package Manager Console to restore package via CLI by executing following command
```
nuget restore
```
## Development server
Execute SQL script and run the project with IIS Express.


