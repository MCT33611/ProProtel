import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { MaterialModule } from '../../../material/material.module';
import { HeaderComponent } from '../../../components/header/header.component';
import { Router, RouterOutlet } from '@angular/router';
import { ProfileComponent } from '../../user-section/profile/profile.component';
import { ProfilePictureComponent } from '../../../components/user-section-children/profile-picture/profile-picture.component';
import { ProfileService } from '../../../services/profile.service';
@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [
    RouterOutlet,
    MaterialModule,
    ProfileComponent,
    ProfilePictureComponent
  ],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css'
})
export class LandingComponent {
  isSideNavOpen = false;
  router = inject(Router)
  profile = inject(ProfileService)


  
  logout() {
    this.profile.logout();
  }
}
