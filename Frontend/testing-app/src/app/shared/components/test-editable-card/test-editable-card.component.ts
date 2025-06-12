import { Component, EventEmitter, Input, Output} from '@angular/core';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { CommonModule, DatePipe } from '@angular/common';
import { AddQuestionForm, AddQuestionFormComponent } from "../add-question-form/add-question-form.component";
import { HttpService } from '../../../core/services/Http.service';
import { TestQuestionCardComponent } from "../test-question-card/test-question-card.component";
import { TestWithQuestions } from '../../interfaces/tests/TestWithQuestions';

@Component({
  selector: 'app-test-editable-card',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent, AddQuestionFormComponent, CommonModule, TestQuestionCardComponent],
  templateUrl: './test-editable-card.component.html',
  styleUrl: './test-editable-card.component.scss'
})
export class TestEditableCardComponent {
  @Input() test?: TestWithQuestions;

  @Output() addQuestion = new EventEmitter<AddQuestionForm>();
  @Output() delete = new EventEmitter<number>();
  @Output() testUpdated = new EventEmitter<TestWithQuestions>();

  isOpenToEdit = false;

  constructor(
    private httpService: HttpService
  ) {
    
  }

  handleDelete() {
    this.delete.emit(this.test?.Id);
  }

  handleDeleteQuestion(questionId: number) {
    const questionUrl = `testing/Question/DeleteQuestion?questionId=${questionId}`;
    this.httpService.deleteRequest(questionUrl)
    .subscribe({
      error: _ => {
        console.log("unknown error");
      },
      complete: () => {
        if(this.test) {
          this.test.Questions = [...this.test.Questions
            .filter(question => question.Id != questionId)];
        }
      }
    });
  }

  handleOpenEditSection(e: MouseEvent) {
    e.stopPropagation();
    this.isOpenToEdit=true;

    const getRequestUrl = `testing/Question/GetTaskQuestions?testId=${this.test?.Id}`;
    this.httpService.getRequest(getRequestUrl)
    .subscribe({
      next: (data:any) => {
        if(this.test) {
          this.test.Questions = [...data.map((question:any) => ({
            Id: question.id,
            TestQuestion: question.testQuestion,
            QuestionWeight: question.questionWeight,
            QuestionType: question.questionType,
            QuestionImages: question.questionImages,
            Answers: [...question.answers.map((answer:any) => ({
              Id: answer.id,
              IsCorrect: answer.isCorrect,
              Answer: answer.answer,
              QuestionId: answer.questionId,
              questionAnswerImages: [...answer.questionAnswersImageUrls]
            }))]
          }))];

          this.testUpdated.emit(this.test);
        }
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }
}
