using System.Data;
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
                            DECLARE @NewID INT;
                            SELECT @NewID = COALESCE(MAX(ID), 0) + 1 FROM Candidate;
                            INSERT INTO Candidate (ID, FirstName, Surname, DateOfBirth, Address1, Town, Country, PostCode, PhoneHome, PhoneMobile, PhoneWork, CreatedDate, UpdatedDate)
                            VALUES (@NewID, @FirstName, @Surname, @DateOfBirth, @Address1, @Town, @Country, @PostCode, @PhoneHome, @PhoneMobile, @PhoneWork, @CreatedDate, @UpdatedDate);
                            SELECT @NewID;";
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
                    var result = await command.ExecuteScalarAsync();

                    if (result == null || result == DBNull.Value)
                    {
                        throw new Exception("Failed to retrieve the inserted candidate ID.");
                    }

                    var candidateId = Convert.ToInt32(result);

                    if (candidate.Skills != null && candidate.Skills.Any())
                    {
                        foreach (var skill in candidate.Skills)
                        {
                            var skillId = await AddOrUpdateSkillAsync(skill, connection);

                            var candidateSkillQuery = @"
                                    INSERT INTO CandidateSkill (ID, CandidateID, SkillID, CreatedDate, UpdatedDate)
                                    VALUES ((SELECT COALESCE(MAX(ID), 0) + 1 FROM CandidateSkill), @CandidateID, @SkillID, @CreatedDate, @UpdatedDate)";
                            using (var candidateSkillCommand = new SqlCommand(candidateSkillQuery, connection))
                            {
                                candidateSkillCommand.Parameters.AddWithValue("@CandidateID", candidateId);
                                candidateSkillCommand.Parameters.AddWithValue("@SkillID", skillId);
                                candidateSkillCommand.Parameters.AddWithValue("@CreatedDate", candidate.CreatedDate);
                                candidateSkillCommand.Parameters.AddWithValue("@UpdatedDate", candidate.UpdatedDate);
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
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"
                    UPDATE Candidate SET FirstName = @FirstName, Surname = @Surname, DateOfBirth = @DateOfBirth,
                    Address1 = @Address1, Town = @Town, Country = @Country, PostCode = @PostCode, PhoneHome = @PhoneHome,
                    PhoneMobile = @PhoneMobile, PhoneWork = @PhoneWork, UpdatedDate = @UpdatedDate WHERE ID = @ID";
                        using (var command = new SqlCommand(query, connection, transaction))
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
                        }

                        // Get existing skills for the candidate
                        var existingSkillsQuery = "SELECT SkillID FROM CandidateSkill WHERE CandidateID = @CandidateID";
                        var existingSkillIds = new List<int>();
                        using (var existingSkillsCommand = new SqlCommand(existingSkillsQuery, connection, transaction))
                        {
                            existingSkillsCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                            using (var reader = await existingSkillsCommand.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    existingSkillIds.Add((int)reader["SkillID"]);
                                }
                            }
                        }

                        // Determine skills to add and remove
                        var newSkillIds = candidate.Skills.Select(s => s.ID).ToList();
                        var skillsToAdd = newSkillIds.Except(existingSkillIds).ToList();
                        var skillsToRemove = existingSkillIds.Except(newSkillIds).ToList();

                        // Remove skills
                        if (skillsToRemove.Any())
                        {
                            var removeSkillsQuery = $"DELETE FROM CandidateSkill WHERE CandidateID = @CandidateID AND SkillID IN ({string.Join(",", skillsToRemove)})";
                            using (var removeSkillsCommand = new SqlCommand(removeSkillsQuery, connection, transaction))
                            {
                                removeSkillsCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                                removeSkillsCommand.Parameters.AddWithValue("@SkillIDs", string.Join(",", skillsToRemove));
                                await removeSkillsCommand.ExecuteNonQueryAsync();
                            }
                        }

                        // Add new skills
                        if (skillsToAdd.Any())
                        {
                            var addSkillsQuery = @"
                        INSERT INTO CandidateSkill (ID, CandidateID, SkillID, CreatedDate, UpdatedDate)
                        VALUES ((SELECT COALESCE(MAX(ID), 0) + 1 FROM CandidateSkill), @CandidateID, @SkillID, @CreatedDate, @UpdatedDate)";
                            using (var addSkillsCommand = new SqlCommand(addSkillsQuery, connection, transaction))
                            {
                                foreach (var skillId in skillsToAdd)
                                {
                                    addSkillsCommand.Parameters.Clear();
                                    addSkillsCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                                    addSkillsCommand.Parameters.AddWithValue("@SkillID", skillId);
                                    addSkillsCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                    addSkillsCommand.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                                    await addSkillsCommand.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
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

        public Task DeleteCandidateAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
