using Neo4j.Driver;
using PetAdoptionApp.Interfaces;
using PetAdoptionApp.DTOs.Shelter;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace PetAdoptionApp.Services
{
    public class ShelterService : IShelterService
    {
        private IDriver _driver;

        public ShelterService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<ShelterDto> CreateShelterAsync(ShelterDto createDto)
        {
            var newId = Guid.NewGuid().ToString();

            var query = @"
                CREATE (s:Shelter {
                    id: $id, 
                    name: $name,
                    address: $address,
                    phone: $phone,
                    email: $email,
                    capacity: $capacity,
                    description: $description,
                    rating: 0.0
                })
                RETURN s";

            var parameters = new
            {
                id = newId,
                name = createDto.name,
                address = createDto.address,
                phone = createDto.phone,
                email = createDto.email,
                capacity = createDto.capacity,
                description = createDto.description,
            };

            // Izvrsavanje i rucno mapiranje:
            await using var session = _driver.AsyncSession();
            var result = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();
                var node = record["s"].As<INode>();

                return new ShelterDto
                {
                    id = node.Properties["id"].As<string>(),
                    name = node.Properties["name"].As<string>(),
                    address = node.Properties["address"].As<string>(),
                    phone = node.Properties["phone"].As<string>(),
                    email = node.Properties["email"].As<string>(),
                    capacity = node.Properties["capacity"].As<int>(),
                    rating = node.Properties["rating"].As<float>(),
                    description = node.Properties["description"].As<string>()
                };

            });

            return result;
        }

        public async Task<List<ShelterDto>> GetAllSheltersAsync()
        {
            var query = "MATCH (s:Shelter) RETURN s";
            var shelters = new List<ShelterDto>();

            await using var session = _driver.AsyncSession();
            await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query);
                await cursor.ForEachAsync(record =>
                {
                    var node = record["s"].As<INode>();
                    shelters.Add(new ShelterDto
                    {
                        id = node.Properties["id"].As<string>(),
                        name = node.Properties["name"].As<string>(),
                        address = node.Properties["address"].As<string>(),
                        phone = node.Properties["phone"].As<string>(),
                        email = node.Properties["email"].As<string>(),
                        capacity = node.Properties["capacity"].As<int>(),
                        rating = node.Properties["rating"].As<float>(),
                        description = node.Properties["description"].As<string>()
                    });
                });
            });

            return shelters;
        }

        public async Task<ShelterDto?> GetShelterByIdAsync(string id)
        {
            var query = "MATCH (s: Shelter {id: $id}) RETURN s";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { id });

                if (await cursor.FetchAsync())
                {
                    var node = cursor.Current["s"].As<INode>();
                    return new ShelterDto
                    {
                        id = node.Properties["id"].As<string>(),
                        name = node.Properties["name"].As<string>(),
                        address = node.Properties["address"].As<string>(),
                        phone = node.Properties["phone"].As<string>(),
                        email = node.Properties["email"].As<string>(),
                        capacity = node.Properties["capacity"].As<int>(),
                        rating = node.Properties["rating"].As<float>(),
                        description = node.Properties["description"].As<string>()
                    };
                }
                return null; //ovo ako nije pronadjen
            });
        }

        public async Task<ShelterDto?> UpdateShelterAsync(string id, ShelterDto updateDto)
        {
            var query = @"
                MATCH (s:Shelter {id: $id})
                SET s.name = $name,
                    s.address = $address,
                    s.phone = $phone,
                    s.email = $email,
                    s.capacity = $capacity,
                    s.rating = $rating,
                    s.description = $description
                RETURN s";

            var parameters = new
            {
                id = id,
                name = updateDto.name,
                address = updateDto.address,
                phone = updateDto.phone,
                email = updateDto.email,
                capacity = updateDto.capacity,
                rating = updateDto.rating,
                description = updateDto.description,
            };

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);

                if (await cursor.FetchAsync())
                {
                    var node = cursor.Current["s"].As<INode>();
                    return new ShelterDto
                    {
                        id = node.Properties["id"].As<string>(),
                        name = node.Properties["name"].As<string>(),
                        address = node.Properties["address"].As<string>(),
                        phone = node.Properties["phone"].As<string>(),
                        email = node.Properties["email"].As<string>(),
                        capacity = node.Properties["capacity"].As<int>(),
                        rating = node.Properties["rating"].As<float>(),
                        description = node.Properties["description"].As<string>()
                    };
                }
                return null;
            });
        }

        public async Task<bool> DeleteShelterAsync(string id)
        {
            //DETACH DELETE - brisace cvor i sve relacije (veze) koje taj cvor ima sa drugim cvorovima.
            var query = @"
                MATCH (s:Shelter {id: $id})

                OPTIONAL MATCH (v:Volunteer)-[vr:VOLUNTEERS_AT]->(s)
                DELETE vr

                WITH s
                OPTIONAL MATCH (a:Animal)-[:HOUSED_IN]->(s)
                OPTIONAL MATCH (req:AdoptionRequest)-[:FOR]->(a)
                
                WITH s,
                        collect(DISTINCT a) AS animals,
                        collect(DISTINCT req) AS requests

                FOREACH (r IN requests | DETACH DELETE r)
                WITH s, animals
                FOREACH (a IN animals | DETACH DELETE a)
                
                WITH s
                DETACH DELETE s";

            await using var session = _driver.AsyncSession();
            var exists = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    "MATCH (s:Shelter {id: $id}) RETURN count(s) > 0 AS exists",
                    new { id });
                var record = await cursor.SingleAsync();
                return record["exists"].As<bool>();
            });

            if (!exists) return false;

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { id });
            });

            return true;
        }
    }
}
