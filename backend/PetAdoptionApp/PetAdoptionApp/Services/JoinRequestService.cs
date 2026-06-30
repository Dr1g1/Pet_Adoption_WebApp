using PetAdoptionApp.DTOs.JoinRequest;
using PetAdoptionApp.Interfaces;
using PetAdoptionApp.Common;
using Neo4j.Driver;

namespace PetAdoptionApp.Services
{
    public class JoinRequestService : IJoinRequestService
    {
        private readonly IDriver _driver;

        public JoinRequestService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<JoinRequestReturnDto> CreateAsync(string volunteerId, JoinRequestCreateDto dto)
        {
            // da li volonter vec pripada nekom azilu i jel vec ima pending zahtev za ovaj azil
            var newId = Guid.NewGuid().ToString();
            var query = @"
                MATCH (v:Volunteer {id:$volunteerId})
                MATCH (s:Shelter {id:$shelterId})

                // odbij ako vec pripada nekom azilu
                OPTIONAL MATCH (v)-[:VOLUNTEERS_AT]->(existing:Shelter)
                WITH v, s, existing
                WHERE existing IS NULL

                // odbij ako vec ima pending zahtev za ovaj azil
                OPTIONAL MATCH (v)-[:WANTS_TO_JOIN]->(dup:JoinRequest)-[:TO_SHELTER]->(s)
                WHERE dup.status = 'Pending'
                WITH v, s, dup
                WHERE dup IS NULL

                CREATE (jr:JoinRequest {
                    id: $id,
                    status: 'Pending',
                    createdAt: datetime(),
                    message: $message
                })
                CREATE (v)-[:WANTS_TO_JOIN]->(jr)
                CREATE (jr)-[:TO_SHELTER]->(s)
                RETURN jr, v.id AS vId, v.name AS vName, s.id AS sId, s.name AS sName";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new
                {
                    volunteerId,
                    shelterId = dto.shelterId,
                    id = newId,
                    message = dto.message ?? ""
                });

                if (!await cursor.FetchAsync())
                    throw new InvalidOperationException("Već pripadaš azilu ili već imaš poslat zahtev ovom azilu.");

                var jr = cursor.Current["jr"].As<INode>();
                return new JoinRequestReturnDto
                {
                    RequestId = jr["id"].As<string>(),
                    VolunteerId = cursor.Current["vId"].As<string>(),
                    VolunteerName = cursor.Current["vName"].As<string>(),
                    ShelterId = cursor.Current["sId"].As<string>(),
                    ShelterName = cursor.Current["sName"].As<string>(),
                    Status = Enum.Parse<Enums.Status>(jr["status"].As<string>()),
                    CreatedAt = DateTime.Parse(jr["createdAt"].As<string>()),
                    Message = jr["message"].As<string?>()
                };
            });
        }

        public async Task<IEnumerable<JoinRequestReturnDto>> GetMyRequestsAsync(string volunteerId)
        {
            var query = @"
                MATCH (v:Volunteer {id:$volunteerId})-[:WANTS_TO_JOIN]->(jr:JoinRequest)-[:TO_SHELTER]->(s:Shelter)
                RETURN jr, v, s
                ORDER BY jr.createdAt DESC";
            return await RunListQuery(query, new { volunteerId });
        }

        public async Task<IEnumerable<JoinRequestReturnDto>> GetPendingForShelterAsync(string shelterId)
        {
            var query = @"
                MATCH (v:Volunteer)-[:WANTS_TO_JOIN]->(jr:JoinRequest)-[:TO_SHELTER]->(s:Shelter {id:$shelterId})
                WHERE jr.status = 'Pending'
                RETURN jr, v, s
                ORDER BY jr.createdAt DESC";
            return await RunListQuery(query, new { shelterId });
        }

        public async Task<bool> ApproveAsync(string requestId, string shelterId)
        {
            var query = @"
                MATCH (v:Volunteer)-[:WANTS_TO_JOIN]->(jr:JoinRequest {id:$requestId})-[:TO_SHELTER]->(s:Shelter {id:$shelterId})
                WHERE jr.status = 'Pending'
                SET jr.status = 'Approved', jr.updatedAt = datetime()
                MERGE (v)-[:VOLUNTEERS_AT]->(s)

                // odbij sve ostale pending zahteve ovog volontera
                WITH v, jr
                OPTIONAL MATCH (v)-[:WANTS_TO_JOIN]->(other:JoinRequest)
                WHERE other.id <> jr.id AND other.status = 'Pending'
                SET other.status = 'Rejected', other.updatedAt = datetime()
                RETURN count(jr) > 0 AS approved";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { requestId, shelterId });
                var record = await cursor.SingleAsync();
                return record["approved"].As<bool>();
            });
        }

        public async Task<bool> RejectAsync(string requestId, string shelterId)
        {
            var query = @"
                MATCH (jr:JoinRequest {id:$requestId})-[:TO_SHELTER]->(s:Shelter {id:$shelterId})
                WHERE jr.status = 'Pending'
                SET jr.status = 'Rejected', jr.updatedAt = datetime()
                RETURN count(jr) > 0 AS rejected";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { requestId, shelterId });
                var record = await cursor.SingleAsync();
                return record["rejected"].As<bool>();
            });
        }

        public async Task<bool> CancelAsync(string requestId, string volunteerId)
        {
            var query = @"
                MATCH (v:Volunteer {id:$volunteerId})-[:WANTS_TO_JOIN]->(jr:JoinRequest {id:$requestId})
                WHERE jr.status = 'Pending'
                WITH jr, count(jr) > 0 AS deleted
                DETACH DELETE jr
                RETURN deleted";

            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { requestId, volunteerId });
                var record = await cursor.SingleAsync();
                return record["deleted"].As<bool>();
            });
        }

        
        private async Task<IEnumerable<JoinRequestReturnDto>> RunListQuery(string query, object parameters)
        {
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var list = new List<JoinRequestReturnDto>();
                while (await cursor.FetchAsync())
                {
                    var jr = cursor.Current["jr"].As<INode>();
                    var v = cursor.Current["v"].As<INode>();
                    var s = cursor.Current["s"].As<INode>();
                    list.Add(new JoinRequestReturnDto
                    {
                        RequestId = jr["id"].As<string>(),
                        VolunteerId = v["id"].As<string>(),
                        VolunteerName = v["name"].As<string>(),
                        ShelterId = s["id"].As<string>(),
                        ShelterName = s["name"].As<string>(),
                        Status = Enum.Parse<Enums.Status>(jr["status"].As<string>()),
                        CreatedAt = DateTime.Parse(jr["createdAt"].As<string>()),
                        Message = jr.Properties.ContainsKey("message") ? jr["message"].As<string?>() : null
                    });
                }
                return list;
            });
        }
    }
}