# dotnet-on-azure
Trying out .NET in Azure.

```mermaid
graph TD
    A[Enrollment] ---> B["ABC Organization (Tenant aka Directory)<br>For eg: abc.onmicrosoft.com"]
    B --> C1["Contoso(Tenant)<br>For eg: contoso.onmicrosoft.com"]
    B --> C2["Fabrikam(Tenant)<br>For eg: fabrikam.onmicrosoft.com"]
    B ---> C3["Some Saas Non Azure Subscription"] %% Extra "-" to push it down a level
    C1 --> D11["Dev Sub"]
    C1 --> D12["Prod Sub"]
    D11 --> E111["Dev RG1"]
    D11 --> E112["Dev RG2"]
    D12 --> E121["Prod RGs"]
    E111 --> F1111["Resources.<br>For eg: AppService, SqlDb etc."]
    E112 --> F1121["..."]
    E121 --> F1211["..."]
    C2 --> D21["Dev Sub"]
    C2 --> D22["Prod Sub"]
    D21 --> E211["Dev RGs"]
    D22 --> E221["Prod RGs"]
    E211 --> F2111["..."]
    E221 --> F2211["..."]
    C3 --> D31["Office 365"]
    C3 --> D32["Dynamics 365"]
    D31 --> E311["For eg: 100 E5 licenses"]
    D32 --> E321["For eg: 50 licenses"]

    classDef hidden display: none;
```

### Common Services
- Azure App Service
- Azure Kubernetes Service
- Azure Functions
- Application Insights
- Azure SignalR
- Azure SQL
- Key Vault
- Container Registry
- Blob Storage
- Managed Identity
- Azure Container Apps
- App Config
- Cosmos Db

### Multitenant Apps
Think of music streaming service or photo sharing service.

