using PetAdoptionApp.Interfaces;
using Neo4j.Driver;
using PetAdoptionApp.DTOs.Animal;
using PetAdoptionApp.Models;
namespace PetAdoptionApp.Services
{
    public class AnimalService : IAnimalService
    {
        private IDriver _driver;
        public AnimalService(IDriver driver)
        {
            _driver = driver;
        }

         public Task<bool> AddCaretakerAsync(string animalId, AnimalAddCaretakerDto dto)
        {
            throw new NotImplementedException();
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
                    arrivedAt: datetime($arrivedAt),
                })
                CREATE (a)-[HOUSED_IN]->(s)
                CREATE (a)-[CARED_BY]->(v)
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
                var node = record["a"].As<INode>();
                return new AnimalDetailResponseDto
                {
                    id = node.Properties["id"].As<string>(),
                    name = node.Properties["name"].As<string>(),

                };
            });
        }

        public Task<bool> DeleteAnimalAsync(string animalId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkAsAdoptedAsync(string animalId, AnimalAdoptedDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<AnimalResponseDto> ReturnAllAnimals(string shelterId)
        {
            throw new NotImplementedException();
        }

        public Task<AnimalResponseDto> ReturnAnimalId(string animalId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAnimalImages(string animalId, AnimalUpdateImagesDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
