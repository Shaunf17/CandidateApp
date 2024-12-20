import { Component } from '@angular/core';
import { CandidateService } from '../../services/candidate.service';

@Component({
  selector: 'app-add-skill',
  templateUrl: './add-skill.component.html',
  styleUrls: ['./add-skill.component.css']
})
export class AddSkillComponent {
  candidateID!: number;
  skillID!: number;

  constructor(private candidateService: CandidateService) { }

  addSkill(): void {
    if (this.candidateID && this.skillID) {
      this.candidateService
        .addSkill({ candidateID: this.candidateID, skillID: this.skillID })
        .subscribe(() => {
          alert('Skill added successfully');
        });
    }
  }
}
