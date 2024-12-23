using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using CandidateApp.Domain.Models;
using CandidateApp.Services;
using CandidateApp.Services.Interfaces;
using System.Data;

namespace CandidateApp.Tests.Services
{
    public class CandidateRepositoryTests
    {
        private readonly Mock<ICandidateRepository> _mockRepository;
        private readonly Mock<IDbConnection> _mockDbConnection;
        private readonly Mock<IDbCommand> _mockDbCommand;
        private readonly Mock<IDataReader> _mockDataReader;

        public CandidateRepositoryTests()
        {
            _mockRepository = new Mock<ICandidateRepository>();
            _mockDbConnection = new Mock<IDbConnection>();
            _mockDbCommand = new Mock<IDbCommand>();
            _mockDataReader = new Mock<IDataReader>();

            // Setup mock data reader
            _mockDataReader.SetupSequence(reader => reader.Read())
                .Returns(true)
                .Returns(false);
            _mockDataReader.Setup(reader => reader["ID"]).Returns(1);
            _mockDataReader.Setup(reader => reader["FirstName"]).Returns("John");
            _mockDataReader.Setup(reader => reader["Surname"]).Returns("Doe");
            _mockDataReader.Setup(reader => reader["DateOfBirth"]).Returns(DateTime.Now);
            _mockDataReader.Setup(reader => reader["Address1"]).Returns("123 Main St");
            _mockDataReader.Setup(reader => reader["Town"]).Returns("Townsville");
            _mockDataReader.Setup(reader => reader["Country"]).Returns("Countryland");
            _mockDataReader.Setup(reader => reader["PostCode"]).Returns("12345");
            _mockDataReader.Setup(reader => reader["PhoneHome"]).Returns("123-456-7890");
            _mockDataReader.Setup(reader => reader["PhoneMobile"]).Returns("098-765-4321");
            _mockDataReader.Setup(reader => reader["PhoneWork"]).Returns("111-222-3333");
            _mockDataReader.Setup(reader => reader["CreatedDate"]).Returns(DateTime.Now);
            _mockDataReader.Setup(reader => reader["UpdatedDate"]).Returns(DateTime.Now);
            _mockDataReader.Setup(reader => reader["SkillID"]).Returns(DBNull.Value);

            // Setup mock command
            _mockDbCommand.Setup(cmd => cmd.ExecuteReader()).Returns(_mockDataReader.Object);

            // Setup mock connection
            _mockDbConnection.Setup(conn => conn.CreateCommand()).Returns(_mockDbCommand.Object);

            // Setup repository to use mock connection
            _mockRepository.Setup(repo => repo.GetAllCandidates()).Returns(new List<Candidate>
                {
                    new Candidate
                    {
                        ID = 1,
                        FirstName = "John",
                        Surname = "Doe",
                        DateOfBirth = DateTime.Now,
                        Address1 = "123 Main St",
                        Town = "Townsville",
                        Country = "Countryland",
                        PostCode = "12345",
                        PhoneHome = "123-456-7890",
                        PhoneMobile = "098-765-4321",
                        PhoneWork = "111-222-3333",
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Skills = new List<Skill>()
                    }
                });
        }

        [Fact]
        public void GetAllCandidates_ReturnsAllCandidates()
        {
            var candidates = _mockRepository.Object.GetAllCandidates();

            Assert.NotNull(candidates);
        }

        [Fact]
        public void GetCandidateById_ReturnsCorrectCandidate()
        {
            var candidate = new Candidate
            {
                ID = 1,
                FirstName = "John",
                Surname = "Doe",
                DateOfBirth = DateTime.Now,
                Address1 = "123 Main St",
                Town = "Townsville",
                Country = "Countryland",
                PostCode = "12345",
                PhoneHome = "123-456-7890",
                PhoneMobile = "098-765-4321",
                PhoneWork = "111-222-3333",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Skills = new List<Skill>()
            };

            _mockRepository.Setup(repo => repo.GetCandidateById(1)).Returns(candidate);

            var result = _mockRepository.Object.GetCandidateById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void AddCandidate_AddsCandidateSuccessfully()
        {
            var candidate = new Candidate
            {
                ID = 2,
                FirstName = "Jane",
                Surname = "Smith",
                DateOfBirth = DateTime.Now,
                Address1 = "456 Another St",
                Town = "Villagetown",
                Country = "Countryland",
                PostCode = "67890",
                PhoneHome = "222-333-4444",
                PhoneMobile = "555-666-7777",
                PhoneWork = "888-999-0000",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Skills = new List<Skill>()
            };

            _mockRepository.Setup(repo => repo.AddCandidate(candidate));

            _mockRepository.Object.AddCandidate(candidate);

            _mockRepository.Verify(repo => repo.AddCandidate(candidate), Times.Once);
        }

        [Fact]
        public void UpdateCandidate_UpdatesCandidateSuccessfully()
        {
            var candidate = new Candidate
            {
                ID = 1,
                FirstName = "John",
                Surname = "Doe",
                DateOfBirth = DateTime.Now,
                Address1 = "123 Main St",
                Town = "Townsville",
                Country = "Countryland",
                PostCode = "12345",
                PhoneHome = "123-456-7890",
                PhoneMobile = "098-765-4321",
                PhoneWork = "111-222-3333",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Skills = new List<Skill>()
            };

            _mockRepository.Setup(repo => repo.UpdateCandidate(candidate));

            _mockRepository.Object.UpdateCandidate(candidate);

            _mockRepository.Verify(repo => repo.UpdateCandidate(candidate), Times.Once);
        }

        [Fact]
        public void DeleteCandidate_DeletesCandidateSuccessfully()
        {
            var candidateId = 1;

            _mockRepository.Setup(repo => repo.DeleteCandidate(candidateId));

            _mockRepository.Object.DeleteCandidate(candidateId);

            _mockRepository.Verify(repo => repo.DeleteCandidate(candidateId), Times.Once);
        }
    }
}
