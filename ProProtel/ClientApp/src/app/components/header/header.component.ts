import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ProfileService } from '../../services/profile.service';
import { User } from '../../models/user';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule,RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  profile = inject(ProfileService)
  router = inject(Router)
  toastr = inject(ToastrService)

  user!:User;
  
  constructor(){
    if(this.profile.isLoggedIn()){
      this.profile.get().subscribe({
        next:(res:User)=>{
          this.user = res;
          this.user.profilePicture = res.profilePicture;
        }
      });

    }
  }

}
