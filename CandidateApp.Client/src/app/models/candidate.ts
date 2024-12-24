import { Skill } from "./skill";

export class Candidate {
  id: number;
  firstName: string;
  surname: string;
  dateOfBirth: Date;
  address1: string;
  town: string;
  country: string;
  postCode: string;
  phoneHome: string;
  phoneMobile: string;
  phoneWork: string;
  createdDate: Date;
  updatedDate: Date;
  skills: Skill[]

  constructor(
    id: number,
    firstName: string,
    surname: string,
    dateOfBirth: Date,
    address1: string,
    town: string,
    country: string,
    postCode: string,
    phoneHome: string,
    phoneMobile: string,
    phoneWork: string,
    createdDate: Date,
    updatedDate: Date,
    skills: Skill[]
  ) {
    this.id = id;
    this.firstName = firstName;
    this.surname = surname;
    this.dateOfBirth = dateOfBirth;
    this.address1 = address1;
    this.town = town;
    this.country = country;
    this.postCode = postCode;
    this.phoneHome = phoneHome;
    this.phoneMobile = phoneMobile;
    this.phoneWork = phoneWork;
    this.createdDate = createdDate;
    this.updatedDate = updatedDate;
    this.skills = skills;
  }
}
