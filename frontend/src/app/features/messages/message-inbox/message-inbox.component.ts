import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MessageService } from '../../../core/message.service';
import { MessageThread } from '../../../core/models';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-message-inbox',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './message-inbox.component.html',
  styleUrls: ['./message-inbox.component.css']
})
export class MessageInboxComponent implements OnInit {
  threads: MessageThread[] = [];
  isLoading = true;
  currentUserId: number | null = null;

  constructor(
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.currentUserId = user?.id || null;
    });
  }

  ngOnInit(): void {
    this.loadThreads();
  }

  loadThreads(): void {
    this.isLoading = true;
    this.messageService.getThreads().subscribe({
      next: (threads) => {
        this.threads = threads;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  getOtherPartyName(thread: MessageThread): string {
    return thread.buyerId === this.currentUserId ? thread.sellerName : thread.buyerName;
  }
}
