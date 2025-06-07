import { Component, inject } from '@angular/core';
import { AuthLayoutComponent } from "../../shared/components/auth-layout/auth-layout.component";
import { PrimaryInputComponent } from "../../shared/components/inputs/primary-input/primary-input.component";
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { HttpService } from '../../core/services/Http.service';
import { HttpClientModule } from '@angular/common/http';
import { timer } from 'rxjs';
import { Router } from '@angular/router';
import { UserCredentialsValidatorService } from '../../core/services/UserCredentialsValidator.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [AuthLayoutComponent, PrimaryInputComponent, 
    PrimaryButtonComponent, HttpClientModule],
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.scss'
})

export class ConfirmEmailComponent {
  form: ConfirmEmail = {
    Email: "",
    Code: ""
  }

  formError: ConfirmEmailError = {
    EmailError: "",
    CodeError: ""
  }

  isCodeSend = false;
  isTimeoutActive = false;
  timerDuration = 60000;
  get btnAction () {
    return this.isCodeSend ? 
      this.handleSendCode : 
      this.handleSendConfirmationCode;
  }
  
  constructor(
    private requestService: HttpService,
    private userValidator: UserCredentialsValidatorService,
    private router: Router) {
  }

  handleSendConfirmationCode() {
    if(!this.userValidator.IsValidEmail(this.form.Email)) {
      this.formError.EmailError = "Invalid email signature"
      return;
    }

    this.requestService.postRequest(
      "auth/Auth/SendEmailConfirmationCode", {
        email: this.form.Email
    })
    .subscribe({
      next: () => {
        this.isCodeSend = true;
      },
      error: (error) => {
        if(error.status === 400) {
          this.formError.EmailError = "Invalid email or already registered";
        }
        else if(error.status === 409) {
          this.formError.EmailError = "Email already registered";
        }
        else if(error.status === 429) {
          this.formError.EmailError = "Too many tries, prease wait";
          this.isTimeoutActive = true;
          timer(this.timerDuration)
          .subscribe(() => {
            this.formError.EmailError = "",
            this.isTimeoutActive = false;
          });
        }
        else {
          console.error("Unknow error");
        }
      }
    });
  }

  handleSendCode() {
    this.requestService.postRequest(
      "auth/Auth/VerifyConfirmationCode", {
        email: this.form.Email,
        code: this.form.Code
    })
    .subscribe({
      next: () => {
        this.router.navigate(["/authorization/sign-up"], {
          state: { "email" : this.form.Email }
        });
      },
      error: (error) => {
        if(error.status === 400) {
          this.formError.CodeError = "Invalid email or code";
        }
        else {
          console.error("Unknow error");
        }
      }
    });
  }

  handleRewriteEmail() {
    this.isCodeSend = false;
    this.form.Email = "";
    this.form.Code = "";
  }

  handleClearErrorForm() {
    this.formError.CodeError = "";
    this.formError.EmailError = "";
  }
}

interface ConfirmEmail {
  Email: string,
  Code: string
}

interface ConfirmEmailError {
  EmailError: string,
  CodeError: string
}
