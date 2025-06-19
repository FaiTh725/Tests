import { Component, ViewContainerRef } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { TestType } from '../../shared/interfaces/tests/TestType';
import { QuestionType } from '../../shared/interfaces/questions/QuestionType';
import { ImageSliderComponent } from "../../shared/components/image-slider/image-slider.component";
import { CommonModule } from '@angular/common';
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { HttpService } from '../../core/services/Http.service';
import { Subscription } from 'rxjs';
import { ModalService } from '../../core/services/Modal.service';
import { TestResultComponent } from '../../shared/components/modals/test-result/test-result.component';
import { UrlService } from '../../core/services/UrlService.service';
import { SignalRService } from '../../core/services/SignalRService.service';

export let browserRefresh = false;

@Component({
  selector: 'app-test-passing',
  standalone: true,
  imports: [ImageSliderComponent, CommonModule, PrimaryButtonComponent],
  templateUrl: './test-passing.component.html',
  styleUrl: './test-passing.component.scss'
})
export class TestPassingComponent {
  test?: TestToPass;
  
  currentQuestionIndex: number | null = null;
  answeredQuestions: number[] = [];
  // dictinary to store user answers and display it
  answersMemory: Record<number, number[]> = {};

  private routeSubscribe!: Subscription;

  get currentQuestion (): CloseQuestion | null {
    if (this.currentQuestionIndex !== null) {
      return this.test?.Questions[this.currentQuestionIndex]!;
    }

    return null;
  }

  constructor(
      private router: Router,
      private httpService: HttpService,
      private modalService: ModalService,
      private viewContainerRef: ViewContainerRef,
      protected urlService: UrlService,
      private signalRService: SignalRService
    ) {
    const navigation = router.getCurrentNavigation();    
    const testToPass = navigation?.extras.state?.["test-to-pass"];

    if(testToPass) {
      this.test = {
        Id: testToPass.id,
        Name: testToPass.name,
        Description: testToPass.description,
        TestType: testToPass.testType === "Progressive" ? 
          TestType.Progressive : TestType.Timed,
        DurationInMinutes: testToPass.durationInMinutes,
        Questions: [... testToPass.questions.map((question: any) => ({
          Id: question.id,
          Question: question.testQuestion,
          QuestionType: question.questionType === "OneAnswer" ? 
            QuestionType.OneAnswer : QuestionType.ManyAnswers,
          Images: [... question.questionImages],
          Answers: question.answers.map((answer:any) => ({
            Id: answer.id,
            Images: [answer.questionAnswerImages],
            Answer: answer.answer
          }))
        }))]
      };

      this.currentQuestionIndex = this.test?.Questions.length === 0 ? null : 0;
    
      this.routeSubscribe = router.events.subscribe((event) => {
        if(event instanceof NavigationStart) {
          this.handleStopTest();
        }
      });
    }
    else {
      router.navigate(["/error"]);
    }
  }

  ngOnInit() {
    if(this.test?.DurationInMinutes != null) {
      this.signalRService.HubSessionConnection.connect();
  
      this.signalRService.HubSessionConnection.connection
      .on("TestStoped", (data: any) => {
        this.modalService.openModa(this.viewContainerRef, TestResultComponent, {
            sessionId: data
          }).subscribe({
            complete: () => {
              this.routeSubscribe.unsubscribe();
              this.router.navigate(["/profile"]);
            }
          });
      });
    }
  }

  handleStopTest() {
    const requestUrl = "testing/TestSession/StopTest";
    this.httpService.postRequest(requestUrl, {})
    .subscribe({
      next: (data: any) => {
        if(this.test?.DurationInMinutes != null) {
          this.signalRService.HubSessionConnection.disconnect();
        }

        this.modalService.openModa(this.viewContainerRef, TestResultComponent, {
          sessionId: data
        }).subscribe({
          complete: () => {
            this.routeSubscribe.unsubscribe();
            this.router.navigate(["/profile"]);
          }
        });
      },
      error: _ => {
        console.error("error with stoping test");
      }
    });
  }

  handleSendAnswerQuestion() {
    if(!this.currentQuestion) {
      console.error("question isnt selected");
      
      return;
    }
    
    const questionId = this.currentQuestion.Id;

    const requestUrl = "testing/TestSession/SendTestAnswer";
    this.httpService.postRequest(requestUrl, {
      questionId: questionId,
      questionAnswersId: this.answersMemory[questionId]
    })
    .subscribe({
      error: _ => {
        console.error("error with sending question answer");
      },
      complete: () => {
        this.answeredQuestions = [... this.answeredQuestions, questionId]
        console.log(this.answeredQuestions);
        
        if(this.currentQuestionIndex != this.test!.Questions.length - 1) {
          this.currentQuestionIndex! ++;
        }
      }
    });
  }

  handleSelectAnswers(answerId: number) {
    if(!this.currentQuestion) {
      return;
    }

    if(this.currentQuestion.QuestionType == 0) {
      this.answersMemory[this.currentQuestion.Id] = [answerId];
      return;
    }
    
    if(this.currentQuestion.Id in this.answersMemory &&
      this.answersMemory[this.currentQuestion.Id].includes(answerId)
    ) {
      this.answersMemory[this.currentQuestion.Id] = [...this.answersMemory[this.currentQuestion.Id]
        .filter(value => value !== answerId)];
    }
    else if (this.currentQuestion.Id in this.answersMemory) {
      this.answersMemory[this.currentQuestion.Id]
        .push(answerId);
    }
    else {
      this.answersMemory[this.currentQuestion.Id] = [answerId];
    }
  }

  valueInRecord(key: number): boolean {
    if(!this.currentQuestion) {
      return false;
    }

    if(this.currentQuestion.Id in this.answersMemory) {
      
      return this.answersMemory[this.currentQuestion.Id]
        .includes(key);
    }

    return false;
  }

  changeImagesDomain(images: string[]): string[] {
    return images.map(image => `url(${image.replace("azurite_storage", "localhost")})`);
  }

  ngOnDestroy() {
    if(this.routeSubscribe) {
      this.routeSubscribe.unsubscribe();
    }
  }
}

interface TestToPass {
  Id: number,
  Name: string,
  Description: string,
  TestType: TestType,
  DurationInMinutes: number | null,
  Questions: CloseQuestion[]
}

interface CloseQuestion {
  Id: number,
  Question: string,
  QuestionType: QuestionType,
  Images: string[],
  Answers: CloseAnswer[]
}

interface CloseAnswer {
  Id: number,
  Images: string[],
  Answer: string
}

