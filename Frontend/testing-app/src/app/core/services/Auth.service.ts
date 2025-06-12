import { Injectable } from '@angular/core';
import { User } from '../../shared/interfaces/authentication/User';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private user?: User | null;
  
  constructor() {
    const jsonUserLocalStorage = localStorage.getItem("user");

    if(jsonUserLocalStorage) {
      this.user = JSON.parse(jsonUserLocalStorage);
    }
  }

  public Login(user: User): void {
    this.user = user;
    localStorage.setItem("user", JSON.stringify(this.user));
  }

  public Logout(): void {
    this.user = null;
    localStorage.removeItem("user");
  }

  public get User() {
    return this.user;
  }

  public get IsAuthentification() {
    return this.user != null;
  }
}
