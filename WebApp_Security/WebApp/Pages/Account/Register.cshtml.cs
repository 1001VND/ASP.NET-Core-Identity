using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Services;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public RegisterModel(
            UserManager<User> userManager,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public RegisterViewModel registerViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            //Create the user
            var user = new User
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email
            };

            var claimDepartment = new Claim("Department", registerViewModel.Department);
            var claimPosition = new Claim("Position", registerViewModel.Position);

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, claimDepartment);

                await _userManager.AddClaimAsync(user, claimPosition);

                var confirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new
                    {
                        userId = user.Id,
                        token = confirmationToken
                    }) ?? "");

                ////////////////////////////////////////////////////////////////////////
                /// To trigger the email confirmation flow, use the code below /////////
                ////////////////////////////////////////////////////////////////////////

                //var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                //    values: new
                //    {
                //        userId = user.Id,
                //        token = confirmationToken
                //    });

                //// server SMTP: https://app.brevo.com/settings/keys/smtp

                //await _emailService.SendAsync("alatso57@gmail.com",
                //    user.Email,
                //    "Please confirm yout email",
                //    $"Please click on this link to confirm your email address: {confirmationLink}");

                //return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return Page();
            }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
