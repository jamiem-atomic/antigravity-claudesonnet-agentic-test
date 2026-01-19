import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { Listing, User, PagedResult } from './models';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  constructor(private api: ApiService) { }

  getPendingListings(): Observable<Listing[]> {
    return this.api.get<Listing[]>('/admin/listings/pending');
  }

  approveListing(id: number): Observable<void> {
    return this.api.post<void>(`/admin/listings/${id}/approve`, {});
  }

  rejectListing(id: number, reason: string): Observable<void> {
    return this.api.post<void>(`/admin/listings/${id}/reject`, { reason });
  }

  getUsers(search?: string, page: number = 1): Observable<PagedResult<User>> {
    let params = new HttpParams().set('page', page.toString());
    if (search) params = params.set('search', search);
    return this.api.get<PagedResult<User>>('/admin/users', params);
  }

  suspendUser(id: number): Observable<void> {
    return this.api.post<void>(`/admin/users/${id}/suspend`, {});
  }

  activateUser(id: number): Observable<void> {
    return this.api.post<void>(`/admin/users/${id}/activate`, {});
  }
}
