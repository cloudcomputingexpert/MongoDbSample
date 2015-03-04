using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MongoDbSample.Models
{
    public class Customer
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public String Country { get; set; }
    }
}