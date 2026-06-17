import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TicketService {
  constructor(private http: HttpClient) {}
  createTicket(data: any): Observable<any> { return this.http.post<any>('/api/tickets', data); }
  getUserTickets(): Observable<any> { return this.http.get<any>('/api/tickets'); }
  getTicketById(id: number): Observable<any> { return this.http.get<any>(`/api/tickets/${id}`); }
  replyToTicket(id: number, message: string): Observable<any> { return this.http.post<any>(`/api/tickets/${id}/reply`, { message }); }
}