import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const managerGuard: CanActivateFn = () => {
  const user = inject(AuthService).user();
  return user?.role === 'Manager' ? true : inject(Router).createUrlTree(['/']);
};
