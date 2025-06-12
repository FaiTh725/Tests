import { ProfileInfo } from "../Profile/ProfileInfo";
import { TestType } from "./TestType";

export interface TestInfo {
  Id: number,
  Name: string,
  Description: string,
  CreatedTime: Date,
  IsPublic: boolean,
  TestType: TestType,
  DurationInMinutes: number | null,
  Owner: ProfileInfo
}
