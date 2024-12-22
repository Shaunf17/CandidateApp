using CandidateApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApp.Services.Interfaces
{
    public interface ICandidateRepository
    {
        IEnumerable<Candidate> GetAllCandidates();
        Candidate GetCandidateById(int id);
        void AddCandidate(Candidate candidate);
        void UpdateCandidate(Candidate candidate);
        void DeleteCandidate(int id);
    }
}
