using System.Collections.Generic;
using CandidateApp.API.Controllers;
using CandidateApp.Domain.Models;
using CandidateApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CandidateApp.Tests.Controllers
{
    public class SkillsControllerTests
    {
        private readonly Mock<ISkillRepository> _mockRepository;
        private readonly SkillsController _controller;

        public SkillsControllerTests()
        {
            _mockRepository = new Mock<ISkillRepository>();
            _controller = new SkillsController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllSkills_ReturnsOkResult_WithListOfSkills()
        {
            // Arrange
            var skills = new List<Skill>
                {
                    new Skill { ID = 1, Name = "C#" },
                    new Skill { ID = 2, Name = "JavaScript" }
                };
            _mockRepository.Setup(repo => repo.GetAllSkillsAsync()).ReturnsAsync(skills);

            // Act
            var result = await _controller.GetAllSkills();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnSkills = Assert.IsType<List<Skill>>(okResult.Value);
            Assert.Equal(2, returnSkills.Count);
        }

        [Fact]
        public async Task GetAllSkills_ReturnsOkResult_WithEmptyList_WhenNoSkillsExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllSkillsAsync()).ReturnsAsync(new List<Skill>());

            // Act
            var result = await _controller.GetAllSkills();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnSkills = Assert.IsType<List<Skill>>(okResult.Value);
            Assert.Empty(returnSkills);
        }
    }
}
