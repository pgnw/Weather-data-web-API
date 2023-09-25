using MongoDB.Bson;
using MongoDB.Driver;
using Weather_data_web_api.Models;
using Weather_data_web_api.Models.DTOs;
using Weather_data_web_api.Models.Filter;
using Weather_data_web_api.Services;

namespace Weather_data_web_api.Repositories
{
    public class UserRepository : IUserDataRepository
    {
        //Create a readonly field top store out connection details for the user colleciton
        //in the Mongo database.
        private readonly IMongoCollection<User> _users;

        //Request the MongoConnectionBuilder from our services
        public UserRepository(MongoConnectionBuilder connection)
        {
            //Use the provided connection from the builder to access the user collection and
            //store a reference to it in the readonly field.
            _users = connection.GetDatabase().GetCollection<User>("Users");
        }

        public IEnumerable<User> GetAll()
        {
            //Create a builder object for setting the required filter parameters.
            var builder = Builders<User>.Filter;
            //Create a filter that accepts everything.
            var filter = builder.Empty;
            //Get all the user in the database and return it.
            var users = _users.Find(filter).ToEnumerable();
            return users;
        }
        public User AuthenticateUser(string apiKey, Roles requiredAccessLevel)
        {
            //Create a filter to find a user based upon their apiKey
            var filter = Builders<User>.Filter.Eq(c => c.ApiKey, apiKey);
            //Pass the filter to Mongo db to see if there is a match for the key
            var user = _users.Find(filter).FirstOrDefault();

            //Check if a user was found and that there access level is higher than the required level
            if (user == null || IsSuitableRole(user.AcessLevel, requiredAccessLevel) == false)
            {
                return null;
            }
            //Return user details if access is authenticated
            return user;
        }

        public User GetById(string id)
        {
            //Takes the provided hashed id string and converts it back to the individual
            //object id sections
            ObjectId objectId = ObjectId.Parse(id);
            //Create a filter that checks for any records matching the id given.
            var filter = Builders<User>.Filter.Eq(n => n._id, objectId);
            //Return the first record that matches the id.
            return _users.Find(filter).FirstOrDefault();
        }

