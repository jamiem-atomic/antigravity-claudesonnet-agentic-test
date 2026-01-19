import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AuthService } from '../../core/auth.service';
import { Listing, PagedResult } from '../../core/models';
import { Observable, map, switchMap } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  activeTab: 'listings' | 'favourites' | 'profile' = 'listings';
  myListings: Listing[] = [];
  favourites: Listing[] = [];
  isLoading = false;
  currentUser$ = this.authService.currentUser$;

  constructor(
    private api: ApiService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loadMyListings();
    this.loadFavourites();
  }

  setActiveTab(tab: 'listings' | 'favourites' | 'profile') {
    this.activeTab = tab;
  }

  loadMyListings() {
    this.isLoading = true;
    this.currentUser$.pipe(
      switchMap(user => {
        if (!user) return [];
        let params = new HttpParams().set('sellerId', user.id);
        return this.api.get<PagedResult<Listing>>('/listings', params).pipe(map(res => res.items));
      })
    ).subscribe({
      next: (listings) => {
        this.myListings = listings;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  loadFavourites() {
    this.api.get<Listing[]>('/favourites').subscribe({
      next: (favs) => {
        this.favourites = favs;
      }
    });
  }

  deleteListing(id: number) {
    if (confirm('Are you sure you want to remove this listing?')) {
      this.api.delete(`/listings/${id}`).subscribe(() => {
        this.loadMyListings();
      });
    }
  }

  removeFavourite(id: number) {
    this.api.delete(`/favourites/${id}`).subscribe(() => {
      this.loadFavourites();
    });
  }
}
