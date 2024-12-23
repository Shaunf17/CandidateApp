using CandidateApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApp.Services.Interfaces
{
    public interface ISkillRepository
    {
        public IEnumerable<Skill> GetAllSkills();
    }
}
