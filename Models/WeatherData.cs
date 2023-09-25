using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using Weather_data_web_api.Models.DTOs;

namespace Weather_data_web_api.Models
{
    public class WeatherData
    {
        [JsonIgnore]
        [BsonId]
        public ObjectId _id { get; set; }
        public string ObjId => _id.ToString();
        [BsonElement("Device Name")]
        public string? DeviceName { get; set; }
        [BsonElement("Precipitation mm/h")]
        public double? Precipitationmmh { get; set; }
        public DateTime Time { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [BsonElement("Temperature (°C)")]
        public double? TemperatureC { get; set; }
        [BsonElement("Atmospheric Pressure (kPa)")]
        public double? AtmosphericPressurekPa { get; set; }
        [BsonElement("Max Wind Speed (m/s)")]
        public double? MaxWindSpeedms { get; set; }
        [BsonElement("Solar Radiation (W/m2)")]
        public double? SolarRadiationWm2 { get; set; }
        [BsonElement("Vapor Pressure (kPa)")]
        public double? VaporPressurekPa { get; set; }
        [BsonElement("Humidity (%)")]
        public double? Humidity { get; set; }
        [BsonElement("Wind Direction (°)")]
        public double? WindDirection { get; set; }

        public WeatherData()
        {

        }
        public WeatherData(WeatherDataBaseDTO baseDTO)
        {
            DeviceName = baseDTO.DeviceName;
            Precipitationmmh = baseDTO.Precipitationmmh;
            Time = baseDTO.Time;
            Latitude = baseDTO.Latitude;
            Longitude = baseDTO.Longitude;
            TemperatureC = baseDTO.TemperatureC ?? 0;
            AtmosphericPressurekPa = baseDTO.AtmosphericPressurekPa;
            MaxWindSpeedms = baseDTO.MaxWindSpeedms;
            SolarRadiationWm2 = baseDTO.SolarRadiationWm2;
            VaporPressurekPa = baseDTO.VaporPressurekPa;
            Humidity = baseDTO.Humidity;
            WindDirection = baseDTO.WindDirection;
        }
    }


}
