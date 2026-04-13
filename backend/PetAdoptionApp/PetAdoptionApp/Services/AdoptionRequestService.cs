using PetAdoptionApp.DTOs.AdoptionRequest;
using PetAdoptionApp.Interfaces;
using Neo4j.Driver;
using PetAdoptionApp.Common;

namespace PetAdoptionApp.Services
{
    public class AdoptionRequestService : IAdoptionRequestService
    {
        private IDriver _driver;
        public AdoptionRequestService(IDriver driver)
        {
            _driver = driver;
        } 

        public async Task<bool> DeleteRequestAsync(string requestId, string userId)
        {
            var query = @"
                MATCH (u:User {id:$userId})-[:REQUESTED]->(ar:AdoptionRequest {id:$requestId})
                WHERE ar.status='Pending'
                WITH ar, count(ar) > 0 AS deleted
                DETACH DELETE ar
                RETURN deleted";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new { requestId, userId });
                var result = await pointer.SingleAsync();
                return result["deleted"].As<bool>();
            });

        }

        public async Task<AdoptionRequestUserResponseDto> CreateAdoptionRequestAsync(string userId, AdoptionRequestUserCreateDto dto)
        {
            //ovaj adoption request kreira korisnik kada hoce da usvoji neku zivotinju.
            var newId = Guid.NewGuid().ToString();
            var query = @"
                MATCH (u:User {id:$userId})
                MATCH (a:Animal {id:$animalId})-[:HOUSED_IN]->(s:Shelter)
                CREATE (ar:AdoptionRequest
                {
                    id: $id,
                    status: 'Pending',
                    createdAt: datetime(),
                    message: $message
                })
                CREATE (u)-[:REQUESTED]->(ar)
                CREATE (ar)-[:FOR]->(a)
                CREATE (ar)-[:REVIEWED_BY]->(s)
                RETURN ar, a.id AS animalId, s.id AS shelterId";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new
                {
                    userId,
                    id = newId,
                    animalId = dto.animalId,
                    message = dto.message
                });
                var result = await pointer.SingleAsync();
                var node = result["ar"].As<INode>();
                return new AdoptionRequestUserResponseDto
                {
                    RequestId = node["id"].As<string>(),
                    Status = Enum.Parse<Enums.Status>(node["status"].As<string>()),
                    CreatedAt = DateTime.Parse(node["createdAt"].As<string>()),
                    AnimalId = result["animalId"].As<string>(),
                    ShelterId = result["shelterId"].As<string>()
                };
            });
        }

        public async Task<IEnumerable<AdoptionRequestReturnDto>> GetPendingRequests(string shelterId)
        {
            var query = @"
                MATCH (u: User)-[:REQUESTED]->(ar: AdoptionRequest)-[:REVIEWED_BY]->(s: Shelter {id: $shelterId})
                MATCH (ar)-[:FOR]->(a:Animal)
                WHERE ar.status = 'Pending'
                RETURN ar, u, a";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new { shelterId });
                var requests = new List<AdoptionRequestReturnDto>();

                while (await pointer.FetchAsync())
                {
                    var ar = pointer.Current["ar"].As<INode>();
                    var u = pointer.Current["u"].As<INode>();
                    var a = pointer.Current["a"].As<INode>();

                    requests.Add(new AdoptionRequestReturnDto
                    {
                        RequestId = ar["id"].As<string>(),
                        UserId = u["id"].As<string>(),
                        UserName = u["name"].As<string>(),
                        AnimalId = a["id"].As<string>(),
                        AnimalName = a["name"].As<string>(),
                        Status = Enum.Parse<Enums.Status>(ar["status"].As<string>()),
                        CreatedAt = DateTime.Parse(ar["createdAt"].As<string>())
                    });
                }
                return requests;
            });
        }

        public async Task<IEnumerable<AdoptionRequestReturnDto>> GetRequestsForAnimal(string animalId)
        {
            var query = @"
                MATCH (u:User)-[:REQUESTED]->(ar:AdoptionRequest)-[:FOR]->(a:Animal {id:$animalId})
                RETURN u, ar, a";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new { animalId });
                var requests = new List<AdoptionRequestReturnDto>();

                while(await pointer.FetchAsync())
                {
                    var ar = pointer.Current["ar"].As<INode>();
                    var a = pointer.Current["a"].As<INode>();
                    var u = pointer.Current["u"].As<INode>();

                    requests.Add(new AdoptionRequestReturnDto
                    {
                        RequestId = ar["id"].As<string>(),
                        UserId = u["id"].As<string>(),
                        UserName = u["name"].As<string>(),
                        AnimalId = a["id"].As<string>(),
                        AnimalName = a["name"].As<string>(),
                        Status = Enum.Parse<Enums.Status>(ar["status"].As<string>()),
                        CreatedAt = DateTime.Parse(ar["createdAt"].As<string>())
                    });
                }
                return requests;
            });
        }

        public async Task<bool> RejectRequestAsync(AdoptionRequestActionDto dto, string shelterId)
        {
            //kada admin odbije zahtev koji je poslao korisnik.
            var query = @"
                MATCH (ar:AdoptionRequest {id:$requestId})-[:REVIEWED_BY]->(s:Shelter {id:$shelterId})
                WHERE ar.status='Pending'
                WITH count(ar) > 0 AS rejected
                SET ar.status = 'Rejected',
                    ar.message = $responseMessage,
                    ar.updatedAt = datetime()
                RETURN rejected";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new
                {
                    requestId = dto.RequestId,
                    responseMessage = dto.ResponseMessage,
                    shelterId
                });
                var result = await pointer.SingleAsync();
                return result["rejected"].As<bool>();
            });
        }

        public async Task<bool> ApprovedRequestAsync(AdoptionRequestActionDto dto, string shelterId)
        {
            //funkcija kad se prihvati zahtev za usvajanje jedne zivotinje.
            var query = @"
                MATCH (ar:AdoptionRequest {id:$requestId})-[:REVIEWED_BY]->(s:Shelter {id:$shelterId})
                WHERE ar.status='Pending'
                SET ar.status='Approved',
                    ar.message=$responseMessage,
                    ar.updatedAt=datetime()
                WITH ar
                MATCH(ar)-[:FOR]->(a:Animal)
                WITH a, ar
                OPTIONAL MATCH (other:AdoptionRequest)-[:FOR]->(a)
                WHERE other.id<>ar.id AND other.status='Pending'
                SET other.status='Rejected',
                    other.updatedAt=datetime()
                WITH a
                SET a.isAdopted=true
                RETURN count(a)>0 AS approved";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new
                {
                    requestId = dto.RequestId,
                    responseMessage = dto.ResponseMessage,
                    shelterId
                });
                var result = await pointer.SingleAsync();
                return result["approved"].As<bool>();
            });
        }
    }
}
