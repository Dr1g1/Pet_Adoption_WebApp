using Neo4j.Driver;
using PetAdoptionApp.Interfaces;
using PetAdoptionApp.DTOs.User;
using System.Xml;

namespace PetAdoptionApp.Services
{
    public class UserService : IUserService
    {
        private readonly IDriver _driver;

        public UserService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<UserSummaryDto>> GetAllAsync()
        {
            var query = @"
                MATCH (u:User)
                RETURN u";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query);
                var records = await cursor.ToListAsync();
                return records.Select(r => MapToSummary(r["u"].As<INode>())).ToList();
            });
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var query = @"
                MATCH (u:User {id: $id})
                WHERE NOT u:Volunteer
                OPTIONAL MATCH (u)-[:LIKED]->(liked:Animal)
                OPTIONAL MATCH (u)-[:ADOPTED]->(adopted:Animal)
                RETURN u,
                        collect(DISTINCT liked.id) AS likedIds,
                        collect(DISTINCT adopted.id) AS adoptedIds";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { id });

                if (!await cursor.FetchAsync())
                    return null;

                var record = cursor.Current;
                var node = record["u"].As<INode>();

                var dto = MapToDetail(node);
                dto.LikedAnimalIds = record["likedIds"].As<List<string>>();
                dto.AdoptedAnimalIds = record["adoptedIds"].As<List<string>>();

                return dto;
            });
        }

        public async Task<UserDto> CreateAsync(UserCreateDto dto)
        {
            var query = @"
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
                    livingSpace: $livingSpace
                })
                RETURN u";

            var parameters = new
            {
                id = Guid.NewGuid().ToString(),
                name = dto.Name,
                surname = dto.Surname,
                email = dto.Email,
                passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                phone = dto.Phone,
                bio = dto.Bio,
                address = dto.Address,
                hasChildren = dto.HasChildren,
                hasPets = dto.HasPets,
                livingSpace = dto.LivingSpace
            };

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();
                return MapToDetail(record["u"].As<INode>());
            });
        }

        public async Task<UserDto?> UpdateAsync(string id, UserUpdateDto dto)
        {
            var setClauses = new List<string>();// menjamo samo polja koja nisu null
            var parameters = new Dictionary<string, object?> { ["id"] = id };

            if (dto.Name != null)
            { setClauses.Add("u.name = $name"); parameters["name"] = dto.Name; }
            if (dto.Surname != null)
            { setClauses.Add("u.surname = $surname"); parameters["surname"] = dto.Surname; }
            if (dto.Phone != null)
            { setClauses.Add("u.phone = $phone"); parameters["phone"] = dto.Phone; }
            if (dto.Bio != null)
            { setClauses.Add("u.bio = $bio"); parameters["bio"] = dto.Bio; }
            if (dto.Address != null)
            { setClauses.Add("u.address = $address"); parameters["address"] = dto.Address; }
            if (dto.HasChildren != null)
            { setClauses.Add("u.hasChildren = $hasChildren"); parameters["hasChildren"] = dto.HasChildren; }
            if (dto.HasPets != null)
            { setClauses.Add("u.hasPets = $hasPets"); parameters["hasPets"] = dto.HasPets; }
            if (dto.LivingSpace != null)
            { setClauses.Add("u.livingSpace = $livingSpace"); parameters["livingSpace"] = dto.LivingSpace; }

            if (!setClauses.Any())
                return await GetByIdAsync(id);

            var query = $@"
                MATCH (u:User {{id: $id}})
                WHERE NOT u:Volunteer
                SET {string.Join(", ", setClauses)}
                RETURN u";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);

                if (!await cursor.FetchAsync()) return null;
                return MapToDetail(cursor.Current["u"].As<INode>());
            });
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var query = @"
                MATCH (u:User {id: $id})
                WHERE NOT u:Volunteer
                DETACH DELETE u
                RETURN count(u) AS deleted";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { id });
                var record = await cursor.SingleAsync();
                return record["deleted"].As<int>() > 0;
            });
        }


        // HELPERI:

        private static UserSummaryDto MapToSummary(INode node) => new()
        {
            Id = node["id"].As<string>(),
            Name = node["name"].As<string>(),
            Surname = node["surname"].As<string>(),
            Email = node["email"].As<string>(),
            Phone = node.Properties.ContainsKey("phone") ? node["phone"].As<string?>():null,
            Address = node["address"].As<string>()
        };

        private static UserDto MapToDetail(INode node) => new()
        {
            Id = node["id"].As<string>(),
            Name = node["name"].As<string>(),
            Surname = node["surname"].As<string>(),
            Email = node["email"].As<string>(),
            Phone = node.Properties.ContainsKey("phone")
                ? node["phone"].As<string?>() : null,
            Bio = node.Properties.ContainsKey("bio")
                ? node["bio"].As<string?>() : null,
            Address = node["address"].As<string>(),
            HasChildren = node["hasChildren"].As<bool>(),
            HasPets = node["hasPets"].As<bool>(),
            LivingSpace = node.Properties.ContainsKey("livingSpace")
                ? node["livingSpace"].As<string?>() : null
        };
    }
}
