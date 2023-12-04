# dotnet-on-azure
Trying out .NET in Azure.

## Common Services
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

## Multitenant Apps
Think of music streaming service or photo sharing service.

## Understanding Tenants and Subscriptions
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
Note: Users live at Tenant/ Directory level.

### Enrollment
- Azure billing and cost management construct for Enterprise Agreement customers. ea.azure.com.
- For large companies.

### Tenant  aka Directory
- Is associated with a single entity, i.e. person, company or org and can own one or several subscriptions.
- In my case, it's a person with domain name: affableashkoutlook.onmicrosoft.com and Organization Id (tenant Id): `9d7f6902-2a61-4363-964c-c464b9eaf716`(Found by going to Settings -> Directories + subscriptions OR Menu -> Azure Active Directory).
- Contains user accounts and groups.
- EVERY TENANT IS LINKED TO A SINGLE AZURE AD INSTANCE which is shared with all tenant's subscriptions.
- Each tenant is referred to as an organization.
- I can create multiple tenants after logging in to Azure Portal.
- Directory Id is TenantId because of one to one relationship between tenant and Azure AD.
- The reason it's called directory is because each directory has an Azure AD service associated with it.
- Every MSFT service is always associated with an Azure AD even if we're not using Azure.
- For eg: If I'm using O365, I'll have Azure AD at the top of it.

<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/019c6117-c1c5-4bde-9da0-e4d022ba2b7c">

### Subscriptions
- Construct for creating separate billing and management boundaries. Is managed in portal.azure.com.
- An agreement with MSFT to use one or more MSFT cloud platforms or services for which charges accrue based on either:
   ○ Per user license fee. For eg: Saas like Office 365 or Dynamics 365.
   ○ Cloud based resource consumption. For eg: Paas and IaaS.
- A subscription is linked to a payment setup and each subscription will result in separate bill.
- Subscription could be CSP (Cloud Service Provider), Pay-As-You-Go, EA etc.
- For customers like Ashish Khanal who can use credit card and do Pay as you go.
- Can create multiple subscriptions in Azure account to create separation.
- A subscription can only be associated with a single Azure AD tenant at any given time.
- Can be linked to existing identity stores for single sign on, or segregated into a separate area.
- Becomes the major separation for assignment of RBAC within services.
- Inside every subscription we can add Resources like VM, SqlDb etc.
- Tenant or Directory has 1:M relationship with Subscription.
  <img src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/1fb07d42-6b49-479d-8dff-706892fb5500" width="450">

### Resource Groups
- A container that holds related resources.
- Like App Service, Sql Db etc.
- Resources in 1 RG are completely isolated from resources in another RG.

