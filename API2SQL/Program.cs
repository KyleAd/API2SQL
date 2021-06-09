using System;
using API2SQL.Data;
using API2SQL.Data.Dto;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace API2SQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Obtaining API data");
            getAllRequestData();
            //getTestApi();
            Console.WriteLine("Your data has been successfully stored to the Database");
        } //Executable class

        public static void getAllRequestData()
        {
            var client = new RestClient("Insert API URI");
            var request = new RestRequest();
            request.AddHeader("TECHNICIAN_KEY", "OAuth PW");
            request.AddParameter("text/plain", "", ParameterType.RequestBody);
            var response = client.Execute(request);
            Console.WriteLine("API obtained...");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResponse = response.Content;
                AllRequests.Rootobject result = JsonConvert.DeserializeObject<AllRequests.Rootobject>(rawResponse);
                Console.WriteLine("Data Deserialized...");

                using (var db = new TransitionContext())
                {
                    //db.RequestDetails.RemoveRange(db.RequestDetails);

                    Console.WriteLine("Wiping Table Data");
                    db.Database.ExecuteSqlRaw("TRUNCATE TABLE [RequestDetails]"); //Quicker solution than Remove Range
                    Console.WriteLine("Table Data wiped");
                    db.RequestDetails.AddRange(result.Operation.Details);
                    db.SaveChanges();
                    Console.WriteLine("New Table Data stored, Application Finished.");
                } //Utilising EF to save data to the DB
            }
        } //Method that calls and stores API data

        public static void getTestApi()
        {
            var client = new RestClient("http://dummy.restapiexample.com/api/v1");
            var request = new RestRequest("employees");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResponse = response.Content;
            }
        } //Simple Test API 
    }
}
