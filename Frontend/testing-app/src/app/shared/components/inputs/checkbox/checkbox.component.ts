import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Checkbox } from '../../../interfaces/inputs/checkbox';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkbox',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="checkbox_main"
      (click)="handleClick()"
      [style.--border-color]="checkboxConfiguration.BorderColor"
      [style.--border-active-color]="checkboxConfiguration.BorderActiveColor"
      [style.--bg-active-color]="checkboxConfiguration.BackgroundActiveColor"
      [style.--toggle-active-color]="checkboxConfiguration.ToggleActiveColor"
      [ngClass]="{'checkbox_active': checkboxConfiguration.IsChecked}" >
      <div class="checkbox_togle"
        [style.background-color]="(checkboxConfiguration.IsChecked) ? 
          checkboxConfiguration.ToggleActiveColor:
          checkboxConfiguration.BorderColor">

      </div>
    </div>
  `,
  styleUrl: './checkbox.component.scss'
})
export class CheckboxComponent {
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


