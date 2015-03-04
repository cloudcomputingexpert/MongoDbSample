using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

    public class Student
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
    }
}