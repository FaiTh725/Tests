import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { IHubConnection } from "../../../shared/interfaces/utils/IHubConnection";
import { HttpService } from "../../services/Http.service";
import { inject } from "@angular/core";

export class NotificationConnection implements IHubConnection {
  private connectionUrl = "wss://localhost:5502/notification/hub";
  private hubConnection!: HubConnection;

  private httpService = inject(HttpService);

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.connectionUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();
  }

  async connect(): Promise<void> {
    try {
      await this.hubConnection.start();

      console.log("connect to notification hub");
    }
    catch {
      this.httpService.refreshToken()
      .subscribe({
        complete: async () => {
          await this.hubConnection.start();
          console.log("reconnect to hub");
        },
        error: _ => {
          console.error("token is realy expired");
        }
      });
    }
  }

  async disconnect(): Promise<void> {
    if(this.hubConnection.state == HubConnectionState.Connected) {
      await this.hubConnection.stop() 
    }
    else {
      console.log("Hub has already disconnected");
    }
  }

  get connection(): HubConnection {
    return this.hubConnection;
  }

}
