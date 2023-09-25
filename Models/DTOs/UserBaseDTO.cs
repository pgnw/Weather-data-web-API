namespace Weather_data_web_api.Models.DTOs
{
    public class UserBaseDTO
    {

        public string? AcessLevel { get; set; }


        public bool IsAllNull()
        {
            if (AcessLevel != null)
                return false;
            return true;
        }
    }
}
