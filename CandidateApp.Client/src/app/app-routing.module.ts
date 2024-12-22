import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CandidateListComponent } from './candidates/candidate-list/candidate-list.component'
import { CandidateFormComponent } from './candidates/candidate-form/candidate-form.component';

const routes: Routes = [
  { path: '', redirectTo: '/candidates', pathMatch: 'full' },
  {
    path: 'candidates', component: CandidateListComponent, children: [
      { path: 'new', component: CandidateFormComponent },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
