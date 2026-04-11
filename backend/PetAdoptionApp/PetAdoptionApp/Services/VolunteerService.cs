using Neo4j.Driver;
using PetAdoptionApp.DTOs.Volunteer;
using PetAdoptionApp.DTOs.Shelter;
using PetAdoptionApp.Interfaces;
using System.Linq.Expressions;

namespace PetAdoptionApp.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IDriver _driver;

        public VolunteerService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<VolunteerSummaryDto>> GetAllAsync()
        {
            var query = @"
                MATCH (v:Volunteer)
                OPTIONAL MATCH (v)-[:VOLUNTEERS_AT]->(s:Shelter)
                RETURN v, s.name AS shelterName";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query);
                var records = await cursor.ToListAsync();
                return records.Select(r => MapToSummary(
                    r["v"].As<INode>(),
                    r["shelterName"].As<string?>()
                    )).ToList();
            });
        }

        public async Task<List<VolunteerSummaryDto>> GetByShelterAsync(string shelterId)
        {
            var query = @"
                MATCH (v:Volunteer)-[:VOLUNTEERS_AT]->(s:Shelter {id: $shelterId})
                RETURN v, s.name AS shelterName";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { shelterId });
                var records = await cursor.ToListAsync();
                return records.Select(r => MapToSummary(
                    r["v"].As<INode>(),
                    r["shelterName"].As<string?>()
                    )).ToList();
            });
        }

        public async Task<VolunteerDto?> GetByIdAsync(string Id)
        {
            var query = @"
                MATCH (v:Volunteer {id: $id})
                OPTIONAL MATCH (v)-[:VOLUNTEERS_AT]->(s:Shelter)
                OPTIONAL MATCH (v)-[:LIKED]->(liked:Animal)
                OPTIONAL MATCH (v)-[:ADOPTED]->(adopted:Animal)
                RETURN v, s,
                        collect(DISTINCT liked.id) AS likedIds,
                        collect(DISTINCT adopted.id) AS adoptedIds";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { Id });
                if (!await cursor.FetchAsync())
                    return null;
                var record = cursor.Current;
                var node = record["v"].As<INode>();
                var shelterNode = record["s"].As<INode?>();

                var dto = MapToDetail(node, shelterNode);
                dto.LikedAnimalIds = record["likedIds"].As<List<string>>();
                dto.AdoptedAnimalIds = record["adoptedIds"].As<List<string>>();

                return dto;
            });
        }

        public async Task<VolunteerDto> CreateAsync(VolunteerCreateDto dto)
        {
            var createQuery = @"
                CREATE (v:User:Volunteer {
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
                    isAdmin:      false,
                    isActive:     true,
                    skills:       $skills,
                    availableDays: $availableDays,
                    joinedAt:     $joinedAt,
                    rating:       null
                })
                RETURN v";

            var newId = Guid.NewGuid().ToString();

            var parameters = new
            {
                id = newId,
                name = dto.Name,
                surname = dto.Surname,
                email = dto.Email,
                passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                phone = dto.Phone,
                bio = dto.Bio,
                address = dto.Address,
                hasChildren = dto.HasChildren,
                hasPets = dto.HasPets,
                livingSpace = dto.LivingSpace,
                skills = dto.Skills ?? Array.Empty<string>(),
                availableDays = dto.AvailableDays ?? Array.Empty<string>(),
                joinedAt = DateTime.UtcNow.ToString("o")
            };

            await using var session = _driver.AsyncSession();

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(createQuery, parameters);
            });

            if(!string.IsNullOrEmpty(dto.ShelterId))
            {
                await AssignToShelterAsync(newId, dto.ShelterId);
            }

            return (await GetByIdAsync(newId))!; //vraca kreiranog volontrea
        }

        public async Task<VolunteerDto?> UpdateAsync(string id, VolunteerUpdateDto dto)
        {
            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { ["id"] = id };

            if (dto.Name != null)
            { setClauses.Add("v.name = $name"); parameters["name"] = dto.Name; }
            if (dto.Surname != null)
            { setClauses.Add("v.surname = $surname"); parameters["surname"] = dto.Surname; }
            if (dto.Phone != null)
            { setClauses.Add("v.phone = $phone"); parameters["phone"] = dto.Phone; }
            if (dto.Bio != null)
            { setClauses.Add("v.bio = $bio"); parameters["bio"] = dto.Bio; }
            if (dto.Address != null)
            { setClauses.Add("v.address = $address"); parameters["address"] = dto.Address; }
            if (dto.HasChildren != null)
            { setClauses.Add("v.hasChildren = $hasChildren"); parameters["hasChildren"] = dto.HasChildren; }
            if (dto.HasPets != null)
            { setClauses.Add("v.hasPets = $hasPets"); parameters["hasPets"] = dto.HasPets; }
            if (dto.LivingSpace != null)
            { setClauses.Add("v.livingSpace = $livingSpace"); parameters["livingSpace"] = dto.LivingSpace; }

            if (dto.IsActive != null)
            { setClauses.Add("v.isActive = $isActive"); parameters["isActive"] = dto.IsActive; }
            if (dto.Skills != null)
            { setClauses.Add("v.skills = $skills"); parameters["skills"] = dto.Skills; }
            if (dto.AvailableDays != null)
            { setClauses.Add("v.availableDays = $availableDays"); parameters["availableDays"] = dto.AvailableDays; }

            if(!string.IsNullOrEmpty(dto.ShelterId))
            {
                await AssignToShelterAsync(id, dto.ShelterId);
            }

            if(setClauses.Any())
            {
                var query = $@"
                        MATCH (v:Volunteer {{id: $id}})
                        SET {string.Join(", ", setClauses)}";

                await using var session = _driver.AsyncSession();
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, parameters);
                });
            }

            return await GetByIdAsync(id);
        }

        public async Task<bool> AssignToShelterAsync(string volunteerId, string shelterId)
        {
            var query = @"
                MATCH (v:Volunteer {id: $volunteerId})
                OPTIONAL MATCH (v)-[old:VOLUNTEERS_AT]->()
                DELETE old
                WITH v
                MATCH (s:Shelter {id: $shelterId})
                CREATE (v)-[:VOLUNTEERS_AT]->(s)";

            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { volunteerId, shelterId });
            });

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var query = @"
                    MATCH (v:Volunteer {id: $id})
                    DETACH DELETE v
                    RETURN count(v) AS deleted";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { id });
                var record = await cursor.SingleAsync();
                return record["deleted"].As<int>() > 0;
            });
        }



        // HELPERI:

        private static VolunteerSummaryDto MapToSummary(INode node, string? shelterName) => new()
        {
            Id = node["id"].As<string>(),
            Name = node["name"].As<string>(),
            Surname = node["surname"].As<string>(),
            Email = node["email"].As<string>(),
            Phone = node.Properties.ContainsKey("phone")
                ? node["phone"].As<string?>() : null,
            Address = node["address"].As<string>(),
            IsActive = node["isActive"].As<bool>(),
            Rating = node.Properties.ContainsKey("rating")
                ? node["rating"].As<float?>() : null,
            ShelterName = shelterName
        };

        private static VolunteerDto MapToDetail(INode node, INode? shelterNode) => new()
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
                ? node["livingSpace"].As<string?>() : null,
            IsAdmin = node["isAdmin"].As<bool>(),
            IsActive = node["isActive"].As<bool>(),
            Rating = node.Properties.ContainsKey("rating")
                ? node["rating"].As<float?>() : null,
            Skills = node.Properties.ContainsKey("skills")
                ? node["skills"].As<string[]?>() : null,
            AvailableDays = node.Properties.ContainsKey("availableDays")
                ? node["availableDays"].As<string[]?>() : null,
            JoinedAt = node.Properties.ContainsKey("joinedAt")
                ? DateTime.Parse(node["joinedAt"].As<string>()) : null,
            Shelter = shelterNode == null ? null : new ShelterDto
            {
                id = shelterNode["id"].As<string>(),
                name = shelterNode["name"].As<string>(),
                address = shelterNode["address"].As<string>(),
                phone = shelterNode["phone"].As<string>(),
                email = shelterNode["email"].As<string>(),
                capacity = shelterNode["capacity"].As<int>(),
                rating = shelterNode.Properties.ContainsKey("rating")
                    ? shelterNode["rating"].As<float?>() : null
            }
        };
    }
}
