using Microsoft.AspNetCore.Mvc;
using CandidateApp.Services.Interfaces;
using CandidateApp.Domain.Models;
using System.Collections.Generic;

namespace CandidateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillRepository _skillRepository;

        public SkillsController(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Skill>> GetAllSkills()
        {
            var skills = _skillRepository.GetAllSkills();
            return Ok(skills);
        }
    }
}
