import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

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
    return this.http.get<any>(`${this.apiUrl}/dashboard/stats`).pipe(
      map(res => res.data)
    );
  }

  // Campaigns
  getCampaigns(): Observable<Campaign[]> {
    return this.http.get<any>(`${this.apiUrl}/campaigns`).pipe(
      map(res => res.data)
    );
  }

  createCampaign(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/campaigns`, data).pipe(
      map(res => res.data)
    );
  }

  updateCampaign(id: number, data: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/campaigns/${id}`, data).pipe(
      map(res => res.data)
    );
  }

  deleteCampaign(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/campaigns/${id}`);
  }

  getCampaignReport(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/campaigns/${id}/report`).pipe(
      map(res => res.data)
    );
  }

  // Suppliers
  getSuppliers(): Observable<Supplier[]> {
    return this.http.get<any>(`${this.apiUrl}/suppliers`).pipe(
      map(res => res.data)
    );
  }

  createSupplier(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/suppliers`, data).pipe(
      map(res => res.data)
    );
  }

  updateSupplier(id: number, data: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/suppliers/${id}`, data).pipe(
      map(res => res.data)
    );
  }

  // Orders
  getOrders(status?: string): Observable<Order[]> {
    const url = status 
      ? `${this.apiUrl}/orders?status=${status}`
      : `${this.apiUrl}/orders`;
    return this.http.get<any>(url).pipe(
      map(res => res.data)
    );
  }

  getOrderDetail(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/orders/${id}`).pipe(
      map(res => res.data)
    );
  }

  updateOrderStatus(id: number, status: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/orders/${id}/status`, { status }).pipe(
      map(res => res.data)
    );
  }

  editInvoice(id: number, changes: any, reason: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/orders/${id}/invoice`, { ...changes, reason }).pipe(
      map(res => res.data)
    );
  }

  confirmPayment(id: number, trackingCode: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/orders/${id}/confirm-payment`, { trackingCode }).pipe(
      map(res => res.data)
    );
  }

  // Products & Import
  getProducts(): Observable<any[]> {
    return this.http.get<any>(`${this.apiUrl}/products`).pipe(
      map(res => res.data)
    );
  }

  importExcel(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.apiUrl}/products/import-excel`, formData);
  }

  confirmImport(items: any[]): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/products/import-excel/confirm`, items);
  }

  scrapeProducts(supplierId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/products/scrape/${supplierId}`, {});
  }
}
