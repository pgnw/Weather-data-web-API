using Weather_data_web_api.Models;
using Weather_data_web_api.Models.DTOs;
using Weather_data_web_api.Models.Filter;

namespace Weather_data_web_api.Repositories
{
    public interface IWeatherDataRepository : IGenericRepository<WeatherData>
    {
        IEnumerable<WeatherData> GetAll(WeatherDataFilter options);
        void CreateMany(List<WeatherData> weatherDatas);
        OperationResponseDTO<WeatherData> DeleteMany(WeatherDataFilter options);
        OperationResponseDTO<WeatherData> UpdateMany(WeatherDataPatchDTO updates);
    }
}
