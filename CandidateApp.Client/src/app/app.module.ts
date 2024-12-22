import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from "@angular/common/http";
import { ReactiveFormsModule } from "@angular/forms";

// Components
import { AppComponent } from './app.component';
import { CandidateListComponent } from './candidates/candidate-list/candidate-list.component';
import { CandidateFormComponent } from './candidates/candidate-form/candidate-form.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { CandidateDetailComponent } from './candidates/candidate-detail/candidate-detail.component';

// Material
import { MaterialComponentsModule } from './material-components/material-components.module';

@NgModule({
  declarations: [
    AppComponent,
    CandidateListComponent,
    CandidateFormComponent,
    CandidateDetailComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    MaterialComponentsModule,
  ],
  providers: [
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
