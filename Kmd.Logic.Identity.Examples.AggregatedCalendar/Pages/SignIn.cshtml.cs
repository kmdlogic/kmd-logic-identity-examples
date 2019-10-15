using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Logic.Identity.Examples.AggregatedCalendar.Pages
{
    [AllowAnonymous]
    public class SignInModel : PageModel
    {
        public IActionResult OnGet()
        {
            var redirectUrl = Url.Page("MyCalendar");

            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}