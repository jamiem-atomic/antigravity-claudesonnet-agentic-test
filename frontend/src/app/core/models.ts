export interface User {
    id: number;
    email: string;
    displayName: string;
    phone?: string;
    location?: string;
    isAdmin: boolean;
    isSuspended: boolean;
    createdAt: string;
}

export interface AuthResponse {
    token: string;
    user: User;
}

export interface Listing {
    id: number;
    title: string;
    description: string;
    price: number;
    make: string;
    model: string;
    year: number;
    mileage: number;
    fuelType: string;
    transmission: string;
    bodyType: string;
    condition: string;
    location: string;
    photos: string[];
    status: string;
    sellerId: number;
    sellerName: string;
    createdAt: string;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface MessageThread {
    id: number;
    listingId: number;
    listingTitle: string;
    buyerId: number;
    buyerName: string;
    sellerId: number;
    sellerName: string;
    createdAt: string;
    updatedAt: string;
    lastMessagePreview?: string;
}

export interface Message {
    id: number;
    threadId: number;
    senderId: number;
    senderName: string;
    body: string;
    sentAt: string;
}

export interface CreateThreadRequest {
    listingId: number;
    initialMessage: string;
}

export interface SendMessageRequest {
    body: string;
}
