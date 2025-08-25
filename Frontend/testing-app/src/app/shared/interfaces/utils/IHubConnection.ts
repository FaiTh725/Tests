import { HubConnection } from "@microsoft/signalr";

export interface IHubConnection {
  connect(): Promise<void>;
  disconnect(): Promise<void>;

  get connection(): HubConnection;
}
