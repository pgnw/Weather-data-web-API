using Weather_data_web_api.Models.Filter;

namespace Weather_data_web_api.Models.DTOs
{
    public class WeatherDataPatchDTO
    {

        public string? DeviceName { get; set; }
        public double? Precipitationmmh { get; set; }
        public DateTime? Time { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? TemperatureC { get; set; }
        public double? AtmosphericPressurekPa { get; set; }
        public double? MaxWindSpeedms { get; set; }
        public double? SolarRadiationWm2 { get; set; }
        public double? VaporPressurekPa { get; set; }
        public double? Humidity { get; set; }
        public double? WindDirection { get; set; }

        public WeatherDataFilter Filter { get; set; }

        public bool IsAllNull()
        {
            // Delegate for allowing short use of the string.IsNullOrWhiteSpace method.
            Func<String, bool> i = string.IsNullOrWhiteSpace;

            if (i(DeviceName) & Precipitationmmh == null & Time == null & Latitude == null & Longitude == null & TemperatureC == null & AtmosphericPressurekPa == null & MaxWindSpeedms == null & SolarRadiationWm2 == null & VaporPressurekPa == null & Humidity == null & WindDirection == null)
            {
                return true;
            }
            return false;
        }
    }
}
