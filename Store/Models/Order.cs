using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Store.Core.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [BsonId]
        public int Id { get; set; }
        [ForeignKey("EmployeeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Employee Employee { get; set; }
        [ForeignKey("CustomerId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Customer Customer { get; set; }
        [ForeignKey("ProductId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Product OrderProduct { get; set; }
        public DateOnly Date { get; set; }
        public int Amount { get; set; }

        public Order(int id, Employee employee, Customer customer, Product product, DateOnly date, int amount)
        {
            Id = id;
            Employee = employee;
            Customer = customer;
            OrderProduct = product;
            Date = date;
            Amount = amount;
        }

        public Order()
        {
            Id = 0;
            Employee = new Employee();
            Customer = new Customer();
            OrderProduct = new Product();
            Date = DateOnly.FromDateTime(DateTime.Now);
            Amount = 0;
        }
    }
}
