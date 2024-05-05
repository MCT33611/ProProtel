import { CanActivateFn } from '@angular/router';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { inject } from '@angular/core';
import { ProfileService } from '../profile.service';


export const authCustomerGuard: CanActivateFn = async (route, state) => {
  let profile = inject(ProfileService);
  let router = inject(Router);

  if (!profile.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  let user: any;
  try {
    user = await firstValueFrom(profile.get());
  } catch (error) {
    console.error("Failed to fetch user:", error);
    profile.logout();
    router.navigate(['/login']);
    return false;
  }


  

  if (!user?.emailConfirmed) {
    router.navigate(["/otp"], { queryParams: { email: user.email }})
    return false;
  }
  
  return true;
};
