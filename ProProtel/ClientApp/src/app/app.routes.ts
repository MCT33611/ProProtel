import { Routes } from '@angular/router';
import { HomeComponent } from './pages/user-section/home/home.component';
import { authCustomerGuard } from './services/guards/auth-customer.guard';
import { priventAuthReturnGuard } from './services/guards/privent-auth-return.guard';
import { ProfileComponent } from './pages/user-section/profile/profile.component';
import { SecurityComponent } from './components/user-section-children/security/security.component';
import { EditProfileComponent } from './components/user-section-children/edit-profile/edit-profile.component';
import { PlanListComponent } from './components/user-section-children/plan-list/plan-list.component';

export const routes: Routes = [
    {
        path:"",component:HomeComponent,title:"proportel",canActivate:[authCustomerGuard]
    },
    {
        path:"register",
        loadComponent:()=>import ("./pages/authentication/register/register.component").then((M)=>M.RegisterComponent),
        title:"proportel registration",
        canActivate:[priventAuthReturnGuard]

    },
    {
        path:"otp",
        loadComponent:()=>import ("./pages/authentication/otp/otp.component").then((M)=>M.OtpComponent),
        title:"otp verification"
    },
    {
        path:"login",
        loadComponent:()=>import ("./pages/authentication/login/login.component").then((M)=>M.LoginComponent),
        title:"proportel login",
        canActivate:[priventAuthReturnGuard]
    },
    {
        path:"forgot-password",
        loadComponent:()=>import ("./pages/authentication/forgot-password/forgot-password.component").then((M)=>M.ForgotPasswordComponent),
        title:"forgot-password",

    },
    {
        path:"profile",
        component:ProfileComponent,
        title:"profile",
        canActivate:[authCustomerGuard],
        children: [
            { path: '', redirectTo: 'editprofile', pathMatch: 'full' }, 
            { path: 'security', component: SecurityComponent },
            { path: 'editprofile', component: EditProfileComponent },
            { path: 'plan-list', component: PlanListComponent },
        ]
    },
    {
        path:"**",
        redirectTo:""
    },
];
