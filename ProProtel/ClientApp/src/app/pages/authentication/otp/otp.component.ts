import { Component, ElementRef, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup, Validators,ReactiveFormsModule } from '@angular/forms';
import { AuthenticationService } from '../../../services/authentication.service';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-otp',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './otp.component.html',
  styleUrl: './otp.component.css'
})
export class OtpComponent {
  countdown: number = 60; 
  isCountdownActive: boolean = false;
  canResendOTP: boolean = false;
  isInValid : boolean = false;
  otpForm: FormGroup;
  email!:string;
  @ViewChildren('otpInput') otpInputs !: QueryList<ElementRef>;

  constructor(
    private fb: FormBuilder,
    private auth:AuthenticationService,
    private router:Router,
    private route: ActivatedRoute,
    private toastr:ToastrService
  ) {
    this.otpForm = this.fb.group({
      otp1: ['', [Validators.required, Validators.maxLength(1)]],
      otp2: ['', [Validators.required, Validators.maxLength(1)]],
      otp3: ['', [Validators.required, Validators.maxLength(1)]],
      otp4: ['', [Validators.required, Validators.maxLength(1)]],
    });
  }

  ngOnInit(): void {
    this.startCountdown();
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.auth.sentOtp(params['email']).subscribe({
        complete:()=>this.toastr.success("please check you mail"),
        error:(err)=>{this.toastr.error(err.error,err.status);this.router.navigate(['/register'])}
      })
    });
    ;

  }

  onInputChange(event: KeyboardEvent, index: number) {
    const target = event.target as HTMLInputElement;
    const value = target.value;

    if (value && index < this.otpInputs.length - 1) {
      this.otpInputs.toArray()[index + 1].nativeElement.focus();
    }

    if (!value && index > 0 && event.key === 'Backspace') {
      this.otpInputs.toArray()[index - 1].nativeElement.focus();
    }
    this.isInValid  = false;
  }

  onSubmit() {
    if (this.otpForm.valid) {
      const otp = `${this.otpForm.value.otp1}${this.otpForm.value.otp2}${this.otpForm.value.otp3}${this.otpForm.value.otp4}`;
      this.auth.confirmEmail(otp,this.email).subscribe({
        complete:()=>{
          this.router.navigate(['/login'])
        },
        error:(err)=>this.toastr.error(err.error,err.status)
      })
      

    }
  }
  startCountdown() {
    this.countdown = 60; 
    this.isCountdownActive = true;
    this.canResendOTP = false;

    const countdownInterval = setInterval(() => {
      if (this.countdown > 0) {
        this.countdown--;
      } else {
        clearInterval(countdownInterval);
        this.isCountdownActive = false;
        this.canResendOTP = true;
      }
    }, 1000);
  }

  resendOTP() {
    if (this.canResendOTP) {
      this.startCountdown();
      this.auth.sentOtp(this.email);
    }
  }
  containsCharacters(otpValue: string): boolean {
    const characterRegex = /[a-zA-Z]/; 
    return characterRegex.test(otpValue);
  }
}
