import { Component, OnInit } from '@angular/core';
import { CandidateService } from '../../app/services/candidate.service';

@Component({
  selector: 'app-candidate-list',
  standalone: false,
  
  templateUrl: './candidate-list.component.html',
  styleUrl: './candidate-list.component.css'
})
export class CandidateListComponent implements OnInit {
  candidates: any[] = [];

  constructor(private candidateService: CandidateService) { }

  ngOnInit(): void {
    this.loadCandidates();
  }

  loadCandidates(): void {
    this.candidateService.getCandidates().subscribe((data) => {
      this.candidates = data;
    });
  }

  editCandidate(candidate: any): void {

  }

  deleteCandidate(id: any): void {

  }
}
