export class Skill {
  id: number;
  name: string;
  createdDate: string;
  updatedDate: string;

  constructor(
    id: number,
    name: string,
    createdDate: string,
    updatedDate: string,
  ) {
    this.id = id;
    this.name = name;
    this.createdDate = createdDate;
    this.updatedDate = updatedDate;
  }
}
