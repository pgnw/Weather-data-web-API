using Weather_data_web_api.Models;
using Weather_data_web_api.Models.DTOs;
using Weather_data_web_api.Models.Filter;

namespace Weather_data_web_api.Repositories
{
    public interface IUserDataRepository
    {
        User AuthenticateUser(string apiKey, Roles requiredAccessLevel);
        void UpdateLoginTime(string apiKey, DateTime loginDate);
        OperationResponseDTO<User> RemoveMany(UserFilter options);
        public IEnumerable<User> GetAll();

        public bool Create(User user);
        public OperationResponseDTO<User> Update(string id, User user);
        public OperationResponseDTO<User> Remove(string id);
        public OperationResponseDTO<User> UpdateMany(UserPatchDTO updates);
        public OperationResponseDTO<User> RemoveInactive();
    }
}
