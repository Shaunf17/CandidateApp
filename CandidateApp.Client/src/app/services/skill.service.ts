import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Skill } from '../models/skill';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class SkillService {
  private apiUrl = environment.apiUrl + '/skills';

  constructor(private http: HttpClient) { }

  getSkills(): Observable<Skill[]> {
    return this.http.get<Skill[]>(`${this.apiUrl}`);
  }
}
