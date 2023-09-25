using Microsoft.AspNetCore.Mvc;
using NotesMongoAPI.Middleware;
using Weather_data_web_api.Models;
using Weather_data_web_api.Models.DTOs;
using Weather_data_web_api.Repositories;

namespace Weather_data_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserDataRepository _repository;
        public UserController(IUserDataRepository repository)
        {
            // Get the user repository from the dependency injection system.
            _repository = repository;
        }

        /// <summary>
        /// Gets all users from the database.
        /// </summary>
        /// <remarks>
        /// Requires a teacher API key.
        /// </remarks>
        /// <returns>Returns every user in the database.</returns>
        [HttpGet]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try
            {
                // Get a copy of all the User objects in the database.
                var users = _repository.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Creates a new user for accessing the database.
        /// </summary>
        /// <remarks>
        /// Requires a teacher API key.
        /// </remarks>
        /// <param name="newUser">New user to be added</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpPost]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status400BadRequest)]
        public IActionResult CreateUser(NewUserDTO newUser)
        {
            // Take the details from the new User dto and insert them into a new User object.
            User user = new User
            {
                UserName = newUser.UserName,
                HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password),
                AcessLevel = newUser.AcessLevel,
            };
            // Add the user to the database
            var result = _repository.Create(user);
            if (result)
                //Return a 200 response code
                return Ok("New user created");
            // Let the user know something went wrong.
            return BadRequest(result);
        }

        /// <summary>
        /// Deletes a new based on id in the database.
        /// </summary>
        /// <remarks>
        /// Requires a teacher API key.
        /// </remarks>
        ///<param name="id">Id of the user to be deleted</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpDelete]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteUser(string id)
        {

            //Pass the id to the repository to process the deletion
            var result = _repository.Remove(id);

            //If the reuqets does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }


        /// <summary>
        /// Deletes every user that has not used the database in over 30 days.
        /// </summary>
        /// <remarks>
        /// Requires a teacher API key.
        /// </remarks>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpDelete("DeleteInActive")]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteInactiveUsers()
        {

            // Delete any users that have not been used in more than 30 days.
            var result = _repository.RemoveInactive();

            //If the requests does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }


        /// <summary>
        /// Updates the access level of a user, based on their Id.
        /// </summary>
        /// <remarks>
        /// Requires a teacher API key.
        /// </remarks>
        ///<param name="id">Id of the user to be updated</param>
        ///<param name="updatedUserDTO">Data containg information on the update</param>
        /// <returns>Returns a status code with information about how the request went.</returns>

        [HttpPut]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status400BadRequest)]
        public IActionResult Put(string id, [FromBody] UserBaseDTO updatedUserDTO)
        {
            //Ensure the id and WeatherData are not empty before continueing
            if (String.IsNullOrWhiteSpace(id) || updatedUserDTO == null)
            {
                return BadRequest("Invalid/Incomplete update details provided. Please send correct data to perform update.");
            }
            //Taking the WeatherDataDTO and using to create a new WeatherData class
            var user = new User { AcessLevel = updatedUserDTO.AcessLevel };
            //Pass the id and WeatherData on to the repository to process the update
            var result = _repository.Update(id, user);

            //If the reuqets does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }


        /// <summary>
        /// Updates several users at once, based on created date.
        /// </summary>
        /// <remarks>
        /// Requires a teacher API key.
        /// </remarks>
        ///<param name="patchDetail">Data containg information on what users to update and what values to change.</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(OperationResponseDTO<User>), StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [ApiKey("Teacher")]
        public IActionResult UpdateMany([FromBody] UserPatchDTO patchDetail)
        {
            //Check that a patchDetails object was provided.
            if (patchDetail == null)
            {
                return BadRequest("No update details provided. Please check details and try again.");
            }
            // Check if an acessLevel update was provided.
            if (patchDetail.AcessLevel == null)
            {
                return BadRequest("Cannot update if no update values are provided.");
            }
            //Check that there is at least one filter parameter to use for our update 
            if (patchDetail.Filter.CreatedFrom == null && patchDetail.Filter.Createdto == null)
            {
                return BadRequest("Update not permitted without at least one valid filter parameter provided.");
            }
            //Forward the detials to the repository for processing
            var result = _repository.UpdateMany(patchDetail);

            //If the reuqets does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }

    }
}
