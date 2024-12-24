using System.Data.SqlClient;
using CandidateApp.Domain.Models;
using CandidateApp.Services.Interfaces;

namespace CandidateApp.Services
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly string _connectionString;

        public CandidateRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
        {
            var candidates = new List<Candidate>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                                SELECT c.*, s.Name AS SkillName, s.ID AS SkillID, s.CreatedDate AS SkillCreatedDate, s.UpdatedDate AS SkillUpdatedDate
                                FROM Candidate c
                                LEFT JOIN CandidateSkill cs ON c.ID = cs.CandidateID
                                LEFT JOIN Skill s ON cs.SkillID = s.ID";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var candidateDictionary = new Dictionary<int, Candidate>();

                    while (await reader.ReadAsync())
                    {
                        var candidateId = (int)reader["ID"];
                        if (!candidateDictionary.TryGetValue(candidateId, out var candidate))
                        {
                            candidate = new Candidate
                            {
                                ID = candidateId,
                                FirstName = reader["FirstName"].ToString(),
                                Surname = reader["Surname"].ToString(),
                                DateOfBirth = (DateTime)reader["DateOfBirth"],
                                Address1 = reader["Address1"].ToString(),
                                Town = reader["Town"].ToString(),
                                Country = reader["Country"].ToString(),
                                PostCode = reader["PostCode"].ToString(),
                                PhoneHome = reader["PhoneHome"].ToString(),
                                PhoneMobile = reader["PhoneMobile"].ToString(),
                                PhoneWork = reader["PhoneWork"].ToString(),
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                UpdatedDate = (DateTime)reader["UpdatedDate"],
                                Skills = new List<Skill>()
                            };
                            candidateDictionary.Add(candidateId, candidate);
                        }

                        // Add the skill if available
                        if (reader["SkillID"] != DBNull.Value)
                        {
                            candidate.Skills.Add(new Skill
                            {
                                ID = (int)reader["SkillID"],
                                Name = reader["SkillName"].ToString(),
                                CreatedDate = (DateTime)reader["SkillCreatedDate"],
                                UpdatedDate = (DateTime)reader["SkillUpdatedDate"]
                            });
                        }
                    }

                    candidates = candidateDictionary.Values.ToList();
                }
            }

            return candidates;
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id)
        {
            Candidate candidate = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                                SELECT c.*, s.Name AS SkillName, s.ID AS SkillID, s.CreatedDate AS SkillCreatedDate, s.UpdatedDate AS SkillUpdatedDate
                                FROM Candidate c
                                LEFT JOIN CandidateSkill cs ON c.ID = cs.CandidateID
                                LEFT JOIN Skill s ON cs.SkillID = s.ID
                                WHERE c.ID = @ID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (candidate == null)
                            {
                                candidate = new Candidate
                                {
                                    ID = (int)reader["ID"],
                                    FirstName = reader["FirstName"].ToString(),
                                    Surname = reader["Surname"].ToString(),
                                    DateOfBirth = (DateTime)reader["DateOfBirth"],
                                    Address1 = reader["Address1"].ToString(),
                                    Town = reader["Town"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    PostCode = reader["PostCode"].ToString(),
                                    PhoneHome = reader["PhoneHome"].ToString(),
                                    PhoneMobile = reader["PhoneMobile"].ToString(),
                                    PhoneWork = reader["PhoneWork"].ToString(),
                                    CreatedDate = (DateTime)reader["CreatedDate"],
                                    UpdatedDate = (DateTime)reader["UpdatedDate"],
                                    Skills = new List<Skill>()
                                };
                            }

                            // Add the skill if available
                            if (reader["SkillID"] != DBNull.Value)
                            {
                                candidate.Skills.Add(new Skill
                                {
                                    ID = (int)reader["SkillID"],
                                    Name = reader["SkillName"].ToString(),
                                    CreatedDate = (DateTime)reader["SkillCreatedDate"],
                                    UpdatedDate = (DateTime)reader["SkillUpdatedDate"]
                                });
                            }
                        }
                    }
                }
            }

            return candidate;
        }

        public async Task AddCandidateAsync(Candidate candidate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                                INSERT INTO Candidate (ID, FirstName, Surname, DateOfBirth, Address1, Town, Country, PostCode, PhoneHome, PhoneMobile, PhoneWork, CreatedDate, UpdatedDate)
                                VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM Candidate), @FirstName, @Surname, @DateOfBirth, @Address1, @Town, @Country, @PostCode, @PhoneHome, @PhoneMobile, @PhoneWork, @CreatedDate, @UpdatedDate);
                                SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", candidate.FirstName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Surname", candidate.Surname ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", candidate.DateOfBirth);
                    command.Parameters.AddWithValue("@Address1", candidate.Address1 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Town", candidate.Town ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Country", candidate.Country ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PostCode", candidate.PostCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneHome", candidate.PhoneHome ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneMobile", candidate.PhoneMobile ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneWork", candidate.PhoneWork ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", candidate.CreatedDate);
                    command.Parameters.AddWithValue("@UpdatedDate", candidate.UpdatedDate);
                    var candidateId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (candidate.Skills != null && candidate.Skills.Any())
                    {
                        foreach (var skill in candidate.Skills)
                        {
                            var skillId = await AddOrUpdateSkillAsync(skill, connection);

                            var candidateSkillQuery = @"
                                    INSERT INTO CandidateSkill (CandidateID, SkillID)
                                    VALUES (@CandidateID, @SkillID)";
                            using (var candidateSkillCommand = new SqlCommand(candidateSkillQuery, connection))
                            {
                                candidateSkillCommand.Parameters.AddWithValue("@CandidateID", candidateId);
                                candidateSkillCommand.Parameters.AddWithValue("@SkillID", skillId);
                                await candidateSkillCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
        }

        public async Task UpdateCandidateAsync(Candidate candidate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                        UPDATE Candidate SET FirstName = @FirstName, Surname = @Surname, DateOfBirth = @DateOfBirth,
                        Address1 = @Address1, Town = @Town, Country = @Country, PostCode = @PostCode, PhoneHome = @PhoneHome,
                        PhoneMobile = @PhoneMobile, PhoneWork = @PhoneWork, UpdatedDate = @UpdatedDate WHERE ID = @ID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", candidate.FirstName);
                    command.Parameters.AddWithValue("@Surname", candidate.Surname);
                    command.Parameters.AddWithValue("@DateOfBirth", candidate.DateOfBirth);
                    command.Parameters.AddWithValue("@Address1", candidate.Address1);
                    command.Parameters.AddWithValue("@Town", candidate.Town);
                    command.Parameters.AddWithValue("@Country", candidate.Country);
                    command.Parameters.AddWithValue("@PostCode", candidate.PostCode);
                    command.Parameters.AddWithValue("@PhoneHome", candidate.PhoneHome);
                    command.Parameters.AddWithValue("@PhoneMobile", candidate.PhoneMobile);
                    command.Parameters.AddWithValue("@PhoneWork", candidate.PhoneWork);
                    command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ID", candidate.ID);
                    await command.ExecuteNonQueryAsync();

                    var deleteSkillsQuery = "DELETE FROM CandidateSkill WHERE CandidateID = @CandidateID";
                    using (var deleteSkillsCommand = new SqlCommand(deleteSkillsQuery, connection))
                    {
                        deleteSkillsCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                        await deleteSkillsCommand.ExecuteNonQueryAsync();
                    }

                    if (candidate.Skills != null && candidate.Skills.Any())
                    {
                        foreach (var skill in candidate.Skills)
                        {
                            var skillId = await AddOrUpdateSkillAsync(skill, connection);

                            var candidateSkillQuery = @"
                                    INSERT INTO CandidateSkill (CandidateID, SkillID)
                                    VALUES (@CandidateID, @SkillID)";
                            using (var candidateSkillCommand = new SqlCommand(candidateSkillQuery, connection))
                            {
                                candidateSkillCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                                candidateSkillCommand.Parameters.AddWithValue("@SkillID", skillId);
                                await candidateSkillCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
        }

        public async Task DeleteCandidateAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DELETE FROM Candidate WHERE ID = @ID", connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<int> AddOrUpdateSkillAsync(Skill skill, SqlConnection connection)
        {
            var skillQuery = @"
                    IF EXISTS (SELECT 1 FROM Skill WHERE Name = @Name)
                    BEGIN
                        SELECT ID FROM Skill WHERE Name = @Name
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Skill (Name, CreatedDate, UpdatedDate)
                        VALUES (@Name, @CreatedDate, @UpdatedDate);
                        SELECT SCOPE_IDENTITY();
                    END";
            using (var skillCommand = new SqlCommand(skillQuery, connection))
            {
                skillCommand.Parameters.AddWithValue("@Name", skill.Name);
                skillCommand.Parameters.AddWithValue("@CreatedDate", skill.CreatedDate);
                skillCommand.Parameters.AddWithValue("@UpdatedDate", skill.UpdatedDate);
                return Convert.ToInt32(await skillCommand.ExecuteScalarAsync());
            }
        }
    }
}
