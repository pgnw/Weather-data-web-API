using Weather_data_web_api.Models.Filter;

namespace Weather_data_web_api.Models.DTOs
{
    public class UserPatchDTO
    {




        public string? AcessLevel { get; set; }


        public UserFilter Filter { get; set; }

    }
}
