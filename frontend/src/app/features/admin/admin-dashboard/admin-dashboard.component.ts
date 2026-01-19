import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/admin.service';
import { Listing, User } from '../../../core/models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  activeTab: 'moderation' | 'users' = 'moderation';
  pendingListings: Listing[] = [];
  users: User[] = [];
  isLoading = false;
  userSearch = '';
  rejectReason = '';
  selectedListingId: number | null = null;

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.loadPendingListings();
    this.loadUsers();
  }

  setActiveTab(tab: 'moderation' | 'users'): void {
    this.activeTab = tab;
  }

  loadPendingListings(): void {
    this.isLoading = true;
    this.adminService.getPendingListings().subscribe({
      next: (listings) => {
        this.pendingListings = listings;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  loadUsers(): void {
    this.adminService.getUsers(this.userSearch).subscribe({
      next: (result) => {
        this.users = result.items;
      }
    });
  }

  approveListing(id: number): void {
    this.adminService.approveListing(id).subscribe(() => {
      this.loadPendingListings();
    });
  }

  openRejectModal(id: number): void {
    this.selectedListingId = id;
    this.rejectReason = '';
  }

  rejectListing(): void {
    if (this.selectedListingId && this.rejectReason.trim()) {
      this.adminService.rejectListing(this.selectedListingId, this.rejectReason).subscribe(() => {
        this.selectedListingId = null;
        this.loadPendingListings();
      });
    }
  }

  toggleUserSuspension(user: User): void {
    if (user.isSuspended) {
      this.adminService.activateUser(user.id).subscribe(() => this.loadUsers());
    } else {
      if (confirm(`Are you sure you want to suspend ${user.displayName}?`)) {
        this.adminService.suspendUser(user.id).subscribe(() => this.loadUsers());
      }
    }
  }
}
