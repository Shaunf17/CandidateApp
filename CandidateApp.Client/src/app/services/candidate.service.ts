import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Candidate } from '../models/candidate';
import { Skill } from '../models/skill';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private apiUrl = environment.apiUrl + '/candidates';

  constructor(private http: HttpClient) { }

  getCandidates(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCandidateById(id: number): Observable<Candidate> {
    return this.http.get<Candidate>(`${this.apiUrl}/${id}`);
  }

  addCandidate(candidate: Candidate): Observable<void> {
    return this.http.post<void>(this.apiUrl, candidate);
  }

  updateCandidate(candidateid: any, candidate: Candidate): Observable<void> {
    return this.http.put<void>(this.apiUrl, candidate);
  }

  deleteCandidate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
