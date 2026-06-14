import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
// Placeholder service - will be implemented later
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { DialogModule } from 'primeng/dialog';

interface Ticket {
  id: number;
  subject: string;
  category: string;
  status: string;
  date: string;
  messages?: any[];
}

@Component({
  selector: 'app-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, InputTextareaModule, DropdownModule, DialogModule],
  templateUrl: './tickets.component.html',
  styleUrls: ['./tickets.component.css']
})
export class TicketsComponent implements OnInit {
  tickets: Ticket[] = [];
  showDialog: boolean = false;
  newTicket: { subject: string; category: string; message: string } = { subject: '', category: '', message: '' };
  
  categories: any[] = [
    { label: 'پشتیبانی فنی', value: 'Technical' },
    { label: 'سفارش‌ها', value: 'Orders' },
    { label: 'پرداخت', value: 'Payment' },
    { label: 'سایر', value: 'Other' }
  ];

  constructor() {}

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    // TODO: Call ticket service to load tickets
    // Placeholder data
    this.tickets = [
      { id: 1, subject: 'مشکل در پرداخت', category: 'Payment', status: 'Open', date: '1403/01/15' },
      { id: 2, subject: 'تاخیر در ارسال', category: 'Orders', status: 'Closed', date: '1403/01/10' }
    ];
  }

  openNewTicketDialog(): void {
    this.newTicket = { subject: '', category: '', message: '' };
    this.showDialog = true;
  }

  submitTicket(): void {
    if (!this.newTicket.subject || !this.newTicket.category || !this.newTicket.message) {
      alert('لطفاً تمام فیلدها را پر کنید.');
      return;
    }
    
    // TODO: Call ticket service to create ticket
    alert('تیکت شما با موفقیت ثبت شد.');
    this.showDialog = false;
    this.loadTickets();
  }

  viewTicket(ticket: Ticket): void {
    // TODO: Implement ticket detail view
    alert('نمایش جزئیات تیکت: ' + ticket.subject);
  }
}
