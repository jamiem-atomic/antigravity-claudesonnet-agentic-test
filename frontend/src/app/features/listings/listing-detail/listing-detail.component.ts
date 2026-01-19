import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api.service';
import { Listing } from '../../../core/models';
import { AuthService } from '../../../core/auth.service';
import { MessageService } from '../../../core/message.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-listing-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './listing-detail.component.html',
  styleUrls: ['./listing-detail.component.css']
})
export class ListingDetailComponent implements OnInit {
  listing: Listing | null = null;
  isLoading = true;
  activePhotoIndex = 0;
  isAuthenticated = false;
  isMessaging = false;
  initialMessage = "Is this still available?";

  constructor(
    private route: ActivatedRoute,
    private api: ApiService,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router
  ) {
    this.authService.isAuthenticated$.subscribe(val => this.isAuthenticated = val);
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadListing(id);
    }
  }

  loadListing(id: string) {
    this.isLoading = true;
    this.api.get<Listing>(`/listings/${id}`).subscribe({
      next: (data) => {
        this.listing = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  setActivePhoto(index: number) {
    this.activePhotoIndex = index;
  }

  contactSeller() {
    if (!this.listing) return;

    this.messageService.createThread({
      listingId: this.listing.id,
      initialMessage: this.initialMessage
    }).subscribe({
      next: (thread) => {
        this.router.navigate(['/messages', thread.id]);
      },
      error: () => {
        // Handle error
      }
    });
  }
}

