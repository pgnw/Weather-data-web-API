using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NotesMongoAPI.Middleware;
using Weather_data_web_api.Models;
using Weather_data_web_api.Models.DTOs;
using Weather_data_web_api.Models.Filter;
using Weather_data_web_api.Repositories;

namespace Weather_data_web_api.Controllers
{
    [EnableCors("GooglePolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherDataController : ControllerBase
    {
        private readonly IWeatherDataRepository _repository;
        private readonly IUserDataRepository _userDataRepository;
        public WeatherDataController(IWeatherDataRepository repository, IUserDataRepository userDataRepository)
        {
            // Get the user and weather data repositories from the dependency injection system.
            _repository = repository;
            _userDataRepository = userDataRepository;
        }

        /// <summary>
        /// Gets all weather data from the database.
        /// </summary>
        /// <remarks>
        /// Requires a student API key.
        /// </remarks>
        /// <returns>Returns every weather data document in the database.</returns>
        // GET: api/<WeatherDatasController>
        [HttpGet]
        [ApiKey("Student")]
        [ProducesResponseType(typeof(WeatherData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<WeatherData>> Get()
        {
            try
            {
                // Get a copy of all the weatherData objects in the database.
                var weatherDatas = _repository.GetAll();
                // Convert the WeatherData Objects to WeatherDataDetails.
                return Ok(weatherDatas);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }

        }

        /// <summary>
        /// Gets weather data from the data base that matches the filters provided.
        /// </summary>
        /// <remarks>
        /// Filters include recorded from, recorded to, devicename match, recorded date match and an option to the return the highest temperture document.
        /// Requires a student API key.
        /// </remarks>
        ///<param name="filter">Filter containg information on what documents to filter out.</param>
        /// <returns>Returns weather data from the data base that matches the filters provided.</returns>
        // GET: api/WeatherDatas/GetFiltered
        [HttpGet("GetFiltered")]
        [ApiKey("Student")]
        [ProducesResponseType(typeof(WeatherData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<WeatherData>> GetFiltered([FromQuery] WeatherDataFilter filter)
        {
            try
            {
                // Get a collection of all the WeatherData that matches the provided filter.
                var weatherDatas = _repository.GetAll(filter);
                // Convert the WeatherData Objects to WeatherDataDetails.
                return Ok(weatherDatas);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET api/<WeatherDatasController>/5
        /// <summary>
        /// Gets weather data from the data base that matches the id provided.
        /// </summary>
        /// <remarks>
        /// Requires a student API key.
        /// </remarks>
        ///<param name="id">Id of the uer to find.</param>
        /// <returns>Returns weather data from the data base that matches the id provided.</returns>
        [HttpGet("{id}")]
        [ApiKey("Student")]
        [ProducesResponseType(typeof(WeatherData), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult Get(string id)
        {
            //Checks if the provided id was not empty or null
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id is required.");
            }
            try
            {
                //Request for the WeatherData to be loctaed by the repository
                var weatherData = _repository.GetById(id);
                if (weatherData == null)
                    return Problem("Given id does not exist");
                // Convert the WeatherData Objects to WeatherDataDetails.
                //Return the WeatherData with an OK repsonse message
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Adds a new wheather data document.
        /// </summary>
        /// <param name="createdWeatherData">Document to add.</param>
        /// <returns>Returns a status code containing information on how the request went.</returns>
        [HttpPost]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult Post([FromBody] WeatherDataBaseDTO createdWeatherData)
        {
            // Check if a weather data object was given.
            if (createdWeatherData == null)
            {
                return BadRequest("Please supply a JSON file for the creating a new weather data document");
            }
            try
            {
                // Convert the WeatherDataBaseDTO provided into a proper WeatherData, and add it to the database.
                var weatherData = new WeatherData(createdWeatherData);
                _repository.Create(weatherData);
                return CreatedAtAction("Post", "New WeatherData Added.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }

        }

        // POST api/WeatherDatas/PostMany
        /// <summary>
        /// Allows for the creating of multiple weather data entries at once.
        /// </summary>
        /// <param name="createdWeatherDatas">Weather data entries to be added.</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpPost("PostMany")]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(CreatedAtActionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult PostMany([FromBody] List<WeatherDataBaseDTO> createdWeatherDatas)
        {
            //Checks if any data was given
            if (createdWeatherDatas == null)
            {
                return BadRequest("No records provided for creation.");
            }
            //Checks that the provided list actually contains entries
            if (createdWeatherDatas.Count == 0)
            {
                return BadRequest("Provided WeatherData list contains no records.");
            }

            try
            {
                // Convert the WeatherDataBaseDTOs provided into a proper WeatherDatas, and add them to the database.
                var weatherDatas = createdWeatherDatas.Select(weatherData => new WeatherData(weatherData)).ToList();
                _repository.CreateMany(weatherDatas);
                return CreatedAtAction("PostMany", "New weatherdata document(s) Added.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Allows for updating a weather data document based on it's id with a new weather data document.
        /// </summary>
        /// <param name="id">Id of the docuement to replace.</param>
        /// <param name="updatedWeatherData">The document to replace the old one.</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpPut("{id}")]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult Put(string id, [FromBody] WeatherDataBaseDTO updatedWeatherData)
        {
            //Ensure the id and WeatherData are not empty before continueing
            if (String.IsNullOrWhiteSpace(id) || updatedWeatherData == null)
            {
                return BadRequest("Invalid/Incomplete update details provided. Please send correct data to perform update.");
            }
            //Taking the WeatherDataDTO and using to create a new WeatherData class
            var weatherData = new WeatherData(updatedWeatherData);
            //Pass the id and WeatherData on to the repository to process the update
            var result = _repository.Update(id, weatherData);

            //If the reuqets does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }

        // PATCH: api/WeatherDatas/UpdateMany
        /// <summary>
        /// Updates multiple weather data documents at once using a series of filter to identify which documents are to be updated. Takes a weather data object to use as a parameter to update any documents matching the filters.
        /// </summary>
        /// <param name="patchDetails">PatchDetails object containg data on the filters and update values.</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpPatch("UpdateMany")]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult UpdateMany([FromBody] WeatherDataPatchDTO patchDetails)
        {
            //Check that a patchDetails object was provided.
            if (patchDetails == null)
            {
                return BadRequest("No update details provided. Please check details and try again.");
            }

            if (patchDetails.IsAllNull())
            {
                return BadRequest("Cannot update if no update values are provided.");
            }
            //Check that there is at least one filter parameter to use for our update 
            if (patchDetails.Filter.isAllNull())
            {
                return BadRequest("Update not permitted without at least one valid filter parameter provided.");
            }
            //Forward the detials to the repository for processing
            var result = _repository.UpdateMany(patchDetails);

            //If the reuqets does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }

        // DELETE api/<WeatherDatasController>/5
        /// <summary>
        /// Deletes a weather data document based off it's id.
        /// </summary>
        /// <param name="id">Id of the document to delete.</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpDelete("{id}")]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult Delete(string id)
        {
            //Check the id is not null or blank before continueing
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No valid Id provided for delete request");
            }

            //Pass the id to the repository to process the deletion
            var result = _repository.Delete(id);

            //If the reuqets does not error but still does nothing/is not right
            if (result.WasSuccessful == false)
            {
                return BadRequest(result);
            }
            //Return an ok response.
            return Ok(result);
        }

        // DELETE api/WeatherDatas/DeleteOlderThanGivenDays
        /// <summary>
        /// Deletes any documents in the weather data collection that are older than the number given in days.
        /// </summary>
        /// <param name="days">Any documents older than this many days will be deleted.</param>
        /// <returns>Returns a status code with information about how the request went.</returns>
        [HttpDelete("DeleteOlderThanGivenDays")]
        [ApiKey("Teacher")]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResponseDTO<WeatherData>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteBasedUponAge([FromQuery] int? days)
        {
            //Check the id is not null or blank before continueing
            if (days == null || days == 0)
            {
                return BadRequest("Cannot delete records more than 0 days old.");
            }
            //Create a WeatherData filter for the desired value
            var filter = new WeatherDataFilter
            {
                //Add a RecordedTo filter using the days value against the current datetime.
                //The multiplying the days value by -1 converts it to a negative value.
                RecordedTo = DateTime.Now.AddDays((int)days * -1)
            };

            //Pass the filter to the repository to process the deletion
            var result = _repository.DeleteMany(filter);

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
