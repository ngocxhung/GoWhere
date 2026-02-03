namespace GoWhere_Api.Model.DTOs.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}
