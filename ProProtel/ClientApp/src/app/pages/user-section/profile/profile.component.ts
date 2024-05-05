import { Component, inject } from '@angular/core';
import { ProfileService } from '../../../services/profile.service';
import { User } from '../../../models/user';
import { Router, RouterOutlet } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { ResetFormComponent } from '../../../components/authentication/reset-form/reset-form.component';
import { EditProfileComponent } from '../../../components/user-section-children/edit-profile/edit-profile.component';
import { SecurityComponent } from '../../../components/user-section-children/security/security.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports:[
    CommonModule,
    EditProfileComponent,
    SecurityComponent,
    RouterOutlet
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  profile = inject(ProfileService);
  toastr = inject(ToastrService);
  router = inject(Router);
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
          this.toastr.success("pocture updated");
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

  logout() {
    this.profile.logout();
    this.router.navigate(['/login']);
  }


}
