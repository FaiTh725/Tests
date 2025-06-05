import { Component } from '@angular/core';
import { AuthLayoutComponent } from "../../shared/components/auth-layout/auth-layout.component";
import { PrimaryInputComponent } from "../../shared/components/inputs/primary-input/primary-input.component";
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { Router, RouterLink } from '@angular/router';
import { PasswordInputComponent } from "../../shared/components/inputs/password-input/password-input.component";
import { UserCredentialsValidatorService } from '../../core/services/UserCredentialsValidator.service';
import { HttpService } from '../../core/services/Http.service';
import { AuthService } from '../../core/services/Auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [AuthLayoutComponent, PrimaryInputComponent, PrimaryButtonComponent, RouterLink, PasswordInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  form: RegistrationForm = {
    Name: "",
    Email: "",
    Password: "",
    RepeatPassword: ""
  }
  formError: RegistrationErrorForm = {
    NameError: "",
    EmailError: "",
    PasswordError: "",
    RepeatPasswordError: ""
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private userValidator: UserCredentialsValidatorService,
    private httpService: HttpService
  ) {
    const navigation = router.getCurrentNavigation();
    const email = navigation?.extras.state?.["email"];
    
    if(email != null) {
      this.form.Email = email
    }
  }

  handleRegister() {
    var isValidForm = true;

    if(!this.userValidator.IsValidEmail(this.form.Email)) {
      this.formError.EmailError = "Invalid signature, must contain @ and a dot after";
      isValidForm = false;
    }
    if(!this.userValidator.IsValidPassword(this.form.Password)) {
      this.formError.PasswordError = "Password must be at least 5 long, contain one letter and one number";
      isValidForm = false;
    }
    if(this.form.Name.length < 2 || 
      this.form.Name.length > 50) {
      this.formError.NameError = "Name must be between 2 and 50";
      isValidForm = false;
    }
    if(this.form.Password != this.form.RepeatPassword) {
      this.formError.RepeatPasswordError = "Passwords dont match";
      isValidForm = false;
    }

    if(!isValidForm) {
      return;
    }

    this.httpService.postRequest("auth/Auth/Register", {
      email: this.form.Email,
      password: this.form.Password,
      userName: this.form.Name
    }).subscribe({
      next: (data: any) => {
        this.authService.Login({
          Email: data.email,
          Name: data.name,
          Role: data.role
        })

        this.router.navigate([""]);
      },
      error: (error) => {
        if(error.status === 400) {
          if(error.error.errors.Password != undefined){
            this.formError.PasswordError = error.error.errors.Password.join(); 
          }
          if(error.error.errors.Email != undefined) {
            this.formError.EmailError = error.error.errors.Email.join(); 
          }
          if(error.error.errors.UserName != undefined) {
            this.formError.NameError = error.error.errors.UserName.join(); 
          }
        }
        else if (error.status === 409) {
          this.formError.EmailError = "Email already registered";
        }
        else if(error.status === 403) {
          this.formError.EmailError = "Confirm email before registration"
        }
        else {
          console.error("unknown error");
        }
      }
    });
  }

  handleClearErrorForm() {
    this.formError.EmailError = "";
    this.formError.NameError = "";
    this.formError.PasswordError = "";
    this.formError.RepeatPasswordError = "";
  }
}

interface RegistrationForm {
  Name: string,
  Email: string,
  Password: string,
  RepeatPassword: string
}

interface RegistrationErrorForm {
  NameError: string,
  EmailError: string,
  PasswordError: string,
  RepeatPasswordError: string
}