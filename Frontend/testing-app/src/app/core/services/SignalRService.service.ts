import { Injectable } from '@angular/core';
import { IHubConnection } from '../../shared/interfaces/utils/IHubConnection';
import { NotificationConnection } from '../classes/signalr-connections/NotificationConnection';
import { TestSessionConnection } from '../classes/signalr-connections/TestSessionConnection';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private notificationConnection!: IHubConnection;
  private testSessionConnection!: IHubConnection;

  constructor() {
    this.notificationConnection = new NotificationConnection();
    this.testSessionConnection = new TestSessionConnection();
  }

  public get HubNotificationConnection(): IHubConnection {
    return this.notificationConnection;
  };

  public get HubSessionConnection(): IHubConnection {
    return this.testSessionConnection;
  }
}
