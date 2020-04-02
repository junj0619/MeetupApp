using Microsoft.AspNetCore.Http;

namespace MeetupApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {   /* Modify HttpResponse header to add extra attribute [Application-Error] to be displayed in the header */

            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

    }
}