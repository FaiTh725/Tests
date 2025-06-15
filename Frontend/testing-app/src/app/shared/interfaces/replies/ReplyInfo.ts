import { ProfileInfo } from "../Profile/ProfileInfo";

export interface ReplyInfo {
  Id: number,
  Text: string,
  FeedbackId: number,
  SendTime: Date,
  UpdateTime: Date,
  Owner: ProfileInfo
}
