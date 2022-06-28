using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Highstreetly.Ids.Models;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Permissions.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Ids.Controllers
{
    [Authorize]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly EmailTemplateOptions _options;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            EmailTemplateOptions options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _options = options;
        }

        [TempData] private string ErrorMessage { get; set; }


        // [HttpPost("magic")]
        // [AllowAnonymous]
        // public async Task<IActionResult> MagicPost(string email = null, string returnUrl = null, string clientId = null, string scopes = null)
        // {
        //     // Clear the existing external cookie to ensure a clean login process
        //     await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        //
        //     var user = await _userManager.FindByEmailAsync(email);
        //
        //     // NOTE: currently not doing this since magic links work for both dash and orders 
        //     // TODO: make it so we switch on the client ID and behave accordingly
        //     // check to see if we have at least one ticket for this user -
        //     // if we do send the link
        //     // if we don't send an email saying we don't have any tickets for this user
        //
        //     // var ordersForEmail = await _draftOrderClient.GetAsync(new Dictionary<string, string>{
        //     //     { "owner-email", user.Email }
        //     // });
        //
        //     // if (ordersForEmail.Count() < 1)
        //     // {
        //     //     _logger.LogInformation($"Sending no orders email for {user.Email}");
        //     //     // send no tickets email
        //     //     await _emailSender.SendNoTicketsHereEmail(user.Email, _options);
        //     //     return Ok();
        //     // }
        //
        //     _logger.LogInformation($"Generating magic token for {user.Email}");
        //     var token = await _userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth");
        //     _logger.LogInformation($"Token generated {token}");
        //
        //     await _emailSender.SendMagicLinkAsync(user.Email, token, returnUrl, clientId, scopes, _options, _logger);
        //
        //     return Ok();
        // }

        // [HttpGet("magic")]
        // [AllowAnonymous]
        // public async Task<IActionResult> MagicGet(string token = null, string email = null, string returnUrl = null, string clientId = null, string scopes = null)
        // {
        //     // Clear the existing external cookie to ensure a clean login process
        //     await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        //
        //     var user = await _userManager.FindByEmailAsync(email);
        //
        //     var isValid = await _userManager.VerifyUserTokenAsync(user, "Default", "passwordless-auth", WebUtility.UrlDecode(token).Replace(" ", "+"));
        //
        //     if (isValid)
        //     {
        //         await _userManager.UpdateSecurityStampAsync(user);
        //
        //         await HttpContext.SignInAsync(
        //             IdentityConstants.ApplicationScheme,
        //             new ClaimsPrincipal(new ClaimsIdentity(
        //                 new List<Claim> { new Claim("sub", user.Id) },
        //                 IdentityConstants.ApplicationScheme)));
        //     }
        //
        //     var scheme = "https://";
        //     var sub = Environment.GetEnvironmentVariable("DOMAIN_PREPEND") != string.Empty ? $"{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}ids." : "ids.";
        //     var uiPort = Environment.GetEnvironmentVariable("ORDERS_UI_PORT");
        //     var uiSub = Environment.GetEnvironmentVariable("DOMAIN_PREPEND");
        //     var domainTld = Environment.GetEnvironmentVariable("DOMAIN_TLD");
        //     var domain = Environment.GetEnvironmentVariable("A_RECORD");
        //
        //     var redirectConnect = $"{scheme}{sub}{domain}.{domainTld}/connect/authorize?client_id={clientId}&redirect_uri={returnUrl}&response_type=id_token token&scope={scopes}&nonce={RandomString(32, false)}";
        //
        //     return Redirect($"{redirectConnect}");
        // }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        return StatusCode(401, new
                        {
                            Reason = "Email not confirmed"
                        });
                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return StatusCode(200, new
                    {
                        Reason = "Success"
                    });
                }
                if (result.RequiresTwoFactor)
                {
                    return StatusCode(401, new
                    {
                        Reason = "2fa required"
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return StatusCode(401, new
                    {
                        Reason = "Account logged out"
                    });
                }
                else
                {
                    return StatusCode(401, new
                    {
                        Reason = "Invalid login attempt"
                    });
                }
            }

            // If we got this far, something failed, redisplay form
            return StatusCode(401, new
            {
                Reason = "Error"
            });
        }

        // /// <summary>
        // /// TODO: this can now be delegated to the permissions microservice
        // /// </summary>
        // /// <param name="model"></param>
        // /// <param name="returnUrl"></param>
        // /// <returns></returns>
        // [HttpPost]
        // [AllowAnonymous]
        // public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailViewModel model)
        // {
        //     var user = await _userManager.FindByEmailAsync(model.Email);
        //     var address = new MailAddress(model.Email);
        //     var host = address.Host;
        //     var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //     await _emailSender.SendEmailConfirmationAsync(model.Email, user.Id, code, _options);
        //     _logger.LogInformation("User created a new account with password.");
        //     return Ok();
        // }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        public class ConfirmEmailViewModel
        {
            public string Email { get; set; }
            public string UserId { get; set; }
            public string Code { get; set; }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{model.UserId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, WebUtility.UrlDecode(model.Code).Replace(" ", "+"));

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Result = "Succeed"
                });
            }
            else
            {
                return StatusCode(400, new
                {
                    Reason = "Something went wrong"
                });
            }
        }

        // [HttpPost]
        // [AllowAnonymous]
        //
        // public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = await _userManager.FindByEmailAsync(model.Email);
        //         if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //         {
        //             // Don't reveal that the user does not exist or is not confirmed
        //             return StatusCode(200, new
        //             {
        //                 Reason = ""
        //             });
        //         }
        //
        //         // For more information on how to enable account confirmation and password reset please
        //         // visit https://go.microsoft.com/fwlink/?LinkID=532713
        //         var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //         var callbackUrl = $"{model.Redirect}?code={code}&userId={user.Id}"; //Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
        //         await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"his is new... Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        //         return StatusCode(200, new
        //         {
        //             Reason = "test"
        //         });
        //     }
        //
        //     // If we got this far, something failed, redisplay form
        //     return StatusCode(400, new
        //     {
        //         Reason = "Something went wrong"
        //     });
        // }
        //
        // [HttpPost]
        // [AllowAnonymous]
        // public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         // TODO - this should return the validation errors
        //         return StatusCode(400, new
        //         {
        //             Reason = "Validation errors"
        //         });
        //     }
        //
        //     var user = await _userManager.FindByEmailAsync(model.Email);
        //     if (user == null)
        //     {
        //         // Don't reveal that the user does not exist
        //         // TODO - this should return the validation errors
        //         return StatusCode(400, new
        //         {
        //             Reason = "Validation errors"
        //         });
        //     }
        //     var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //     if (result.Succeeded)
        //     {
        //         // TODO - this should return the validation errors
        //         return StatusCode(200, new
        //         {
        //             Reason = "Done"
        //         });
        //     }
        //     // AddErrors(result);
        //     // return View();
        //     // TODO - this should return the validation errors
        //     return StatusCode(400, new
        //     {
        //         Reason = result.Errors.Select(x => x.Code + " : " + x.Description + "\n")
        //     });
        // }
    }
}
