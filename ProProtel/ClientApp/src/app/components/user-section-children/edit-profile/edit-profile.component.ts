import { Component, OnInit } from '@angular/core';
import { User } from '../../../models/user';
import { ProfileService } from '../../../services/profile.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AsyncPipe, CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.css'
})
export class EditProfileComponent implements OnInit {
  user!: User;
  profileForm!: FormGroup;

  constructor(
    private profile: ProfileService,
    private fb: FormBuilder,
    private toastr: ToastrService
  ) {
    this.profileForm = this.fb.group({
      firstName: ["", Validators.required],
      lastName: [""],
      postalCode: [""],
      state: [""],
      city: [""]
    });
  }

  ngOnInit(): void {
    if (this.profile) {
      this.profile.get().subscribe({
        next: (res: User) => {
          this.user = res;
          this.initializeForm();
        }
      });
    }
  }

  initializeForm(): void {
    this.profileForm = this.fb.group({
      firstName: [this.user.firstName, Validators.required],
      lastName: [this.user.lastName],
      postalCode: [this.user.postalCode],
      state: [this.user.state],
      city: [this.user.city]
    });
  }

  onSubmit(): void {
    if (this.profileForm.valid) {
      const updatedUserData : User = this.profileForm.value;
      console.log(updatedUserData);
      
      this.profile.update(updatedUserData).subscribe({
        complete: () => {
          this.toastr.success('Profile updated successfully');
        },
        error: (err: any) => {
          this.toastr.error(err.error,err.status);
        }
      });
    } else {
      this.toastr.error('Invalid form data');
    }
  }

}
