# API Documentation

The VehicleMarket API is a RESTful service available at `http://localhost:5217/api`.

## Endpoints

### Authentication
- `POST /api/auth/register`: Register a new user.
- `POST /api/auth/login`: Login and receive a JWT.
- `GET /api/auth/me`: Get current authenticated user details.

### Listings
- `GET /api/listings`: Browse listings (supports filtering, sorting, pagination).
- `GET /api/listings/{id}`: Get listing details.
- `POST /api/listings`: Create a new listing (Auth required).
- `PUT /api/listings/{id}`: Edit a listing (Owner only).
- `DELETE /api/listings/{id}`: Delete a listing (Owner only).

### Favourites
- `GET /api/favourites`: Get user's favourite listings (Auth required).
- `POST /api/favourites/{listingId}`: Add to favourites (Auth required).
- `DELETE /api/favourites/{listingId}`: Remove from favourites (Auth required).

### Messaging
- `GET /api/threads`: List all message threads for the user (Auth required).
- `GET /api/threads/{id}`: Get thread details.
- `POST /api/threads`: Create a thread with an initial message.
- `GET /api/threads/{id}/messages`: Get messages in a thread.
- `POST /api/threads/{id}/messages`: Send a message.

### Admin
- `GET /api/admin/listings/pending`: Get all listings awaiting approval.
- `POST /api/admin/listings/{id}/approve`: Approve a listing.
- `POST /api/admin/listings/{id}/reject`: Reject a listing.
- `GET /api/admin/users`: List all users (supports search).
- `POST /api/admin/users/{id}/suspend`: Suspend a user.
- `POST /api/admin/users/{id}/activate`: Reactivate a user.

## Data Structures

### Listing DTO
```json
{
  "id": 1,
  "title": "2020 Porsche 911",
  "price": 120000,
  "status": "Published",
  "sellerName": "John Doe",
  ...
}
```

## Error Handling
The API returns standard HTTP status codes (200, 201, 400, 401, 403, 404). Validation errors include a descriptive message.
