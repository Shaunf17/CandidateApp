import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CandidateService } from '../../services/candidate.service';

@Component({
  selector: 'app-candidate-form',
  standalone: false,
  
  templateUrl: './candidate-form.component.html',
  styleUrl: './candidate-form.component.css'
})
export class CandidateFormComponent {
  candidateForm: FormGroup;

  constructor(private fb: FormBuilder, private candidateService: CandidateService) {
    this.candidateForm = this.fb.group({
      id: [null],
      firstName: ['', Validators.required],
      surname: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.candidateForm.valid) {
      const candidate = this.candidateForm.value;

      if (candidate.id) {
        this.candidateService.updateCandidate(candidate).subscribe(() => {
          alert('Candidate updated successfully!');
        });
      } else {
        this.candidateService.addCandidate(candidate).subscribe(() => {
          alert('Candidate added successfully!');
        });
      }
    }
  }
}
