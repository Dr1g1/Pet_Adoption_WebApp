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
                TokenExpiry = DateTime.UtcNow.AddDays(30),
                UserInfo = await GetUserInfo(user.Id, user.Role, user.ShelterId)
            };
        }

        
        public string GenerateJwtToken(string userId, string role, string? shelterId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //jedinstveni id tokena.
            };

            if (!string.IsNullOrEmpty(shelterId))
                claims.Add(new Claim("shelterId", shelterId));

            var token = new JwtSecurityToken
            (
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                 expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            await using var session = _driver.AsyncSession();
            var emailExists = await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                                      MATCH (n)
                                      WHERE (n: User OR n:Volunteer) AND n.email = $email
                                      RETURN count(n)>0 AS exists",
                                      new { email = dto.Email });
                var result = await pointer.SingleAsync();
                return result["exists"].As<bool>();
            });

            if (emailExists)
                throw new InvalidOperationException("Ovaj email se vec koristi");

            var queryV = @"CREATE (v:Volunteer {
                    id:           $id,
                    name:         $name,
                    surname:      $surname,
                    email:        $email,
                    passwordHash: $passwordHash,
                    phone:        $phone,
                    bio:          $bio,
                    address:      $address,
                    hasChildren:  $hasChildren,
                    hasPets:      $hasPets,
                    livingSpace:  $livingSpace,
                    role:         'Volunteer',
                    isAdmin:      false,
                    isActive:     true,
                    skills:       $skills,
                    availableDays:$availableDays,
                    joinedAt:     $joinedAt,
                    createdAt:    $createdAt
                })";
            var queryU = @"CREATE (u:User {
                    id:           $id,
                    name:         $name,
                    surname:      $surname,
                    email:        $email,
                    passwordHash: $passwordHash,
                    phone:        $phone,
                    bio:          $bio,
                    address:      $address,
                    hasChildren:  $hasChildren,
                    hasPets:      $hasPets,
                    livingSpace:  $livingSpace,
                    role:         'User',
                    createdAt:    datetime()
                })";

            var newId = Guid.NewGuid().ToString();

            await session.ExecuteWriteAsync(async x =>
            {
                var query = dto.Role == "Volunteer" ? queryV : queryU;
                await x.RunAsync(query, new {/*ovde idu podaci koji se dodaju*/});
            });
            if(dto.Role == "Volunteer" && !string.IsNullOrEmpty(dto.ShelterId))
            {
                await session.ExecuteWriteAsync(async x =>
                {
                    await x.RunAsync(@"
                                      MATCH (v:Volunteer {id:$volunteerId})
                                      MATCH (s:Shelter {id: $shelterId})
                                      CREATE (v)-[:VOLUNTEERS_AT]->(s)",
                                      new { vollunteerId = newId, shelterId = dto.ShelterId });
                });
            }
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(newId, dto.Role, dto.ShelterId),
                TokenExpiry = DateTime.UtcNow.AddDays(30),
                UserInfo = await GetUserInfo(newId, dto.Role, dto.ShelterId)
            };
        }
    }
}
