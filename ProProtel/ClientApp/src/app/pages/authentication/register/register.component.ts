import { Component, NgZone, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms'
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService } from '../../../services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Registration } from '../../../models/registration';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap'
import { GoogleLoginButtonComponent } from '../../../components/authentication/google-login-button/google-login-button.component';
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    GoogleLoginButtonComponent
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  registrationForm: FormGroup;
  constructor(
    private fb: FormBuilder,
    private auth: AuthenticationService,
    private toastr: ToastrService,
    private router: Router) {
    this.registrationForm = this.fb.group({
      email: [
        '',
        [Validators.required, Validators.email]
      ],
      firstName: [
        '',
        [Validators.required,this.noNumbersOrSpecialCharacters]
      ],
      lastName: [
        '',
        [Validators.required,this.noNumbersOrSpecialCharacters]
      ],
      password: [
        '',
        [Validators.required, this.passwordStrengthValidator]
      ],
      confirmPassword: [
        '',
        [Validators.required]
      ]
    }, {
      validators: this.matchPasswords
    });

  }



  matchPasswords(formGroup: FormGroup) {

    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;

    return password === confirmPassword ? null : { passwordsDoNotMatch: true };
  }

  passwordStrengthValidator(control: any) {
    const password = control.value;
    const strongRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\W]).{8,}$/;
    return strongRegex.test(password) ? null : { weakPassword: true };
  }

  noNumbersOrSpecialCharacters(control : any) {
    const regex = /^[a-zA-Z\s]*$/;
    if (!regex.test(control.value)) {
      return { containsNumbersOrSpecialCharacters: true };
    }
    return null;
  }
  onSubmit() {
    if (this.registrationForm.valid) {
      let user: Registration = {
        email: this.registrationForm.value.email,
        firstName: this.registrationForm.value.firstName,
        lastName: this.registrationForm.value.lastName,
        Password: this.registrationForm.value.password,
      }
      this.auth.register(user).subscribe({
        complete: () => {
          console.log(user.email);
          this.router.navigate(["/otp"], { queryParams: { email: user.email } })
        },
        error: err => {
          console.error(err);
          this.toastr.error(err.error,err.status);
        }

      })

    } else {
      console.error('Form is not valid');
    }
  }
}
