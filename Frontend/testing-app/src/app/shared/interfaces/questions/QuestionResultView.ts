import { AnswerProfile } from "../answers/AnswerProfile";
import { AnswerResultView } from "../answers/AnswerResultView";

export interface QuestionResultView { 
  Id: number,
  Text: string,
  ImagesUrl: string[],
  Answers: AnswerResultView[],
  ProfileAnswers: AnswerProfile[]
}
