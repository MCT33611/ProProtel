import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ResetFormComponent } from '../../authentication/reset-form/reset-form.component';
import { ProfileService } from '../../../services/profile.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../../../services/authentication.service';

@Component({
  selector: 'app-security',
  standalone: true,
  imports: [
    CommonModule,
    ResetFormComponent
  ],
  templateUrl: './security.component.html',
  styleUrl: './security.component.css'
})
export class SecurityComponent implements OnInit{
  @Input({ required: true }) email!: string;
  isEmailValid = false;
  constructor(
    private profile: ProfileService,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
    private auth : AuthenticationService
  ) { }

  delete() {
    if (confirm("do you want to delete this accout?")) {
      this.profile.delete(this.profile.getUserIdFromToken()!).subscribe({
        complete: () => {
          this.toastr.success("User Delete successfully");
          this.profile.logout();
          this.router.navigate(['/login'])
        }
      });
    }

  }
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
    });
    this.auth.forgetPassword(this.email).subscribe({
      complete:()=>{
        this.isEmailValid = true;
      },
      error:(err)=>this.toastr.error(err.error,err.status)
    });
  }
}
