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

        public IEnumerable<Candidate> GetAllCandidates()
        {
            var candidates = new List<Candidate>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT c.*, s.Name AS SkillName, s.ID AS SkillID, s.CreatedDate AS SkillCreatedDate, s.UpdatedDate AS SkillUpdatedDate
                            FROM Candidate c
                            LEFT JOIN CandidateSkill cs ON c.ID = cs.CandidateID
                            LEFT JOIN Skill s ON cs.SkillID = s.ID";

                var command = new SqlCommand(query, connection);
                using (var reader = command.ExecuteReader())
                {
                    var candidateDictionary = new Dictionary<int, Candidate>();

                    while (reader.Read())
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

        public Candidate GetCandidateById(int id)
        {
            Candidate candidate = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT c.*, s.Name AS SkillName, s.ID AS SkillID, s.CreatedDate AS SkillCreatedDate, s.UpdatedDate AS SkillUpdatedDate
                            FROM Candidate c
                            LEFT JOIN CandidateSkill cs ON c.ID = cs.CandidateID
                            LEFT JOIN Skill s ON cs.SkillID = s.ID
                            WHERE c.ID = @ID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
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

            return candidate;
        }

        public void AddCandidate(Candidate candidate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                            INSERT INTO Candidate (ID, FirstName, Surname, DateOfBirth, Address1, Town, Country, PostCode, PhoneHome, PhoneMobile, PhoneWork, CreatedDate, UpdatedDate)
                            VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM Candidate), @FirstName, @Surname, @DateOfBirth, @Address1, @Town, @Country, @PostCode, @PhoneHome, @PhoneMobile, @PhoneWork, @CreatedDate, @UpdatedDate);
                            SELECT SCOPE_IDENTITY();";
                var command = new SqlCommand(query, connection);
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
                var candidateId = Convert.ToInt32(command.ExecuteScalar());
                command.ExecuteNonQuery();

                if (candidate.Skills != null && candidate.Skills.Any())
                {
                    foreach (var skill in candidate.Skills)
                    {
                        var skillQuery = @"
                            INSERT INTO Skill (ID, Name, CreatedDate, UpdatedDate)
                            VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM Skill), @Name, @CreatedDate, @UpdatedDate);
                            SELECT SCOPE_IDENTITY();";
                        var skillCommand = new SqlCommand(skillQuery, connection);
                        skillCommand.Parameters.AddWithValue("@Name", skill.Name);
                        skillCommand.Parameters.AddWithValue("@CreatedDate", skill.CreatedDate);
                        skillCommand.Parameters.AddWithValue("@UpdatedDate", skill.UpdatedDate);
                        var skillId = Convert.ToInt32(skillCommand.ExecuteScalar());

                        var candidateSkillQuery = @"
                            INSERT INTO CandidateSkill (CandidateID, SkillID)
                            VALUES (@CandidateID, @SkillID)";
                        var candidateSkillCommand = new SqlCommand(candidateSkillQuery, connection);
                        candidateSkillCommand.Parameters.AddWithValue("@CandidateID", candidateId);
                        candidateSkillCommand.Parameters.AddWithValue("@SkillID", skillId);
                        candidateSkillCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateCandidate(Candidate candidate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE Candidate SET FirstName = @FirstName, Surname = @Surname, DateOfBirth = @DateOfBirth,
                    Address1 = @Address1, Town = @Town, Country = @Country, PostCode = @PostCode, PhoneHome = @PhoneHome,
                    PhoneMobile = @PhoneMobile, PhoneWork = @PhoneWork, UpdatedDate = @UpdatedDate WHERE ID = @ID";
                var command = new SqlCommand(query, connection);
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
                command.ExecuteNonQuery();

                var deleteSkillsQuery = "DELETE FROM CandidateSkill WHERE CandidateID = @CandidateID";
                var deleteSkillsCommand = new SqlCommand(deleteSkillsQuery, connection);
                deleteSkillsCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                deleteSkillsCommand.ExecuteNonQuery();

                if (candidate.Skills != null && candidate.Skills.Any())
                {
                    foreach (var skill in candidate.Skills)
                    {
                        var skillQuery = @"
                            INSERT INTO Skill (ID, Name, CreatedDate, UpdatedDate)
                            VALUES ((SELECT COALESCE(MAX(id), 0) + 1 FROM Skill), @Name, @CreatedDate, @UpdatedDate);
                            SELECT SCOPE_IDENTITY();";
                        var skillCommand = new SqlCommand(skillQuery, connection);
                        skillCommand.Parameters.AddWithValue("@Name", skill.Name);
                        skillCommand.Parameters.AddWithValue("@CreatedDate", skill.CreatedDate);
                        skillCommand.Parameters.AddWithValue("@UpdatedDate", skill.UpdatedDate);
                        var skillId = Convert.ToInt32(skillCommand.ExecuteScalar());

                        var candidateSkillQuery = @"
                            INSERT INTO CandidateSkill (CandidateID, SkillID)
                            VALUES (@CandidateID, @SkillID)";
                        var candidateSkillCommand = new SqlCommand(candidateSkillQuery, connection);
                        candidateSkillCommand.Parameters.AddWithValue("@CandidateID", candidate.ID);
                        candidateSkillCommand.Parameters.AddWithValue("@SkillID", skillId);
                        candidateSkillCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteCandidate(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Candidate WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
