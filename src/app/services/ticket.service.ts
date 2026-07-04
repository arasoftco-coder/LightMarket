import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class TicketService {
  constructor(private http: HttpClient) {}

  createTicket(data: any): Observable<any> { 
    return this.http.post<any>('/api/tickets', data).pipe(
      map(res => res.data)
    ); 
  }

  getUserTickets(): Observable<any> { 
    return this.http.get<any>('/api/tickets').pipe(
      map(res => res.data)
    ); 
  }

  getTicketById(id: number): Observable<any> { 
    return this.http.get<any>(`/api/tickets/${id}`).pipe(
      map(res => res.data)
    ); 
  }

  replyToTicket(id: number, message: string): Observable<any> { 
    return this.http.post<any>(`/api/tickets/${id}/reply`, { message }).pipe(
      map(res => res.data)
    ); 
  }
}