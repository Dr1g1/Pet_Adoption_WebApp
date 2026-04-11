using PetAdoptionApp.DTOs.MedicalRecord;
using PetAdoptionApp.Interfaces;
using Neo4j.Driver;

namespace PetAdoptionApp.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private IDriver _driver;

        public MedicalRecordService (IDriver driver)
        {
            _driver = driver;
        }

        public async Task<MedicalRecordResponseDto> CreateMedicalRecord(string animalId, MedicalRecordCreateDto dto)
        {
            //kreiramo jedan medical record za jednu zivotinju ciji nam je id dat.
            var newId = Guid.NewGuid().ToString();
            var query = @"
                MATCH (a: Animal {id: $animalId})
                CREATE (a)-[:HAS]->(mr: MedialRecord {
                                    id: $id
                                    description: $description
                                    date: datetime($date)
                                    clinicPhone: $clinicPhone
                                    vetName: $vetName
                                    nextDueDate: datetime($nextDueDate)
                                    vaccines: $vaccines
                                    })
                RETURN mr";
            var parameters = new
            {
                id = newId,
                description = dto.description,
                date = dto.date.ToString("yyyy-MM-dd"),
                clinicPhone = dto.clinicPhone,
                vetName = dto.vetName,
                nextDueDate = dto.nextDueDate.ToString("yyyy-MM-dd"),
                vaccines = dto.vaccines ?? Array.Empty<string>(),
            };
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, parameters);
                var result = await pointer.SingleAsync();
                return MapNodeToMedRecResp(result["mr"].As<INode>());
            });
        }

        public async Task<bool> DeleteRecordAsync(string recordId)
        {
            //izbrisi jedan record.
            var query = @"
                MATCH (mr: MedicalRecord {id: $recordId})
                DETACH DELETE mr
                RETURN count(mr) > 0 AS exists
                ";
            await using var session = _driver.AsyncSession();
            return await session.ExecuteWriteAsync(async x =>
            {
                var pointer = await x.RunAsync(query, new { recordId });
                var result = await pointer.SingleAsync();
                return result["exists"].As<bool>();
            });
        }

        public async Task<IEnumerable<MedicalRecordResponseDto>> GetMedicalRecordsForAnimal(string animalId)
        {
            //vratiti sve medical records koje ima jedna zivotinja prema njenom id-ju.
            await using var session = _driver.AsyncSession();

            return await session.ExecuteReadAsync(async x =>
            {
                var query = @"
                MATCH (a: Animal {id: $animalId})-[:HAS]->(mr: MedicalRecord)
                RETURN mr";
                var pointer = await x.RunAsync(query, new { animalId });
                var medicalRecords = new List<MedicalRecordResponseDto>();
                while (await pointer.FetchAsync())
                    medicalRecords.Add(MapNodeToMedRecResp(pointer.Current["mr"].As<INode>()));
                return medicalRecords;
            });
        }

        //pomocna funkcija:
        private MedicalRecordResponseDto MapNodeToMedRecResp(INode node)
        {
            return new MedicalRecordResponseDto
            {
                id = node.Properties["id"].As<string>(),
                description = node.Properties["description"].As<string>(),
                date = DateTime.Parse(node.Properties["date"].As<string>()),
                clinicPhone = node.Properties["clinicPhone"].As<string>(),
                vetName = node.Properties["vetName"].As<string>(),
                nextDueDate = DateTime.Parse(node.Properties["nextDueDate"].As<string>()),
                vaccines = node.Properties["vaccines"].As<string[]>(),
            };
        }
    }
}
