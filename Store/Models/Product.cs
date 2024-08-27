using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Store.Core.Enums;

namespace Store.Core.Models
{
    public class Product
    {
        [Key]
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ProductCategory Category { get; set; }

        public Product(int id, string name, decimal price, int stock, ProductCategory category)
        {
            Id = id;
            Name = name;
            Price = Math.Floor(price * 100M) * 0.01M;
            Stock = stock;
            Category = category;
        }

        public Product()
        {
            Id = 0;
            Name = string.Empty;
            Price = 0M;
            Stock = 0;
        }
    }
}
