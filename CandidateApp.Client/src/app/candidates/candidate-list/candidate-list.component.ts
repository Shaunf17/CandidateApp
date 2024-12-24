import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Router } from '@angular/router';
import { CandidateService } from '../../services/candidate.service';
import { Candidate } from '../../models/candidate';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-candidate-list',
  standalone: false,
  templateUrl: './candidate-list.component.html',
  styleUrl: './candidate-list.component.css'
})
export class CandidateListComponent implements OnInit, OnDestroy {
  expandedCandidate: Candidate | null = null;
  mainColumns: string[] = ['firstName', 'surname', 'dateOfBirth', 'address1', 'town', 'country', 'phoneMobile', 'actions'];
  dataSource = new MatTableDataSource<Candidate>([]);
  private subscriptions: Subscription = new Subscription();
  errorMessage: string | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private candidateService: CandidateService) { }

  ngOnInit(): void {
    this.loadCandidates();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadCandidates(): void {
    const candidatesSubscription = this.candidateService.getCandidates().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.dataSource.paginator = this.paginator;
        console.log('Candidates loaded:', data);
      },
      error: (err) => {
        this.errorMessage = 'Failed to load candidates. Please try again later.'; 
        console.error('Failed to load candidates', err);
      }
    });
    this.subscriptions.add(candidatesSubscription);
  }

  getCandidateSkills(candidate: Candidate): string {
    if (!candidate.skills || candidate.skills.length === 0) {
      return 'None';
    }
    return candidate.skills.map(skill => skill.name).join(', ');
  }

  deleteCandidate(id: number): void {
    // Implement delete logic here
  }
}
