using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;
using MongoDbSample.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Linq;

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
            //MongoDbOpeartions();
            //InsertBatch();
            //InsertUsingBulk();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "My tasks perform here..";
            return View();
        }

        // Added by apurva
        public ActionResult Grid()
        {
            var collection = Db.GetCollection<Student>("student");
            MongoCursor<Student> result = collection.FindAllAs<Student>();
            return View(result);
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
        //insert data in employee..
        [HttpPost]
        public ActionResult Adddata(FormCollection form)
        {
            string result;
            try
            {
                var collectionmployee = Db.GetCollection<Employee>("employee");
                string firstName = string.Empty;
                string lastName = string.Empty;
                string contactNo = string.Empty;
                string address = string.Empty;
                if (!string.IsNullOrEmpty(form.GetValue("FirstName").AttemptedValue))
                {
                    firstName = form.GetValue("FirstName").AttemptedValue;
                }
                if (!string.IsNullOrEmpty(form.GetValue("LastName").AttemptedValue))
                {
                    lastName = form.GetValue("LastName").AttemptedValue;
                }
                if (!string.IsNullOrEmpty(form.GetValue("ContactNo").AttemptedValue))
                {
                    contactNo = form.GetValue("ContactNo").AttemptedValue;
                }
                if (!string.IsNullOrEmpty(form.GetValue("Address").AttemptedValue))
                {
                    address = form.GetValue("Address").AttemptedValue;
                }

                Employee emp1 = new Employee { id = Guid.NewGuid().ToString(), FirstName = firstName, LastName = lastName, ContactNo = contactNo, Address = address };
                collectionmployee.Insert(emp1);
                result = "Success";
            }
            catch (Exception)
            {

                throw new Exception();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetEmployeeData()
        {
            List<Employee> objEmployees = new List<Employee>();
            try
            {
                var collectionmployee = Db.GetCollection<Employee>("employee");
                var employee = collectionmployee.FindAll();
                objEmployees.AddRange(employee);
            }
            catch (Exception)
            {

                throw new Exception();
            }
            return Json(objEmployees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentGrid()
        {
            try
            {
                var studentobj = Db.GetCollection<Student>("student");

                //insert
                Student student1 = new Student
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Rashmi Mehta",
                    Grade = "11",
                    Address = "Madri Road",
                    City = "Kolapur",
                    Phone = "9945399999"
                };
                studentobj.Insert(student1);

                Student student2 = new Student
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Shruti Purohit",
                    Address = "Sector-11",
                    City = "Udaipur",
                    Phone = "9945787433"
                };
                studentobj.Insert(student2);

                Student student3 = new Student
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Misha Collins",
                    Grade = "8",
                    Address = "11th block",
                    City = "Noida",
                    Phone = "9887584686"
                };
                studentobj.Insert(student3);

                Student student4 = new Student
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Dean Williams",
                    Grade = "7",
                    Address = "10th Street",
                    City = "Bhagalpur"
                };
                studentobj.Insert(student4);

                var id = student2.Id;

                //find
                var query = Query<Student>.EQ(e => e.Id, id);
                Student stu = studentobj.FindOne(query);

                //save
                stu.Address = "Madri Road, Udaipur, Rajasthan";
                studentobj.Save(stu);

                id = student1.Id;
                //find
                query = Query<Student>.EQ(e => e.Id, id);

                //update
                var update = Update<Student>.Set(e => e.Address, "Sector-13");
                studentobj.Update(query, update);

                //remove
                //studentobj.Remove(query);

                // drop collection
                //studentobj.Drop();
            }
            catch (Exception)
            {
                {
                }
                throw;
            }
            return RedirectToAction("Grid", "Home");
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

        public ActionResult DeleteStudent(string id)
        {
            var studentobj = Db.GetCollection<Student>("student");
            var query = Query<Student>.EQ(e => e.Id, id);
            studentobj.Remove(query);
            return RedirectToAction("Grid","Home");
        }
    }
}
