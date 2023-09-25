namespace Weather_data_web_api.Models.DTOs
{
    public class WeatherDataBaseDTO
    {
        public string DeviceName { get; set; }
        public double? Precipitationmmh { get; set; }
        public DateTime Time { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? TemperatureC { get; set; }
        public double? AtmosphericPressurekPa { get; set; }
        public double? MaxWindSpeedms { get; set; }
        public double? SolarRadiationWm2 { get; set; }
        public double? VaporPressurekPa { get; set; }
        public double? Humidity { get; set; }
        public double? WindDirection { get; set; }
        public WeatherDataBaseDTO() { }


        public WeatherDataBaseDTO(WeatherData fullData)
        {
            Precipitationmmh = fullData.Precipitationmmh ?? 0;
            Time = fullData.Time;
            Latitude = fullData.Latitude;
            Longitude = fullData.Longitude;
            TemperatureC = fullData.TemperatureC;
            AtmosphericPressurekPa = fullData.AtmosphericPressurekPa ?? 0;
            MaxWindSpeedms = fullData.MaxWindSpeedms ?? 0;
            SolarRadiationWm2 = fullData.SolarRadiationWm2 ?? 0;
            VaporPressurekPa = fullData.VaporPressurekPa ?? 0;
            Humidity = fullData.Humidity ?? 0;
            WindDirection = fullData.WindDirection ?? 0;
        }
    }
}
