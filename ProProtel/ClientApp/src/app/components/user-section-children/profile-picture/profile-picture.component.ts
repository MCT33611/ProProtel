import { Component, inject } from '@angular/core';
import { ProfileService } from '../../../services/profile.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../../models/user';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-picture',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-picture.component.html',
  styleUrl: './profile-picture.component.css'
})
export class ProfilePictureComponent {
  profile = inject(ProfileService);
  toastr = inject(ToastrService)
  user!: User;
  constructor() {
    this.profile.get().subscribe({
      next: (res: User) => {
        this.user = res
      }
    })

  }
  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.profile.uploadProfilePicture( file).subscribe(
        () => {
          this.toastr.success("picture updated");
          this.profile.get().subscribe({
            next: (res: User) => {
              this.user = res
            }
          });
        },
        (err) => {
          this.toastr.error(err.error,err.status)
        }
      );
    }
  }
}
