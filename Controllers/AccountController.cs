using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Services;
using WebApi.Authorization;
using WebApi.Models.Users;
using Microsoft.VisualBasic;


    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }



        //POST : Add New Account 
        [AllowAnonymous]
        [Authorize(Role.Admin)]
        [HttpPost("[action]")]
        public IActionResult AddAccount([FromBody] User model)

            {
                try
                {
                   var response = _accountService.AddAccount(model);
                    return StatusCode(StatusCodes.Status200OK,response);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);

                }

            }



        //PUT : Edit  Account 
        [AllowAnonymous]
        [Authorize(Role.Admin)]
        [HttpPut("{id}")]
    public IActionResult EditAccount([FromBody] User model,int id)
        {
            try
            {
              
                var response = _accountService.EditAccount(id,model);
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        //Delete : Delete Account 
        [AllowAnonymous]
        [Authorize(Role.Admin)]
        [HttpDelete("DeleteAccount/{id}")]
        public IActionResult DeleteAccount(int id)
        {
            try
            {
                var response = _accountService.DeleteAccount(id);
                if (response!=null)
                {
                    return StatusCode(StatusCodes.Status200OK, "Account deleted successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Account not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    [AllowAnonymous]
    [Authorize(Role.Admin)]
    [HttpPost("EditRole/{id}")]
    public IActionResult EditAccountRole(int id ,[FromBody] User model)
    {
        try
        {
            var response = _accountService.EditAccountRole(id,model.Role.ToString());
            if (response != null)
            {
                return StatusCode(StatusCodes.Status200OK, "Account role updated successfully.");
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "Account not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    // Get all accounts
    [Authorize(Role.Admin)]
    [HttpGet("getAllAccounts")]
    public IActionResult GetAll()
    {
        var users = _accountService.GetAllAccounts();
        return Ok(users);
    }



}

