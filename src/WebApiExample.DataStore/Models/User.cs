using System;
using WebApiExample.Common.DataAccess;

namespace WebApiExample.DataStore.Models
{
    public class User : IRootEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
