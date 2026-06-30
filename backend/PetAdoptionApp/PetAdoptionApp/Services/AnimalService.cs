// Services/AnimalService.cs
using Neo4j.Driver;
using PetAdoptionApp.Common;
using PetAdoptionApp.DTOs.Animal;
using PetAdoptionApp.Interfaces;

namespace PetAdoptionApp.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IDriver _driver;
        private readonly IWebHostEnvironment _env;

        public AnimalService(IDriver driver, IWebHostEnvironment env)
        {
            _driver = driver;
            _env = env;
        }


        public async Task<AnimalDetailResponseDto> CreateAnimalAsync(AnimalCreateDto dto)
        {
            var newId = Guid.NewGuid().ToString();
            var query = @"
                MATCH (s:Shelter {id: $shelterId})
                MATCH (v:Volunteer {id: $caretakerId})
                CREATE (a:Animal {
                    id:           $id,
                    name:         $name,
                    species:      $species,
                    breed:        $breed,
                    age:          $age,
                    gender:       $gender,
                    size:         $size,
                    isVaccinated:  $isVaccinated,
                    isSterilized:  $isSterilized,
                    isGoodWithKids: $isGoodWithKids,
                    isGoodWithPets: $isGoodWithPets,
                    description:  $description,
                    isAdopted:    false,
                    arrivedAt:    $arrivedAt
                })
                CREATE (a)-[:HOUSED_IN]->(s)
                CREATE (a)-[:CARED_BY]->(v)
                RETURN a";

            var parameters = new
            {
                id = newId,
                name = dto.Name,
                species = dto.Species,
                breed = dto.Breed,
                age = dto.Age,
                gender = dto.Gender.ToString(),
                size = dto.Size.ToString(),
                isVaccinated = dto.IsVaccinated.ToString(),
                isSterilized = dto.IsSterilized.ToString(),
                isGoodWithKids = dto.IsGoodWithKids.ToString(),
                isGoodWithPets = dto.IsGoodWithPets.ToString(),
                description = dto.Description ?? "",
                arrivedAt = dto.ArrivedAt.ToString("yyyy-MM-dd"),
                shelterId = dto.ShelterId,
                caretakerId = dto.CaretakerId
            };

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();
                return MapToDetail(record["a"].As<INode>());
            });
        }


        public async Task<IEnumerable<AnimalResponseDto>> GetAllAnimalsAsync(string shelterId)
        {
            var query = @"
                MATCH (s:Shelter {id: $shelterId})<-[:HOUSED_IN]-(a:Animal)
                WHERE a.isAdopted = false
                RETURN a";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { shelterId });
                var records = await cursor.ToListAsync();
                return records.Select(r => MapToResponse(r["a"].As<INode>()));
            });
        }

        public async Task<AnimalResponseDto?> GetAnimalByIdAsync(string animalId)
        {
            var query = @"
                MATCH (a:Animal {id: $animalId})
                RETURN a";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { animalId });
                if (!await cursor.FetchAsync()) return null;
                return MapToResponse(cursor.Current["a"].As<INode>());
            });
        }


        public async Task<bool> DeleteAnimalAsync(string animalId)
        {
            await using var session = _driver.AsyncSession();

            var exists = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    "MATCH (a:Animal {id: $animalId}) RETURN count(a) > 0 AS exists",
                    new { animalId });
                var record = await cursor.SingleAsync();
                return record["exists"].As<bool>();
            });

            if (!exists) return false;

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(
                    "MATCH (a:Animal {id: $animalId}) DETACH DELETE a",
                    new { animalId });
            });

            return true;
        }

        public async Task<bool> MarkAsAdoptedAsync(string animalId, string userId)
        {
            var query = @"
                MATCH (a:Animal {id: $animalId})
                MATCH (u:User {id: $userId})
                SET a.isAdopted = true
                MERGE (u)-[:ADOPTED]->(a)
                RETURN count(a) > 0 AS adopted";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { animalId, userId });
                var record = await cursor.SingleAsync();
                return record["adopted"].As<bool>();
            });
        }

        public async Task<bool> AddCaretakerAsync(string animalId, string caretakerId)
        {
            var query = @"
                MATCH (a:Animal {id: $animalId})
                MATCH (v:Volunteer {id: $caretakerId})
                MERGE (a)-[:CARED_BY]->(v)
                RETURN count(a) > 0 AS added";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { animalId, caretakerId });
                var record = await cursor.SingleAsync();
                return record["added"].As<bool>();
            });
        }

        public async Task<bool> AddRelativeAsync(string animalId, string relativeId)
        {
            var query = @"
                MATCH (a:Animal {id: $animalId})
                MATCH (r:Animal {id: $relativeId})
                MERGE (a)-[:RELATED]->(r)
                RETURN count(a) > 0 AS added";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { animalId, relativeId });
                var record = await cursor.SingleAsync();
                return record["added"].As<bool>();
            });
        }


        public async Task<string?> AddImageAsync(string animalId, IFormFile file)
        {
            var countQuery = @"
                MATCH (a:Animal {id: $animalId})
                RETURN coalesce(size(a.images), 0) AS imageCount";

            await using var session = _driver.AsyncSession();

            var currentCount = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(countQuery, new { animalId });
                if (!await cursor.FetchAsync()) return -1;
                return cursor.Current["imageCount"].As<int>();
            });

            if (currentCount == -1) return null;
            if (currentCount >= 5) return null;

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var absolutePath = Path.Combine(_env.ContentRootPath, "images", uniqueFileName);
            var relativePath = Path.Combine("images", uniqueFileName);

            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var updateQuery = @"
                MATCH (a:Animal {id: $animalId})
                SET a.images = coalesce(a.images, []) + [$imagePath]";

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(updateQuery, new { animalId, imagePath = relativePath });
            });

            return relativePath;
        }

        public async Task<bool> RemoveImageAsync(string animalId, string fileName)
        {
            var filePath = Path.Combine("images", fileName);

            var query = @"
                MATCH (a:Animal {id: $animalId})
                WHERE $filePath IN coalesce(a.images, [])
                SET a.images = [img IN a.images WHERE img <> $filePath]
                RETURN count(a) > 0 AS removed";

            await using var session = _driver.AsyncSession();

            var removed = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { animalId, filePath });
                if (!await cursor.FetchAsync()) return false;
                return cursor.Current["removed"].As<bool>();
            });

            if (!removed) return false;

            var absolutePath = Path.Combine(_env.ContentRootPath, filePath);
            if (File.Exists(absolutePath))
                File.Delete(absolutePath);

            return true;
        }

        // MAPPING HELPERI 

        private static AnimalResponseDto MapToResponse(INode node)
        {
            var images = node.Properties.ContainsKey("images")
                ? node["images"].As<List<object>>().Select(x => x.ToString()!).ToList()
                : new List<string>();

            return new AnimalResponseDto
            {
                id = node["id"].As<string>(),
                name = node["name"].As<string>(),
                breed = node["breed"].As<string>(),
                gender = Enum.Parse<Enums.Gender>(node["gender"].As<string>()),
                age = node.Properties.ContainsKey("age")
                    ? node["age"].As<int?>() : null,
                isVaccinated = Enum.Parse<Enums.AnimalBoolean>(
                    node["isVaccinated"].As<string>()),
                isSterilized = Enum.Parse<Enums.AnimalBoolean>(
                    node["isSterilized"].As<string>()),
                primaryImgUrl = images.FirstOrDefault()
            };
        }

        private static AnimalDetailResponseDto MapToDetail(INode node)
        {
            var images = node.Properties.ContainsKey("images")
                ? node["images"].As<List<object>>().Select(x => x.ToString()!).ToList()
                : new List<string>();

            return new AnimalDetailResponseDto
            {
                id = node["id"].As<string>(),
                name = node["name"].As<string>(),
                species = node["species"].As<string>(),
                breed = node["breed"].As<string>(),
                age = node.Properties.ContainsKey("age")
                    ? node["age"].As<int?>() : null,
                gender = Enum.Parse<Enums.Gender>(node["gender"].As<string>()),
                size = Enum.Parse<Enums.Size>(node["size"].As<string>()),
                isVaccinated = Enum.Parse<Enums.AnimalBoolean>(
                    node["isVaccinated"].As<string>()),
                isSterilized = Enum.Parse<Enums.AnimalBoolean>(
                    node["isSterilized"].As<string>()),
                description = node.Properties.ContainsKey("description")
                    ? node["description"].As<string?>() : null,
                arrivedAt = node.Properties.ContainsKey("arrivedAt")
                    ? DateTime.Parse(node["arrivedAt"].As<string>()) : DateTime.UtcNow,
                images = images
            };
        }
    }
}