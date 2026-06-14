import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CampaignService {
  private apiUrl = '/api/campaigns';

  constructor(private http: HttpClient) {}

  getActiveCampaign(): Observable<any> {
    return this.http.get(`${this.apiUrl}/active`);
  }

  getCampaignBySlug(slug: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/${slug}`);
  }

  getCampaignProducts(campaignId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${campaignId}/products`);
  }

  validateCampaignAccess(campaignId: number, userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${campaignId}/validate-access?userId=${userId}`);
  }
}
