using CandidateApp.Domain.Models;
using CandidateApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CandidateApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : Controller
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidatesController(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var candidates = await _candidateRepository.GetAllCandidatesAsync();
            return Ok(candidates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidateByID(int id)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (candidate == null) return NotFound();
            return Ok(candidate);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Candidate candidate)
        {
            if (candidate == null) return BadRequest();

            candidate.CreatedDate = DateTime.UtcNow;
            candidate.UpdatedDate = DateTime.UtcNow;
            await _candidateRepository.AddCandidateAsync(candidate);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Candidate candidate)
        {
            if (candidate == null || candidate.ID != id) return BadRequest();

            var existingCandidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (existingCandidate == null) return NotFound();

            candidate.UpdatedDate = DateTime.UtcNow;
            await _candidateRepository.UpdateCandidateAsync(candidate);

            return Ok();
        }

        [HttpPost("{id}/skills")]
        public async Task<IActionResult> AddSkill(int id, [FromBody] Skill skill)
        {
            if (skill == null) return BadRequest();

            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (candidate == null) return NotFound();

            if (candidate.Skills == null)
            {
                candidate.Skills = new List<Skill>();
            }

            skill.CreatedDate = DateTime.UtcNow;
            skill.UpdatedDate = DateTime.UtcNow;
            candidate.Skills.Add(skill);
            candidate.UpdatedDate = DateTime.UtcNow;

            await _candidateRepository.UpdateCandidateAsync(candidate);

            return Ok();
        }

        [HttpDelete("{id}/skills/{skillId}")]
        public async Task<IActionResult> RemoveSkill(int id, int skillId)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (candidate == null) return NotFound();

            var skill = candidate.Skills?.FirstOrDefault(s => s.ID == skillId);
            if (skill == null) return NotFound();

            candidate.Skills.Remove(skill);
            candidate.UpdatedDate = DateTime.UtcNow;

            await _candidateRepository.UpdateCandidateAsync(candidate);

            return Ok();
        }
    }
}
