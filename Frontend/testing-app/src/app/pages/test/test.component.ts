import { Component } from '@angular/core';
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

@Component({
  selector: 'app-test',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent, FeedbackCardComponent, FeedbackRatingComponent, PaginationComponent],
  templateUrl: './test.component.html',
  styleUrl: './test.component.scss'
})
export class TestComponent {
  test?: TestInfo;
  testFeedbacks: FeedbackInfo[] = [];

  feedbacksPagination: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 10
  }
  
  constructor(
    private httpService: HttpService,
    private router: Router,
    private activedRoute: ActivatedRoute,
    private dialog: MatDialog
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

    this.executeGetTest(testId);
    this.executeGetTestFeedbacks(testId, this.feedbacksPagination.Page, this.feedbacksPagination.PageSize);
  }

  executeGetTest(testId: number) {
    const requestUrl = `testing/Test/GetTestInfo?testId=${testId}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data:any) => {
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
        }
      },
      error: error => {
        if(error.status === 404) {
          this.router.navigate(["/not-found"]);
        }
        else {
          console.error("unknown error");
        }
      }
    });
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
        this.executeGetTestFeedbacks(
        this.test!.Id, 
        this.feedbacksPagination.Page, 
        this.feedbacksPagination.PageSize);
        }
    });
  }

  executePagination(pagination: Pagination) {
    this.feedbacksPagination = {...pagination};

    this.executeGetTestFeedbacks(this.test!.Id, 
      this.feedbacksPagination.Page, 
      this.feedbacksPagination.PageSize);
  }

  executeGetTestFeedbacks(testId: number, page:number, pageSize: number) {
    const getFeedbacksRequestUrl = `feedback/Feedback/GetTestFeedbacks?TestId=${testId}&Page=${page}&PageSize=${pageSize}`;
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
        this.executeGetTestFeedbacks(this.test!.Id, 
        this.feedbacksPagination.Page, 
        this.feedbacksPagination.PageSize);
        }
    });
  }
}
