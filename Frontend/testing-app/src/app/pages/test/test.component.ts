import { Component, ViewContainerRef } from '@angular/core';
import { HttpService } from '../../core/services/Http.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TestInfo } from '../../shared/interfaces/tests/TestInfo';
import { DatePipe } from '@angular/common';
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { Pagination } from '../../shared/interfaces/utils/Pagination';
import { FeedbackInfo } from '../../shared/interfaces/feedbacks/FeedbackInfo';
import { FeedbackCardComponent } from "../../shared/components/feedback-card/feedback-card.component";
import { FeedbackRatingComponent } from "../../shared/components/inputs/feedback-rating/feedback-rating.component";
import { SendFeedbackComponent } from '../../shared/components/send-feedback/send-feedback.component';
import { MatDialog } from '@angular/material/dialog';
import { PaginationComponent } from "../../shared/components/pagination/pagination.component";
import { TestRating } from '../../shared/interfaces/tests/TestRating';
import { Rating, TestRatingsComponent } from "../../shared/components/test-ratings/test-ratings.component";
import { catchError, Observable, tap, throwError } from 'rxjs';
import { ModalService } from '../../core/services/Modal.service';
import { WarningBeforeTestComponent } from '../../shared/components/modals/warning-before-test/warning-before-test.component';

@Component({
  selector: 'app-test',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent, FeedbackCardComponent, FeedbackRatingComponent, PaginationComponent, TestRatingsComponent],
  templateUrl: './test.component.html',
  styleUrl: './test.component.scss'
})
export class TestComponent {
  test?: TestInfo;
  testRating?: TestRating;
  testFeedbacks: FeedbackInfo[] = [];

  selectedRatingFilter: number | null = null;
  countFeedbacks = 0;

  get executingGetFeedbacks() {
    if(!this.test) {
      return ;
    }
    
    return this.selectedRatingFilter ? 
    this.executeGetTestFeedbacksByRating(this.test.Id, this.selectedRatingFilter) :
    this.executeGetTestFeedbacks(this.test.Id);
  }

