import { Component, Input } from '@angular/core';
import { ImageSliderComponent } from "../image-slider/image-slider.component";
import { QuestionResultView } from '../../interfaces/questions/QuestionResultView';
import { CommonModule } from '@angular/common';
import { AnswerProfile } from '../../interfaces/answers/AnswerProfile';
import { UrlService } from '../../../core/services/UrlService.service';

@Component({
  selector: 'app-question-result-view',
  standalone: true,
  imports: [ImageSliderComponent, CommonModule],
  templateUrl: './question-result-view.component.html',
  styleUrl: './question-result-view.component.scss'
})
export class QuestionResultViewComponent {
  @Input() questionResult?: QuestionResultView;

  constructor(
    protected urlService: UrlService
  ) {
    
  }

  get profileAnswer(): AnswerProfile | undefined {
    return this.questionResult?.ProfileAnswers
    .find(profileAnswer => profileAnswer.QuestionId == this.questionResult?.Id);
  }

  getQuestionClass(answerId: number) {
    // const profileAnswer = this.questionResult?.ProfileAnswers
    //   .find(profileAnswer => 
    //     profileAnswer.QuestionId == this.questionResult?.Id);

    if(this.profileAnswer && this.profileAnswer.IsCorrect && 
      this.profileAnswer.AnswersIdList.includes(answerId)) {
      return 'question-result-questions__question-true';
    }
    else if(this.profileAnswer && !this.profileAnswer.IsCorrect && 
      this.profileAnswer.AnswersIdList.includes(answerId)) {
      return 'question-result-questions__question-false';
    }

    return '';
  }
}
