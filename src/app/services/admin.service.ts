import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Campaign {
  id: number;
  name: string;
  supplierId: number;
  startDate: string;
  endDate: string;
  status: string;
}

export interface Supplier {
  id: number;
  name: string;
  website: string;
  contactInfo: string;
  requiresTrackingCode: boolean;
}

export interface Order {
  id: number;
  userId: number;
  campaignId: number;
  totalAmount: number;
  status: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = '/api/admin';

  constructor(private http: HttpClient) {}

  // Dashboard
  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard/stats`);
  }

  // Campaigns
  getCampaigns(): Observable<Campaign[]> {
    return this.http.get<Campaign[]>(`${this.apiUrl}/campaigns`);
  }

  createCampaign(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/campaigns`, data);
  }

  updateCampaign(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/campaigns/${id}`, data);
  }

  deleteCampaign(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/campaigns/${id}`);
  }

  getCampaignReport(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/campaigns/${id}/report`);
  }

  // Suppliers
  getSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.apiUrl}/suppliers`);
  }

  createSupplier(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/suppliers`, data);
  }

  updateSupplier(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/suppliers/${id}`, data);
  }

  // Orders
  getOrders(status?: string): Observable<Order[]> {
    const url = status 
      ? `${this.apiUrl}/orders?status=${status}`
      : `${this.apiUrl}/orders`;
    return this.http.get<Order[]>(url);
  }

  getOrderDetail(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/orders/${id}`);
  }

  updateOrderStatus(id: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/orders/${id}/status`, { status });
  }

  editInvoice(id: number, changes: any, reason: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/orders/${id}/invoice`, { changes, reason });
  }

  confirmPayment(id: number, trackingCode: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders/${id}/confirm-payment`, { trackingCode });
  }

  // Products & Import
  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/products`);
  }

  importExcel(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.apiUrl}/products/import-excel`, formData);
  }

  scrapeProducts(supplierId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/products/scrape/${supplierId}`, {});
  }
}
