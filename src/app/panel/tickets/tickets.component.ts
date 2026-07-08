import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TicketService } from '../../services/ticket.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';

interface Ticket {
  id: number;
  subject: string;
  category: string;
  status: string;
  createdAt: string;
  messages?: any[];
}

@Component({
  selector: 'app-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, InputTextareaModule, DropdownModule, DialogModule, TagModule],
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

  constructor(
    private ticketService: TicketService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.ticketService.getUserTickets().subscribe({
      next: (data: any) => {
        this.tickets = data || [];
      },
      error: (err: any) => console.error('Error loading tickets:', err)
    });
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
    
    this.ticketService.createTicket(this.newTicket).subscribe({
      next: () => {
        alert('تیکت شما با موفقیت ثبت شد.');
        this.showDialog = false;
        this.loadTickets();
      },
      error: (err: any) => {
        console.error('Error creating ticket:', err);
        alert('خطا در ثبت تیکت.');
      }
    });
  }

  viewTicket(ticket: Ticket): void {
    this.router.navigate(['/panel/tickets', ticket.id]);
  }

  getCategoryFa(cat: string): string {
    const map: { [key: string]: string } = {
      'Technical': 'پشتیبانی فنی',
      'Orders': 'سفارش‌ها',
      'Payment': 'پرداخت',
      'Other': 'سایر'
    };
    return map[cat] || cat;
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('fa-IR');
  }
}
