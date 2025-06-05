import { Component } from '@angular/core';
import { AuthLayoutComponent } from "../../shared/components/auth-layout/auth-layout.component";
import { PrimaryInputComponent } from "../../shared/components/inputs/primary-input/primary-input.component";
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { PasswordInputComponent } from "../../shared/components/inputs/password-input/password-input.component";
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/Auth.service';
import { UserCredentialsValidatorService } from '../../core/services/UserCredentialsValidator.service';
import { HttpService } from '../../core/services/Http.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [AuthLayoutComponent, PrimaryInputComponent, PrimaryButtonComponent, PasswordInputComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})

export class LoginComponent {
  form: LoginForm = {
    Email: "",
    Password: ""
  }
  errorForm: LoginErrorForm = {
    EmailError: "",
    PasswordError: ""
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private userValidator: UserCredentialsValidatorService,
    private httpService: HttpService
  ) {}

  handleLogin() {
    var isValidForm = true;

    if(!this.userValidator.IsValidEmail(this.form.Email)) {
      this.errorForm.EmailError = "Invalid signature, must contain @ and a dot after";
      isValidForm = false;
    }
    if(!this.userValidator.IsValidPassword(this.form.Password)) {
      this.errorForm.PasswordError = "Password must be at least 5 long, contain one letter and one number";
      isValidForm = false;
    }

    if(!isValidForm) {
      return;
    }

    const requestUrl = `auth/Auth/Login?Email=${this.form.Email}&Password=${this.form.Password}`;

    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.authService.Login({
          Email: data.email,
          Name: data.name,
          Role: data.role
        });
        this.router.navigate([""]);
      },
      error: (error) => {
        if(error.status === 400) {
          if(error.error.errors?.Password != undefined){
            this.errorForm.PasswordError = error.error.errors.Password.join(); 
            return;
          }
          if(error.error.errors?.Email != undefined) {
            this.errorForm.EmailError = error.error.errors.Email.join(); 
            return;
          }

          this.errorForm.EmailError = "Incorrect email or password"; 
        }
        else {
          console.error("unknown error");
        }
      }
    });
  }

  handleClearErrorForm() {
    this.errorForm.EmailError = "";
    this.errorForm.PasswordError = "";
  }
}

interface LoginForm {
  Email: string,
  Password: string
}

interface LoginErrorForm {
  EmailError: string,
  PasswordError: string
}
