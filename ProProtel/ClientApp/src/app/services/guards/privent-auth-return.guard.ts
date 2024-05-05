import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ProfileService } from '../profile.service';

export const priventAuthReturnGuard: CanActivateFn = (route, state) => {
  const profile = inject(ProfileService);
  const router = inject(Router);
  if(profile.isLoggedIn()){
    router.navigate(["/home"])
    return false;
  }
  return true;
};
