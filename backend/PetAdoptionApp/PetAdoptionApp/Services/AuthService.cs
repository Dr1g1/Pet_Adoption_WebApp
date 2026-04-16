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
        private readonly int _accessTokenExpiryMinutes;
        private readonly int _refreshTokenExpiryDays;

        public AuthService(IDriver driver, IConfiguration config)
        {
            _driver = driver;
            _config = config;

            _secret = config["Jwt:Secret"]!;
            _issuer = config["Jwt:Issuer"]!;
            _audience = config["Jwt:Audience"]!;
            _accessTokenExpiryMinutes = int.Parse(config["Jwt:AccessTokenExpiryMinutes"]!);
            _refreshTokenExpiryDays = int.Parse(config["Jwt:RefreshTokenExpiryDays"]!);
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
            return await GenerateResponseAsync(user.Id, dto.Email, user.Role, user.ShelterId);
        }

        
        private string GenerateJwtToken(string userId, string email,string role, string? shelterId, DateTime expiry)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(shelterId))
                claims.Add(new Claim("shelterId", shelterId));

            var token = new JwtSecurityToken(
                        issuer: _issuer,
                        audience: _audience,
                        claims: claims,
                        expires: expiry,
                        signingCredentials: credentials);
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
                await x.RunAsync(query, new 
                {
                    id = newId,
                    name = dto.Name,
                    surname = dto.Surname,
                    email = dto.Email,
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    phone = dto.Phone ?? "",
                    bio = dto.Bio ?? "",
                    address = dto.Address ?? "",
                    hasChildren = dto.HasChildren,
                    hasPets = dto.HasPets,
                    livingSpace = dto.LivingSpace ?? "",
                    //volunteer polja:
                    skills = dto.Skills ?? Array.Empty<string>(),
                    availableDays = dto.AvailableDays ?? Array.Empty<string>(),
                    joinedAt = DateTime.UtcNow.ToString("o"),
                    createdAt = DateTime.UtcNow.ToString("o")
                });
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
            var shelterId = await GetVolunteerShelterId(newId);
            return await GenerateResponseAsync(newId, dto.Email, dto.Role, shelterId);
        }

        public async Task<bool> RevokeTokenAsync(string refresh)
        {
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                                                MATCH (rt: RefreshToken {token: $token})
                                                SET rt.isRevoked = true",
                                                new { token = refresh });
            });
            return true;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refresh)
        {
            await using var session = _driver.AsyncSession();
            var tokenData = await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                                                MATCH (rt:RefreshToken {token: $token})
                                                MATCH (u:User {id: $rt.userId})
                                                OPTIONAL MATCH (u)-[:VOLUNTEERS_AT]->(s:Shelter)
                                                RETURN rt.userId AS userId,
                                                       rt.expiresAt AS expiresAt,
                                                       rt.isRevoked AS isRevoked,
                                                       rt.email AS email,
                                                       rt.role AS role,
                                                       rt.id AS shelterId",
                                                 new { token = refresh });
                if (!await pointer.FetchAsync()) return null;

                var record = pointer.Current;
                return new
                {
                    UserId = record["userId"].As<string>(),
                    ExpiresAt = DateTime.Parse(record["expiresAt"].As<string>()),
                    IsRevoked = record["isRevoked"].As<bool>(),
                    Email = record["email"].As<string>(),
                    Role = record["role"].As<string>(),
                    ShelterId = record.Keys.Contains("shelterId") ?
                                record["shelterId"].As<string>() : null
                };
            });
            if (tokenData == null)
                throw new UnauthorizedAccessException("Refresh Token nije validiran");
            if (tokenData.IsRevoked)
                throw new UnauthorizedAccessException("Refresh Token je pozvan");
            if (tokenData.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh Token je istekao");

            await RevokeTokenAsync(refresh);

            return await GenerateResponseAsync(
                tokenData.UserId,
                tokenData.Email,
                tokenData.Role,
                tokenData.ShelterId);
        }

        private async Task<AuthResponseDto> GenerateResponseAsync(string userId, string email, string role, string? shelterId)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);
            var accessToken = GenerateJwtToken(userId, email, role, shelterId, accessTokenExpiry);
            var refreshToken = await SaveRefreshToken(userId);

            var userInfo = await GetUserInfo(userId, role, shelterId);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                UserInfo = userInfo
            };
        }

        private async Task<string> SaveRefreshToken(string userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async x =>
            {
                await x.RunAsync(@"
                                   CREATE (rt:RefreshToken{
                                           token: $token,
                                           userId: $userId,
                                           expiresAt: $expiresAt,
                                           isRevoked: $isRevoked,
                                           createdAt: $createdAt})",
                                  new
                                  {
                                      token,
                                      userId,
                                      expiresAt = expiresAt.ToString("o"),
                                      createdAt = DateTime.UtcNow.ToString("o")
                                  });
            });
            return token;
        }

        private async Task<string> GetVolunteerShelterId(string volunteerId)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(@"
                                                MATCH (v:Volunteer {id: $id})-[:VOLUNTEERS_AT]->(s:Shelter)
                                                RETURN s.id AS shelterId",
                                                new
                                                {
                                                    id = volunteerId
                                                });
                if (!await pointer.FetchAsync()) return null;
                return pointer.Current["shelterId"].As<string>();
            });
        }
    }
}
