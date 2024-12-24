using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CandidateApp.API.Controllers;
using CandidateApp.Domain.Models;
using CandidateApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CandidateApp.Tests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateRepository> _mockRepository;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockRepository = new Mock<ICandidateRepository>();
            _controller = new CandidatesController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfCandidates()
        {
            // Arrange
            var candidates = new List<Candidate>
            {
                new Candidate { ID = 1, FirstName = "John", Surname = "Doe" },
                new Candidate { ID = 2, FirstName = "Jane", Surname = "Smith" }
            };
            _mockRepository.Setup(repo => repo.GetAllCandidatesAsync()).ReturnsAsync(candidates);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCandidates = Assert.IsType<List<Candidate>>(okResult.Value);
            Assert.Equal(2, returnCandidates.Count);
        }

        [Fact]
        public async Task GetCandidateByID_ReturnsOkResult_WithCandidate()
        {
            // Arrange
            var candidate = new Candidate { ID = 1, FirstName = "John", Surname = "Doe" };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync(candidate);

            // Act
            var result = await _controller.GetCandidateByID(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCandidate = Assert.IsType<Candidate>(okResult.Value);
            Assert.Equal(1, returnCandidate.ID);
        }

        [Fact]
        public async Task GetCandidateByID_ReturnsNotFound_WhenCandidateNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync((Candidate)null);

            // Act
            var result = await _controller.GetCandidateByID(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WhenCandidateIsCreated()
        {
            // Arrange
            var candidate = new Candidate { FirstName = "John", Surname = "Doe" };

            // Act
            var result = await _controller.Create(candidate);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockRepository.Verify(repo => repo.AddCandidateAsync(candidate), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCandidateIsNull()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenCandidateIsUpdated()
        {
            // Arrange
            var candidate = new Candidate { ID = 1, FirstName = "John", Surname = "Doe" };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync(candidate);

            // Act
            var result = await _controller.Update(1, candidate);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockRepository.Verify(repo => repo.UpdateCandidateAsync(candidate), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenCandidateIsNull()
        {
            // Act
            var result = await _controller.Update(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenCandidateNotFound()
        {
            // Arrange
            var candidate = new Candidate { ID = 1, FirstName = "John", Surname = "Doe" };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync((Candidate)null);

            // Act
            var result = await _controller.Update(1, candidate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddSkill_ReturnsOkResult_WhenSkillIsAdded()
        {
            // Arrange
            var candidate = new Candidate { ID = 1, FirstName = "John", Surname = "Doe", Skills = new List<Skill>() };
            var skill = new Skill { ID = 1, Name = "C#" };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync(candidate);

            // Act
            var result = await _controller.AddSkill(1, skill);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockRepository.Verify(repo => repo.UpdateCandidateAsync(candidate), Times.Once);
        }

        [Fact]
        public async Task AddSkill_ReturnsBadRequest_WhenSkillIsNull()
        {
            // Act
            var result = await _controller.AddSkill(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddSkill_ReturnsNotFound_WhenCandidateNotFound()
        {
            // Arrange
            var skill = new Skill { ID = 1, Name = "C#" };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync((Candidate)null);

            // Act
            var result = await _controller.AddSkill(1, skill);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveSkill_ReturnsOkResult_WhenSkillIsRemoved()
        {
            // Arrange
            var skill = new Skill { ID = 1, Name = "C#" };
            var candidate = new Candidate { ID = 1, FirstName = "John", Surname = "Doe", Skills = new List<Skill> { skill } };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync(candidate);

            // Act
            var result = await _controller.RemoveSkill(1, 1);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockRepository.Verify(repo => repo.UpdateCandidateAsync(candidate), Times.Once);
        }

        [Fact]
        public async Task RemoveSkill_ReturnsNotFound_WhenCandidateNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync((Candidate)null);

            // Act
            var result = await _controller.RemoveSkill(1, 1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveSkill_ReturnsNotFound_WhenSkillNotFound()
        {
            // Arrange
            var candidate = new Candidate { ID = 1, FirstName = "John", Surname = "Doe", Skills = new List<Skill>() };
            _mockRepository.Setup(repo => repo.GetCandidateByIdAsync(1)).ReturnsAsync(candidate);

            // Act
            var result = await _controller.RemoveSkill(1, 1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
