import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProfileService } from './profile.service';
import { WindowRefService } from './window-ref.service';
import { environment } from '../../environments/environment';
declare var Razorpay: any;

@Injectable({
  providedIn: 'root'
})
export class RazorpayService {
  private headers = new HttpHeaders({ 'Content-Type': 'application/json' });

  constructor(private http: HttpClient, private profile: ProfileService, private winRef: WindowRefService) { }

  private async getOrderId(UserId: string, PlanId: number) {
    const response = await this.http.get<any>(`${environment.API_BASE_URL}/Worker/Payment/Init?UserId=${UserId}&PlanId=${PlanId}`, { headers: this.headers }).toPromise();
    return response.orderId;
  }

  private async getPlan(PlanId: number) {
    const response = await this.http.get<any>(`${environment.API_BASE_URL}/Worker/Payment/Plan?PlanId=${PlanId}`, { headers: this.headers }).toPromise();
    return response.plan;
  }

  async pay(
    PlanId: number,
    success: (Subscription: { paymentId: string, userId: string, planId: number }) => void,
    failed: () => void,
    prefill?: {
      name?: string; email?: string; phone?: string | null

    }) {
    let plan: any;
    let orderId: string;

    try {
      plan = await this.getPlan(PlanId);
      orderId = await this.getOrderId(this.profile.getUserIdFromToken()!, PlanId);
    } catch (error) {
      console.error('Error fetching plan or order ID:', error);
      failed();
      return;
    }

    const razorpayOptions = {
      currency: 'INR',
      name: 'ProPortel',
      description: 'Subscription Plan',
      amount: plan.price,
      key: environment.RazorpaySettings.key_id,
      image: 'https://i.imgur.com/FApqk3D.jpeg',
      order_id: orderId,
      prefill,
      theme: {
        color: '#6466e3'
      },
      modal: {
        escape: false,
        ondismiss: () => {
          console.log('dismissed');
          failed(); // Consider handling dismissal as failed payment
        }
      },
      handler: (response: any) => {
        if (response.razorpay_payment_id) {
          success({paymentId:response.razorpay_payment_id,userId: this.profile.getUserIdFromToken()!,planId: PlanId});
        } else {
          failed();
        }
      }
    };

    const rzp = new this.winRef.nativeWindow.Razorpay(razorpayOptions);
    rzp.open();
  }
}
