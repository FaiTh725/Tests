import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  hubUrl = "wss://localhost:5502/notification/hub";
  private hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl, {
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();
  }

  public get HubConnection() {
    return this.hubConnection;
  }

  async connection(): Promise<void> {
    try {
      this.hubConnection.start();
      console.log("connected to hub");
    }
    catch (error) {
      console.log(error);
    }
  }
}
