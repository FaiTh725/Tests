import { ProfileInfo } from "../Profile/ProfileInfo";

export interface FeedbackInfo {
  Id: number,
  Images: string[],
  Text: string,
  TestId: number,
  Rating: number,
  SendTime: Date,
  UpdateTime: Date,
  Owner: ProfileInfo
}