  feedbacksPagination: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 10
  }
  
  constructor(
    private httpService: HttpService,
    private router: Router,
    private activedRoute: ActivatedRoute,
    private dialog: MatDialog,
    private modalService: ModalService,
    private viewContainerRef: ViewContainerRef
  ) {
    
  }

  ngOnInit() {
    var testId: number | null = null;

    this.activedRoute.queryParamMap
    .subscribe((params) => {
      testId = Number(params.get("id"))
    })

    if(!testId) {
      console.error("testId from query params not found");
      this.router.navigate(["/not-found"]);
      return;
    }

    this.executeGetTest(testId).subscribe(() => {
      this.executingGetFeedbacks;
      this.executeGetTestRating();
    });
  }

  handleStartTest() {
    this.modalService.openModal(
      this.viewContainerRef, WarningBeforeTestComponent)
    .subscribe(result => {
      if(result !== "confirm") {
        return;
      }

      const requestUrl = "testing/TestSession/StartTest";
      this.httpService.postRequest(requestUrl, {
        testId: this.test?.Id
      }).subscribe({
        next: (data: any) => {
          this.router.navigate(["/test-passing"], {
            state: { "test-to-pass": data}
          });
        },
        error: _ => {
          console.error("unknown error");
        }
      });
    });
  }

  handleFilterByRating(rating: number) {
    this.selectedRatingFilter = this.selectedRatingFilter === rating ?
      null : rating;

    this.executingGetFeedbacks;
  }

  executeGetTestRating() {
    const requestUrl = `feedback/Feedback/GetTestStatistics?testId=${this.test?.Id}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.testRating = {
          Id: this.test!.Id,
          AverageRating: data.averageRating,
          Ratings: {
            1: data.ratingDistribution[1] ?? 0,
            2: data.ratingDistribution[2] ?? 0,
            3: data.ratingDistribution[3] ?? 0,
            4: data.ratingDistribution[4] ?? 0,
            5: data.ratingDistribution[5] ?? 0,
            6: data.ratingDistribution[6] ?? 0,
            7: data.ratingDistribution[7] ?? 0,
            8: data.ratingDistribution[8] ?? 0,
            9: data.ratingDistribution[9] ?? 0,
            10: data.ratingDistribution[10] ?? 0,
          }
        };
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }

  executeGetTest(testId: number): Observable<any> {
    const requestUrl = `testing/Test/GetTestInfo?testId=${testId}`;
    return this.httpService.getRequest(requestUrl).pipe(
    tap((data: any) => {
      this.test = {
        Id: data.id,
        Name: data.name,
        Description: data.description,
        CreatedTime: data.createdTime,
        IsPublic: data.isPublic,
        TestType: data.testType == "Timed" ? 0 : 1,
        DurationInMinutes: data.durationInMinutes,
        Owner: {
          Id: data.owner.id,
          Name: data.owner.name,
          Email: data.owner.email
        }
      };
    }),
    catchError(error => {
      if (error.status === 404) {
        this.router.navigate(["/not-found"]);
      } else {
        console.error("unknown error");
      }
      return throwError(() => error);
    }));
  }

  handleOpenSendFeedbackForm() {
    const dialogRef = this.dialog.open(SendFeedbackComponent, {
      width: "1200px",
      
      data: {
        testId: this.test?.Id
      }
    });

    dialogRef.afterClosed().subscribe(data => {
      if(data?.isSuccess) {
        this.executingGetFeedbacks;
        this.executeGetTestRating();
      }
    });
  }

  getRecordKeys(): Rating[] {
    if(this.testRating) {
      return Object.keys(this.testRating.Ratings)
      .map(x => (
        {
          Rating: Number(x), 
          Count: this.testRating!.Ratings[Number(x)]
        }))
      .sort(x => x.Rating);
    }

    return [];
  }

  executePagination(pagination: Pagination) {
    this.feedbacksPagination = {...pagination};

    this.executingGetFeedbacks;
  }

  executeGetTestFeedbacks(testId: number) {
    const getFeedbacksRequestUrl = `feedback/Feedback/GetTestFeedbacks?TestId=${testId}&Page=${this.feedbacksPagination.Page}&PageSize=${this.feedbacksPagination.PageSize}`;
    this.httpService.getRequest(getFeedbacksRequestUrl)
    .subscribe({
      next: (data: any) => {
        this.testFeedbacks = [... data.items.map((feedback:any) => ({
          Id: feedback.id,
          Images: [...feedback.feedbackImages],
          Text: feedback.text,
          TestId: feedback.testId,
          Rating: feedback.rating,
          SendTime: feedback.sendTime,
          UpdateTime: feedback.updateTime,
          CountPositiveReviews: feedback.countPositiveReviews,
          CountNegativeReviews: feedback.countNegativeReviews,
          Owner: {
            Id: feedback.profile.id,
            Name: feedback.profile.name,
            Email: feedback.profile.email
          }
        }))]

        this.feedbacksPagination = {
          MaxSize: data.maxCount,
          Page: data.page,
          PageSize: data.pageCount
        };

        this.countFeedbacks = this.feedbacksPagination.MaxSize;
      },
      error: error => {
        if(error.status === 404) {
          console.error("internal server critical error");
        }
        else {
          console.error("unknown error");
        }
      }
    });
  }

  executeGetTestFeedbacksByRating(testId: number, rating: number) { 
    const requestUrl = `feedback/Feedback/GetFeebacksByFilter?` + 
    `TestId=${testId}&Rating=${rating}&Page=${this.feedbacksPagination.Page}&PageSize=${this.feedbacksPagination.PageSize}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.testFeedbacks = [... data.items.map((feedback:any) => ({
          Id: feedback.id,
          Images: [...feedback.feedbackImages],
          Text: feedback.text,
          TestId: feedback.testId,
          Rating: feedback.rating,
          SendTime: feedback.sendTime,
          UpdateTime: feedback.updateTime,
          CountPositiveReviews: feedback.countPositiveReviews,
          CountNegativeReviews: feedback.countNegativeReviews,
          Owner: {
            Id: feedback.profile.id,
            Name: feedback.profile.name,
            Email: feedback.profile.email
          }
        }))]

        this.feedbacksPagination = {
          MaxSize: data.maxCount,
          Page: data.page,
          PageSize: data.pageCount
        }
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }

  handleDeleteFeedback(feedbackId: number) {
    const deleteRequestUrl = `feedback/Feedback/DeleteFeedback`;
    this.httpService.deleteRequest(deleteRequestUrl, {
      feedbackId: feedbackId
    })
    .subscribe({
      error: _ => {
        console.error("unknow error");
      },
      complete: () => {
        this.executingGetFeedbacks;
        this.executeGetTestRating();
      }
    });
  }

  getFeedback(feedbackId: number) {
    const requestUrl = `feedback/Feedback/GetFeedback?feedbackId=${feedbackId}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        const feedbackIndex = this.testFeedbacks
          .findIndex(x => x.Id == feedbackId);
      
        this.testFeedbacks[feedbackIndex] = {
          Id: data.id,
          Images: [...data.feedbackImages],
          Text: data.text,
          TestId: data.testId,
          Rating: data.rating,
          SendTime: data.sendTime,
          UpdateTime: data.updateTime,
          CountPositiveReviews: data.countPositiveReviews,
          CountNegativeReviews: data.countNegativeReviews,
          Owner: {
            Id: data.profile.id,
            Name: data.profile.name,
            Email: data.profile.email
          }
        }; 
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }
}