References:  
1. [Subscriptions, licenses, accounts, and tenants](https://learn.microsoft.com/en-us/microsoft-365/enterprise/subscriptions-licenses-accounts-and-tenants-for-microsoft-cloud-offerings?view=o365-worldwide)
2. [Tenants and Subscriptions](https://azure-training.com/2022/02/28/understanding-tenants-and-subscriptions-in-azure/)
3. [Difference between Tenant and Subscription](https://stackoverflow.com/a/61702511/8644294)

## View TenandId and Subscription in Azure Portal
### Open account in Azure
Go to portal.azure.com. It's pretty self-explanatory.

### Setup cloud shell
#### Install homebrew
Follow instructions [here](https://brew.sh/).

#### Install Azure CLI for macOS using homebrew
Follow instructions [here](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-macos#install-with-homebrew).
```
sudo chown -R $(whoami) /usr/local/var/homebrew
sudo chown -R $(whoami) /usr/local/opt
chmod u+w /usr/local/opt
brew update && brew install azure-cli
```
 
#### Login to Azure using cloud shell
````
az login
````
<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/82aadcff-b17b-4f72-8e6b-2def70a6e555">

#### Register `Microsoft.CloudShell` namespace to your subscription
Why?

Cloud Shell needs access to manage resources.
Access is provided through namespace that must be registered to your subscription.

Get your subscription Id using
```
az account list (Grab Id)
````
Then
````
az account set --subscription <Subscription Name or Id>
az provider register --namespace Microsoft.CloudShell
````

### View tenantId
<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/a91baa41-f750-4659-a695-c327e4376497">

Note: `tenantId` is my DirectoryId and `id` is my SubscriptionID.

### View Subscription
Search for Subscription from the search bar:

<img width="700" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/9e74b73c-ab1f-4984-b395-886c16f5d583">

### View Subscription costs
You can see cost of your services inside the Subscription. Click on the Azure subscription 1 shown above.

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/d1ecaf88-2803-4d7a-91f6-4db425fd3559">

## Create apps to deploy to Azure
Create a web app (`MunsonPickles.Web`) and an API (`MunsonPickles.API`). Take a look at the code to see how they look.

Note about Blazor Web App in .NET 8:

With .NET 8 comes a new template for Blazor applications simply called `Blazor Web App` and by default all components use Server-Side-Rendering.
To add interactivity to it, you need to add following service and middleware. More info [here](https://chrissainty.com/blazor-in-dotnet-8-server-side-and-streaming-rendering/).

```
builder.Services.AddRazorComponents() // 👈 Adds services required to server-side render components. Added by default.
	.AddServerComponents(); // 👈 Stuff I added for Server Side Interactivity

app.MapRazorComponents<App>() // 👈 Discovers routable components and sets them  up as endpoints. Added by default.
	.AddServerRenderMode();// 👈 Stuff I added for Server Side Interactivity
```

Add interactivity to the new Blazor Web App in .NET 8 using [this guide](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0).

### Db Seed code in API
Line 13 will create the tables, and line 14 will seed the database.

<img width="800" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/2d2cdf2f-ebf0-4fef-8c03-f4f8b33b5333">

Explicit migration is not required in the above approach that looks like
````
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database update
````

So when line `db.Database.EnsureCreated()` runs, it'll create the database and the next line will initialize the database.

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/e170506f-1fb0-49f4-9f54-5bceba2a6f77">

## Create resource group
````
ashish@Azure:~$ az group create -g rg-pitstop-eastus-dev-001 -l eastus
````
👇
`-g` is for resource group name, `-l` is for location (Remember RALEIgh).

So the convention that I'll be using is:
````
ResourceGroup-AppName-Location-Environment-Instance
````

## Create Azure App Service
app-APPNAME-WEB(Because it's a web app)-LOCATION-ENV-INSTANCE

<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/e9f06473-84b1-4c08-b46b-e5e6f9db36b2">

Web app runs on **App Service Plan** which determines CPU and memory of it.

You can name it like this:
````
asp-APPNAME-LOCATION-ENV-INSTANCE
````
asp (App Service Plan)

Notice it doesn't have type like web, api etc. after APPNAME. It's because I want to put both the web app and the web api in that app service plan.

## Database
### Create Database server
**Db Server:** sqlserver-munson-eastus-dev-001

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/727db0cf-e869-46c0-809d-f21122d699ee">

Allow connections to this SQL server from your IP.  
They appear under Firewall rules. Only do this for dev scenarios, NOT for PROD. 

<img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/92042627-6f46-4e6c-a0d0-1c59e8831f0b">

And notice that's my IP address:  
<img width="400" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/68a8b79d-6787-40d6-894b-5ea09714075a">

### Create Database
**Db Name:** sqldb-munson-eastus-dev-001

Grab connection string (ADO.NET SQL auth):  
`Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Persist Security Info=False;User ID=munson;Password={your_password};`

### Connect to Database using config (Go to the next section to see a better way of doing this)
Add this conncection string to `dotnet-secrets`.

1. Go into the project folder and init (`dotnet user-secrets init`).
2. This will appear in `.csproj` file.
   
   <img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/a4e82a9a-0cca-4279-962f-4491810ab5f1">

Set the connection string with this command:
````
dotnet user-secrets set ConnectionStrings:Default "Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Persist Security Info=False;User ID=munson;Password={your_password};"
````

This will set the connection string like this in the secrets.json:

<img width="200" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/7c06cfb3-4502-4060-afed-9a13b0f441ce">

It actually looks like this (after installing this [plugin](https://plugins.jetbrains.com/plugin/10183--net-core-user-secrets)):

<img width="400" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/b7374a63-8e5b-4eb0-92ed-71e81a4e49a9">

This is where that file is stored. [Reference](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=linux).

<img width="500" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/4e337b64-6ae2-4967-ab06-0e087f486e06">

### Connect to Database without password (my preferred way, a better way)
[Reference](https://learn.microsoft.com/en-us/azure/azure-sql/database/azure-sql-dotnet-entity-framework-core-quickstart?view=azuresql&tabs=visual-studio%2Cservice-connector%2Cportal#add-the-code-to-connect-to-azure-sql-database)

Grab the connection string
`Server=tcp:sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";`

The passwordless connection string includes a configuration value of `Authentication=Active Directory Default`, which enables Entity Framework Core to use `DefaultAzureCredential` to connect to Azure services. When the app runs locally, it authenticates with the user you're signed into Visual Studio with. Once the app deploys to Azure, the same code discovers and applies the **managed identity** that is associated with the hosted app, which you'll configure later.

At this point you need to be logged into Azure using Azure CLI (`az login`), if you are not logged in, you'll get this exception if you try to run the app:

<img width="800" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/8b20fb3a-16eb-4a4f-ae1d-2b98bc98166e">

Azure CLI login is shows [here](#### Login to Azure using cloud shell).

**So far:**

Resource Group: `rg-munson-eastus-dev-001`
App Service: `app-munson-web-eastus-dev-001`
App Service Plan: `asp-munson-eastus-dev-001`
Db Server: `sqlserver-munson-eastus-dev-001`
Db Name: `sqldb-munson-eastus-dev-001`

## Managed Identity
A managed identity from Azure Active Directory (Azure AD) allows App Service to access resources through role-based access control (RBAC), without requiring app credentials. After assigning a managed identity to your web app, Azure takes care of the creation and distribution of a certificate. People don't have to worry about managing secrets or app credentials.

This is secret-less way of doing this, that's why I love it. For eg: No credentials in the connection string.

Any service that supports managed identity (B in the following image) can be securely accessed.

<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/6b13f2d9-08c4-4527-a09c-4d2fbb07994e">

Internally, managed identities are service principals of a special type which are locked to only be used with Azure resources. 

References
1. [Microsoft Learn](https://learn.microsoft.com/en-us/azure/app-service/scenario-secure-app-access-storage?tabs=azure-portal)
2. [Stackoverflow](https://stackoverflow.com/questions/61322079/difference-between-service-principal-and-managed-identities-in-azure)

### Service Principal vs Managed Identity in Azure
<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/52891608-dfcb-49c0-b7d4-70389946f38a">

### User Principal vs Service Principal in Azure
[Reference](https://youtu.be/RLnQqJY7Hss?si=2xGIlR0XHsukXbgY)

<p align="left">
  <img width="400" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/29a2fabe-9ac5-4e52-9cee-26f083c0ebdd">
&nbsp; &nbsp; &nbsp; &nbsp;
  <img width="387" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/217d6c13-cdb8-4b06-ab2d-decedf292ff5">
</p>

While I'm interacting with my Azure resources, I also talk to my AD to get my token and make requests. Look example here:

<img width="950" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/3edd26ca-1a60-4e4b-84c4-bedc3a8d8ab7">

## Deploy web app to Azure
### Install Azure toolkit for Rider
Go to Plugins and install [Azure Toolkit for Rider](https://plugins.jetbrains.com/plugin/11220-azure-toolkit-for-rider).

### Sign into Azure toolkit
Go to Tools -> Azure -> Azure Sign In

<img width="500" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/f924cf07-fc32-4b6c-9d44-4802e4ee759c">

Go with Device Login

<img width="500" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/6aae3f62-9011-4093-8dcf-f7443df7203c">

Select my Subscription

<img width="400" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/e9e2249c-e612-4d49-aea6-bc5c88d24210">

### Publish to Azure
Right click on Project -> Publish -> Azure

<img width="200" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/bae42276-638e-47fb-a258-08579fa70acb">

Select 'Use Existing Web App' and click on the app shown below, like so:

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/64f726fc-0f6b-4815-a242-3c9698395b63">

Click Apply -> Run

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/1ed99a0b-fe1a-49ae-8881-fbf95be194ea">

To publish it again, click configuration dropdown in the top right:

<img width="450" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/6d3b33bb-6685-4817-aee7-c9c73cde3f42">

At this point, the app doesn't work correctly on Azure. You still need to configure the secure connection between the App Service and the SQL database to retrieve your data. [Read it all about it here](https://learn.microsoft.com/en-us/azure/azure-sql/database/azure-sql-dotnet-entity-framework-core-quickstart?view=azuresql&tabs=dotnet-cli%2Cazure-portal%2Cportal#connect-the-app-service-to-azure-sql-database).

<img width="700" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/e82e4683-eefa-4186-a31d-903b3df6c77b">

### Connect App service instance to Azure SQL database
The following steps are required to connect the App Service instance to Azure SQL Database:
1. Create a managed identity for the App Service. The `Microsoft.Data.SqlClient` library included in your app will automatically discover the managed identity, just like it discovered your local machine Azure User.
2. Create a SQL database user and associate it with the App Service managed identity.
3. Assign SQL roles to the database user that allow for read, write, and potentially other permissions.

Use **Service connector** to accomplish this:  
Service Connector is a tool that streamlines authenticated connections between different services in Azure. Service Connector currently supports connecting an App Service to a SQL database via the Azure CLI using the `az webapp connection create sql` command. This single command completes the three steps mentioned above for you.

Go to Azure Portal and into the app service. You can see that it doesn't have anything under Identity -> System assigned

<img width="550" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/c8b94fc3-2072-4de2-8d1b-d71f1048d1b4">

Now run this command:  
<img width="450" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/6157bb24-c3f7-404c-99e0-51a6e775f037">


which translates to (run it in Cloud Shell or Azure CLI):
````
az webapp connection create sql -g rg-sampleapp-eastus-dev-001 -n app-munson-web2-eastus-dev-001 --tg rg-sampleapp-eastus-dev-001 --server sqlserver-munson1-eastus-dev-001 --database sqldb-munson-eastus-dev-001 --system-identity --connection ThisCanBeAnything --client-type dotnet
````
At this point, you'll have managed Identity showing:

<img width="550" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/e24c0a25-5bda-4eed-a2ea-084936d67b78">

This connection string created by the above command will show up inside Configuration:

<img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/f532452e-3dfd-441d-a5f2-601df599da80">

`Data Source=sqlserver-munson1-eastus-dev-001.database.windows.net,1433;Initial Catalog=sqldb-munson-eastus-dev-001;Authentication=ActiveDirectoryManagedIdentity`

The user should show up in the SQL Db as well

<img width="900" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/f0996ebb-9008-42bc-9f5b-4c0822e60d5f">

### Troubleshooting app startup
Now go to the app url to see your app running.

Unfortunately, it didn't start. :(

<img width="450" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/cde49004-fc7b-4eb9-86db-6b639225f2ac">


Go to App Service -> Diagnose and solve problems -> Availability and Performance

<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/b1a42b4a-58bd-4703-a3b9-de1cf9ccc08a">

Container crash ->

<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/d764391b-1a42-435e-8de9-6be3b16df454">

**UPDATE:** Running the command again solved the issue for me:

<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/6be023dc-be0a-488e-9ef3-ecc087a8271d">

Now the app runs from Azure! 🎉

<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/7cf31c4b-433d-49cb-8d7a-37256739be7d">

Keep in mind that when you deploy a web app to Azure, [it treats it as Production](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0#azure-app-service). 'Production' is default if DOTNET_ENVIORNMENT and ASPNETCORE_ENVIRONMENT is not set. [Reference](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0).

Environment values set in `launchSettings.json` override values set in the system environment. That file is only used on the local dev machine.

## Using Blob Storage
A general purpose v2 storage account provides access to all of the Azure Storage Services: blobs, files, queues, table and disks.

Blobs in Azure storage are organized into containers.  
Before we can upload a blob, we must first create a container.

A binary large object (blob) is a collection of binary data stored as a single entity. Blobs are typically images, audio or other multimedia objects, though sometimes binary executable code is stored as a blob.

### Create a storage account
<img width="450" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/30d4e933-26d7-4b1d-a040-861203730256">

### Add a container to put images of my web app
For eg: I gave 'web' as a name of my folder.

<img width="500" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/de1c2ed8-be64-4f18-a36e-5b4254bf00e3">

The access level of this container is private by default. To change this to public, go to Configuration -> Allow Blob anonymous access -> Enabled -> Save

<img width="650" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/611d72b5-1228-4fd2-9746-db8fc7ec0aa5">

Now change access level of this container: -> Blob

<img width="450" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/5c57644c-35d9-4ca6-96ff-acf0c8ce1bd6">

### Grant web app access to storage account
We need to grant our web app access to the storage account before we can create, read or delete blobs.

Using Azure RBAC, we can give the managed Identity of the web app access to another Azure resource just like any security principal (User Principal or Service Principal explained earlier in this page).

The 'Storage Blob Contributor' role gives the web app (represented by system assigned managed identity) read, write, and delete access to the blob container and data.

Go to my storage account to grant my web app access to it.

Go to IAM -> Role Assignments
This shows who has access to this resource. There's ME!

<img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/ad7ec846-bda5-4d70-adc5-70cee884716e">

Let's add role assignment to a robot 🤖 (Managed Identity)

Select Add -> Add role assignment

Search for 'Storage block data contributor' role

<img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/13fea670-12b6-45db-9773-1651317d773e">

Click Next to Select who needs this role

Managed Identity -> Select Members -> Subscription -> App Service (Web App) -> 

The managed Identity shows up.

<img width="900" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/fdc35ea1-74fc-4db7-a356-7277991eb922">

Select it and hit Next. 

<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/5f951241-dfd1-4572-a53a-24a912d34e55">

Hit 'Review + assign'.

The IAM page looks like this after the assignment:

<img width="950" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/5a6c02d4-6607-43b9-aeac-dcfffd40508d">

Now go ahead and upload images to 'web' container using Azure portal. It's a simple file upload from your computer. I uploaded few images of pickles and preserves. 😃

### Put CDN on top of blob storage
The thing is these images are only available in the eat US. If I try to hit the blob url from Asia, it'll have to make bunch of internet hops to get to it.
So what we can do is put a CDN on top of our blob storage.

CDN lives on the Azure edge.

Go to CDN -> 

Give Profile name, Endpoint name and specify Query string caching behavior. 

Hit create:

<img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/05a1743b-e151-41ea-b586-fa92da8a7c92">

Go to CDN endpoint now

Grab the endpoint hostname that's served through CDN.
Origin hostname is being served through the storage living in eastus.

Notice the urls.

<img width="900" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/c4fd5d9b-5535-46a7-8312-63acc54b3fba">

Now grab the endpoint hostname + web + filename and update the db:

<img width="900" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/6b2aedeb-402b-4228-8050-041b7b5641e4">

Update the code to show product photo in a "col" class.

<img width="800" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/95a6485b-c99d-4d14-8153-b39655a935df">

Now the page looks like this:

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/2094e5b2-c78d-40c3-97fd-ea8f2140a061">

Explanation on Query string caching behavior options:  
1. **Ignore Query String:** The first request is served from the origin server and the response is cached. Subsequent requests are served from the cache whatever the query string is. This sounds ludicrous!
   ```
   Request1:
   Browser (mydomain.com/articles?page=3) -> Azure CDN -> Server (mydomain.com/articles?page=3)
   Request2:
   Browser (mydomain.com/articles?page=42) -> Azure CDN (from cached whatever the query string)
   ```
2. **Bypass caching for query string:** Azure CDN doesn't cache the requests that have a query string.
   ```
   Request1:
   Browser (mydomain.com/articles?page=3) -> Azure CDN -> Server (mydomain.com/articles?page=3)
   Request2:
   Browser (mydomain.com/articles?page=3) -> Azure CDN -> Server (mydomain.com/articles?page=3)
   ```
3. **Use query string:** Each request with a unique url including the query string is treated as a unique asset with its own cache.
   ```
   Request1:
   Browser (www.example.ashx?q=test1) -> Azure CDN -> Server (www.example.ashx?q=test1)
   Request2:
   Browser (www.example.ashx?q=test1) -> Azure CDN (from cache)
   ```

The order of the query string parameters doesn't matter. For example, if the Azure CDN environment includes a cached response for the URL `www.example.ashx?q=test1&r=test2`, then a request for `www.example.ashx?r=test2&q=test1` is also served from the cache.

### Upload images using APIs
Now we want to give users the ability to upload images while giving a review of a product.

For this we need Azure SDKs.

Go to Dependencies -> Manage NuGet Packages and add these packages to the project:
1. `Azure.Storage.Blobs` : To work with Blob storage.
2. `Microsoft.Extensions.Azure` : Azure client SDK integration with `Microsoft.Extensions` libraries.  
   For eg: To get this line to work:  
   <img width="275" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/343177b9-4801-4af8-ae24-e19a4cb5b384">

To setup connection to Blob. This [article](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/?tabs=command-line) helped.

**IMPORTANT:** (This wasted few hours and caused a lot of frustration)  
Your account needs to have Role Assignment to upload files even though I'm the owner.

<img width="850" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/c38e272b-4a32-41ad-8bad-a0e8310a20b4">

Your account comes into the picture from `DefaultAzureCredential` used to setup the `BlobServiceClient` during local development.

Also as you can see in the screenshot above, Azure App Service (Web app) already has access to it through Managed Identity when it runs in the cloud.

<img width="750" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/42020bbb-3014-4dbd-b496-caa460629554">

Take a look at the code to see how I implemented file upload using minimal APIs. It's pretty nice!

## Add Auth
Everything about adding auth to the app is documented [here](https://github.com/affableashish/blazor-api-aadb2c).

## Deploy .NET Apps to Containers
Benefits of containerizing an app:
1. All components in a single package.
2. Assured new instances the same.
3. Quickly spin up new instances.
4. Instances can be deployed in many places.
5. Helps with agile development because you don't have to pull all the services, you can just work with your "micro" service at a time.

Azure container services:
1. Azure Container registry (sort of like Nuget but for images)
2. Azure App Service
3. Azure Functions
4. Azure Container Apps  
   Abstraction over Kubernetes which is really nice!
5. Azure Kubernetes Service (AKS)
6. Azure Container Instances  
   Allows to new up containers in the cloud and run them. Not really great for prod scenarios because for eg: if the container goes down, it goes down, no orchestrator to replace it. 

### Create Azure Container Registry from Azure portal
For eg: `munsonpicklesacr.azurecr.io`

### Create Dockerfiles for both API and Web app
https://github.com/affableashish/dotnet-on-azure/blob/1855e458b8b5bda547a8503609d6d8cbba38096e/MunsonPickles.API.Dockerfile#L1-L22

https://github.com/affableashish/dotnet-on-azure/blob/1855e458b8b5bda547a8503609d6d8cbba38096e/MunsonPickles.Web.Dockerfile#L1-L22

[Reference](https://github.com/affableashish/docker-with-classlib).

### Build docker images from Dockerfile and push it to ACR
You can build it in your local computer and push it to the Azure Container Registry (ACR).
But for this example, I want to use cloud shell to do this.

Notice that you have docker in there already!

<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/294b198c-edd7-4e84-822c-0cc9401ebc23">

**Steps**
1. Clone your repo (with Dockerfile checked in)
   ```
   gh repo clone https://github.com/affableashish/dotnet-on-azure.git
   ```
2. Go to the repo folder
   ```
   cd ./dotnet-on-azure
   ```
3. Build and push the image to your Azure Container Registry
   ```
   az acr build --file MunsonPickles.API.Dockerfile . --image pickles/my-cool-api:0.1.0 --registry munsonpicklesacr.azurecr.io
   ```
   It looks similar to building an image locally
   ```
   docker build -f MunsonPickles.API.Dockerfile . -t pickles/my-cool-api:0.1.0
   ```

### Test running the image locally (by pulling it from acr)
Login to Azure ACR from your computer:
```
az acr login -n munsonpicklesacr
```

Pull the image down:

MunsonPicklesACR registry -> Repositories -> find the image -> copy the 'docker pull command'
```
docker pull munsonpicklesacr.azurecr.io/pickles/my-cool-api:0.1.0
```

Run the image:
```
docker run --rm -it -p 8000:8080 -e ASPNETCORE_ENVIRONMENT=Development munsonpicklesacr.azurecr.io/pickles/my-cool-api:0.1.0
```

### Deploy the container to Azure App Service
Create Web App (Azure App Service)

Name: munson-api-linux-westus  
Publish: Docker Container

...


After the deployment is complete, go to `munson-api-linux-westus` Web App -> Identity (under Settings)

Turn On managed Identity.

Now go to `MunsonPicklesACR` registry, give access to the managed identity you just created so the web app can pull images from this registry.

Go to Accss Control -> Add -> Role Assignment (acr pull) -> Assign access to: managed identity -> choose your app service -> Next -> Review + assign

Now go back to app service to tell it which registry it should go to and which image it should pull.

Go to Deployment Center (under Deployment) -> Settings

Container type: Single container  
Registry source: Azure Container Registry  
Authentication: Managed Identity  
Registry: MunsonPicklesACR  
Image: pickles/api  
Tag: 0.1.0  

-> Save

Restart the web app and take it for a test ride by clicking the url. It should work at this point. 🎉

## Serverless with Azure Functions
1. Cloud provider manages infrastructure
2. Allocates resources as necessary
3. Scales down to zero. Reap this benefit by making your function focus on a specific task, and not do a whole lot so when it's free, it can scale down to zero.
4. Lets you focus on business logic
5. It launches in response to events
6. Integrates with other Azure services

### Scenarios of using Functions
1. Build a web api
2. Process file uploaded to Blob storages
3. Respond to database changes
4. Process message queues
5. Analyze IoT streams
6. Real time data with SignalR

### Azure Functions "hooks"
#### Triggers
- Events that start the function.
- Have incoming data: For eg: let's say a HTTP request that triggered a function. You can get the request body or query string of that request as incoming parameters.
- There are triggers for many different services like:
    - HTTP
    - Timer
    - Storage
    - Data
    - Event Grid
    - etc.

#### Bindings
- Connect to another service like Send Grid if you wanted to send emails.
- Input and Output bindings so you can have data come in or you can be writing data to various services.
- Can have multiple bindings per Azure function. For eg: let's say we had a HTTP request coming in that triggered a function, we could have a binding to table storage that pull out data and we can have another binding to let's say blob storage that pulls out a blob and pulls that into your function and do something with it.

### Example scenario
Whenever some image (picture) is uploaded, it's going to write a message to an Azure storage queue, once that starts it's going to kick off an Azure function queue trigger. The function runs and it's going to use a table output binding just to write some data to table storage.

<img width="600" alt="image" src="https://github.com/affableashish/dotnet-on-azure/assets/30603497/f3abf68a-9d7c-48ad-ae01-f088430d5ae0">

It'll show input binding, trigger and an output table binding.










