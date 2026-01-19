import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { AuthResponse, User } from './models';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly TOKEN_KEY = 'auth_token';
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();
    public isAuthenticated$ = new BehaviorSubject<boolean>(false);

    constructor(private api: ApiService) {
        this.loadUserFromStorage();
    }

    private loadUserFromStorage() {
        const token = localStorage.getItem(this.TOKEN_KEY);
        if (token) {
            // Create a dummy user from storage if we had it, or just validate token
            // Ideally we would verify token with backend 'me' endpoint
            this.isAuthenticated$.next(true);
            this.getCurrentUser().subscribe();
        }
    }

    register(data: any): Observable<AuthResponse> {
        return this.api.post<AuthResponse>('/auth/register', data).pipe(
            tap(response => this.handleAuthResponse(response))
        );
    }

    login(data: any): Observable<AuthResponse> {
        return this.api.post<AuthResponse>('/auth/login', data).pipe(
            tap(response => this.handleAuthResponse(response))
        );
    }

    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
        this.currentUserSubject.next(null);
        this.isAuthenticated$.next(false);
    }

    getCurrentUser(): Observable<User> {
        return this.api.get<User>('/auth/me').pipe(
            tap(user => {
                this.currentUserSubject.next(user);
                this.isAuthenticated$.next(true);
            },
                error => {
                    this.logout();
                })
        );
    }

    getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    private handleAuthResponse(response: AuthResponse) {
        localStorage.setItem(this.TOKEN_KEY, response.token);
        this.currentUserSubject.next(response.user);
        this.isAuthenticated$.next(true);
    }

    get currentUserValue(): User | null {
        return this.currentUserSubject.value;
    }
}
