import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChatRequest {
  message: string;
}

export interface ChatResponse {
  response: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  // Calling the ChatbotService via ApiGateway
  private readonly baseUrl = 'http://localhost:7101/api/chatbot';

  constructor(private http: HttpClient) {}

  askChatbot(message: string): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(this.baseUrl, { message });
  }
}
