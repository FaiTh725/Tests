import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FeedbackInfo } from '../../interfaces/feedbacks/FeedbackInfo';
import { CommonModule, DatePipe } from '@angular/common';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { AuthService } from '../../../core/services/Auth.service';

@Component({
  selector: 'app-feedback-card',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent, CommonModule],
  templateUrl: './feedback-card.component.html',
  styleUrl: './feedback-card.component.scss'
})
export class FeedbackCardComponent {
  @Input() feedback?: FeedbackInfo;

  @Output() delete = new EventEmitter<number>();

  constructor(
    protected authService:AuthService
  ) {
    
  }

  createArray(countElements: number) {
    return new Array(countElements);
  }

  handleDeleteFeedback() {
    this.delete.emit(this.feedback?.Id);
  }

  changeImageHost(imageUrl: string) {
    const correctHostUrl = imageUrl.replace("azurite_storage", "localhost");
    return `url(${correctHostUrl})`;
  }
}
