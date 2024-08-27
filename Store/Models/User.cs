using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Store.Core.Enums;

namespace Store.Core.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [BsonIgnore]
        [NotMapped]
        public bool bIsEmployee { get; set; }

        public User(int id, string name, string surname, string email, string phoneNumber)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = ValidateEmail(email);
            PhoneNumber = ValidatePhoneNumber(phoneNumber);
            bIsEmployee = this is Employee;
        }

        public User()
        {
            Id = 0;
            Name = string.Empty;
            Surname = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            bIsEmployee = this is Employee;
        }

        public string ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return string.Empty;

            phoneNumber = RemoveNonNumeric(phoneNumber);
            if (phoneNumber.Length == 11 && phoneNumber.StartsWith("3706"))
                return "+" + phoneNumber;
            else
                return string.Empty;
        }

        public string RemoveNonNumeric(string phoneNumber)
        {
            return Regex.Replace(phoneNumber, @"[^0-9]+", "");
        }

        public string ValidateEmail(string email)
        {
            if (IsValidEmail(email))
                return email.ToLower();
            else
                return string.Empty;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            return new EmailAddressAttribute().IsValid(email);
        }
    }
}
