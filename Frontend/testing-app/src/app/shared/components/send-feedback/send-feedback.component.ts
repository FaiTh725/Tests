import { Component, EventEmitter, Inject, inject, Input, Output, output } from '@angular/core';
import { LargeInputComponent } from "../inputs/large-input/large-input.component";
import { FeedbackRatingComponent } from "../inputs/feedback-rating/feedback-rating.component";
import { FileInputComponent } from "../inputs/file-input/file-input.component";
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpService } from '../../../core/services/Http.service';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-send-feedback',
  standalone: true,
  providers: [HttpService],
  imports: [LargeInputComponent, HttpClientModule, FeedbackRatingComponent, FileInputComponent, PrimaryButtonComponent],
  templateUrl: './send-feedback.component.html',
  styleUrl: './send-feedback.component.scss'
})
export class SendFeedbackComponent {
  form: FeedbackForm = {
    TestId: 0,
    Text: "",
    Rating: 0,
    Images: []
  };

  errorForm: FeedbackErrorForm = {
    TextError: "",
    CommonError: ""
  }

  constructor(
    private dialogRef: MatDialogRef<SendFeedbackComponent>,
    private httpService: HttpService,
    @Inject(MAT_DIALOG_DATA) data: any
  ) {
    this.form.TestId = data.testId;
  }

  handleClearErrorForm() {
    this.errorForm = {
      TextError: "",
      CommonError: ""
    }
  }

  handleSendFeedback() {
    if(this.form.Text.trim() == "") {
      this.errorForm.TextError = "Please describe your feeling about the test"
      return;
    }

    const formData = new FormData();
    formData.append("Text", this.form.Text);
    formData.append("TestId", String(this.form.TestId));
    formData.append("Rating", String(this.form.Rating));

    this.form.Images.forEach(image => {
      formData.append("Images", image);
    })

    this.httpService.postFormDataRequest("feedback/Feedback/SendFeedback", formData)
    .subscribe({
      error: error => {
        if(error.status === 409) {
          this.errorForm.CommonError = "You already sent feedback for this test";

          setTimeout(() => {
            this.handleClearErrorForm();
          }, 3000);
        }
        else {
          console.log("unknown error");
        }
      },
      complete: () => {
        this.dialogRef.close({
          isSuccess: true
        });
      }
    });
  }

  handleCloseForm() {
    this.dialogRef.close({
      isSuccess: false
    });
  }
}

interface FeedbackForm {
  TestId: number,
  Text: string,
  Rating: number,
  Images: File[]
}

interface FeedbackErrorForm {
  TextError: string,
  CommonError: string
}
