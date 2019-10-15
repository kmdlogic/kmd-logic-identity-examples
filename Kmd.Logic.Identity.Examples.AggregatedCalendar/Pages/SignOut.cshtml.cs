using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Logic.Identity.Examples.AggregatedCalendar.Pages
{
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            var callbackUrl = Url.Page("Index", null, values: null, protocol: Request.Scheme);

            return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}