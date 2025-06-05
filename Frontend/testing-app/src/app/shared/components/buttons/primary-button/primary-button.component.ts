import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Btn } from '../../../interfaces/inputs/btn';

@Component({
  selector: 'app-primary-button',
  standalone: true,
  imports: [],
  template: `
    <div class="primary-btn-main">
      <button class="primary-btn"
        (click)="handleClick.emit($event)"
        [style.--bg-color]="btnConfiguration.BackgroundColor"
        [style.--border-color]="btnConfiguration.BorderColor"
        [style.--txt-color]="btnConfiguration.TextColor"
        [style.--fs]="btnConfiguration.FontSize + 'px'"
        [style.--border-width]="btnConfiguration.BorderWith + 'px'"
        [style.--hover-bg-color]="btnConfiguration.HoverBackgroundColor"
        [style.--hover-txt-color]="btnConfiguration.HoverColor">
        {{ btnConfiguration.Text }}
      </button>
    </div>
  `,
  styleUrl: './primary-button.component.scss'
})
export class PrimaryButtonComponent {
  @Input() btnConfiguration: Btn = {
    Text: "",
    BackgroundColor: "#49c0f8",
    BorderColor: "#19a0e0",
    BorderWith: 2,
    FontSize: 14,
    HoverBackgroundColor: "#61c9fa",
    HoverColor: "#173145",
    TextColor: "#173145"
  };
  @Output() handleClick = new EventEmitter<MouseEvent>();
}
