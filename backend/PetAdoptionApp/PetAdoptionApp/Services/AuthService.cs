using PetAdoptionApp.DTOs.Auth;
using PetAdoptionApp.Interfaces;
using Neo4j.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace PetAdoptionApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDriver _driver;
        private readonly IConfiguration _config;

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secret;

        public AuthService(IDriver driver, IConfiguration config)
        {
            _driver = driver;
            _config = config;

            _secret = config["Jwt:Secret"]!;
            _issuer = config["Jwt:Issuer"]!;
            _audience = config["Jwt:Audience"]!;
        }

        public async Task<AuthResponseDto> LoginUserAsync(LoginDto dto)
        {
            await using var session = _driver.AsyncSession();
            var user = await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                                MATCH (u:User {email: $email})
                                OPTIONAL MATCH (u)-[:VOLUNTEERS_AT]->(s:Shelter)
                                RETURN u.id AS userId,
                                       u.passwordHash AS userPassword,
                                       u.role AS role,
                                       s.id AS shelterId",
                                new { email = dto.Email });
                if (!await pointer.FetchAsync()) return null;
                var record = pointer.Current;
                return new
                {
                    Id = record["id"].As<string>(),
                    PasswordHash = record["userPassword"].As<string>(),
                    Role = record["role"].As<string>(),
                    ShelterId = record["shelterId"].As<string>()
                };
            });
            if (user == null)
                throw new UnauthorizedAccessException("Pogresan email ili lozinka.");
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Pogresna lozinka");
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user.Id, user.Role, user.ShelterId),
                //token expiry
                UserInfo = await GetUserInfo(user.Id, user.Role, user.ShelterId)
            };
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterUserDto dto)
        {
            await using var session = _driver.AsyncSession();

            var emailExists = await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                               MATCH (u:User {email: $email})
                               RETURN count(u)>0 AS exists",
                               new { email = dto.Email });
                var record = await pointer.SingleAsync();
                return record["exists"].As<bool>();
            });

            if (emailExists)
                throw new InvalidOperationException("Korisnik sa ovim email-om vec postoji");

            //email ne postoji, idemo dalje:
            var newId = Guid.NewGuid().ToString();
            await session.ExecuteWriteAsync(async x =>
            {
                await x.RunAsync(@"
                                 CREATE (u:User {
                                         id: $id,
                                         name: $name,
                                         surname: $surname,
                                         email: $email,
                                         passwordHash: $passwordHash,
                                         phone: $phone,
                                         bio: $bio,
                                         address: $address,
                                         hasChildren: $hasChildren,
                                         hasPets: $hasPets,
                                         livingSpace: $livingSpace,
                                         role: $role,
                                         createdAt: $createdAt})",
                                         new
                                         {
                                             id = newId,
                                             name = dto.Name,
                                             surname = dto.Surname,
                                             email = dto.Email,
                                             passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                                             phone = dto.Phone ?? "",
                                             bio = dto.Bio ?? "",
                                             address = dto.Address,
                                             hasChildren = dto.HasChildren,
                                             hasPets = dto.HasPets,
                                             livingSpace = dto.LivingSpace ?? "",
                                             createdAt = DateTime.UtcNow.ToString("o")
                                         });
            });
                return new AuthResponseDto
                {
                    Token = GenerateJwtToken(newId, "User", null),
                    //token expiry
                    UserInfo = await GetUserInfo(newId, "User", null)
                };
        }

        //public async Task<AuthResponseDto> RegisterVolunteerAsync(RegisterVolunteerDto dto)
        //{
        //    await using var session = _driver.AsyncSession();

        //    var emailExists = await session.ExecuteReadAsync(async x =>
        //    {
        //        var pointer = await x.RunAsync(@"
        //                                ")
        //    });
        //}

        public string GenerateJwtToken(string userId, string role, string? shelterId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimName.Sub, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimName.Jti, Guid.NewGuid().ToString()),
            };

        }

        public async Task<UserInfoDto> GetUserInfo(string userId, string role, string? shelterId)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                                MATCH (u:User {id: $id})
                                RETURN u.id AS id,
                                       u.name AS name,
                                       u.surname AS surname,
                                       u.email AS email",
                                new { id = userId });
                await pointer.FetchAsync();
                var record = pointer.Current;
                return new UserInfoDto
                {
                    Id = record["id"].As<string>(),
                    Email = record["email"].As<string>(),
                    Role = record["role"].As<string>(),
                    ShelterId = record["shelterId"].As<string>()
                };
            });
        }
    }
}
