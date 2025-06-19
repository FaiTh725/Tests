import { Component, ElementRef, ViewChild } from '@angular/core';
import { AuthService } from '../../core/services/Auth.service';
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { Router } from '@angular/router';
import { HttpService } from '../../core/services/Http.service';
import { Pagination } from '../../shared/interfaces/utils/Pagination';
import { PaginationComponent } from "../../shared/components/pagination/pagination.component";
import { TestEditableCardComponent } from "../../shared/components/test-editable-card/test-editable-card.component";
import { AddQuestionForm } from '../../shared/components/add-question-form/add-question-form.component';
import { TestWithQuestions } from '../../shared/interfaces/tests/TestWithQuestions';
import { CommonModule } from '@angular/common';
import { TestResult } from '../../shared/interfaces/results/TestResult';
import { TestResultCardComponent } from "../../shared/components/test-result-card/test-result-card.component";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [PrimaryButtonComponent, PaginationComponent, TestEditableCardComponent, CommonModule, TestResultCardComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  profileCreatedTests: TestWithQuestions[] = []
  createdTestsPagination: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 5
  }

  profileResults: TestResult[] = [];
  resultPagination: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 5
  };

  currentTab: string = "Tests";
  @ViewChild('profileTabs', {static: true, read: ElementRef}) profileTabs!: ElementRef;

  constructor(
    public userService: AuthService,
    private router: Router,
    private httpService: HttpService
  ) {

  }
  
  handleExit() {
    this.userService.Logout();
    this.router.navigate(["/authorization/sign-in"]);
  }
  
  ngOnInit() {
    this.getResult();
    this.getTests();

    this.profileTabs.nativeElement.scrollTo(0, 0);
  }

  handleDeleteTest(testId: number) {
    this.httpService.deleteRequest(`testing/Test/DeleteTest?testId=${testId}`)
    .subscribe({
      next: _ => {
        this.getTests();
      },
      error: _ => {
        console.log("unknown error");
      }
    });
  }

  handleUpdateTest(updatedTest: TestWithQuestions) {
    const testIndex = this.profileCreatedTests
      .findIndex(x => x.Id == updatedTest.Id);

    if(testIndex !== -1) {
      this.profileCreatedTests[testIndex] = {...updatedTest};
    }
    else {  
      console.error("Profiles tests doesnt contain test for cur test id");
    }
  }

  handleAddTestQuestion(testId: number, questionToAdd: AddQuestionForm) {
    var formData = new FormData();
    formData.append("TestId", String(testId));
    formData.append("TestQuestion", questionToAdd.QuestionText);
    formData.append("QuestionWeight", String(questionToAdd.QuestionWeight));
    formData.append("QuestionType", String(questionToAdd.HasOneAnswer ? 0 : 1));
    
    for(const questionImage of questionToAdd.QuestionImages) {
      formData.append("QuestionImages", questionImage);
    }

    for(const [index, answer] of questionToAdd.Answers.entries()) {
      formData.append(`Answers[${index}].Answer`, answer.Answer);
      formData.append(`Answers[${index}].IsCorrect`, String(answer.IsCorrect));
      answer.AnswerImages.forEach(image => {
        formData.append(`Answers[${index}].AnswerImages`, image);
      });
    }

    const testIndex = this.profileCreatedTests
      .findIndex(x => x.Id == testId);
    
    this.httpService.postFormDataRequest("testing/Question/AddQuestion", formData)
      .subscribe(
      {
        next: (data: any) => {
          this.profileCreatedTests[testIndex].Questions
          .push({
            Id: data.id,
            TestQuestion: data.testQuestion,
            QuestionWeight: data.questionWeight,
            QuestionType: data.questionType,
            QuestionImages: data.questionImages,
            Answers: [...data.answers.map((answer:any) => ({
              Id: answer.id,
              IsCorrect: answer.isCorrect,
              Answer: answer.answer,
              QuestionId: answer.questionId,
              questionAnswerImages: [...answer.questionAnswersImageUrls]
            }))]
          });
        },
        error: error => {
          console.log(error);
        }
      });
  }

  executeTestsPagination(pagination: Pagination) {
    this.createdTestsPagination = {...pagination};

    this.getTests();
  }

  executeResultsPagination(pagination: Pagination) {
    this.resultPagination = {...pagination};

    this.getResult();
  }

  getTests() {
    var requetsUrl = `testing/Profile/GetProfileTests?` +
      `ProfileEmail=${this.userService.User?.Email}&` +
      `Page=${this.createdTestsPagination.Page}&` +
      `PageCount=${this.createdTestsPagination.PageSize}`;
    this.httpService.getRequest(requetsUrl)
      .subscribe({
        next: (data: any) => {
          this.profileCreatedTests = data.data.map((test: any) => ({
            Id: test.id,
            Name: test.name,
            Description: test.description,
            CreatedTime: new Date(test.createdTime),
            IsPublic: test.isPublic,
            TestType: test.testType == "Timed" ? 0 : 1,
            DurationInMinutes: test.durationInMinutes, 
            Questions: [],
            Owner: {
              Email: test.owner.email,
              Name: test.owner.name,
              Id: test.owner.id
            }
          }));

          this.createdTestsPagination = {
            MaxSize: data.maxSize,
            Page: data.page,
            PageSize: data.pageSize
          };       
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }

  getResult() {
    const requestUrl = `testing/TestSession/GetProfileSessions?page=${this.resultPagination.Page}&pageSize=${this.resultPagination.PageSize}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.profileResults = data.data.map((result: any) => ({
          Id: result.id,
          TestId: result.testId,
          TestName: result.testName,
          ProfileId: result.profileId,
          StartTime: result.startTime,
          EndTime: result.endTime,
          Percent: result.percent
        }));

        this.resultPagination = {
          Page: data.page,
          PageSize: data.pageSize,
          MaxSize: data.maxSize
        }
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }

  handleSwitchTab(tabName: string) {
    this.currentTab = tabName;

    const scrollTo = this.currentTab === "Tests" ?
      0:
      this.profileTabs.nativeElement.scrollWidth;

    this.profileTabs.nativeElement.scrollTo({
      top: 0,
      left: scrollTo,
      behavior: "smooth",
    });
  }
}
