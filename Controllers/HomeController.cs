using System;
using System.Collections.Generic;
using System.Linq;
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
            MongoDbOpeartions();
            InsertBatch();
            InsertUsingBulk();

            FindQuery();

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
            Findstudent();
            return View(result);
        }

        public void MongoDbOpeartions()
        {
            var collectionCustomer = Db.GetCollection<Customer>("customer1");

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
                string Id = string.Empty;
                if (!string.IsNullOrEmpty(form.GetValue("hiddenid").AttemptedValue))
                {
                    Id = form.GetValue("hiddenid").AttemptedValue;
                }
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

                if (!string.IsNullOrEmpty((Id)))
                {
                    var query = Query<Employee>.EQ(e => e.id , Id);
                    Employee emp = collectionmployee.FindOne(query);
                    var update = Update<Employee>.Set(e => e.FirstName, firstName).Set(e => e.LastName, lastName).Set(e => e.Address, address).Set(e => e.ContactNo, contactNo);
                    collectionmployee.Update(query, update);
                    result = "Success";
                }
                else
                {
                    Employee emp1 = new Employee { id = Guid.NewGuid().ToString(), FirstName = firstName, LastName = lastName, ContactNo = contactNo, Address = address };
                    collectionmployee.Insert(emp1);
                    result = "Success";
                }
               
                

            }
            catch (Exception)
            {

                throw new Exception();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetEmployeeData()
        {
            string sectiondata;
            List<Employee> objEmployees = new List<Employee>();
            try
            {
                var collectionmployee = Db.GetCollection<Employee>("employee");
                var employee = collectionmployee.FindAll();
                objEmployees.AddRange(employee);
                sectiondata = objEmployees.Aggregate("<table border='1' style='width: 100%;border-spacing: 'inherit' id='section_table'><thead><tr><th width='150'>First Name</th><th width='150'>Last Name</th><th width='150'>Contact</th><th width='150'>Address</th></tr></thead><tbody id='section_data'>", (current, r) => current + ("<tr><td>" + r.FirstName + "</td> <td>" + r.LastName + "</td><td> " + r.ContactNo + " </td><td>" + r.Address + "</td><td><a style='cursor:pointer;' onclick=editrecord('" + r.id + "'); >edit</a></td><td><a style='cursor:pointer;' onclick=deleterecord('" + r.id + "'); >delete</a></td></tr>"));
                sectiondata += " </tbody></table>";
            }
            catch (Exception)
            {

                throw new Exception();
            }
            return Json(sectiondata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetEditRecord(string Id)
        {
            string EditData;
            try
            {
                var collectionmployee = Db.GetCollection<Employee>("employee");
                var query = Query<Employee>.EQ(e => e.id , Id);
                Employee emp = collectionmployee.FindOne(query);
                EditData = "<form id='UpdateDataforEmployee' action='post'><div> <label>First Name</label><input type='text' placeholder='Enter FirstName' name='FirstName' id='FirstName' value='" + emp.FirstName + "'/></div><div> <label>Last Name</label><input type='text' placeholder='Enter LastName' name='LastName' id='LastName' value='" + emp.LastName + "'/></div><div> <label>Contact No.</label><input type='text' placeholder='Enter ContactNo' name='ContactNo' id='ContactNo' value='" + emp.ContactNo + "'/></div><div> <label>Address</label><input type='text' placeholder='Enter Address' name='Address' value='" + emp.Address + "' id='Address'/></div><input type='hidden' value='" + emp.id + "' name='hiddenid'><input type='button' name='Update' value='Update' id='Updaterow' onclick='Updatedata();'/></form>";
            }
            catch (Exception)
            {
                throw new Exception();
            }
            return Json(EditData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDeleteRecord(string Id)
        {
            string result;
            try
            {
                var collectionmployee = Db.GetCollection<Employee>("employee");
                var query = Query<Employee>.EQ(e => e.id, Id);
                Employee emp = collectionmployee.FindOne(query);
                collectionmployee.Remove(query);
                result = "Success";
            }
            catch (Exception)
            {
                throw new Exception();
                result = "Fail";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
               
                throw new Exception();
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

        public ActionResult DeleteStudent(string id)
        {
            var studentobj = Db.GetCollection<Student>("student");
            var query = Query<Student>.EQ(e => e.Id, id);
            studentobj.Remove(query);
            return RedirectToAction("Grid","Home");
        }

        public void Findstudent()
        {
            var studentobj = Db.GetCollection<Student>("student");
            var query = Query.Or(Query.EQ(studentobj.Name, "John"), Query.EQ(studentobj.Name, "abc"));
            MongoCursor<Student> cursorOr = studentobj.Find(query);
            foreach (var cur in cursorOr)
            {
                Console.WriteLine(cur);
            }
        }
    }
}
