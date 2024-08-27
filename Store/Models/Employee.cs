using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Store.Core.Models
{
    public class Employee : User
    {
        public Employee(int id, string name, string surname, string email, string phoneNumber) : base(id, name, surname, email, phoneNumber) { }

        public Employee() : base() { }
    }
}
