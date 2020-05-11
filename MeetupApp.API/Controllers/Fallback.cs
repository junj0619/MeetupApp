using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetupApp.API.Controllers
{
    [AllowAnonymous]
    public class Fallback : Controller
    {   /* Tell API Server if there is no route match from API controller then go find in angular route */
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
            "index.html"), "text/HTML");
        }
    }
}