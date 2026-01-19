# Vehicle Marketplace Website — Agentic Development Benchmark Specification (v1.0)

## 1. Purpose

Build a **fully functional vehicle marketplace website** to benchmark agentic development solutions.

The system must include:
- **Angular** front end (SPA)
- **C# ASP.NET Core** backend (REST API)
- **SQL database** (SQLite acceptable)
- **Complete in-repo documentation**
- **No dead links** anywhere in the UI

This specification is intentionally comprehensive but bounded, allowing objective comparison between different AI agents on correctness, completeness, and engineering quality.

---

## 2. Target Users & Roles

### Roles

1. **Guest (Anonymous)**
   - Browse vehicle listings
   - View listing details
   - Search, filter, and sort listings
   - View public seller profiles

2. **Registered User**
   - All guest capabilities
   - Save listings as favourites
   - Message sellers internally
   - Manage their own profile

3. **Seller (Registered User)**
   - Create, edit, publish, and unpublish their own listings
   - Manage enquiries/messages

4. **Admin**
   - Moderate and manage listings
   - Manage users
   - View reports
   - View platform metrics

> A single user account can act as both buyer and seller. “Seller” is a capability, not a separate account type.

---

## 3. Navigation (No Dead Links Rule)

The UI **must include a top navigation bar** with the following links.  
**Every link must route to a real, functional page.**

- Home
- Browse Listings
- Sell a Vehicle
- Favourites (auth required)
- Messages (auth required)
- Admin (admin only)
- Help / Docs
- About
- Privacy

---

## 4. Functional Requirements

## 4.1 Authentication & Accounts

- Email + password authentication
- Register / login / logout
- Password minimum length: 8 characters
- User profile page:
  - Display name
  - Email (read-only)
  - Phone (optional)
  - Location (optional)
- Admin can suspend or unsuspend users

### Rules
- Suspended users cannot:
  - Create or modify listings
  - Send messages

### Acceptance Criteria
- Users can register and log in
- Suspended users are blocked from restricted actions

---

## 4.2 Vehicle Listings

### Listing Fields (Minimum)

- Id
- Title (required)
- Description (required)
- Price (required, > 0)
- Make (required)
- Model (required)
- Year (required, 1950 → current year + 1)
- Mileage (required, ≥ 0)
- FuelType (Petrol, Diesel, Hybrid, Electric, Other)
- Transmission (Manual, Automatic, Other)
- BodyType (Hatchback, Sedan, SUV, Van, Truck, Coupe, Convertible, Other)
- Condition (New, Used, ForParts)
- Location (required, string)
- Photos (1–8 images)
- Status:
  - Draft
  - PendingApproval
  - Published
  - Rejected
  - Unpublished
  - Removed
- SellerId
- CreatedAt
- UpdatedAt
- RejectionReason (nullable)
- RemovalReason (nullable)

### Listing Lifecycle

1. Seller creates Draft
2. Seller submits → PendingApproval
3. Admin approves → Published
4. Admin rejects → Rejected (reason required)
5. Seller can unpublish → Unpublished
6. Admin can remove → Removed (reason required)

### Acceptance Criteria
- Drafts visible only to seller and admin
- Published listings visible to all
- Rejection/removal reasons visible to seller
- Removed listings hidden from non-admin users

---

## 4.3 Browse, Search, Filter, Sort

### Browse Page
- Paginated list of published listings
- Listing card shows:
  - Main photo
  - Title
  - Price
  - Year
  - Make & model
  - Mileage
  - Location

### Filters
- Free text search (title, make, model)
- Make
- Model
- Price min/max
- Year min/max
- Mileage max
- Fuel type
- Transmission
- Body type
- Location (contains)

### Sorting
- Price ascending / descending
- Year descending
- Newest first

### Acceptance Criteria
- Filters combine using AND logic
- Pagination preserves filters and sort
- Search is case-insensitive

---

## 4.4 Listing Details Page

- Full listing information
- Photo gallery
- Seller public profile snippet:
  - Display name
  - Member since date
- Actions:
  - Favourite / Unfavourite (auth required)
  - Message seller (auth required)
  - Report listing (auth required)

### Acceptance Criteria
- Favouriting persists and appears on Favourites page
- Messaging opens or creates a thread scoped to the listing

---

## 4.5 Favourites

- View list of favourited listings
- Remove listings from favourites
- If a listing becomes unpublished or removed, indicate clearly

---

## 4.6 Messaging (Internal)

### Message Threads
- Scoped to a listing
- Participants: buyer + seller (+ admin read access)

