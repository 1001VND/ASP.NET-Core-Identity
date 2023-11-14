using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public string Message { get; set; } = string.Empty; 
        public ConfirmEmailModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGet(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    this.Message = "Email address is successfully confirmred. You can now try to login.";
                    return Page();
                }
            }

            this.Message = "Failed to validate email.";
            return Page();
        }
    }
}
