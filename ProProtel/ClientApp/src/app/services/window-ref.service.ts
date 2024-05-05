import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class WindowRefService {

  constructor(private router: Router) { }
  get nativeWindow(): any {
    return getWindow();
  }
  reload()
  {
    const currentUrl = this.router.url;
    this.router.navigateByUrl('/empty').then(() => {
      this.router.navigateByUrl(currentUrl);
    });
  }
}
function getWindow(): any {
  return window;
}