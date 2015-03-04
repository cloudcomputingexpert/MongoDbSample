using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;
using MongoDbSample.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Linq;

namespace MongoDbSample.Controllers
{
    public class HomeController : Controller
    {
        public MongoDatabase db;

        string uri = WebConfigurationManager.ConnectionStrings["MongoDbUri"].ConnectionString;

        public HomeController()
        {
            //const string uri = "mongodb://admin:admin@ds031277.mongolab.com:31277/sampledb";
            MongoUrl url = new MongoUrl(uri);
            MongoClient client = new MongoClient(url);
            MongoServer server = client.GetServer();
            db  = server.GetDatabase("sampledb");
        }

        public ActionResult Index()
        {
            //MongoDbOpeartions();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "My tasks perform here..";
            return View();
        }

        public ActionResult Contact()
        {
            IEnumerable<Student> result = StudentGrid();
            return View(result);
        }

        public  void MongoDbOpeartions()
        {
            var collectionCustomer = db.GetCollection<Customer>("customer");

            //BsonDocument[] seedData = CreateSeedData();
            //customer.InsertBatch(seedData);

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

        private IEnumerable<Student> StudentGrid()
        {
            try
            {
                var studentobj = db.GetCollection<Student>("student");

                //insert
                Student student1 = new Student { Name = "Mehul Jain", Grade = "10", Address = "Udaipur", City = "Udaipur", Phone = "999999999" };
                studentobj.Insert(student1);

                Student student2 = new Student { Name = "Charvi Purohit", Address = "Sector-13", City = "Udaipur", Phone = "9887594812" };
                studentobj.Insert(student2);
                var id = student2.Id;

                //find
                var query = Query<Student>.EQ(e => e.Id, id);
                Student stu = studentobj.FindOne(query);

                //save
                stu.Address = "Sector-13, Udaipur, Rajasthan";
                studentobj.Save(stu);

                id = student1.Id;
                //find
                query = Query<Student>.EQ(e => e.Id, id);

                //update
                var update = Update<Student>.Set(e => e.Address, "Bedla");
                studentobj.Update(query, update);

                //remove
                //studentobj.Remove(query);

                var collection = db.GetCollection<Student>("student");
                MongoCursor<Student> result = collection.FindAllAs<Student>();
                return result.ToList();
            }
            catch (Exception)
            {
                { }
                throw;
            }
        }
    }
}
