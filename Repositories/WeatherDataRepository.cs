using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Weather_data_web_api.Models;
using Weather_data_web_api.Models.DTOs;
using Weather_data_web_api.Models.Filter;
using Weather_data_web_api.Services;

namespace Weather_data_web_api.Repositories
{
    public class WeatherDataRepository : IWeatherDataRepository
    {
        private readonly IMongoCollection<WeatherData> _weatherDatas;

        //Request the mongo builder from services.
        public WeatherDataRepository(MongoConnectionBuilder connection)
        {
            //Using the builder to connect to the database and then the desired collection and storing the
            //connecton in our read only field.
            _weatherDatas = connection.GetDatabase().GetCollection<WeatherData>("WeatherData");
        }

        public void Create(WeatherData createdWeatherData)
        {
            _weatherDatas.InsertOne(createdWeatherData);
        }

        public void CreateMany(List<WeatherData> weatherDataList)
        {
            _weatherDatas.InsertMany(weatherDataList);
        }

        public OperationResponseDTO<WeatherData> Delete(string id)
        {
            ObjectId objectId;
            // If the user entered a string that cannot be converited to an object id stop the method and let them know somthing went wrong.
            try
            {             //Takes the provided hashed id string and converts it back to the individual
                          //object id sections
                objectId = ObjectId.Parse(id);
            }
            catch
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = $"Unable to parse id: {id}",
                    WasSuccessful = false,
                };
            }

            //Create a filter that checks if the id provided matches anything in the database.
            var filter = Builders<WeatherData>.Filter.Eq(n => n._id, objectId);
            //Use the filter to find and delete the desired entry before storing the result details.
            var result = _weatherDatas.DeleteOne(filter);

