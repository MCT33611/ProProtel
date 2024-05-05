declare var google : any;
import { Component, NgZone, OnInit } from '@angular/core';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { AuthenticationService } from '../../../services/authentication.service';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-google-login-button',
  standalone: true,
  imports: [],
  templateUrl: './google-login-button.component.html',
  styleUrl: './google-login-button.component.css'
})
export class GoogleLoginButtonComponent implements OnInit {
  constructor(
    private _ngZone: NgZone,
    private auth: AuthenticationService,
    private router: Router
  ){

  }
  ngOnInit(): void {

    google.accounts.id.initialize({
      client_id: environment.GoogeClientId,
      callback: this.handleCredentialResponse.bind(this)

    });
    google.accounts.id.renderButton(
      document.getElementById("google-btn"),
      { theme: "filled_blue", size: "large", width: "100%" }
    );

    google.accounts.id.prompt((notification: PromptMomentNotification) => { });
  }
  async handleCredentialResponse(response: CredentialResponse) {
    console.log("response.credential: ",response.credential);
    
    await this.auth.LoginWithGoogle(response.credential).subscribe(
      (x: any) => {
        localStorage.setItem("token", x.token);
        this._ngZone.run(() => {
          this.router.navigate(['/home']);
        })
      },
      (error: any) => {
        console.log(error);
      }
    );
  }
}
