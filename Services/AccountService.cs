using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Validators;
using BCryptNet = BCrypt.Net.BCrypt;
namespace WebApi.Services
{
    
    public interface IAccountService
    {
        // Get a specific user account by its ID
        User GetAccount(int id);

        // Add a new user account and return authentication response
        User AddAccount(User model);

        // Edit an existing user account and return authentication response
        User EditAccount(int id, User model);

        // Delete a user account by its ID
        User DeleteAccount(int id);

        // Edit the role of a user account and return authentication response
        User EditAccountRole(int id, string newRole);
        IEnumerable<User> GetAllAccounts();
    }

    public class AccountService : IAccountService
    {

        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly Validators.EmailValidator _emailValidator = new EmailValidator();
        public AccountService(
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings,
            ApplicationDbContext context
            )
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;

        }

        User IAccountService.GetAccount(int id)
        {
            throw new NotImplementedException();
        }

        public User AddAccount(User model)
        {
            // Check if the username already exists
            if (_context.Users.Any(x => x.Username == model.Username))
                //throw new AppException("Username is already taken ");
                //return new AppException("Username is already taken ");
                if (_emailValidator.ValidateEmail(model.Email))
                {
                    throw new AppException("Email is not valid ");
                }
            if (VerifyUsername(model.Username))
            {
                throw new AppException("Username not valid ");
            }
            // Create a new user
            var user = new User
            {
                Username = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                Role = model.Role
            };



            // Hash the user's password
            user.PasswordHash = BCryptNet.HashPassword(model.PasswordHash);



            // Save the user to the database
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User EditAccount(int id, User model)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);
            // Update the existing user properties
            existingUser.Username = model.Username;
            existingUser.FullName = model.FullName;
            existingUser.Email = model.Email;
            existingUser.PasswordHash = BCryptNet.HashPassword(model.PasswordHash);

            // Save the updated user to the database
            _context.Users.Update(existingUser);
            _context.SaveChanges();

            return existingUser;
        }

        public User DeleteAccount(int id)
        {
            var existingUser = _context.Users.Find(id);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Remove the user from the database
            _context.Users.Remove(existingUser);
            _context.SaveChanges();
            return existingUser;
        }

        public User EditAccountRole(int id, string newRole)
        {
            var existingUser = _context.Users.Find(id);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Update the role of the existing user
            existingUser.Role = newRole == "Admin" ? Role.Admin : Role.User;

            // Save the updated user to the database
            _context.Users.Update(existingUser);
            _context.SaveChanges();

            return existingUser;
        }


        public bool VerifyUsername(string username)
        {
            if (username.Length > 8 && !username.All(char.IsDigit))
            {
                return true; // Username is valid
            }



            return false; // Username is invalid
        }


        public IEnumerable<User> GetAllAccounts()
        {
            return _context.Users;
        }

       

    }





}
