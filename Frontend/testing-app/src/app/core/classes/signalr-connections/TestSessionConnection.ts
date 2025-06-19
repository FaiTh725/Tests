import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { IHubConnection } from "../../../shared/interfaces/utils/IHubConnection";

export class TestSessionConnection implements IHubConnection {
  private connectionUrl = "wss://localhost:5502/testing/hub";
  private hubConnection!: HubConnection;

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
      console.error("connection to the hub with error");
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
