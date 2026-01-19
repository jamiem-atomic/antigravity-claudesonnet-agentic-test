import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ListingListComponent } from './features/listings/listing-list/listing-list.component';
import { ListingDetailComponent } from './features/listings/listing-detail/listing-detail.component';
import { ListingFormComponent } from './features/listings/listing-form/listing-form.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { MessageInboxComponent } from './features/messages/message-inbox/message-inbox.component';
import { MessageThreadComponent } from './features/messages/message-thread/message-thread.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { AboutComponent } from './features/static/about/about.component';
import { PrivacyComponent } from './features/static/privacy/privacy.component';
import { HelpComponent } from './features/static/help/help.component';
import { NotFoundComponent } from './shared/not-found/not-found.component';
import { authGuard } from './core/auth.guard';
import { adminGuard } from './core/admin.guard';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'auth/login', component: LoginComponent },
    { path: 'auth/register', component: RegisterComponent },
    { path: 'listings', component: ListingListComponent },
    { path: 'listings/:id', component: ListingDetailComponent },
    { path: 'sell', component: ListingFormComponent, canActivate: [authGuard] },
    { path: 'listings/:id/edit', component: ListingFormComponent, canActivate: [authGuard] },
    { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
    { path: 'messages', component: MessageInboxComponent, canActivate: [authGuard] },
    { path: 'messages/:id', component: MessageThreadComponent, canActivate: [authGuard] },
    { path: 'admin', component: AdminDashboardComponent, canActivate: [adminGuard] },
    { path: 'about', component: AboutComponent },
    { path: 'privacy', component: PrivacyComponent },
    { path: 'help', component: HelpComponent },
    { path: '404', component: NotFoundComponent },
    { path: '**', redirectTo: '404' }
];
