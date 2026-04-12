using Neo4j.Driver;
using PetAdoptionApp.Common;
using PetAdoptionApp.DTOs.Animal;
using PetAdoptionApp.DTOs.MedicalRecord;
using PetAdoptionApp.Interfaces;
using PetAdoptionApp.Models;
using System.Reflection;

namespace PetAdoptionApp.Services
{
    public class AnimalService : IAnimalService
    {
        private IDriver _driver;
        public AnimalService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> AddCaretakerAsync(string animalId, AnimalAddCaretakerDto dto)
        {
            var query = @"
                    MATCH (a: Animal {id: $animalId})
                    MATCH (v: Volunteer {id: $volunteerId})
                    MERGE (a)-[:CARED_BY]->(v)
                    RETURN count(a) > 0 AS caredBy
                    ";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new
                {
                    animalId,
                    volunteerId = dto.caretakerId
                });
                var result = await pointer.SingleAsync();

                return result["caredBy"].As<bool>(); //nisam sigurna da moze ovako, treba da se proveri.
            });

        }
     
        public async Task<AnimalDetailResponseDto> CreateAnimalAsync(AnimalCreateDto dto)
        {
           var newId = Guid.NewGuid().ToString();
            var query = @"
                MATCH (s: Shelter {id: $shelterId})
                MATCH (v: Volunteer {id: $caretakerId})
                CREATE(a: Animal {
                    id: $id,
                    name: $name,
                    species: $species,
                    breed: $breed,
                    age: $age, 
                    gender: $gender,                 
                    size: $size,
                    isVaccinated: $isVaccinated,
                    isSterilized: $isSterilized,
                    isGoodWithKids: $isGoodWithKids,
                    isGoodWithPets: $isGoodWithPets,
                    description: $description,
                    isAdopted: false,
                    arrivedAt: datetime($arrivedAt)
                })
                CREATE (a)-[:HOUSED_IN]->(s)
                CREATE (a)-[:CARED_BY]->(v)
                RETURN a, s.id as shelterId, v.id as caretakerId
                ";
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
                description = dto.Description,
                arrivedAt = dto.ArrivedAt.ToString("yyyy-MM-dd"),
                shelterId = dto.ShelterId,
                caretakerId = dto.CaretakerId
            };
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async res =>
            {
                var cursor = await res.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();
                return MapNodeToAnimalDetailResponse(record["a"].As<INode>());
            });
        }

        public async Task<bool> DeleteAnimalAsync(string animalId)
        {
            //to do: razmisli o tome sta se brise, a sta treba da se sacuva od podataka kad se obrise zivotinja.
            var query = @"
                MATCH (a: Animal {id: $animalId})
                DETACH DELETE a
                ";
            await using var session = _driver.AsyncSession();
            var exists = await session.ExecuteWriteAsync(async x =>
            {
                //rezultat upita imenujemo kao exists.
                var pointer = await x.RunAsync(
                    "MATCH (a: Animal {id: $animalId})" +
                    "RETURN count(a) > 0 AS exists",
                    new { animalId });
                var result = await pointer.SingleAsync();
                return result["exists"].As<bool>();
            });
            if (!exists) return false;
            await session.ExecuteWriteAsync(async x =>
            {
                await x.RunAsync(query, new { animalId });
            });
            return true;
        }

        public async Task<bool> MarkAsAdoptedAsync(string animalId, AnimalAdoptedDto dto)
        {
            var query = @"
                MATCH (a: Animal {id: $animalId})
                SET a.isAdopted = true
                RETURN count(a)>0 AS adopted
                ";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new { animalId });
                var result = await pointer.SingleAsync();
                return result["adopted"].As<bool>();
            });
        }

        public async Task<IEnumerable<AnimalResponseDto>> ReturnAllAnimals(string shelterId)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async x =>
            {
                var query = @"
                    MATCH (s: Shelter {id: $shelterId})<-[:HOUSED_IN]-(a:Animal)
                    WHERE a.isAdopted = false
                    RETURN a
                    ";
                var cursor = await x.RunAsync(query, new { shelterId });
                var animals = new List<AnimalResponseDto>();
                while (await cursor.FetchAsync())
                    animals.Add(MapNodeToAnimalResponse(cursor.Current["a"].As<INode>()));
                return animals;
            }); 
        }

        public async Task<AnimalResponseDto> ReturnAnimalId(string animalId)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async x =>
            {
                var query = @"
                    MATCH (a: Animal {id = $animalId})
                    RETURN a
                    ";
                var cursor = await x.RunAsync(query, new { animalId });
                var record = await cursor.SingleAsync();
                return MapNodeToAnimalResponse(record["a"].As<INode>());
            });
        }

        public async Task<bool> UpdateAnimalImages(string animalId, AnimalUpdateImagesDto dto)
        {
            //proveriti implementaciju.
            if (dto.Images.Count > 5)
                return false;
            var query = @"
                MATCH (a: Animal {id: $animalId})
                SET a.images = $images
                RETURN count(a) > 0 AS imagesUpdate
                ";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new { animalId, images = dto.Images });

                var result = await pointer.SingleAsync();
                return result["imagesUpdate"].As<bool>();
            });
        }

        public async Task<string?> AddImageAsync(string animalId, IFormFile file)
        {
            // provera koliko slika vec ima:
            var countQry = @"
                MATCH (a:Animal {id: $animalId})
                RETURN coalesce(size(a.images), 0) AS imageCount";
            // coalesce() - fja koja prima listu izraza i vraca prvu vrednost koja nije null

            await using var session = _driver.AsyncSession();

            var currentCount = await session.ExecuteReadAsync(async tx =>
            {
                var pointer = await tx.RunAsync(countQry, new { animalId });
                if (!await pointer.FetchAsync())
                    return -1; //zivotinja ne postoji
                return pointer.Current["imageCount"].As<int>();
            });

            if (currentCount == -1)
                return null; // zivotinja nije pronadjena
            if (currentCount >= 5)
                return null; // ima vec 5 slika

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine("images", uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var updateQry = @"
                MATCH (a:Animal {id: $animalId})
                SET a.images = coalesce(a.images, []) + [$imagePath]
                RETURN a.images AS images";

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(updateQry, new { animalId, imagePath = filePath });
            });

            return filePath;
        }

        public async Task<bool> RemoveImageAsync(string animalId, string fileName)
        {
            // filename je ime fajla, a filepath je putanja koja se cuva u bazi
            var filePath = Path.Combine("images", fileName);

            // uklanjamo iz neo4j liste
            var query = @"
                MATCH (a:Animal {id: $animalId})
                WHERE $filePath IN coalesce(a.images, [])
                SET a.images = [img IN a.images WHERE img <> $filePath]
                RETURN count(a) > 0 AS removed";

            await using var session = _driver.AsyncSession();

            var removedFromDb = await session.ExecuteWriteAsync(async tx =>
            {
                var pointer = await tx.RunAsync(query, new { animalId, filePath });
                if (!await pointer.FetchAsync()) return false;
                return pointer.Current["removed"].As<bool>();
            });

            if (!removedFromDb) return false;

            // brisemo fajl sa diska
            if (File.Exists(filePath))
                File.Delete(filePath);

            return true;
        }

        public async Task<bool> AddRelativeToAnimal(string animalId, AnimalAddRelativeDto dto)
        {
            var query = @"
                MATCH (a:Animal {id:$animalId})
                MATCH (related:Animal {id:$relativeId})
                MERGE (a)-[:RELATED]->(related)
                RETURN count(a) > 0 AS exists";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new
                {
                    animalId,
                    relatedId = dto.RelativeId
                });

                var result = await pointer.SingleAsync();
                return result["exists"].As<bool>();
            });
        }

        //pomocne funkcije:
        private AnimalResponseDto MapNodeToAnimalResponse(INode node)
        {
            var allImages = node.Properties.ContainsKey("images") ? node.Properties["images"].As<List<string>>() :
               new List<string>();
            string firstImage = allImages.Count > 0 ? allImages[0] : null;

            return new AnimalResponseDto
            {
                id = node.Properties["id"].As<string>(),
                breed = node.Properties["breed"].As<string>(),
                name = node.Properties["name"].As<string>(),
                gender = Enum.Parse<Enums.Gender>(node.Properties["gender"].As<string>()),
                age = node.Properties.ContainsKey("age") ? node.Properties["age"].As<int>() : null,
                isVaccinated = Enum.Parse<Enums.AnimalBoolean>(node.Properties["isVaccinated"].As<string>()),
                isSterilized = Enum.Parse<Enums.AnimalBoolean>(node.Properties["isSterilized"].As<string>()),
                primaryImgUrl = firstImage
            };
        }

        private AnimalDetailResponseDto MapNodeToAnimalDetailResponse(INode node)
        {
            return new AnimalDetailResponseDto
            {
                id = node.Properties["id"].As<string>(),
                name = node.Properties["name"].As<string>(),
                species = node.Properties["species"].As<string>(),
                breed = node.Properties["breed"].As<string>(),
                age = node.Properties.ContainsKey("age") ? node.Properties["age"].As<int>() : null,

                gender = Enum.Parse<Enums.Gender>(node.Properties["gender"].As<string>()),
                size = Enum.Parse<Enums.Size>(node.Properties["size"].As<string>()),

                isVaccinated = Enum.Parse<Enums.AnimalBoolean>(node.Properties["isVaccinated"].As<string>()),
                isSterilized = Enum.Parse<Enums.AnimalBoolean>(node.Properties["isSterilized"].As<string>()),
                description = node.Properties["description"].As<string>(),
                arrivedAt = DateTime.Parse(node.Properties["arrivedAt"].As<string>()),
                images = node.Properties.ContainsKey("images") ? node.Properties["images"].As<List<string>>() : new List<string>(),
            };
        }
    }
}
