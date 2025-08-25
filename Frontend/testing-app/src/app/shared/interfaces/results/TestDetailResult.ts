import { QuestionResultView } from "../questions/QuestionResultView";
import { TestResult } from "./TestResult";

export interface TestDetailResult extends TestResult {
  QuestionsResults: QuestionResultView[];
}
