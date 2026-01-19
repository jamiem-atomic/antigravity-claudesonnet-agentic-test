import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MessageService } from '../../../core/message.service';
import { Message, MessageThread } from '../../../core/models';
import { AuthService } from '../../../core/auth.service';
import { Subscription, timer, switchMap } from 'rxjs';

@Component({
  selector: 'app-message-thread',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './message-thread.component.html',
  styleUrls: ['./message-thread.component.css']
})
export class MessageThreadComponent implements OnInit, OnDestroy {
  thread: MessageThread | null = null;
  messages: Message[] = [];
  newMessage = '';
  isLoading = true;
  currentUserId: number | null = null;
  private pollingSub?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.currentUserId = user?.id || null;
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadThread(+id);
      this.startPolling(+id);
    }
  }

  ngOnDestroy(): void {
    this.pollingSub?.unsubscribe();
  }

  loadThread(id: number): void {
    this.messageService.getThread(id).subscribe({
      next: (thread) => {
        this.thread = thread;
      },
      error: () => {
        // Handle error
      }
    });
  }

  startPolling(id: number): void {
    // Simple polling for new messages every 5 seconds
    this.pollingSub = timer(0, 5000).pipe(
      switchMap(() => this.messageService.getMessages(id))
    ).subscribe({
      next: (messages) => {
        this.messages = messages;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  sendMessage(): void {
    if (!this.newMessage.trim() || !this.thread) return;

    this.messageService.sendMessage(this.thread.id, this.newMessage).subscribe({
      next: (message) => {
        this.messages.push(message);
        this.newMessage = '';
      },
      error: () => {
        // Handle error
      }
    });
  }

  getOtherPartyName(): string {
    if (!this.thread) return '';
    return this.thread.buyerId === this.currentUserId ? this.thread.sellerName : this.thread.buyerName;
  }
}
