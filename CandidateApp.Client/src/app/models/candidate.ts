export class Candidate {
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

  constructor(
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
    updatedDate: Date
  ) {
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
  }
}
