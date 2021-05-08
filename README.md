# Homework: ASP.NET Core Web API Serverless Application - Payments

## Architecture and source code structure

This application starts with two web API controllers.

1. AuthenticationController - authenticates user accounts and return JWT for other APIs access
2. UserController - provides user accounts related functionalities, mainly

- getallusers - list all user accounts in the system
- create payment - make a payment to another user account in the system
- getpayments - show current user's balance and list of payments

MediatR is employed to keep those controller thin and loosely coupled with CQRS pattern based heavy lifting classes. Dependencies like commands, queries and handlers are automatically injected.

More libraries like AutoMapper, FluentValidation are used to make the classes extremely loosely coupled.

Persistence wise, NHiberate is used so that I can focus more on OOP instead of RDBMS programming.

Extensibility wise, changes can be made to commands and queries easily to incorporate more business needs. E.g. to add support for getting payments in a date range, only very minimal changes are needed around the controller class, GetPaymentsQuery and its handler.

To help you navigte the code base, here are main folders and corresponding use:

- .github\workflows - Github actions. Two workflows are provided take care of CI and CD respectively
- database - sql script to create the backend database and initialize demo data
- src - web api implementation.
- src\Command - CQRS.commands
- src\Controllers - web API controllers
- src\CustomExceptions - a good practice to follow: our business logic specific exceptions
- src\Extensions - a bunch of service extension classes to seamlessly weave validation, persistence, JWT auth etc into .net core API pipeline
- src\mappers - NHibernate mapping classes
- src\Middlewares - error handling and JWT middleware to attach current user ID to current http context
- src\Models - mainly POCO entities
- src\Persistence - NHibernate based persitence access classes
- src\Queries - CQRS.queries
- src\Services - password and auth service
- test - test project
- test\IntegrationTests - the name says it all
- test\UnitTests - the name says it all

## Infrastructure

For demo purpose, I have used AWS to host this web API. Services used:

- CloudFormation - for lambda and API gateway deployment
- API Gateway - end user facing HTTP gateway
- Lambda - where the real meat is
- Cloudwatch - monitoring and logging

## TODOs, notes and assumptions

- Main focus of this excersise is architecture design instead of coding
- Use terraform and helm charts instead for Infrastructure as Code
- Apply branch protection rules like 'no direct commit to main', 'build has to be green before merge' etc. Some are not implemented due to free Github account limitation, others could be easily done via web hooks or Github actions
- Code generate all database objects instead of using sql scripts
- More tests. The current ones are very basic to demonstrate the approach.
- Paged records retrival for payments

## Source code structure

## How to build and run locally

1. Create a SQL server database and then run .\database\seed.sql in the newly created database
2. Open the top level folder in Visual studio code
3. Edit .\src\appsettings.json to set "IsCloudDeployment": "false", and "MSSQLConnectionLocal" to a SQL server connection string that connects to your database created in step 1
4. Terminal -> New Terminal (Powershell)

```Powershell
cd src
dotnet build .
dotnet run .
```

5. I personally use Postman for calling web APIs but you can use either bash, powershell, or whatever client for such a purpose. Here are some expamle Powershell scripts for your reference. Terminal -> New Terminal (Powershell)

- To get JWT token

```Powershell
$params = @{
 "username"="admin";
 "password"="UGFzc3dvcmQyMDIx";
}

Invoke-WebRequest -Uri http://localhost:5000/api/authentication/authenticate `
	-Method POST `
	-Body ($params|ConvertTo-Json) -ContentType "application/json" `
	| Select -ExpandProperty "Content"
```

Note down the token in the response and store it in a variable for other web API calls, e.g.:

```Powershell
$jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImExMDRmNTcxLTNhYTktNGFkMC1hZjEyLTVhMmU5NDcyNmI5ZCIsInVpZCI6IjRlNTU4MjFhLWRiMTktNDhlYy1hNjliLWE1ZTk3MDAwOTg0NCIsImV4cCI6MTYyMDQwODA1NywiaXNzIjoiTW91bGEiLCJhdWQiOiJNb3VsYSJ9.5fLJZuT9ibd_gswbR6HxR1f7Y5zkajuN0e90j_OcRjo"
```

- To list all users

```Powershell
$headers = @{Authorization = "Bearer $jwt"}

$params = @{
}

Invoke-WebRequest -Uri http://localhost:5000/api/user/getallusers `
	-Method POST -Headers $headers `
	-Body ($params|ConvertTo-Json) -ContentType "application/json" `
	| Select -ExpandProperty "Content"
```

- To make a payment

```Powershell
$headers = @{Authorization = "Bearer $jwt"}

$params = @{
	"payTo"="669293e1-1138-4f85-b32f-703271d8f216";
    "Amount"=11;
}

Invoke-WebRequest -Uri http://localhost:5000/api/user/createpayment `
	-Method POST -Headers $headers `
	-Body ($params|ConvertTo-Json) -ContentType "application/json" `
	| Select -ExpandProperty "Content"
```

- To get current user's balance and payments

```Powershell
$headers = @{Authorization = "Bearer $jwt"}

$params = @{
}

Invoke-WebRequest -Uri http://localhost:5000/api/user/getpayments `
	-Method POST -Headers $headers `
	-Body ($params|ConvertTo-Json) -ContentType "application/json" `
	| Select -ExpandProperty "Content"
```

## How to deploy

Deployment is implemented via Github action. Simply tag the repo with a name that matches pattern release.\*, e.g. release.0.1, to kick off auto-deployment to AWS.

## How to access the production instance

Production instance URL can be found in the output of deployment workflow. These are the current web API endpoints you can use:

- https://tlzskdiym8.execute-api.ap-southeast-2.amazonaws.com/api/authentication/authenticate
- https://tlzskdiym8.execute-api.ap-southeast-2.amazonaws.com/api/user/getallusers
- https://tlzskdiym8.execute-api.ap-southeast-2.amazonaws.com/api/user/createpayment
- https://tlzskdiym8.execute-api.ap-southeast-2.amazonaws.com/api/user/getpayments

Demo account: "username"="admin"; "password"="UGFzc3dvcmQyMDIx";

## Test coverage

Execute this command to run tests and colect coverage:

```powershell
cd .\test
dotnet add package coverlet.collector
dotnet test --collect:"XPlat Code Coverage"
```
