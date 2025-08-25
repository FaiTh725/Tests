import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NotificationInfo } from '../../interfaces/notifications/Notification';
import { DatePipe } from '@angular/common';
import { ClearButtonComponent } from "../buttons/clear-button/clear-button.component";

@Component({
  selector: 'app-notification-card',
  standalone: true,
  imports: [DatePipe, ClearButtonComponent],
  templateUrl: './notification-card.component.html',
  styleUrl: './notification-card.component.scss'
})
export class NotificationCardComponent {
  @Input() notification?: NotificationInfo;

  @Output() read = new EventEmitter<number>();

  
}
