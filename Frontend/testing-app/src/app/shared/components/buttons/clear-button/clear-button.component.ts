import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-clear-button',
  standalone: true,
  imports: [],
  template: `
    <div class="clear-btn-main">
      <button class="clear-btn-btn"
        (click)="handleClick.emit($event)">
        <ng-content></ng-content>
      </button>
    </div>
  `,
  styleUrl: './clear-button.component.scss'
})
export class ClearButtonComponent {
  @Output() handleClick = new EventEmitter<MouseEvent>();
}
