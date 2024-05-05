import { Component } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginModel } from '../../../models/login-model';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../../services/authentication.service';
import { MaterialModule } from '../../../material/material.module';
import { GoogleLoginButtonComponent } from '../../../components/authentication/google-login-button/google-login-button.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    RouterLink,
    ReactiveFormsModule,
    MaterialModule,
    ReactiveFormsModule,
    FormsModule,
    GoogleLoginButtonComponent
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private auth: AuthenticationService,
    private router: Router
  ) {

    this.loginForm = this.fb.group({
      email: [
        '',
        [Validators.required, Validators.email]
      ],
      password: [
        '',
        Validators.required
      ]
    });
  }




  onSubmit() {

    if (this.loginForm.valid) {
      let user: LoginModel = {
        email: this.loginForm.value.email,
        password: this.loginForm.value.password
      }

      this.auth.login(user).subscribe({
        complete:()=>{
          this.toastr.success("wecome to proportel")
          this.router.navigate(['/home']);
        },
        error:()=>this.toastr.error("something went wrong")

      });

    } else {
      this.toastr.error("cridentials are not valid")
    }
  }
}