### Message Fields
- SenderId
- Body (required, max 2000 chars)
- SentAt

### UI
- Inbox (list of threads)
- Thread view with history
- Message compose box

### Rules
- Only participants can view/send
- Suspended users cannot send messages

---

## 4.7 Reports

- Logged-in users can report listings
- Report fields:
  - ListingId
  - Reason: Scam / Offensive / Misleading / Other
  - Optional comment
  - CreatedAt
- Admin can view and act on reports

---

## 4.8 Admin Dashboard

### Admin Pages
- Listings moderation queue (PendingApproval)
- Listings management (search all listings)
- User management (suspend/unsuspend)
- Reports overview
- Metrics dashboard:
  - Listings by status
  - Total users
  - Total messages
  - Total reports

### Acceptance Criteria
- Admin routes protected
- All admin actions require confirmation and record reasons

---

## 5. Non-Functional Requirements

### 5.1 Tech Constraints
- Frontend: Angular (v16+)
- Backend: ASP.NET Core (v8+)
- Database: SQLite acceptable
- Runs locally on macOS, Windows, Linux

### 5.2 Quality
- Consistent formatting
- Meaningful error handling
- No stack traces exposed in UI
- Client-side routing supports refresh

### 5.3 Security
- Password hashing via standard libraries
- Auth via JWT or cookies
- Ownership checks for seller actions

---

## 6. Seed Data Requirements

On first run, the system must auto-create and seed:

- 1 admin user (credentials documented)
- 3 regular users
- ≥25 listings across various makes/models
- ≥5 pending approval listings
- ≥2 rejected listings with reasons
- ≥3 message threads with messages
- ≥3 reports

### Acceptance Criteria
- Fresh clone + run produces a usable demo without manual DB steps

---

## 7. API Requirements

Swagger/OpenAPI **must be enabled and accurate**.

### Authentication
- POST /auth/register
- POST /auth/login
- POST /auth/logout (if cookie-based)
- GET /me

### Listings
- GET /listings
- GET /listings/{id}
- POST /listings
- PUT /listings/{id}
- POST /listings/{id}/submit
- POST /listings/{id}/unpublish

### Admin Listings
- POST /admin/listings/{id}/approve
- POST /admin/listings/{id}/reject
- POST /admin/listings/{id}/remove
- GET /admin/listings

### Favourites
- GET /favourites
- POST /favourites/{listingId}
- DELETE /favourites/{listingId}

### Messaging
- GET /threads
- GET /threads/{id}
- POST /threads
- POST /threads/{id}/messages

### Reports
- POST /reports
- GET /admin/reports

### Users (Admin)
- GET /admin/users
- POST /admin/users/{id}/suspend
- POST /admin/users/{id}/unsuspend

---

## 8. Frontend Pages (Must Exist)

- Home
- Browse Listings
- Listing Details
- Login
- Register
- Profile
- Sell a Vehicle (Create/Edit)
- My Listings
- Favourites
- Messages (Inbox + Thread)
- Admin:
  - Moderation
  - Listings
  - Users
  - Reports
  - Metrics
- Help / Docs
- About
- Privacy
- 404 Not Found

---

## 9. Documentation Requirements

All docs must exist **in-repo** and be linked from the Help / Docs page.

### Required Files
- README.md
- ARCHITECTURE.md
- API.md
- TESTING.md
- CONTRIBUTING.md

### Acceptance Criteria
- All links from the Help / Docs page work
- Docs accurately reflect the implementation

---

## 10. Testing Requirements

### Backend
- Unit tests for listing filtering logic
- Authorization tests for seller ownership
- Lifecycle transition tests

### Frontend
- Filter/search component test
- Route guard test for admin pages

### Acceptance Criteria
- `dotnet test` passes
- `ng test` passes

---

## 11. Running the Project

### Local Run (Mandatory)
- Backend: `dotnet run`
- Frontend: `ng serve`

### Optional
- Docker Compose for full stack

### Acceptance Criteria
- Fresh clone + README steps result in a running system

---

## 12. Evaluation Rubric

1. Correctness – 40%
2. Documentation – 20%
3. Engineering Quality – 20%
4. Reproducibility – 10%
5. Testing – 10%

---

## 13. Stretch Goals (Optional)

- Image thumbnails
- Full-text search
- Admin audit log
- Email notifications (mocked)
- CI pipeline

---

## 14. Required Deliverables

- Angular frontend with all pages functional
- ASP.NET Core API with Swagger
- SQLite persistence
- Seed data
- In-repo documentation
- Tests
- **No dead links**

---