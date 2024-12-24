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
        public IActionResult GetAll()
        {
            var candidates = _candidateRepository.GetAllCandidates();
            return Ok(candidates);
        }

        [HttpGet("{id}")]
        public IActionResult GetCandidateByID(int id)
        {
            var candidate = _candidateRepository.GetCandidateById(id);
            return Ok(candidate);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Candidate candidate)
        {
            if (candidate == null) return BadRequest();

            candidate.CreatedDate = DateTime.UtcNow;
            candidate.UpdatedDate = DateTime.UtcNow;
            _candidateRepository.AddCandidate(candidate);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Candidate candidate)
        {
            if (candidate == null || candidate.ID != id) return BadRequest();

            var existingCandidate = _candidateRepository.GetCandidateById(id);
            if (existingCandidate == null) return NotFound();

            candidate.UpdatedDate = DateTime.UtcNow;
            _candidateRepository.UpdateCandidate(candidate);

            return Ok();
        }

        [HttpPost("{id}/skills")]
        public IActionResult AddSkill(int id, [FromBody] Skill skill)
        {
            if (skill == null) return BadRequest();

            var candidate = _candidateRepository.GetCandidateById(id);
            if (candidate == null) return NotFound();

            if (candidate.Skills == null)
            {
                candidate.Skills = new List<Skill>();
            }

            skill.CreatedDate = DateTime.UtcNow;
            skill.UpdatedDate = DateTime.UtcNow;
            candidate.Skills.Add(skill);
            candidate.UpdatedDate = DateTime.UtcNow;

            _candidateRepository.UpdateCandidate(candidate);

            return Ok();
        }

        [HttpDelete("{id}/skills/{skillId}")]
        public IActionResult RemoveSkill(int id, int skillId)
        {
            var candidate = _candidateRepository.GetCandidateById(id);
            if (candidate == null) return NotFound();

            var skill = candidate.Skills?.FirstOrDefault(s => s.ID == skillId);
            if (skill == null) return NotFound();

            candidate.Skills.Remove(skill);
            candidate.UpdatedDate = DateTime.UtcNow;

            _candidateRepository.UpdateCandidate(candidate);

            return Ok();
        }
    }
}
