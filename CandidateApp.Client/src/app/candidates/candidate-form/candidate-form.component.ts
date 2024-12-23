import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CandidateService } from '../../services/candidate.service';
import { SkillService } from '../../services/skill.service';
import { Skill } from '../../models/skill';
import { Candidate } from '../../models/candidate';

@Component({
  selector: 'app-candidate-form',
  standalone: false,
  
  templateUrl: './candidate-form.component.html',
  styleUrl: './candidate-form.component.css'
})
export class CandidateFormComponent {
  candidateForm: FormGroup;
  skills: Skill[] = [];
  candidateId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private candidateService: CandidateService,
    private skillService: SkillService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.candidateForm = this.fb.group({
      id: [null],
      firstName: ['', Validators.required],
      surname: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      address1: ['', Validators.required],
      town: ['', Validators.required],
      postcode: ['', Validators.required],
      country: ['', Validators.required],
      phoneMobile: ['', Validators.required],
      skills: [[]]
    });
  }

  ngOnInit(): void {
    this.loadSkills();
  }

  loadSkills(): void {
    // Load skills from the service
    this.skillService.getSkills().subscribe({
      next: (data) => {
        this.skills = data;
      },
      error: (err) => {
        console.error('Failed to load skills', err);
      }
    });
  }

  onSubmit(): void {
    if (this.candidateForm.valid) {
      const candidate: Candidate = this.candidateForm.value;
      console.log('Submitting candidate:', candidate);

      if (this.candidateId) {
        this.candidateService.updateCandidate(this.candidateId, candidate).subscribe({
          next: (updatedCandidate) => {
            console.log('Candidate updated:', updatedCandidate);
            this.router.navigate(['/candidates']);
          },
          error: (err) => {
            console.error('Failed to update candidate', err);
          }
        });
      } else {
        this.candidateService.addCandidate(candidate).subscribe({
          next: (newCandidate) => {
            console.log('Candidate added:', newCandidate);
            this.router.navigate(['/candidates']);
          },
          error: (err) => {
            console.error('Failed to add candidate', err);
          }
        });
      }
    }
  }
}
