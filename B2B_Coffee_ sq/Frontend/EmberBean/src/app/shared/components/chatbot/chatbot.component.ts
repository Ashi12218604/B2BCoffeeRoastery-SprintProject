import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatbotService } from '../../../core/services/chatbot.service';

interface Message {
  text: string;
  isUser: boolean;
  time: Date;
}

@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chatbot.component.html',
  styleUrl: './chatbot.component.scss'
})
export class ChatbotComponent {
  isOpen = false;
  userInput = '';
  messages: Message[] = [
    { text: 'Hello! I am your Ember & Bean assistant. How can I help you today?', isUser: false, time: new Date() }
  ];
  isLoading = false;

  constructor(private chatbotService: ChatbotService) {}

  toggleChat(): void {
    this.isOpen = !this.isOpen;
  }

  sendMessage(): void {
    if (!this.userInput.trim() || this.isLoading) return;

    const userMsg = this.userInput;
    this.messages.push({ text: userMsg, isUser: true, time: new Date() });
    this.userInput = '';
    this.isLoading = true;

    this.chatbotService.askChatbot(userMsg).subscribe({
      next: (res) => {
        this.messages.push({ text: res.response, isUser: false, time: new Date() });
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Chatbot error:', err);
        this.messages.push({ text: 'Sorry, I encountered an error. Please try again later.', isUser: false, time: new Date() });
        this.isLoading = false;
      }
    });
  }
}
