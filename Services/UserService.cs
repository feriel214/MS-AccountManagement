namespace WebApi.Services;

using BCrypt.Net;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Text.RegularExpressions;
using WebApi.Validators;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
    AuthenticateResponse Register(User model);
}

public class UserService : IUserService
{

    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private readonly ApplicationDbContext _context;
    private readonly Validators.EmailValidator _emailValidator = new EmailValidator();
    public UserService(
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        ApplicationDbContext context
        )
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;

    }



    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);





        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))

            throw new AppException("Username or password is incorrect");

        // authentication successful so generate jwt token
        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, jwtToken);
    }



    public AuthenticateResponse Register(User model)
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

        model.Role = Role.User;
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



        // Generate a JWT token for the registered user
        var jwtToken = _jwtUtils.GenerateJwtToken(user);




        return new AuthenticateResponse(user, jwtToken);
    }


    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }


    public bool VerifyUsername(string username)
    {
        if (username.Length > 8 && !username.All(char.IsDigit))
        {
            return true; // Username is valid
        }



        return false; // Username is invalid
    }

}









 

