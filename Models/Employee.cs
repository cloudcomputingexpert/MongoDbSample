﻿using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbSample.Models
{
    public class Employee
    {
        [BsonId]
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
    }
}