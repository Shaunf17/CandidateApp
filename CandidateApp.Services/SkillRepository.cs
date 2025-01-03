﻿using System.Collections.Generic;
using System.Data.SqlClient;
using CandidateApp.Domain.Models;
using CandidateApp.Services.Interfaces;

namespace CandidateApp.Services
{
    public class SkillRepository : ISkillRepository
    {
        private readonly string _connectionString;

        public SkillRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
        {
            var skills = new List<Skill>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Skill";
                var command = new SqlCommand(query, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var skill = new Skill
                        {
                            ID = (int)reader["ID"],
                            Name = reader["Name"].ToString(),
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            UpdatedDate = (DateTime)reader["UpdatedDate"]
                        };
                        skills.Add(skill);
                    }
                }
            }

            return skills;
        }
    }
}
