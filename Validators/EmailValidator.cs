using System.Text.RegularExpressions;

namespace WebApi.Validators;

public class EmailValidator
{
    private readonly Regex _emailRegex;



    public EmailValidator()
    {
        // Define the email pattern using a regular expression
        _emailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
    }



    public bool ValidateEmail(string email)
    {
        // Check if the email matches the defined pattern
        return _emailRegex.IsMatch(email);
    }
}


