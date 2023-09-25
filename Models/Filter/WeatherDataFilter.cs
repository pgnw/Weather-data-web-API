namespace Weather_data_web_api.Models.Filter
{
    public class WeatherDataFilter
    {
        public DateTime? RecordedFrom { get; set; }
        public DateTime? RecordedTo { get; set; }
        public string? DeviceMatch { get; set; }
        public DateTime? DateTimeMatch { get; set; }
        public bool? HighestTemp { get; set; }

        /// <summary>
        /// Checks if all the filters in the WeatherDataFilter class are null.
        /// </summary>
        /// <returns>True if everything is null false otherwise.</returns>
        public bool isAllNull()
        {
            if (RecordedFrom == null & RecordedTo == null & DeviceMatch == null & DateTimeMatch == null & HighestTemp == null)
                return true;
            return false;
        }
    }
}

