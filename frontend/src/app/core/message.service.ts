import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Message, MessageThread, CreateThreadRequest, SendMessageRequest } from './models';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private api: ApiService) { }

  getThreads(): Observable<MessageThread[]> {
    return this.api.get<MessageThread[]>('/threads');
  }

  getThread(id: number): Observable<MessageThread> {
    return this.api.get<MessageThread>(`/threads/${id}`);
  }

  getMessages(threadId: number): Observable<Message[]> {
    return this.api.get<Message[]>(`/threads/${threadId}/messages`);
  }

  createThread(request: CreateThreadRequest): Observable<MessageThread> {
    return this.api.post<MessageThread>('/threads', request);
  }

  sendMessage(threadId: number, body: string): Observable<Message> {
    return this.api.post<Message>(`/threads/${threadId}/messages`, { body });
  }
}
