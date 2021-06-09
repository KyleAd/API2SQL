# API2SQL
API &amp; SQL Server integration tool.

## Description
The project objective is to establish a communication between ServiceDesk API data and an SQL server. For this example, we are pulling API data from ServiceDesk (ManageEngine) and storing it into an SQL server.

API2SQL -> Program.cs, the program file is the Project executable that will process API calls and save the data to a Server. 

## Tasks
DONE
- Created sufficient GET requests that receive ServiceDesk API data.
- Created TEST GET requests to experiment with API calls.
- Created DTO (Data Transferable Object) files that will store the necessary API data into objects.
- Created successful GET requests that store the API data into the DTO files. (Deserialization working correctly)
- Built a connection string that connects to the database.
- Implemeneted Entity Framework to map DTO files to SQL tables.
- Data can be successfully saved and stored into the SQL server. 
- Is built to be an automated process.

TO DO
- More API files (such as Contracts) to be added. 
- Implement App.Config file correctly. (At the moment the connection string is set in TransitionDbContext.cs)
- API2SQL User Guide.
- Find a way to implement Bulk API data for full Request tickets.
- (Optional) Find a way for the process to not have to delete data but add only new data instead. 
- (Tasks to be updated) 

## Usage
This project can be reused for other web servers rather than just ServiceDesk. There are three main data entities to adhere to which are **Executable**, **DTO** and **Context**. This is a helpful format for extracting API data and storing it into an SQL server or just a LocalDb. If you are starting a new project from scratch, make sure to install the exact same packages this project already has and reference them properly.

Here is a step by step process following the previously discussed entity format. 

### Executable
The executable typically refers to the Program.cs file. This is where your compiler runs, everything that happens in your code is processed through this executable. In this executable file we will be calling a method that calls an API. In this current example, we are using RestSharp to get an API call from ServiceDesk. 

```
var client = new RestClient("Insert API URI");
            var request = new RestRequest();
            request.AddHeader("TECHNICIAN_KEY", "OAuth PW");
            request.AddParameter("text/plain", "", ParameterType.RequestBody);
            var response = client.Execute(request);
```
This simple GET request in RestSharp is pulling all request data from ServiceDesk all while processing the authorization. There are numerous different techniques to use when making API calls, and different web servers will use different authorizations. When building an API call, make sure you follow the Hosts API doc so that you use the correct Authorization so you can successfully pull code. A good tool to test this with is POSTMAN. 

In the Program.cs file you can have as many methods as you want, each method should have its own seperate API call. Make sure to then reference these methods into the static main method which is the master class that is the program compiler. 

Now that an API call is being made, and "var response" successfully holds the raw API data. That data will likely need to be deserialized into a JSON format so that it can be stored into an SQL format. It is best to do this in the same method as the API call. Here is the deserialization of that very API call being made below. 

```
if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResponse = response.Content;
                AllRequests.Rootobject result = JsonConvert.DeserializeObject<AllRequests.Rootobject>(rawResponse);
            }
```
The response status code will validate if the HTTP response is healthy. If it is, then the raw data can be stored and converted. This is done with the assistance of NewtonJson package, that provides a quick conversion technique. The string "rawResponse" which was previously "var response" will now become result. Result will store Json objects of API calls which can now easily be saved into a databse. But in order to do this, we need DTO's (Data Transferable Object) that will store temporarily store data and structure the database, so that it's formatted correctly. As you can see, the result variable is referencing "AllRequests.Rootobject" which is a DTO class file in the project, we will go over this in the next section. 

### DTO
In the field of programming a data transfer object (DTO) is an object that carries data between processes. The motivation for its use is that communication between processes is usually done resorting to remote interfaces (e.g., web services), where each call is an expensive operation.

Here is the AllRequests DTO file that the API call was referencing. 

```
    public class AllRequests
    {
        public class Rootobject
        {
            public Operation Operation { get; set; }
        }

        public class Operation
        {
            public Result Result { get; set; }
            public Detail[] Details { get; set; }
        }

        public class Result
        {
            public string Message { get; set; }
            public string Status { get; set; }
        }

        public class Detail
        {
            public int Id { get; set; }
            public string Requester { get; set; }
            public string WorkOrderId { get; set; }
            public string AccountName { get; set; }
            public string CreatedBy { get; set; }
            public string Subject { get; set; }
            public string Technician { get; set; }
            public string IsOverDue { get; set; }
            public string DueByTime { get; set; }
            public string Priority { get; set; }
            public string CreatedTime { get; set; }
            public string IgnoreRequest { get; set; }
            public string Status { get; set; }
        }
    }
```
This is the typical JSON format for an API. It is the inherited public class Detail that we are interested in. That Detail class is stored as an array in Operation and once again in Root so that it can store blocks of data at once. The Detail class holds all the request data and keys which will be stored in our database. The structure is already set and it can temporarily hold API data until it is saved into a Db. This class is also important for automatically building an SQL database table through Entity Framework. Something we will be going over next in the Context section. 

A handy tip to create a quick DTO class file, is to get an example request (typically on a hosts API doc) then copy the example request, and use special paste into an empty class file. This will automatically create the class for you. To use special paste (in Visual Studio 2019) you go to Edit -> Paste Special -> JSON. 

### Context
Context is implementing an SQL integration. We will use Entity Framework which is a very helpful tool for quickly building databases and integrating them into Visual Studio Applications. 

In the TransitionDbContext.cs file it will provide the link between this application and your SQL database. Here is the current API2SQL context file. 
```
public class TransitionDbContext : DbContext
    {
        private const string connectionString = @"Connection String";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<AllRequests.Detail> RequestDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllRequests.Detail>().HasKey(r => r.Id);
        }
    }
```
First, the connectionString must be set. Each and every Db has a connection string, replace it with the correct format so that it now connects to your database. 

Secondly, we want to initialise the DTO file so that it uses its structure and automatically moulds it into a table. This table will then be synced appropriately to the API code and can comfortably store data. Sometimes it will require more context (for example if your Api data doesn't have an ID key, you will need to implement a modelBuilder as shown below. If errors are showing up, they are easily rectified through the EntityFramework doc. 

Lastly, we now want to run migrations through package manager console. This process will officially build your database and update its contents. This is done through just two simple commands through Package Manager Console. 

First Command, Run Migrations and give it a name (for this example, we will use InitialCreate)
```
add-migration InitialCreate
```

You will now notice a new folder has appeared in your side directory, a migration folder. This stores metadata and other contents that you won't physically alter. 

Now the final command which will build the tables. 
```
update-database
```

Once you have done that, go to SQL object explorer and have a look if your new tables are there. If you make any more changes in the code make sure you run the update-database command again each time you do it. 

But now for the final stage, we have to update our API class in the Executable section. This will now be a quick context reference that will save the pulled API data into the referenced DB we are using. Here is an example for how it is currently used for our ServiceDesk get all requests call. 

```
using (var db = new TransitionDbContext())
                {
                    db.RequestDetails.AddRange(result.Operation.Details);
                    db.SaveChanges();
                } 
```

This should be added at the bottom of your method after the deserialization process. This references the TransitionDbContext object as a "var db". This now enables data to be added and saved to the db as shown above. 