            // If anything was deleted return a message letting the user know.
            if (result.DeletedCount > 0)
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "Record deleted successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            // Otherwise let the user know it failed.
            else
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "No record with matching id found, nothing deleted",
                    WasSuccessful = false,
                };
            }
        }

        public OperationResponseDTO<WeatherData> DeleteMany(WeatherDataFilter options)
        {
            //Pass the filter options over to be converted into mongo db filter definitions.
            var filter = GenerateFilterDefinitions(options);
            //Pass the filter definitions to mongo db to delete any documents that meet the filter parameters. 
            var result = _weatherDatas.DeleteMany(filter);

            //If there was any records that were deleted, return a response object with the relevant details
            if (result.DeletedCount > 0)
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "Record/s deleted successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            else
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "No weather data was found that matched the filter provided, nothing deleted.",
                    WasSuccessful = false,
                };
            }
        }

        public IEnumerable<WeatherData> GetAll()
        {
            //Create a builder object for setting the required filter parameters.
            var builder = Builders<WeatherData>.Filter;
            //Create a filter that accepts everything.
            var filter = builder.Empty;
            //Get all the WeatherData in the database and return it.
            var weatherDataResults = _weatherDatas.Find(filter).ToEnumerable();
            return weatherDataResults;
        }

        public IEnumerable<WeatherData> GetAll(WeatherDataFilter options)
        {
            //Passes the filter options to the method that will convert them into mongo db filter definitions.
            var filter = GenerateFilterDefinitions(options);

            //Calls the find method in the mongo db driver and passes it the filter to be used.
            var weatherDataResults = _weatherDatas.Find(filter).ToEnumerable();
            return weatherDataResults;
        }

        public WeatherData GetById(string id)
        {
            ObjectId objectId;
            try
            {
                //Takes the provided hashed id string and converts it back to the individual
                //object id sections
                objectId = ObjectId.Parse(id);
            }
            catch
            { return null; }

            //Create a filter that checks for any records matching the id given.
            var filter = Builders<WeatherData>.Filter.Eq(n => n._id, objectId);
            //Return the first record that matches the id.
            return _weatherDatas.Find(filter).FirstOrDefault();
        }

        public OperationResponseDTO<WeatherData> Update(string id, WeatherData updatedWeatherData)
        {
            ObjectId objectId;
            try
            {
                //Takes the provided hashed id string and converts it back to the individual
                //object id sections
                objectId = ObjectId.Parse(id);
            }
            catch
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "Unable to parse given id.",
                    WasSuccessful = false,
                };
            }

            //Create a builder to allow to build a set of update parameters.
            var builder = Builders<WeatherData>.Update;

            //Create a list of update definitons for every property in the WeatherData class excluding ids.
            var updateDefs = new List<UpdateDefinition<WeatherData>>();
            for (int i = 0; i < updatedWeatherData.GetType().GetProperties().Length; i++)
            {
                // Get current property.
                var field = updatedWeatherData.GetType().GetProperties()[i];
                if (field.Name != "_id" & field.Name != "ObjId")
                {
                    //Add the current property type and value from updatedWeatherData to the list.
                    var updateDef = builder.Set(field.Name, field.GetValue(updatedWeatherData));
                    updateDefs.Add(updateDef);
                }
            }

            //Merge the update definitions.
            UpdateDefinition<WeatherData> update = builder.Combine(updateDefs);

            //Create a filter that performs an equals check on the weatherDatas _id against the objectId
            var filter = Builders<WeatherData>.Filter.Eq(n => n._id, objectId);
            //Assign the object id to the id field of the weatherData so it matches the one being replaced.
            updatedWeatherData._id = objectId;
            //Pass the weatherData and the filert to mongo db to find and replace the desired weatherData entry.
            var result = _weatherDatas.UpdateOne(filter, update);

            //If there were any records that were updated, return a response object with the relevant details
            if (result.ModifiedCount > 0)
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "Record updated successfully successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "No weatherDatas updated.",
                    WasSuccessful = false,
                };
            }
        }

        public OperationResponseDTO<WeatherData> UpdateMany(WeatherDataPatchDTO updates)
        {
            //Make filter definitions from the provided filter.
            var filter = GenerateFilterDefinitions(updates.Filter);
            //Generate the update definitions.
            var updateDefintions = GenerateUpdateDefinition(updates);
            //Pass the filter and update rules to mongo db to be executed.
            var result = _weatherDatas.UpdateMany(filter, updateDefintions);

            //If there were any records that were updated, return a response object with the relevant details
            if (result.ModifiedCount > 0)
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "Record/s updated successfully successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new OperationResponseDTO<WeatherData>
                {
                    Message = "No records matching the given filters where found.",
                    WasSuccessful = false,
                };
            }
        }


        /// <summary>
        /// Generates FilterDefinitions from a WeatherDataFilter instance.
        /// </summary>
        /// <param name="weatherDataFilter">The filter object to base the FilterDefinitions from</param>
        /// <returns>FilterDefinitions based off of the WeatherDataFilter provided</returns>
        private FilterDefinition<WeatherData> GenerateFilterDefinitions(WeatherDataFilter weatherDataFilter)
        {
            //Gets us a filter builder object for setting ant required filter parameters
            var builder = Builders<WeatherData>.Filter;
            //Sets the filter to have no parameters to check.
            var filter = builder.Empty;

            if (String.IsNullOrWhiteSpace(weatherDataFilter?.DeviceMatch) == false)
            {
                //This first line escapes out any special characters in the filter to
                //prevent potential regex attacks or errors
                var regexStatement = Regex.Escape(weatherDataFilter.DeviceMatch);

                filter &= builder.Regex(w => w.DeviceName, BsonRegularExpression.Create(regexStatement));
            }
            if ((weatherDataFilter?.DateTimeMatch) != null)
            {
                // Checks if the records have the same datetime as the WeatherDataFilter provided.
                filter &= builder.Eq(w => w.Time, weatherDataFilter.DateTimeMatch.Value);
            }
            if (weatherDataFilter?.RecordedFrom != null)
            {
                //Creates a filter checking if the created date is after the provided datetime.
                filter &= builder.Gte(n => n.Time, weatherDataFilter.RecordedFrom.Value);
            }
            if (weatherDataFilter?.RecordedTo != null)
            {
                //Creates a filter checking if the created date is before the provided datetime.
                filter &= builder.Lte(n => n.Time, weatherDataFilter.RecordedTo.Value);
            }
            if (weatherDataFilter?.HighestTemp != null)
            {
                //Creates a sort filter based on the temperature of the weatherdata.
                var sortFilter = Builders<WeatherData>.Sort.Descending(w => w.TemperatureC);
                // Finds the record with the highest temp.
                var highestTempDoc = _weatherDatas.Find(filter).Sort(sortFilter).SortByDescending(w => w.TemperatureC).FirstOrDefault();
                //Create a filter checking if the temperature matches the one found above.
                if (highestTempDoc != null)
                    filter &= builder.Eq(w => w.TemperatureC, highestTempDoc.TemperatureC);

            }
            //Returns the generated filter to the caller
            return filter;
        }

        /// <summary>
        /// Generates update definitions based off it a WeatherPatchDTO object.
        /// </summary>
        /// <param name="updates">WeatherDataPatchDTO containing the filter information and what data to update.</param>
        /// <returns>A new update Definition object containing the data and filter for updating.</returns>
        private UpdateDefinition<WeatherData> GenerateUpdateDefinition(WeatherDataPatchDTO updates)
        {
            //Create an update builder to help build the update rules
            var builder = Builders<WeatherData>.Update.Combine();
            //Create update definition object that starts out empty/null
            UpdateDefinition<WeatherData> updateDefinition = null;

            //If the DeviceName property of the patch object is not null
            if (String.IsNullOrWhiteSpace(updates.DeviceName) == false)
            {
                //Create a new filter definition to update the DeviceName field
                updateDefinition = builder.Set(weatherData => weatherData.DeviceName, updates.DeviceName);
            }
            // Use reflection to get all the properties of updates and put them into an UpdateDefinition.
            for (int i = 0; i < updates.GetType().GetProperties().Length; i++)
            {
                // Current property.
                var property = updates.GetType().GetProperties()[i];
                // Value of current property.
                var value = property.GetValue(updates);

                // Don't use add DeviceName because it is done above and don't add Filter because that does not need to be added to the database.
                if (property.Name == "DeviceName" | property.Name == "Filter")
                    continue;
                if (property.GetValue(updates) != null)
                {
                    if (updateDefinition == null)
                        updateDefinition = builder.Set(property.Name, value);
                    else
                        updateDefinition = updateDefinition.Set(property.Name, value);
                }

            }
            //Return the completed definitions
            return updateDefinition;
        }
    }
}
