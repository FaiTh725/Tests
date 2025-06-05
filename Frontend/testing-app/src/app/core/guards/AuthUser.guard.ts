import { inject } from '@angular/core';
import { Router, type CanActivateChildFn } from '@angular/router';
import { AuthService } from '../services/Auth.service';

export const authUserGuard: CanActivateChildFn = 
(childRoute, state) => {
  if (inject(AuthService).IsAuthentification) {
    return true;
  }
  else {
    inject(Router).navigate(["/authorization/sign-in"]);
    return false;
  }
};