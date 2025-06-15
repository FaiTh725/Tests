import { Component} from '@angular/core';
import { SignalRService } from '../../../core/services/SignalRService.service';
import { NotificationInfo } from '../../interfaces/notifications/Notification';
import { NotificationCardComponent } from "../notification-card/notification-card.component";
import { ClearButtonComponent } from "../buttons/clear-button/clear-button.component";
import { CommonModule } from '@angular/common';
import { HttpService } from '../../../core/services/Http.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [NotificationCardComponent, ClearButtonComponent, CommonModule],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss'
})
export class NotificationsComponent {
  notifications: NotificationInfo[] = [];

  notificationIsOpen = false;

  constructor(
    private signlaRService: SignalRService,
    private httpService: HttpService) {
    
  }
  
  ngOnInit() {
    this.signlaRService.connection();

    this.signlaRService.HubConnection
    .on("SendListNotifications", (data: any) => {
      this.notifications = data.map((notification: any) => ({
        Id: notification.id,
        UserEmail: notification.consumerEmail,
        Title: notification.title,
        Message: notification.message,
        SendTime: notification.sendTime,
        IsRead: notification.isRead
      }));
    });
    
    this.signlaRService.HubConnection
    .on("Send", (data: any) => {
      console.log("get data from signalr");
      
      this.notifications.unshift({
        Id: data.id,
        UserEmail: data.consumerEmail,
        Title: data.title,
        Message: data.message,
        SendTime: data.sendTime,
        IsRead: data.isRead
      });
    });
  }

  handleReadAll() {
    const requestUrl = "notification/Notification/ReadAllNotifications"
    this.httpService.patchRequest(requestUrl)
    .subscribe({
      error: _ => {
        console.error("Unknown error");
      },
      complete: () => {
        this.notifications = [];
      }
    });
  }

  handleReadNotification(notificationId: number) {
    const requestUrl = "notification/Notification/ReadNotification";
    this.httpService.patchRequest(requestUrl, {
      notificationId: notificationId
    }).subscribe({
      error: _ => {
        console.error("unknown error");
      },
      complete: () => {
        this.notifications = this.notifications
          .filter(notification => notification.Id != notificationId);
      }
    });
  }
}
