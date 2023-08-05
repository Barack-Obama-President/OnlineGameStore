using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OnlineGameStore.Models;

namespace OnlineGameStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
		//tryingsomestuffout//

		private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;
		private readonly RoleManager<ApplicationRole> _roleManager;
		//tryingsomestuffout//
		private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
			OnlineGameStore.Data.OnlineGameStoreContext context,

		 RoleManager<ApplicationRole> roleManager,
			UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
			_roleManager = roleManager;
		}

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class MinimumAgeAttribute : ValidationAttribute
        {
            private readonly int _minimumAge;

            public MinimumAgeAttribute(int minimumAge)
            {
                _minimumAge = minimumAge;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime dateOfBirth = (DateTime)value;
                DateTime minimumDateOfBirth = DateTime.Today.AddYears(-_minimumAge);

                if (dateOfBirth > minimumDateOfBirth)
                {
                    return new ValidationResult($"You must be at least {_minimumAge} years old.");
                }

                return ValidationResult.Success;
            }
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [Required] 
            [Display(Name = "Account name")]
            [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid account name. Only letters, numbers, and underscores are allowed.")]
            public string FullName { get; set; }

            [Required]
            [Display(Name = "Date of birth")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [MinimumAge(13)]
            public DateTime BirthDate { get; set; }
		}

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
				var user = new ApplicationUser
				{
					UserName = Input.Email,
					Email = Input.Email,
					FullName = Input.FullName,
                    BirthDate = Input.BirthDate,
                    
				};
				var result = await _userManager.CreateAsync(user, Input.Password);
               

				if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

					var auditrecord = new AuditRecord();
					auditrecord.AuditActionType = "Account registered";
					auditrecord.DateTimeStamp = DateTime.Now;
					auditrecord.KeyGameFieldID = 999;

					auditrecord.Username = Input.Email;
					// save the email used for registering

					// await _userManager.AddToRoleAsync(user, "User");
					IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Users");

					//return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });

					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //await _userManager.AddToRoleAsync(user, "User");
						 await _userManager.AddToRoleAsync(user, "Users");
						if (roleResult.Succeeded)
						{
							TempData["message"] = "Role added to this user successfully";
							return RedirectToPage("Manage");
						}
						//ApplicationRole AppRole = await _roleManager.FindByNameAsync("User");
						//IdentityResult roleResult = await _userManager.AddToRoleAsync(user, AppRole.Name);
						_logger.LogInformation("User added to the 'User' role.");


						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
           

            return Page();
        }
    }
}
