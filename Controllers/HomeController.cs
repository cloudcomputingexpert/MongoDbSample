using System;
using System.Web.Configuration;
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
            //MongoDbOpeartions();
            InsertBatch();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        static void MongoDbOpeartions()
        {
            string uri = WebConfigurationManager.ConnectionStrings["MongoDbUri"].ConnectionString;
            MongoUrl url = new MongoUrl(uri);
            MongoClient client = new MongoClient(url);
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("sampledb");

            var collectionCustomer = db.GetCollection<Customer>("customer");

            //insert
            Customer customer1 = new Customer { Name = "Apurva Jain", Address = "Udaipur", Country = "India", Phone = "999999999" };
            collectionCustomer.Insert(customer1);
            
            Customer customer2 = new Customer { Name = "Ronak Jain",Address = "Rishabhdeo", Country = "India", Phone = "9887594812"};
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

        public void FindQuery()
        {
            var collectionCustomer = Db.GetCollection("customer");

            //Greater than
            var query = Query.GT("quantity", 30);
            MongoCursor<BsonDocument> cursorGt = collectionCustomer.Find(query);
            foreach (var cur in cursorGt)
            {
                Console.WriteLine(cur);
            }

            //Or operator
            query = Query.Or(Query.EQ("quantity", 50), Query.EQ("quantity", 40));
            MongoCursor<BsonDocument> cursorOr = collectionCustomer.Find(query);
            foreach (var cur in cursorOr)
            {
                Console.WriteLine(cur);
            }

            //And operator
            query = Query.And(Query.EQ("quantity", 25), Query.EQ("size", "S"));
            MongoCursor<BsonDocument> cursorAnd = collectionCustomer.Find(query);
            foreach (var cur in cursorAnd)
            {
                Console.WriteLine(cur);
            }

            //And with Or operator
            query = Query.And(Query.EQ("quantity", 25), Query.Or(Query.EQ("size", "S"), Query.EQ("size", "N")));
            MongoCursor<BsonDocument> cursorAndOr = collectionCustomer.Find(query);
            foreach (var cur in cursorAndOr)
            {
                Console.WriteLine(cur);
            }
        }
    }
}
