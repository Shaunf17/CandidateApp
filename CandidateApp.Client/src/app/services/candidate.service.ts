import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private apiUrl = 'http://localhost:5000/api/candidates';

  constructor(private http: HttpClient) { }

  getCandidates(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  addCandidate(candidate: any): Observable<void> {
    return this.http.post<void>(this.apiUrl, candidate);
  }

  updateCandidate(candidate: any): Observable<void> {
    return this.http.put<void>(this.apiUrl, candidate);
  }

  deleteCandidate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
