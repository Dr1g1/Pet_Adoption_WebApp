namespace PetAdoptionApp.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public UserInfoDto UserInfo { get; set; }
    }
}
