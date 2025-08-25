import { QuestionInfo } from "../questions/QuestionInfo";
import { TestInfo } from "./TestInfo";

export interface TestWithQuestions extends TestInfo {
  Questions: QuestionInfo[]
}
