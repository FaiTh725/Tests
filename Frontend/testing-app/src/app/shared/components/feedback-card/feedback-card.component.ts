import { Component, ElementRef, EventEmitter, Input, output, Output, ViewChild } from '@angular/core';
import { FeedbackInfo } from '../../interfaces/feedbacks/FeedbackInfo';
import { CommonModule, DatePipe } from '@angular/common';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { AuthService } from '../../../core/services/Auth.service';
import { HttpService } from '../../../core/services/Http.service';
import { Pagination } from '../../interfaces/utils/Pagination';
import { ReplyInfo } from '../../interfaces/replies/ReplyInfo';
import { ReplyCardComponent } from "../reply-card/reply-card.component";
import { LargeInputComponent } from "../inputs/large-input/large-input.component";

@Component({
  selector: 'app-feedback-card',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent, CommonModule, ReplyCardComponent, LargeInputComponent],
  templateUrl: './feedback-card.component.html',
  styleUrl: './feedback-card.component.scss'
})
export class FeedbackCardComponent {
  replies: ReplyInfo[] = [];
  sendReplyText: string = "";
  sendReplyErrorText: string = "";
  
  @Input() feedback?: FeedbackInfo;

  @Output() delete = new EventEmitter<number>();
  @Output() changed = new EventEmitter<number>();

  @ViewChild('imageSlider', {static: false, read: ElementRef}) imageSlider!: ElementRef;

  pagination: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 5
  };

  constructor(
    protected authService:AuthService,
    private httpService: HttpService
  ) 
  {
  }

  ngOnInit() {
    this.getReplies();
  }

  handleSendReply() {
    if(this.sendReplyText.length === 0) {
      this.sendReplyErrorText = "This field is required";
      return;
    }
    // TODO: add check api response
    this.httpService.postRequest("feedback/FeedbackReply/SendFeedbackReply", {
      feedbackId: this.feedback?.Id,
      text: this.sendReplyText
    })
    .subscribe({
      next: (data: any) => {
        this.replies.push({
          Id: data.id,
          Text: data.text,
          FeedbackId: data.feedbackId,
          SendTime: data.sendTime,
          UpdateTime: data.updateTime,
          Owner: {
            Id: data.owner.id,
            Name: data.owner.name,
            Email: data.owner.email
          }
        });

        this.sendReplyText = "";
      },
      error: error => {
        if(error.status === 409) {
          this.sendReplyErrorText = "You has already sent a reply on this feedback";
        }
        else {
          console.error("unknown error");
        }
      }
    });
  }

  handleSendReview(isPositive: boolean) {
    this.httpService.postRequest("feedback/Feedback/SendReview", {
      feedbackId: this.feedback?.Id,
      isPositive: isPositive
    }).subscribe({
      error: _ => {
        console.error("unknown error");
      },
      complete: () => {
        this.changed.emit(this.feedback?.Id);
      }
    });
  } 

  getReplies() {
    const requestUrl = `feedback/FeedbackReply/GetFeedbackReplies?FeedbackId=${this.feedback?.Id}&Page=${this.pagination.Page}&PageSize=${this.pagination.PageSize}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data:any) => {
        this.replies = [... data.items.map((reply: any) => ({
          Id: reply.id,
          Text: reply.text,
          FeedbackId: reply.feedbackId,
          SendTime: reply.sendTime,
          UpdateTime: reply.updateTime,
          Owner: {
            Id: reply.owner.id,
            Name: reply.owner.name,
            Email: reply.owner.email
          }
        }))]
      },
      error: _ => {
        console.error("unknown error");  
      }
    });
  }

  handleDeleteReply(replyId: number) {
    this.httpService.deleteRequest(
      "feedback/FeedbackReply/DeleteFeedbackReply", {
        replyId: replyId
      }).subscribe({
        error: _ => {
          console.error("unknown error");
        },
        complete: () => {
          this.getReplies();
        }
      });
  }

  handleUploadReplies() {
    if((this.pagination.Page + 1) * this.pagination.PageSize > this.pagination.MaxSize) {
      return;
    }

    this.pagination.Page ++;

    this.getReplies();
  }

  createArray(countElements: number) {
    return new Array(countElements);
  }

  handleDeleteFeedback() {
    this.delete.emit(this.feedback?.Id);
  }

  changeImageHost(imageUrl: string) {
    const correctHostUrl = imageUrl.replace("azurite_storage", "localhost");
    return `url(${correctHostUrl})`;
  }

  handleScroll(direction: string) {
    const scrollUnit = 60;
    
    if(direction === "left") {
      this.imageSlider.nativeElement.scrollBy({
        top: 0,
        left: -scrollUnit,
        behavior: "smooth",
      });
      console.log("left");
      
    }
    else if(direction === "right") {
      this.imageSlider.nativeElement.scrollBy({
        top: 0,
        left: scrollUnit,
        behavior: "smooth",
      });
      console.log("right");
    }
  }
}
