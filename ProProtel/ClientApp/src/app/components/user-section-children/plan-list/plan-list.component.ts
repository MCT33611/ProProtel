import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../../services/profile.service';
import { CurrencyPipe } from '@angular/common';
import { RazorpayService } from '../../../services/razorpay.service';
import { User } from '../../../models/user';
import { ToastrService } from 'ngx-toastr';
import { WindowRefService } from '../../../services/window-ref.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-plan-list',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './plan-list.component.html',
  styleUrl: './plan-list.component.css'
})
export class PlanListComponent implements OnInit {
  plans: any[] = [];
  user: User = {}
  constructor(
    private profile: ProfileService,
    private razorpay: RazorpayService,
    private toastr: ToastrService,
    private winRef: WindowRefService
  ) { }

  ngOnInit(): void {
    this.profile.getPlans().subscribe({
      next: (res: any) => {
        this.plans = [...res.plans];
      },
      error: (err) => console.error("Failed to fetch plans details from the API")
    });
    this.profile.get().subscribe({
      next: (res: any) => {

        this.user = { ...res };
      },
      error: (err) => console.error("Failed to fetch plans details from the API")
    });

  }

  buy(planId: number) {
    this.razorpay.pay(
      planId,
      this.onSuccess,
      () => { this.toastr.error("payment failed") },
      {
        email: this.user.email,
        name: this.user.firstName,
        phone: this.user.phoneNumber
      }
    )
  }

  onSuccess = async (subscription: { paymentId: string, userId: string, planId: number }) => {
    console.log("cheked:success");

    this.profile.Subscribe({
      paymentId: subscription.paymentId,
      planId: subscription.planId,
      status: "Success",
      workerId: subscription.userId
    }).subscribe({
      complete: () => {
        console.log("cheked:subscribe:complete");
        this.toastr.success("payment seccessfully completed");
      },
      error: (err) => this.toastr.error(err.error, err.status)
    })

  }


}
