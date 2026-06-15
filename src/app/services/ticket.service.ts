import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = '/api/tickets';

  constructor(private http: HttpClient) {}

  createTicket(ticketData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, ticketData);
  }

  getUserTickets(userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/${userId}`);
  }

  getTicketDetails(ticketId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${ticketId}`);
  }

  updateTicketStatus(ticketId: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${ticketId}/status`, { status });
  }
}
