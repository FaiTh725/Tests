import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReplyInfo } from '../../interfaces/replies/ReplyInfo';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../../core/services/Auth.service';
import { ClearButtonComponent } from "../buttons/clear-button/clear-button.component";

@Component({
  selector: 'app-reply-card',
  standalone: true,
  imports: [DatePipe, ClearButtonComponent],
  templateUrl: './reply-card.component.html',
  styleUrl: './reply-card.component.scss'
})
export class ReplyCardComponent {
  @Input() reply?: ReplyInfo;
  
  @Output() delete = new EventEmitter<number>();

  constructor(
    protected authService: AuthService
  ) {
    
  }

  handleDelete() {
    this.delete.emit(this.reply?.Id);
  }
}
