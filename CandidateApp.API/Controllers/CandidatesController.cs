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
    }
}
