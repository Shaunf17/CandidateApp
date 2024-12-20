import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { HttpClientModule } from "@angular/common/http";
import { CandidateListComponent } from './components/candidate-list/candidate-list.component';
import { AddSkillComponent } from './components/add-skill/add-skill.component'

@NgModule({
  declarations: [
    AppComponent,
    CandidateListComponent,
    AddSkillComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
