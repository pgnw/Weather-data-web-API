using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Weather_data_web_api.Repositories;

namespace NotesMongoAPI.Middleware
{
    //Specifies where this annotation can be used.
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        //Create a private backing field/variable for our property
        private string requiredRole;
        //Property to access the private field with read only access
        public string RequiredRole
        {
            get { return requiredRole; }
        }

        //Create a constructor that takes a role value or defaults to 'Teacher' if one is not provided
        public ApiKeyAttribute(string role = "Teacher")
        {
            requiredRole = role;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Check the context(request data) to see if an apiKey was provided in the query parameters
            if (context.HttpContext.Request.Query.TryGetValue("apiKey", out var key) == false)
            {
                //If not, prepare a custom error response and return
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "No API key provided."
                };

                return;
            }
            //Take the key that was retrieved from the query and convert it to a string then remove
            //the trailing curly braces that would have been provided by default when it was retrieved.
            var validKey = key.ToString().Trim('{', '}');

            //Check that the specified role for the api Key authetication matches one of the valid
            //role enum values
            if (Enum.TryParse(requiredRole, out Weather_data_web_api.Models.Roles userRole) == false)
            {
                //If not, prepare a custom error response and return
                context.Result = new ContentResult
                {
                    StatusCode = 500,
                    Content = "Something went wrong. Please try again or contact your IT department."
                };

                return;
            }
            //Request the user repository directly from the services ionstread of through the constructor. 
            var userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();

            //Send the key and needed role to the repository to check if the key has the required access level
            if (userRepo.AuthenticateUser(validKey, userRole) == null)
            {
                //If not, prepare a custom error response and return
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "User key was invalid or did not have required permissions."
                };

                return;
            }

            //If authorised, update the user's last login.
            userRepo.UpdateLoginTime(validKey, DateTime.Now);

            //If everything passed until this point. Trigger the next item in the pipeline.
            //This could be another attribute, the controller or a specific endpoint.
            await next();
        }
    }
}