### Service Principal and Managed Identities
[Reference](https://stackoverflow.com/questions/61322079/difference-between-service-principal-and-managed-identities-in-azure)
Internally, managed identities are service principals of a special type which are locked to only be used with Azure resources. 

#### User Principal vs Service Principal in Azure. [Reference](https://youtu.be/RLnQqJY7Hss?si=2xGIlR0XHsukXbgY)
<img width="400" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/29a2fabe-9ac5-4e52-9cee-26f083c0ebdd">
<img width="400" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/217d6c13-cdb8-4b06-ab2d-decedf292ff5">

While I'm interacting with my Azure resources, I also talk to my AD to get my token and make requests. Look example here:

<img width="950" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/3edd26ca-1a60-4e4b-84c4-bedc3a8d8ab7">

### Understanding Tenants and Subscriptions
References:  
1. [Subscriptions, licenses, accounts, and tenants](https://learn.microsoft.com/en-us/microsoft-365/enterprise/subscriptions-licenses-accounts-and-tenants-for-microsoft-cloud-offerings?view=o365-worldwide)
2. [Tenants and Subscriptions](https://azure-training.com/2022/02/28/understanding-tenants-and-subscriptions-in-azure/)
3. [Difference between Tenant and Subscription](https://stackoverflow.com/a/61702511/8644294)





                                                            Enrollment 
- Azure billing and cost management construct for Enterprise Agreement customers. ea.azure.com
- For large companies
                                                                    |
                                                   Tenant  aka Directory

- For eg: affableashkoutlook.onmicrosoft.com (Found by going to Settings -> Directories + subscriptions OR Menu -> Azure Active Directory)
- Directory Id is TenantId because of one to one relationship between tenant and Azure AD.
-The reason it's called directory is because each directory has an Azure AD service associated with it.
-Every MSFT service is always associated with Azure AD even if we're not using Azure
-For eg: If I'm using O365, I'll have Azure AD at the top of it.
                                                                    |
                                           Subscription1, Subscription2 
- Construct for creating separate billing and management boundaries. Is managed in portal.azure.com
- For customers like Ashish Khanal who can use credit card and do Pay as you go.
- Can create multiple subscriptions in Azure account to create separation.
- Can be linked to existing identity stores for single sign on, or segregated into a separate area.
- Becomes the major separation for assignment of RBAC within services
                                                                    |
                                 ResourceGroup1           ResourceGroup2
-For eg: Create each resource group for each application
                                                 |                                        |
                           Resource11, Resource12           Resource21
- Like App Service, Sql Db etc.
-Resources in 1 RG are completely isolated from resources in another RG


Another variation of this hierarchy:

                                                                                 ABC Organization(Tenant)
                                                                  affableashkoutlook.onmicrosoft.com
                                                                                                |______________________
                                                                                                                                                  |
               Contoso (Tenant)                    Fabrikam (Tenant)                                               |
        contoso.onmicrosoft.com    fabirakam.onmicrosoft.com                                      |
                              |                                                    |                                                            |
         Dev Sub               Prod Sub          Dev Sub         Prod Sub        Some SaaS Non-Azure Subscription
                |                            |                        |                        |                                               |
 DRG1           DRG2    PRG1, PRG2            RG                     RG                     Office 365,        Dynamics 365
     |                    |                 |                        |                        |                                |                            | 
AppService   SqlDb            …                        …                      …                     100 E5 licenses     50 Licenses


Note: Users live at Tenant/ Directory level.




Tenant:
	- Is associated with a single entity, i.e. person, company or org and can own one or several subscriptions. In my case, it's a person with domain name: affableashkoutlook.onmicrosoft.com and Organization Id (tenant Id): 
	9d7f6902-2a61-4363-964c-c364b9eaf716
	- Each tenant is referred to as an organization.
	- I can create multiple tenants after logging in to Azure Portal.
	- EVERY TENANT IS LINKED TO A SINGLE AZURE AD INSTANCE which is shared with all tenant's subscriptions.
	- Contains user accounts and groups
	
Subscription:
	- An agreement with MSFT to use one or more MSFT cloud platforms or services for which charges accrue based on either:
		○ Per user license fee. For eg: Saas like Office 365 or Dynamics 365.
		○ Cloud based resource consumption. For eg: Paas and IaaS. 
	- A subscription is linked to a payment setup and each subscription will result in separate bill.
	- Subscription could be CSP (Cloud Service Provider), Pay-As-You-Go, EA etc.
	- A subscription can only be associated with a single Azure AD tenant at any given time.
	- Inside every subscription we can add Resources like VM, SqlDb etc.
	- Tenant or Directory has 1:M relationship with Subscription

	

For eg:
When I opened an Azure Account, this is what I got:
Note: tenantId is my DirectoryId and id is my SubscriptionID.




Search for Subscription from the search bar:


You can see cost of your services inside the Subscription. Click on the Azure subscription 1 shown above.



Creating resource group:
ashish@Azure:~$ az group create -g rg-pitstop-eastus-dev-001 -l eastus

👇
-g is for resource group name, -l is for location (Remember RALEIgh)
ResourceGroup-AppName-Location-Environment-Instance



Open account at Azure

1.
Register Microsoft.CloudShell namespace to your subscription.
Why?
Cloud Shell needs access to manage resources.
Access is provided through namespace that must be registered to your subscription.

az account set --subscription <Subscription Name or Id>
az provider register --namespace Microsoft.CloudShell

Get your subscription Id using
az account list (Grab Id)


Azure App Service:
App service app naming: (Remember RALEIgh. R: ResourceType)

app-APPNAME-WEB(Because it's a web app)-LOCATION-ENV-INSTANCE



Web app runs on App Service Plan which determines CPU and memory of it.

You can name it like this:
asp-APPNAME-LOCATION-ENV-INSTANCE
asp (App Service Plan)

Notice it doesn't have type like web, api etc. after APPNAME. It's because I want to put both the web app and the web api in that app service plan.

Note about Blazor Web App in .NET 8:
With .NET 8 comes a new template for Blazor applications simply called Blazor Web App and by default all components user Server-Side-Rendering.
To add interactivity to it, you need to add this service and middleware.
More info here.

builder.Services.AddRazorComponents() //👈Adds services required to server-side render components. Added by default.
.AddServerComponents(); //👈Stuff I added for Server Side Interactivity

app.MapRazorComponents<App>() //👈Discovers routable components and sets them  up as endpoints. Added by default.
.AddServerRenderMode();//👈Stuff I added for Server Side Interactivity

Database:
Sql Db

Password: Tara1028!

Allow connections to this SQL server from your IP.
They appear under Firewall rules:

Only do this for dev scenarios, NOT for PROD. 


And notice that's my IP address:


Server: sqlserver-munson-eastus-dev-001
Db Name: sqldb-munson-eastus-dev-001


So far:
Resource Group: rg-munson-eastus-dev-001
App Service: app-munson-web-eastus-dev-001
App Service Plan: asp-munson-eastus-dev-001
Db Server: sqlserver-munson-eastus-dev-001
Db Name: sqldb-munson-eastus-dev-001

Grab connection string (ADO.NET SQL auth):
Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Persist Security Info=False;User ID=munson;Password={your_password};

Add this conncection string to dotnet-secrets.

1.Go into the project folder and init (dotnet user-secrets init)
2.This will appear in .csproj file


Set the connection string with this command:
dotnet user-secrets set ConnectionStrings:Default "Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Persist Security Info=False;User ID=munson;Password={your_password};"

This will set the connection string like this in the secrets.json:


It actually looks like this (after installing this plugin):


Safe storage of app secrets in development in ASP.NET Core


Update:
I decided to do Passwordless authentication to connect to the Azure SQL Db shown here.
Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";

The passwordless connection string includes a configuration value of Authentication=Active Directory Default, which enables Entity Framework Core to use DefaultAzureCredential to connect to Azure services. When the app runs locally, it authenticates with the user you're signed into Visual Studio with. Once the app deploys to Azure, the same code discovers and applies the managed identity that is associated with the hosted app, which you'll configure later.

Keep in mind that when you deploy a web app to Azure, it treats it as Production. 'Production' is default if DOTNET_ENVIORNMENT and ASPNETCORE_ENVIRONMENT is not set. Reference. 
Environment values set in launchSettings.json override values set in the system environment. That file is only used on the local dev machine.


Setup the Munson's Blazor app and run it.

You'll see this exception:



Go to Plugins and install Azure Toolkit for Rider.

Now install Azure CLI for macOS. (But install homebrew first.)
	1. sudo chown -R $(whoami) /usr/local/var/homebrew
	2. sudo chown -R $(whoami) /usr/local/opt
	3. chmod u+w /usr/local/opt
	4. brew update && brew install azure-cli


Now login to Azure: az login


This will satisfy this login requirement:
Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";


Now run the app:
Line 13 will create the tables, and line 14 will seed the database.



(Migration is not required in the above approach:
Ashishs-MacBook-Pro:MunsonPickles.Web ashishkhanal$ 
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database update
)




Add interactivity to the new Blazor Web App in .NET 8 using this guide:
ASP.NET Core Blazor render modes | Microsoft Learn

The app works now!

Publish to Azure:
Go to Tools -> Azure -> Azure Sign In


Go with Device Login


Select my Subscription:


Now Right click on Project -> Publish -> Azure


Select 'Use Existing Web App' and click on the app shown below, like so:


Click Apply -> Run


To publish it again:

Go to Run -> ReRun the publish profile


The easiest way is from the configuration dropdown in the top right:


At this point the app doesn't work correctly on Azure. You still need to configure the secure connection between the App Service and the SQL database to retrieve your data. Read it all about it here.


The following steps are required to connect the App Service instance to Azure SQL Database:
	1. Create a managed identity for the App Service. The Microsoft.Data.SqlClient library included in your app will automatically discover the managed identity, just like it discovered your local machine Azure User.
	2. Create a SQL database user and associate it with the App Service managed identity.
	3. Assign SQL roles to the database user that allow for read, write, and potentially other permissions.

Use Service connector to accomplish this:
Service Connector is a tool that streamlines authenticated connections between different services in Azure. Service Connector currently supports connecting an App Service to a SQL database via the Azure CLI using the az webapp connection create sql command. This single command completes the three steps mentioned above for you.

Now it doesn't have anything under Identity -> System assigned:


Run this command:


which translates to (Run it in Cloud Shell or Azure CLI):

az webapp connection create sql -g rg-sampleapp-eastus-dev-001 -n app-munson-web2-eastus-dev-001 --tg rg-sampleapp-eastus-dev-001 --server sqlserver-munson1-eastus-dev-001 --database sqldb-munson-eastus-dev-001 --system-identity --connection ThisCanBeAnything --client-type dotnet

Now you have managed Identity showing:


This connection string shows up inside Configuration:

Data Source=sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Authentication=ActiveDirectoryManagedIdentity


The app still didn't start :(


Go to App Service -> Diagnose and solve problems
-> Availability and Performance




Container crash ->


UPDATE: Running the command again solved the issue for me:


The user shows up in the SQL Db as well:



Also make sure you're admin to the database in SQL Server. This was here by default for me.


Now the app runs from Azure. Big fuckin Yaaay!



Upload files to a server: Reference

Using Managed Identity to access Azure Services that support Azure AD. Reference
A managed identity from Azure Active Directory (Azure AD) allows App Service to access resources through role-based access control (RBAC), without requiring app credentials. After assigning a managed identity to your web app, Azure takes care of the creation and distribution of a certificate. People don't have to worry about managing secrets or app credentials.

This is secret-less way of doing this, that's why I love it. For eg: No credentials in the connection string.

Any service that supports managed identity (B in the following image) can be securely accessed.



Go to my app service -> Identity to see that I've already enabled System assigned identity previously.



The note says: 
"This resource is registered with Azure Active Directory. The managed identity can be configured to allow access to other resources. Be careful when making changes to the access settings for the managed identity because it can result in failures."

A general purpose v2 storage account provides access to all of the Azure Storage Services: blobs, files, queues, table and disks.

Blobs in Azure storage are organized into containers.
Before we can upload a blob, we must first create a container.

A binary large object (blob) is a collection of binary data stored as a single entity. Blobs are typically images, audio or other multimedia objects, though sometimes binary executable code is stored as a blob.



Add a container to put images of my web app. For eg: I gave 'web' as a name of my folder.


The access level of this container is private by default. To change this to public, go to Configuration -> Allow Blob anonymous access -> Enabled -> Save


Now change access level of this container: -> Blob


We need to grant our web app access to the storage account before we can create, read or delete blobs.

Using Azure RBAC, we can give the managed Identity of the web app access to another Azure resource just like any security principal (User Principal or Service Principal explained earlier in this page).

The 'Storage Blob Contributor' role gives the web app (represented by system assigned managed identity) read, write, and delete access to the blob container and data.


Go to my storage account to grant my web app access to it.

IAM -> Role Assignments
This shows who has access to this resource. There's ME!




Let's add role assignment to a robot (Managed Identity)
Select Add -> Add role assignment

Search for 'Storage block data contributor' role


Click Next to Select who needs this role

Managed Identity -> Select Members -> Subscription -> App Service (Web App) -> 
The managed Identity shows up.


Select it and hit Next. 


Hit 'Review + assign'.

The IAM page looks like this after the assignment:



Now go ahead and upload images to 'web' container

The thing is these images are only available in the eat US. If I try to hit the blob url from Asia, it'll have to make bunch of internet hops to get to it.

So what we can do is put a CDN on top of our blob storage.
CDN lives on the Azure edge.

Go to CDN

Give Profile name, Endpoint name and specify Query string caching behavior. 
Hit create:



Query string caching behaviors:
1. Ignore Query String: The first request is served from the origin server and the response is cached. Subsequent requests are served from the cache whatever the query string is. This sounds ludicrous!

Request1:
Browser (mydomain.com/articles?page=3) -> Azure CDN -> Server (mydomain.com/articles?page=3)
Request2:
Browser (mydomain.com/articles?page=42) -> Azure CDN (from cached whatever the query string)
2. Bypass caching for query string: Azure CDN doesn't cache the requests that have a query string.
Request1:
Browser (mydomain.com/articles?page=3) -> Azure CDN -> Server (mydomain.com/articles?page=3)
Request2:
Browser (mydomain.com/articles?page=3) -> Azure CDN -> Server (mydomain.com/articles?page=3)

3.Use query string: Each request with a unique url including the query string is treated as a unique asset with its own cache.
Request1:
Browser (www.example.ashx?q=test1) -> Azure CDN -> Server (www.example.ashx?q=test1)
Request2:
Browser (www.example.ashx?q=test1) -> Azure CDN (from cache)

The order of the query string parameters doesn't matter. For example, if the Azure CDN environment includes a cached response for the URL www.example.ashx?q=test1&r=test2, then a request for www.example.ashx?r=test2&q=test1 is also served from the cache.

Go to CDN endpoint now

Grab the endpoint hostname that's served through CDN.
Origin hostname is being served through the storage living in eastus.
Notice the urls.



Now grab the endpoint hostname + web + filename and update the db:



Update the code to show product photo in a "col" class.


Now the page looks like this:


Now we want to give users the ability to upload images while giving a review of a product.

For this we need Azure SDKs.

Go to Dependencies -> Manage NuGet Packages and add these packages to the project:
1. Azure.Storage.Blobs : To work with Blob storage
2. Microsoft.Extensions.Azure : Azure client SDK integration with Microsoft.Extensions libraries. For eg: To get this line to work:



To setup connection to Blob. This article helped.
How to authenticate .NET applications with Azure services - .NET | Microsoft Learn

IMPORTANT: (This wasted few hours and caused a lot of frustration)
Your account needs to have Role Assignment to upload files even though I'm the owner.



Your account comes into the picture from DefaultAzureCredential used to setup the BlobServiceClient during local development.

Also as you can see in the screenshot above, Azure App Service (Web app) already has access to it through Managed Identity when it runs in the cloud.



Add Auth to our app:

But first, let's learn some Auth basics.

Which OAuth 2.0 Flow Should I Use? (auth0.com)

Oauth 2.0 Authorization framework supports several different flows (or grants). Flow are ways of getting an access token.

Terminology: RACUR (Remember it as "Racer" but e is replaced by u)

Resource Owner: Entity that can grant access to protected resource. Typically the end user.

Authorization Server: Server that authenticates the Resource owner and issues access tokens after getting proper authorization. For eg: Auth0, Azure AD B2C.

Client: Application requesting access to a protected resource on behalf of Resource owner. For eg: Outlook or some web app that calls some protected API.
Client can live on a server (as a web app) or on the device (as a mobile app) or in the browser(JS app or WASM app). 
If it lives on the server, it's considered a confidential client (can keep secrets). If it lives on the device or in the browser, it's a public client.
What type of client we have, determines which Oauth grant to use.
On public clients, you cannot have the client authenticate with the authorization server (only user is authenticated) and therefore you can't get refresh tokens from the authorization server.

User Agent: Agent used by the resource owner to interact with the Client (for eg: a browser or a native application like my Outlook app).

Resource Server: Server hosting the protected resource. For eg: Exchange server.

OAuth2.0 is really designed to allow users (Resource owners) to give access to a third party Client to their resources.

Case 1: Client is also the Resource owner
Application is the Resource Owner meaning a Machine is requesting access to a protected resource and it itself can grant access to it.
So it's a machine-machine communication.
No end user auth required.
For eg: CRON job that yes an API to import information to a database.
CRON job talks to that API (machine - machine)
CRON job holds the ClientId and ClientSecret and uses them to get an Access Token from Authorization Server.

It involves an application exchanging its application credentials, such as client Id and client secret for an access token.



This uses Client Credentials Flow.

Case 2: Client is a web app executing on the server
If the Client is a regular web app executing on a server, the we should use Authorization Code Flow.
Using this, the Client can retrieve Access Token and optionally a Refresh Token.
It's considered the safest choice since Access Token is passed directly to the web server hosting the Client without going through the user's browser and risking exposure.

It involves exchanging authorization code for a token.


Case 3: Client is absolutely trusted with user credentials
This uses Resource Owner Password Flow.
Because this Flow involves the application handling the user's password, it must not be used by third-party clients.

Because credentials are sent to the backend and can be stored for future use before being exchanged for an Access Token, it is important that the application is absolutely trusted with this information.

This flow should only be used when redirect based flows like Authorization Code Flow cannot be used.


First-party apps: Let's say I created both API 'api.contoso.com' and web app: 'contoso.com'. You log into contoso.com and consume api.contoso.com. Both of these apps here are first party apps. I'd register both web app and api under the same Auth0 domain. 

Third Party apps: Apps that don't have admin access to my Auth0 domain.
Third party apps enable external parties or partners to securely access protected resources behind my API.

Case 4: Client is a SPA
There are 2 options here:
Authorization Code Flow with Proof Key for Code Exchange (PKCE) ✅
Implicit Flow with Form Post

The first one is the recommended approach because the Access Token is not exposed on the client side and this flow can return Refresh Tokens.

Auth0's SDK creates a cryptographically-random code_verifier and from this generates a code_challenge. More info here.



If the SPA doesn't need an Access Token, we can use the Implicit Flow with Form Post. Implicit Flow with Form Post applies to traditional web apps as opposed to SPAs. You get ID tokens as opposed to access tokens.



Case 5: Client is a Native/ Mobile App
Use Authorization Code Flow with Proof Key for Code Exchange (PKCE) ✅

Case 6: Application that needs to talk to different Resource Servers
If a single app needs access tokens for different resource servers, then multiple calls to /authorize (that is multiple executions of the same or different Authorization Flow) needs to be performed. Each authorization will use a different value for audience which will result in a different access token at the end of the flow.

Back to our app:

Choosing right auth library:


MSAL is used for fetching access tokens for accessing protected APIs (not shown here), as well as ID tokens. ASPNET Core middleware is capable of obtaining ID token on its own.

Scenario: Sample



Our web app will get access token from Azure AD B2C and will be able to call either Microsoft Graph or our backend Web API.

Build a web app that authenticates users and calls web APIs - Microsoft Entra | Microsoft Learn

Start by creating a new Azure AD B2C TENANT in our subscription:

Search for AD B2C




Tutorial - Create an Azure Active Directory B2C tenant | Microsoft Learn

Befor my app can interact with Azure AD B2C, it must be registered in a tenant that I manage.


Settings -> Directories + subscriptions.
Switch to the directory that contains your Subscription.
I'm already on the directory that has my subscription, so it's all good for me.



Ensure that my Subscription has Microsoft.AzureActiveDirectory as a Resource Provider by going to Subscription -> Resource Providers




Select the Row -> Register




Fill up the form:


The directory is created now:



Search for Azure AD B2C and click on the Star to make it appear under Favirotes





Switch to your directory:



Register a web app in Azure AD B2C. Reference.

Search for and select Azure AD B2C -> App Registration -> New Registration

Fill up the form:


Add one more Redirect URI using info from launchsettings.json


https://localhost:7032/signin-oidc


Record client Id: 171b3d8f-8ff1-48b7-a5be-31b0413955ee

Create a client secret
For this web app we just registered, we need to create an application secret. This is also known as application password. Our app will  exchange authorization code (our app receives this from auth server when user authenticates and consent. See pic from the section where I talk about Oauth flows for more info) + client Id + client secret for an Access Token.

App Registrations page -> Munson Web -> Certificates and secrets -> New Client Secret


Id: 31601f7b-88ba-4de1-b3d0-76902e7cc36b
Value: fA58Q~6MzNJ3yk.YTq9iP51R1niJFWuxaGxTIcub

Enable ID token implicit grant
If we register this app and configure it with jwt.ms for testing a user flow or custom policy, we need to enable implicit grant flow in the app registration.

Authentication -> Select both options -> Save


Create User Flows. Reference.
A user flow lets us determine how users interact with our application when they do things like sign-in, sign-up, edit a profile or reset a password.

Select Azure AD B2C -> User Flows (Under Policies) -> New User flow

Create a User flow -> Sign up and sign in -> Recommended (Under Version)  -> Create

Name: B2C_1_ ----> SignUpSignIn


-> Create

Test it

Open the user flow you just created -> Run user flow


I run the flow now and get this Id token back



Enable self service password reset

Select the SignUpSignIn user flow that we just created -> Properties -> Self service password reset -> Save



Enable Self Service Profile Editing

User flows -> New user flow
Create a user flow -> Profile editing -> Recommended -> Create

-> Create

Test this flow

Login using the credentials you used earlier -> You'll see a page to update your display name and job title.


You'll get back the token:


Turn the project into API -> Web App
Make Http Requests.

Configure auth in a Web app that calls a Web API. Reference.



Now register the Web API (APP ID 2):
App Registration -> New Registration



Record the client ID: 2d491ecb-81e4-40a2-abbb-659c2484303a

Configure Web API app scopes:

Do a bit of reading here first.

Permission is a declaration of an action that can be executed on a resource.
Resources (For eg: your web API) expose permissions.

My Blazor Server web app has permissions to call Microsoft Graph and Munson API as shown in the App registration -> API permissions:


Users have privileges when permissions are assigned to them.
Simply put, privileges are assigned permissions.

So, resources expose permissions. Users have privileges.


Scopes enable a mechanism to define what an application can do on behalf of the user.
Scopes are permissions of a resource (scopes are exposed in API so API is the resource here) that the application wants to exercise on behalf of the user. These permissions are in Web app.

Application wants to exercise the user's privileges on a resource such as reading their email.


On the resource side, user's privileges must be checked even in the presence of granted scopes.



Whenever the Client Blazor Server app calls the API, the logged in user will grant "read" access to the Blazor Server app which will present that scope to the backend API which is protected by a policy that requires "read" scope.





Expose an API -> Application ID URI -> Add -> Change the GUID to more human readable like: munson-api



Add Scopes: read and write


Grant the web app permissions for the web API

App Registrations -> Munson Web -> API Permissions -> Add a permission

-> APIs my organization uses
-> Munson API (This is the API to which the web app should be granted access)
-> Select Permissions: read and write
-> Add permissions



Status said "Not Granted"











I ended up here:


Copy the scope name:
https://munsonpickles3.onmicrosoft.com/munson-api/read
https://munsonpickles3.onmicrosoft.com/munson-api/write

Setup the API project to use Azure AD B2C. Reference. 

Now fill up appsettings in my API project:

Azure Ad B2C Instance Name. Reference.
Instance: The first part of your Azure B2C tenant domain name combined with b2clogin.com. It should look like mydomain.b2clogin.com.



The appsettings.json looks like this:



Add required packages:
1.Microsoft.Identity.Web
Parses the HTTP authnetiation header, validates the token and extracts claims.

Add it to Program.cs

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

Set it up with steps outlined in the referenced page above. 

Setup the Web App project to use Azure AD B2C. Reference. 

Blazor WASM somewhat updated docs.

 
Configure the appsettings.json. Reference. 
The appsettings.json looks like this:


Add required packages:
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.Identity.Web.UI

Microsoft.Identity.Web sets up the authentication pipeline with cookie based auth.
Takes care of sending and receiving HTTP authentication messages, token validation, claims extraction and more.

Program.cs will need these lines:
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

Set it up with steps outlined in the referenced page above. 

Wrap your Router in App.razor with CascadingAuthenticationState


Add a LoginDisplay.razor component in Shared folder.

Add that to MainLayout.razor


Register required services in Program.cs

    "Instance": "https://munsonpickles3.b2clogin.com/",
    "ClientId": "171b3d8f-8ff1-48b7-a5be-31b0413955ee",
    "CallbackPath": "/signin-oidc",
    "Domain": "munsonpickles3.onmicrosoft.com",
    "SignedOutCallbackPath": "/signout/B2C_1_SignUpSignIn",
    "SignUpSignInPolicyId": "B2C_1_SignUpSignIn",
    "ResetPasswordPolicyId": "",
    "EditProfilePolicyId": ""




The token returned after the login:



Calling the protected API:
Configure a web app that calls web APIs - Microsoft Entra | Microsoft Learn

https://learn.microsoft.com/en-us/azure/active-directory/develop/scenario-web-app-call-api-app-configuration?tabs=aspnetcore#option-2-call-a-downstream-web-api-other-than-microsoft-graph
