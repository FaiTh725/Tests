import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private apiBaseUrl = "https://localhost:5502/";
  private refreshTokenUrl = this.apiBaseUrl + "auth/Auth/Refresh";

  constructor (
    private httpClient: HttpClient,
    private router: Router) {}

  postRequest(address: string, data: any) {
    return this.httpClient.post(this.apiBaseUrl + address, 
      data, {
      withCredentials: true
    }).pipe(catchError(error => this.processError(error, 'POST', address, data)))
  }

  getRequest(address: string) {
    return this.httpClient.get(this.apiBaseUrl + address, {
      withCredentials: true
    }).pipe(catchError(error => this.processError(error, 'GET', address)))
  }

  deleteRequest(address: string, data?: any) {
    return this.httpClient.delete(this.apiBaseUrl + address, {
      body: data,
      withCredentials: true
    }).pipe(catchError(error => this.processError(error, 'DELETE', address, data)));
  }
  
  private processError(
    error: any, method: string, 
    url: string, data?: any) {
    
    if(error.status === 401)
    {
      console.log("occured request that returned unauthorize error");
      
      return this.refreshToken().pipe(switchMap(() => {
        console.log("sending request to refresh tokens");

        switch(method) {
          // TODO: refactoring to enum
          case 'POST': {
            return this.httpClient.post(
              this.apiBaseUrl + url, data, {withCredentials: true});
          }
          case 'GET': {
            return this.httpClient.get(
              this.apiBaseUrl + url, {withCredentials: true});
          }
          case 'PATCH': {
            return this.httpClient.patch(
              this.apiBaseUrl + url, data, {withCredentials: true});
          }
          case 'DELETE': {
            return this.httpClient.delete(
              this.apiBaseUrl + url, {
                body: data, 
              });
          }
          default:
            return throwError(() => new Error("Unknow error"))
        }
      }), catchError(_ => {
        this.router.navigate(["/authorization/sign-in"]);
        return throwError(() => new Error("Token is expired"));
      }));
    }

    return throwError(() => error);
  }

  private refreshToken() {
    return this.httpClient.post(this.refreshTokenUrl, {}, {
      withCredentials: true
    });
  }
}
