﻿using System.Web.Configuration;
using System.Web.Mvc;
using MongoDbSample.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoDbSample.Controllers
{
    public class HomeController : Controller
    {
        public MongoDatabase Db;

        readonly string _uri = WebConfigurationManager.ConnectionStrings["MongoDbUri"].ConnectionString;

        public HomeController()
        {
            //const string uri = "mongodb://admin:admin@ds031277.mongolab.com:31277/sampledb";
            MongoUrl url = new MongoUrl(_uri);
            MongoClient client = new MongoClient(url);
            MongoServer server = client.GetServer();
            Db = server.GetDatabase("sampledb");
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            MongoDbOpeartions();
            InsertBatch();
            InsertUsingBulk();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "My tasks perform here..";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public void MongoDbOpeartions()
        {


            var collectionCustomer = Db.GetCollection<Customer>("customer");

            //BsonDocument[] seedData = CreateSeedData();
            //customer.InsertBatch(seedData);

            //insert
            Customer customer1 = new Customer { Name = "Apurva Jain", Address = "Udaipur", Country = "India", Phone = "999999999" };
            collectionCustomer.Insert(customer1);

            Customer customer2 = new Customer { Name = "Ronak Jain", Address = "Rishabhdeo", Country = "India", Phone = "9887594812" };
            collectionCustomer.Insert(customer2);
            var id = customer2.Id;


            //find
            var query = Query<Customer>.EQ(e => e.Id, id);
            Customer cust = collectionCustomer.FindOne(query);

            //save
            cust.Address = "Rishabhdeo, Udaipur, Rajasthan";
            collectionCustomer.Save(cust);

            //update
            var update = Update<Customer>.Set(e => e.Name, "Ronak Kumar Jain");
            collectionCustomer.Update(query, update);

            //remove
            collectionCustomer.Remove(query);
        }

        public void InsertBatch()
        {
            var collectionCustomer = Db.GetCollection("customer");
            BsonDocument subBson1 = new BsonDocument
            {
                {"model", "14Q3"},
                {"manufacturer", "XYZ Company"}
            };

            BsonDocument subBson2 = new BsonDocument
            {
                {"size", "S"},
                {"qty", "25"}
            };

            BsonDocument subBson3 = new BsonDocument
            {
                {"size", "M"},
                {"qty", "50"}
            };

            var documents = new BsonArray { subBson2, subBson3 };

            BsonDocument bson = new BsonDocument
            {
                {"item", "ABC1"},
                {"details", subBson1},
                {"stock", documents},
                {"category", "clothing"}
            };

            BsonDocument[] seedData = { bson, subBson1, subBson2, subBson3 };
            collectionCustomer.InsertBatch(seedData);
            //collectionCustomer.InitializeOrderedBulkOperation()
        }

        public void InsertUsingBulk()
        {
            var collectionCustomer = Db.GetCollection("customer");
            BsonDocument subBson1 = new BsonDocument
            {
                {"model", "14Q3"},
                {"manufacturer", "XYZ Company"}
            };

            BsonDocument subBson2 = new BsonDocument
            {
                {"size", "S"},
                {"qty", "25"}
            };

            BsonDocument subBson3 = new BsonDocument
            {
                {"size", "M"},
                {"qty", "50"}
            };

            var documents = new BsonArray { subBson2, subBson3 };

            BsonDocument bson = new BsonDocument
            {
                {"item", "ABC1"},
                {"details", subBson1},
                {"stock", documents},
                {"category", "clothing"}
            };

            BulkWriteOperation bulkWrite = collectionCustomer.InitializeOrderedBulkOperation();
            bulkWrite.Insert(bson);
            bulkWrite.Insert(subBson1);
            bulkWrite.Insert(subBson2);
            bulkWrite.Insert(subBson3);
            bulkWrite.Execute();
        }
    }
}
