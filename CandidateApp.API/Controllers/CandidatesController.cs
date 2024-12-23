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
    }
}
