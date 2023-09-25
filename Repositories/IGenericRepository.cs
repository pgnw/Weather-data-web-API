

using Weather_data_web_api.Models.DTOs;

namespace Weather_data_web_api.Repositories
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(string id);
        void Create(T item);
        OperationResponseDTO<T> Update(string id, T item);
        OperationResponseDTO<T> Delete(string id);
    }
}
