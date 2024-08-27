using Moq;
using Serilog;
using Store.Core.Contracts;
using Store.Core.Enums;
using Store.Core.Models;
using Store.Core.Services;

namespace Store.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetAllUsers()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            User user1 = new User
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            User user2 = new User
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            User user3 = new User
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            List<User> users = new List<User>();
            users.Add(user1);
            users.Add(user2);
            users.Add(user3);

            _userDbRepository.Setup(x => x.GetAllUsers()).Returns(Task.FromResult(users));
            _userCacheRepository.Setup(x => x.GetAllUsers()).Returns(Task.FromResult(users));
            _userCacheRepository.Setup(x => x.AddUsers(new List<User>()));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            List<User> items = await userService.GetAllUsers();
            //Assert
            Assert.Equal(items, users);
        }

        [Fact]
        public async Task GetAllEmployees()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            List<Employee> employees = new List<Employee>();
            employees.Add(employee1);
            employees.Add(employee2);
            employees.Add(employee3);

            _userDbRepository.Setup(x => x.GetAllEmployees()).Returns(Task.FromResult(employees));
            _userCacheRepository.Setup(x => x.GetAllEmployees()).Returns(Task.FromResult(employees));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            List<Employee> items = await userService.GetAllEmployees();
            //Assert
            Assert.Equal(items, employees);
        }

        [Fact]
        public async Task GetAllCustomers()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Customer customer1 = new Customer
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Customer customer2 = new Customer
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            List<Customer> customers = new List<Customer>();
            customers.Add(customer1);
            customers.Add(customer2);
            customers.Add(customer3);

            _userDbRepository.Setup(x => x.GetAllCustomers()).Returns(Task.FromResult(customers));
            _userCacheRepository.Setup(x => x.GetAllCustomers()).Returns(Task.FromResult(customers));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            List<Customer> items = await userService.GetAllCustomers();
            //Assert
            Assert.Equal(items, customers);
        }

        [Fact]
        public async Task AddNewUser()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            User user = new User
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            _userDbRepository.Setup(x => x.CheckUser(user)).Returns(Task.FromResult(string.Empty));
            _userCacheRepository.Setup(x => x.CheckUser(user)).Returns(Task.FromResult(string.Empty));
            _userDbRepository.Setup(x => x.AddNewUser(user)).Returns(Task.FromResult(user));
            _userCacheRepository.Setup(x => x.AddNewUser(user));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            string result = await userService.AddNewUser(user);
            //Assert
            Assert.Equal(result, string.Empty);
        }

        [Fact]
        public async Task GetEmployeeById()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Employee employee1 = new Employee
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Employee employee2 = new Employee
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Employee employee3 = new Employee
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            List<Employee> employees = new List<Employee>();
            employees.Add(employee1);
            employees.Add(employee2);
            employees.Add(employee3);

            _userDbRepository.Setup(x => x.GetEmployeeById(13)).Returns(Task.FromResult(employee2));
            _userCacheRepository.Setup(x => x.GetEmployeeById(13)).Returns(Task.FromResult(employee2));
            _userCacheRepository.Setup(x => x.AddNewUser(employee2));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            Employee item = await userService.GetEmployeeById(13);
            //Assert
            Assert.Equal(item, employee2);
        }

        [Fact]
        public async Task GetCustomerById()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            Customer customer1 = new Customer
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            Customer customer2 = new Customer
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            Customer customer3 = new Customer
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            List<Customer> customers = new List<Customer>();
            customers.Add(customer1);
            customers.Add(customer2);
            customers.Add(customer3);

            _userDbRepository.Setup(x => x.GetCustomerById(13)).Returns(Task.FromResult(customer2));
            _userCacheRepository.Setup(x => x.GetCustomerById(13)).Returns(Task.FromResult(customer2));
            _userCacheRepository.Setup(x => x.AddNewUser(customer2));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            Customer item = await userService.GetCustomerById(13);
            //Assert
            Assert.Equal(item, customer2);
        }

        [Fact]
        public async Task GetUserByNameAndSurname()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            User user1 = new User
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            User user2 = new User
            {
                Id = 13,
                Name = "Jonas",
                Surname = "Jonaitis",
                Email = "jonas@gmail.com",
                PhoneNumber = "37067654321"
            };

            User user3 = new User
            {
                Id = 9,
                Name = "Lukas",
                Surname = "Lukaitis",
                Email = "lukas@gmail.com",
                PhoneNumber = "37061111111"
            };

            List<User> users = new List<User>();
            users.Add(user3);

            _userDbRepository.Setup(x => x.GetUsersByNameAndSurname("Lukas", "Lukaitis")).Returns(Task.FromResult(users));
            _userCacheRepository.Setup(x => x.GetUsersByNameAndSurname("Lukas", "Lukaitis")).Returns(Task.FromResult(users));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            List<User> items = await userService.GetUserByNameAndSurname("Lukas", "Lukaitis");
            //Assert
            Assert.Equal(items, users);
        }

        [Fact]
        public async Task UpdateUser()
        {
            //Arrange
            Mock<IUserRepository> _userDbRepository = new Mock<IUserRepository>();
            Mock<IUserCacheRepository> _userCacheRepository = new Mock<IUserCacheRepository>();
            Mock<ILogger> _logger = new Mock<ILogger>();

            User user = new User
            {
                Id = 15,
                Name = "Petras",
                Surname = "Petraitis",
                Email = "petras@gmail.com",
                PhoneNumber = "37061234567"
            };

            _userDbRepository.Setup(x => x.CheckUser(user)).Returns(Task.FromResult(string.Empty));
            _userCacheRepository.Setup(x => x.CheckUser(user)).Returns(Task.FromResult(string.Empty));
            _userDbRepository.Setup(x => x.UpdateUser(user));
            _userCacheRepository.Setup(x => x.UpdateUser(user));
            IUserService userService = new UserService(_userDbRepository.Object, _userCacheRepository.Object, _logger.Object);

            //Act
            string result = await userService.UpdateUser(user);
            //Assert
            Assert.Equal(result, string.Empty);
        }
    }
}