        public bool Create(User user)
        {
            //Create a filter to check the collection based upon the user name.
            var filter = Builders<User>.Filter.Eq(c => c.UserName, user.UserName);
            //Check the collectoin using the filter to see if there are any matches.
            var existingUser = _users.Find(filter).FirstOrDefault();
            //If there is a match, meaning the email is already registered. Return false and
            //don't add the user.
            if (existingUser != null)
            {
                return false;
            }
            //Generate a new GUID to be the key for the user
            user.ApiKey = Guid.NewGuid().ToString();
            //Set the last access data to the current datetime
            user.LastAccessed = DateTime.Now;

            user.CreatedDate = DateTime.Now;
            user.Active = true;
            //Add the user to the collection
            _users.InsertOne(user);
            //Return true to confirm the user was successfully added.
            return true;
        }
        public OperationResponseDTO<User> Remove(string? id)
        {
            //Check the id is not null or blank before continuing
            if (String.IsNullOrWhiteSpace(id))
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Empty id provided.",
                    WasSuccessful = false,
                };
            }
            ObjectId objectId;
            try
            {
                //Takes the provided hashed id string and converts it back to the individual
                //object id sections
                objectId = ObjectId.Parse(id);
            }
            catch
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Unable to parse given id.",
                    WasSuccessful = false,
                };
            }


            //Create a filter that checks if the id provided matches anything in the database.
            var filter = Builders<User>.Filter.Eq(n => n._id, objectId);
            //Use the filter to find and delete the desired entry before storing the result details.

            // Delete the user.
            var result = _users.DeleteOne(filter);
            // Return true to siginify the user was succesfully deleted.
            // If anything was deleted return a message letting the user know.
            if (result.DeletedCount > 0)
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Record deleted successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            // Otherwise let the user know it failed.
            else
            {
                return new OperationResponseDTO<User>
                {
                    Message = "No record with matching id found, nothing deleted",
                    WasSuccessful = false,
                };
            }
        }

        public OperationResponseDTO<User> RemoveMany(UserFilter options)
        {
            //Pass the filter options over to be converted into mongo db filter definitions.
            var filter = GenerateFilterDefinitions(options);
            //Pass the filter definitions to mongo db to delete any documents that meet the filter parameters. 
            var result = _users.DeleteMany(filter);

            //If there was any records that were deleted, return a response object with the relevant details
            if (result.DeletedCount > 0)
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Record/s deleted successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };

            }
            else
            {
                return new OperationResponseDTO<User>
                {
                    Message = "No users deleted",
                    WasSuccessful = false,
                };
            }
        }
        /// <summary>
        /// Remove any users with a last acessed date over 30 days old.
        /// </summary>
        /// <returns>returns a operation reponse to let the user know how it went.</returns>
        public OperationResponseDTO<User> RemoveInactive()
        {
            //Gets a filter builder object for setting the required filter parameters
            var builder = Builders<User>.Filter;
            //Sets the filter to have no parameters to check.
            var filter = builder.Empty;

            // make a filter to match on any users that have the last accessed date older than 30 days.
            filter &= builder.Lte(n => n.LastAccessed, DateTime.Now.AddDays(-30));
            // Add another filter to ensure that admins are not deleted.
            filter &= builder.Ne(n => n.AcessLevel, "Admin");

            // Delete the users.
            var result = _users.DeleteMany(filter);

            //If there were any records that were removed, return a response object with the relevant details
            if (result.DeletedCount > 0)
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Record updated successfully successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            else
            {
                return new OperationResponseDTO<User>
                {
                    Message = "No users updated.",
                    WasSuccessful = false,
                };
            }
        }
        public OperationResponseDTO<User> Update(string id, User userUpdate)
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
                return new OperationResponseDTO<User>
                {
                    Message = "Unable to parse given id.",
                    WasSuccessful = false,
                };
            }

            //Create a builder to allow to build a set of update parameters.
            var builder = Builders<User>.Update;

            //Merge the update definitions.
            UpdateDefinition<User> update = builder.Set("AcessLevel", userUpdate.AcessLevel);

            //Create a filter that performs an equals check on the User's _id against the objectId
            var filter = Builders<User>.Filter.Eq(n => n._id, objectId);
            //Assign the object id to the id field of the user so it matches the one being replaced.
            userUpdate._id = objectId;
            //Pass the user and the filert to mongo db to find and replace the desired User entry.
            var result = _users.UpdateOne(filter, update);

            //If there were any records that were updated, return a response object with the relevant details
            if (result.ModifiedCount > 0)
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Record updated successfully successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new OperationResponseDTO<User>
                {
                    Message = "No users updated.",
                    WasSuccessful = false,
                };
            }
        }
        public OperationResponseDTO<User> UpdateMany(UserPatchDTO updates)
        {
            //Make filter definitions from the provided filter.
            var filter = GenerateFilterDefinitions(updates.Filter);
            //Generate the update definitions.
            var updateDefintions = GenerateUpdateDefinition(updates);
            //Pass the filter and update rules to mongo db to be executed.
            var result = _users.UpdateMany(filter, updateDefintions);

            //If there were any records that were updated, return a response object with the relevant details
            if (result.ModifiedCount > 0)
            {
                return new OperationResponseDTO<User>
                {
                    Message = "Record/s updated successfully successfully.",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new OperationResponseDTO<User>
                {
                    Message = "No records matching the given filters where found.",
                    WasSuccessful = false,
                };
            }
        }

        /// <summary>
        /// Generates update definitions based off it a WeatherPatchDTO object.
        /// </summary>
        /// <param name="updates">WeatherDataPatchDTO containing the filter information and what data to update.</param>
        /// <returns>A new update Definition object containing the data and filter for updating.</returns>
        private FilterDefinition<User> GenerateFilterDefinitions(UserFilter userFilter)
        {
            //Gets us a filter builder object for setting ant required filter parameters
            var builder = Builders<User>.Filter;
            //Sets the filter to have no parameters to check.
            var filter = builder.Empty;

            if (userFilter?.CreatedFrom != null)
            {
                //Creates a filter checking if the created date is after the provided datetime.
                filter &= builder.Gte(n => n.CreatedDate, userFilter.CreatedFrom.Value);
            }
            if (userFilter?.Createdto != null)
            {
                //Creates a filter checking if the created date is before the provided datetime.
                filter &= builder.Lte(n => n.CreatedDate, userFilter.Createdto.Value);
            }
            return filter;
        }

        /// <summary>
        /// Generates update definitions based off it a WeatherPatchDTO object.
        /// </summary>
        /// <param name="updates">UserPatchDTO object containing the filter information and what data to update.</param>
        /// <returns>A new update Definition object containing the data and filter for updating.</returns>
        private UpdateDefinition<User> GenerateUpdateDefinition(UserPatchDTO updates)
        {
            //Create an update builder to help build the update rules
            var builder = Builders<User>.Update.Combine();
            //Create update definition object that starts out empty/null
            UpdateDefinition<User> updateDefinition = null;

            //If the DeviceName property of the patch object is not null
            if ((updates.AcessLevel == null) == false)
            {
                //Create a new filter definition to update the AcessLevel field
                updateDefinition = builder.Set(user => user.AcessLevel, updates.AcessLevel);
            }

            //Return the completed definition.
            return updateDefinition;
        }

        /// <summary>
        /// Updates the user who holds the api key given to the login data given.
        /// </summary>
        /// <param name="apiKey">Api key of the user to update</param>
        /// <param name="loginDate">Date to set the last accessed to</param>
        public void UpdateLoginTime(string apiKey, DateTime loginDate)
        {
            //Create a new mongo filter to search for users matching the provided API key
            var filter = Builders<User>.Filter.Eq(c => c.ApiKey, apiKey);
            //Create an update rule to change the LastAccess property of the user
            var update = Builders<User>.Update.Set(u => u.LastAccessed, loginDate);

            //Pass the filter and update rule to Mongo to be Processed
            _users.UpdateOne(filter, update);
        }

        /// <summary>
        /// Checks if the user role holds more or equal power than the required role.
        /// </summary>
        /// <param name="userRole">user role to check</param>
        /// <param name="requiredRole">required role to check the user role against</param>
        /// <returns></returns>
        private bool IsSuitableRole(string userRole, Roles requiredRole)
        {
            //Check if the provided user role matches one of the roles in the enum
            //If it can be found assign the role to the roleNumber out parameter.
            if (Enum.TryParse(userRole, out Roles roleNumber) == false)
            {
                //If not stop the method and return false
                return false;
            }
            //Get the role number value from the enum for the user
            int userRoleNumber = (int)roleNumber;
            //Get the role number value from the enum for the required level
            int requiredRoleNumber = (int)requiredRole;
            //If the user number is equal to or lower then the required role (meaning better access)
            //return true, otherwise return false.
            return userRoleNumber <= requiredRoleNumber;
        }

    }
}
