import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { HttpService } from './core/services/Http.service';
import { UserCredentialsValidatorService } from './core/services/UserCredentialsValidator.service';
import { SignalRService } from './core/services/SignalRService.service';
import { ModalService } from './core/services/Modal.service';
import { UrlService as UrlService } from './core/services/UrlService.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule],
  providers: [UrlService, ModalService, HttpService, UserCredentialsValidatorService, SignalRService],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Testing';
}
