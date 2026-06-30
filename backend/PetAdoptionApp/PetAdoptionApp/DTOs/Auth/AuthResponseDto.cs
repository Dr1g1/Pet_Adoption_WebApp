namespace PetAdoptionApp.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public UserInfoDto UserInfo { get; set; }
    }
}
