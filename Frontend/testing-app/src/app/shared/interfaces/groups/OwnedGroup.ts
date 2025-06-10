import { ProfileInfo } from "../Profile/ProfileInfo";
import { Group } from "./Group";

export interface OwnedGroup extends Group {
  Members: ProfileInfo[]
}
