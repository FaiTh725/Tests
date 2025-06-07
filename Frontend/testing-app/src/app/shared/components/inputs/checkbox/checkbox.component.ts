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
    BorderColor: "#446373",
    BorderActiveColor: "#49c0f8",
    BackgroundActiveColor: "#49c0f8",
    ToggleActiveColor: "#fdf7e6"
  }
  @Output() check = new EventEmitter<MouseEvent>();

  handleClick() {
    this.checkboxConfiguration.IsChecked = !this.checkboxConfiguration.IsChecked;
    this.check.emit();
  }
}


