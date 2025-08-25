import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserCredentialsValidatorService {

  public constructor() { }

  public IsValidEmail(email: string): boolean {
    if(email.length < 2 || email.length > 100) {
      return false;
    }

    const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;
  
    return emailRegex.test(email);
  }

  public IsValidPassword(password: string): boolean {
    if(password.length < 5 || 
      password.length > 30) {
      return false;
    }

    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d).+$/;

    return passwordRegex.test(password);
  }
}
