import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { jwtDecode } from 'jwt-decode';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  auth  = inject(AuthenticationService);
  constructor(private http:HttpClient) { }


  logout() {
    this.auth.unsetToken();
  }

  get() {
    let userId = this.getUserIdFromToken();
    const headers = this.headers;
    let user = this.http.get(environment.API_BASE_URL + `/Profile/${userId}`, { headers })
    return user;
  }
  update(user: User): Observable<any> {
    const headers = this.headers;
    return this.http.put(environment.API_BASE_URL + `/Profile/${this.getUserIdFromToken()}`, user, { headers })
  }

  delete(userId: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http.delete(environment.API_BASE_URL + `/Profile/${userId}`, { headers })

  }

  uploadProfilePicture(file: File): Observable<any> {
    let id = this.getUserIdFromToken();
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);

    return this.http.put<any>(environment.API_BASE_URL + `/Profile/picture/${id}`, formData);
  }

  getPlans() {
    return this.http.get(environment.API_BASE_URL + `/Profile/plans`, { headers:this.headers })
  }

  Subscribe(subscription : {
    paymentId: string,
    status: string,
    workerId: string,
    planId: number
  }){
    return this.http.post(environment.API_BASE_URL + `/Worker/Subscribe`, subscription ,{ headers :this.headers})
  }


  getUserIdFromToken(): string | null {
    try {
      const decodedToken: any = jwtDecode(this.getToken())
      const userIdClaimKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

      return decodedToken[userIdClaimKey] || null;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  getUserEmailFromToken(): string | null {
    try {
      const decodedToken: any = jwtDecode(this.getToken())
      const userEmailClaimKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email';

      return decodedToken[userEmailClaimKey] || null;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  getToken(): string {
    return localStorage.getItem(environment.JwtTokenKey) as string;
  }


  
  isLoggedIn(): boolean {

    return !!localStorage.getItem(environment.JwtTokenKey);
  }
}
