import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../../services/authentication.service';
import { Router } from '@angular/router';
import { LoginModel } from '../../../models/login-model';
import { ResetFormComponent } from '../../../components/authentication/reset-form/reset-form.component';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule,FormsModule,ResetFormComponent],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  isEmailValid = false;
  forgotForm!: FormGroup;
  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private auth: AuthenticationService,
  ) {

    this.forgotForm = this.fb.group({
      email: [
        '',
        [Validators.required, Validators.email]
      ],
    });
  }

  onEmailSubmit() {

    if (this.forgotForm.valid) {

      this.auth.forgetPassword(this.forgotForm.value.email).subscribe({
        complete:()=>{
          this.isEmailValid = true;
        },
        error:(err)=>this.toastr.error(err.error,err.status)
      });

    } else {
      this.toastr.error("cridentials are not valid")
    }
  }


}
