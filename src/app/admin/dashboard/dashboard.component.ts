import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  reportData: any = null;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.adminService.getReportsDashboard().subscribe({
      next: (data) => {
        this.reportData = data;
      },
      error: (err) => console.error('Error loading stats', err)
    });
  }

  downloadReport(type: string): void {
    this.adminService.exportReport(type).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${type}_report_${new Date().toISOString().slice(0, 10)}.csv`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Error exporting report', err)
    });
  }
}
