using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;
using MongoDbSample.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

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
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            MongoDbOpeartions();
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
        //insert data in employee..
        [HttpPost]
        public ActionResult Adddata(FormCollection form)
        {
            string result = string.Empty;
            try
            {
                var collectionmployee = db.GetCollection<Employee>("employee");
                string FirstName = string.Empty;
                string LastName = string.Empty;
                string ContactNo = string.Empty;
                string Address = string.Empty;
                if (!string.IsNullOrEmpty(form.GetValue("FirstName").AttemptedValue))
                {
                    FirstName = form.GetValue("FirstName").AttemptedValue;
                }
                if (!string.IsNullOrEmpty(form.GetValue("LastName").AttemptedValue))
                {
                    LastName = form.GetValue("LastName").AttemptedValue;
                }
                if (!string.IsNullOrEmpty(form.GetValue("ContactNo").AttemptedValue))
                {
                    ContactNo = form.GetValue("ContactNo").AttemptedValue;
                }
                if (!string.IsNullOrEmpty(form.GetValue("Address").AttemptedValue))
                {
                    Address = form.GetValue("Address").AttemptedValue;
                }
                
                Employee emp1 = new Employee {id=Guid.NewGuid().ToString(), FirstName = FirstName, LastName = LastName, ContactNo = ContactNo,Address = Address };
                collectionmployee.Insert(emp1);
                result = "Success";
            }
            catch (Exception)
            {

                throw new Exception();
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeData()
        {
            List<Employee> objEmployees = new List<Employee>();
             try
            {
                var collectionmployee = db.GetCollection<Employee>("employee");
                var employee = collectionmployee.FindAll();
                foreach (var r in employee)
                {
                    objEmployees.Add(r);
                }
            }
             catch (Exception)
             {

                 throw new Exception();
             }
             return Json(objEmployees, JsonRequestBehavior.AllowGet);
        }
    }
}
