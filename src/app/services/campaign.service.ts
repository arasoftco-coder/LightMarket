import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CampaignService {
  constructor(private http: HttpClient) {}
  
  getActiveCampaign(): Observable<any> { 
    return this.http.get<any>('/api/campaigns/active').pipe(
      map(res => res.data)
    ); 
  }
  
  getCampaignProducts(id: number): Observable<any> { 
    return this.http.get<any>(`/api/campaigns/${id}/products`).pipe(
      map(res => res.data)
    ); 
  }
}