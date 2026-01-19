import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../../core/api.service';
import { Listing, PagedResult } from '../../../core/models';
import { FormsModule } from '@angular/forms';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-listing-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './listing-list.component.html',
  styleUrls: ['./listing-list.component.css']
})
export class ListingListComponent implements OnInit {
  listings: Listing[] = [];
  totalCount = 0;
  isLoading = false;

  // Filters
  filter = {
    search: '',
    make: '',
    model: '',
    priceMin: null,
    priceMax: null,
    sortBy: 'newest'
  };

  constructor(
    private api: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.filter = { ...this.filter, ...params };
      this.loadListings();
    });
  }

  loadListings() {
    this.isLoading = true;
    let params = new HttpParams();

    Object.keys(this.filter).forEach(key => {
      const value = (this.filter as any)[key];
      if (value) params = params.append(key, value);
    });

    this.api.get<PagedResult<Listing>>('/listings', params).subscribe({
      next: (result) => {
        this.listings = result.items;
        this.totalCount = result.totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  applyFilters() {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.filter,
      queryParamsHandling: 'merge'
    });
  }

  clearFilters() {
    this.filter = {
      search: '',
      make: '',
      model: '',
      priceMin: null,
      priceMax: null,
      sortBy: 'newest'
    };
    this.applyFilters();
  }
}
