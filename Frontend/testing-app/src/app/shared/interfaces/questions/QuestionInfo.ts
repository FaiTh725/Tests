import { Answer } from "../answers/Answer";
import { QuestionType } from "./QuestionType";

export interface QuestionInfo {
  // TestId: number,
  Id: number,
  TestQuestion: string,
  QuestionWeight: number,
  QuestionType: QuestionType,
  QuestionImages: string[],
  Answers: Answer[]
}
