import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';
import { Observable, catchError, tap } from 'rxjs';
import { environment as env } from '../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import { Registration } from '../models/registration';
import { LoginModel } from '../models/login-model';
import { CredentialResponse } from 'google-one-tap';
@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  headers = new HttpHeaders({ 'Content-Type': 'application/json' });

  constructor(
    private http: HttpClient,
  ) { }

  register(user: Registration): Observable<any> {
    return this.http.post(`${env.API_BASE_URL}/Authentication/Register/`, user, { headers: this.headers })
  }

  confirmEmail(otp: string, email: string): Observable<any> {
    return this.http.put(`${env.API_BASE_URL}/Authentication/ConfirmEmail`, { "otp": otp, "email": email }, { headers: this.headers })
  }

  login(user: LoginModel): Observable<any> {
    return this.http.post<any>(`${env.API_BASE_URL}/Authentication/Login/`, user, { headers: this.headers }).pipe(
      tap((res: { token: string }) => {
        this.setToken(res.token);
      }),
      catchError(error => {
        console.error(error);

        throw error;
      })
    );
  }

  LoginWithGoogle(credentials: string): Observable<any> {
    return this.http.post<any>(
      `${env.API_BASE_URL}/Authentication/LoginWithGoogle?credential=${credentials}`,
      {}, // empty body
      { headers: this.headers }
    ).pipe(
      tap((res: { token: string }) => {
        this.setToken(res.token);
      }),
      catchError(error => {
        console.error(error);
        throw error;
      })
    );
  }

  forgetPassword(email: string): Observable<any> {
    return this.http.post<any>(`${env.API_BASE_URL}/Authentication/ForgotPassword/${ email }`, { headers: this.headers }).pipe(
      tap((res: { restToken: string }) => {
        this.setToken(res.restToken, env.RestTokenKey)
      }),
      catchError(error => {
        console.error(error);
        throw error;
      })
    );
  }

  resetPassword(email: string, password: string): Observable<any> {
    const resetToken = localStorage.getItem(env.RestTokenKey);
    if (!resetToken) {
      throw new Error('Reset token not found');
    }
    return this.http.put<any>(`${env.API_BASE_URL}/Authentication/ResetPassword/`, { email, password, token: resetToken }, { headers: this.headers }).pipe(
      tap(() => {
        this.unsetToken(env.RestTokenKey);
      }),
      catchError(error => {
        console.error(error);
        throw error;
      })
    );
  }

  sentOtp(email: string): Observable<any> {
    return this.http.post(`${env.API_BASE_URL}/Authentication/SentOtp/${ email }`, { headers: this.headers });
  }

  setToken(token: string, TokenKey: string = env.JwtTokenKey) {
    localStorage.setItem(TokenKey, token)
  }

  unsetToken(TokenKey: string = env.JwtTokenKey) {
    localStorage.removeItem(TokenKey);
  }
}
