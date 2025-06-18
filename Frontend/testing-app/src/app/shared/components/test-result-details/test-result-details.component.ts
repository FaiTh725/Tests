import { Component, Input } from '@angular/core';
import { HttpService } from '../../../core/services/Http.service';
import { TestDetailResult } from '../../interfaces/results/TestDetailResult';
import { DatePipe } from '@angular/common';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { ModalService } from '../../../core/services/Modal.service';
import { QuestionResultViewComponent } from "../question-result-view/question-result-view.component";

@Component({
  selector: 'app-test-result-details',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent, QuestionResultViewComponent],
  templateUrl: './test-result-details.component.html',
  styleUrl: './test-result-details.component.scss'
})
export class TestResultDetailsComponent {
  testResultDetails?: TestDetailResult;
  @Input() sessionId?: number;

  constructor(
    private httpService: HttpService,
    private modalService: ModalService
  ) {
    
  }

  ngOnInit() {
    if(this.sessionId) {
      this.getResultDetails(this.sessionId);
    }
  }

  getResultDetails(sessionId: number) {
    const requestUrl = `testing/TestSession/GetSession?sessionId=${sessionId}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.testResultDetails = {
          Id: data.id,
          TestId: data.testId,
          TestName: data.testName,
          ProfileId: data.profileId,
          StartTime: data.startTime,
          EndTime: data.endTime,
          Percent: data.percent,
          QuestionsResults: data.questionsDetailResult
          .map((question: any) => ({
            Id: question.questionId,
            Text: question.questionText,
            ImagesUrl: question.questionImages,
            Answers: question.answers.map((answer: any) => ({
              Id: answer.id,
              ImagesUrls: answer.questionAnswerImages,
              Answer: answer.answer
            })),
            ProfileAnswers: question.profileAnswers.map((profileAnswer: any) => ({
              QuestionId: profileAnswer.questionId,
              AnswersIdList: profileAnswer.profileAnswersId,
              IsCorrect: profileAnswer.isCorrectAnswer
            }))
          }))
        };
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }
  
  handleClose() {
    this.modalService.handleClose();
  }
}
