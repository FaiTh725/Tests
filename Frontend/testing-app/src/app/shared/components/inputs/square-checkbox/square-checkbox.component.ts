import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Checkbox } from '../../../interfaces/inputs/checkbox';
import { CommonModule } from '@angular/common';
import { isReactive } from '@angular/core/primitives/signals';

@Component({
  selector: 'app-square-checkbox',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="square-checkbox_main"
      (click)="handleClick()"
      [style.--border-color]="checkboxConfiguration.BorderColor"
      [style.--border-active-color]="checkboxConfiguration.BorderActiveColor"
      [style.--bg-active-color]="checkboxConfiguration.BackgroundActiveColor"
      [style.--toggle-active-color]="checkboxConfiguration.ToggleActiveColor"
      [ngClass]="{'square-checkbox_active': checkboxConfiguration.IsChecked}" >
      <div class="square-checkbox_content">
        @if (checkboxConfiguration.IsChecked) {
          <svg xmlns="http://www.w3.org/2000/svg" height="40px" viewBox="0 -960 960 960" width="40px"><path d="M379.33-244 154-469.33 201.67-517l177.66 177.67 378.34-378.34L805.33-670l-426 426Z"/></svg>
        }  
      </div>
    </div>
  `,
  styleUrl: './square-checkbox.component.scss'
})
export class SquareCheckboxComponent {
  @Input() checkboxConfiguration: Checkbox = {
      IsChecked: false,
      BorderColor: 'var(--shadow-color)',
      BorderActiveColor: 'var(--light-blue-color)',
      BackgroundActiveColor: 'var(--light-blue-color)',
      ToggleActiveColor: 'var(--primary-write-color)',
      IsReadonly: false
    }
  @Output() check = new EventEmitter<MouseEvent>();

  handleClick() {
    if(this.checkboxConfiguration.IsReadonly) {
      return;
    }

    this.checkboxConfiguration.IsChecked = !this.checkboxConfiguration.IsChecked;
    this.check.emit();
  }
}
