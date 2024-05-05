import { Component, Input } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../../services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reset-form',
  standalone: true,
  imports: [ReactiveFormsModule,FormsModule],
  templateUrl: './reset-form.component.html',
  styleUrl: './reset-form.component.css'
})
export class ResetFormComponent {
  @Input({required:true}) email!:string;
  @Input() redirectTo!:string;
  resetForm!: FormGroup;
  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private auth: AuthenticationService,
    private router: Router
  ){
    this.resetForm = this.fb.group({
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

  onPasswordSubmit(){
    if (this.resetForm.valid) {

      this.auth.resetPassword(this.email,this.resetForm.value.password).subscribe({
        complete:()=>{
          this.toastr.success("password changed successfully")
          if(this.redirectTo)
            this.router.navigate([this.redirectTo]);
        },
        error:(err)=>this.toastr.error(err.error,err.status)
      });

    } else {
      this.toastr.error("cridentials are not valid")
    }
  }
}
