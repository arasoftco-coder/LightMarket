import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TicketService } from '../../../services/ticket.service';
import { ButtonModule } from 'primeng/button';

interface TicketMessage {
  id: number;
  sender: 'user' | 'admin';
  message: string;
  createdAt: string;
}

interface Ticket {
  id: number;
  subject: string;
  category: string;
  status: 'open' | 'closed' | 'pending';
  priority: 'low' | 'medium' | 'high';
  createdAt: string;
  messages: TicketMessage[];
}

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule],
  templateUrl: './ticket-detail.component.html',
  styleUrls: ['./ticket-detail.component.css']
})
export class TicketDetailComponent implements OnInit {
  ticket: Ticket | null = null;
  isLoading: boolean = true;
  replyMessage: string = '';
  isSubmitting: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private ticketService: TicketService
  ) {}

  ngOnInit(): void {
    const ticketId = this.route.snapshot.paramMap.get('id');
    if (ticketId) {
      this.loadTicket(+ticketId);
    }
  }

  loadTicket(id: number): void {
    this.isLoading = true;
    // TODO: Replace with actual API call
    this.ticketService.getTicketById(id).subscribe({
      next: (data: any) => {
        this.ticket = data;
        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
      }
    });
  }

  submitReply(): void {
    if (!this.replyMessage.trim() || !this.ticket) return;

    this.isSubmitting = true;
    // TODO: Replace with actual API call
    this.ticketService.replyToTicket(this.ticket.id, this.replyMessage).subscribe({
      next: (data: any) => {
        this.replyMessage = '';
        this.loadTicket(this.ticket!.id);
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.isSubmitting = false;
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'open': return 'status-open';
      case 'closed': return 'status-closed';
      case 'pending': return 'status-pending';
      default: return '';
    }
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'open': 'باز',
      'closed': 'بسته',
      'pending': 'در انتظار بررسی'
    };
    return labels[status] || status;
  }

  getCategoryLabel(category: string): string {
    const labels: Record<string, string> = {
      'Technical': 'پشتیبانی فنی',
      'Orders': 'سفارش‌ها',
      'Payment': 'پرداخت',
      'Other': 'سایر'
    };
    return labels[category] || category;
  }

  getPriorityClass(priority: string): string {
    switch (priority) {
      case 'high': return 'priority-high';
      case 'medium': return 'priority-medium';
      case 'low': return 'priority-low';
      default: return '';
    }
  }

  getPriorityLabel(priority: string): string {
    const labels: Record<string, string> = {
      'high': 'بالا',
      'medium': 'متوسط',
      'low': 'پایین'
    };
    return labels[priority] || priority;
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('fa-IR');
  }
}
