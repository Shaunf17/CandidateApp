import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface CandidateWithSkills {
  candidateID: number;
  firstName: string;
  surname: string;
  skills: string[];
}

interface AddSkillRequest {
  candidateID: number;
  skillID: number;
}

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private apiUrl = 'http://localhost:7093/api/candidateskills'; // Adjust if the backend uses a different port

  constructor(private http: HttpClient) { }

  getCandidatesWithSkills(): Observable<CandidateWithSkills[]> {
    return this.http.get<CandidateWithSkills[]>(`${this.apiUrl}`);
  }

  addSkill(request: AddSkillRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/add`, request);
  }

  removeSkill(request: AddSkillRequest): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/remove`, {
      body: request
    });
  }
}
