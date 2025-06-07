import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { InputConf } from '../../../interfaces/inputs/input';
import { FormsModule } from '@angular/forms';
import { timer } from 'rxjs';

@Component({
  selector: 'app-primary-input',
  standalone: true,
  template: `
    <div class="primary-input-main">
      <div class="primary-input-wrapper">
        <input class="primary-input" type="text" 
          [placeholder]="inputConfiguration.PlaceHolder"
          [(ngModel)]="inputConfiguration.Value"
          [readonly]="inputConfiguration.IsReadonly"
          [name]="inputConfiguration.InputName"
          (ngModelChange)="change($event)">
        <div class="primary-input__clear"
          (click)="handleClear()">
          <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#fdf7e6"><path d="m336-280 144-144 144 144 56-56-144-144 144-144-56-56-144 144-144-144-56 56 144 144-144 144 56 56ZM480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Zm0-80q134 0 227-93t93-227q0-134-93-227t-227-93q-134 0-227 93t-93 227q0 134 93 227t227 93Zm0-320Z"/></svg>
        </div>
      </div>
      @if (inputConfiguration.ErrorMessage != "") {
        <div class="primary-input-error">
          <p><svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#ee5555"><path d="M480-280q17 0 28.5-11.5T520-320q0-17-11.5-28.5T480-360q-17 0-28.5 11.5T440-320q0 17 11.5 28.5T480-280Zm-40-160h80v-240h-80v240Zm40 360q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Zm0-80q134 0 227-93t93-227q0-134-93-227t-227-93q-134 0-227 93t-93 227q0 134 93 227t227 93Zm0-320Z"/></svg></p>
          <p class="primary-input-error__message">
            {{inputConfiguration.ErrorMessage}}
          </p>
        </div>
      }
    </div>
  `,
  imports: [FormsModule],
  styleUrl: './primary-input.component.scss'
})

export class PrimaryInputComponent {
  @Input() inputConfiguration: InputConf = {
    Value: "",
    IsReadonly: false,
    InputName: "",
    PlaceHolder: "",
    ErrorMessage: ""
  };
  @Output() onChange = new EventEmitter<string>();
  @Output() clearError = new EventEmitter();
  timeToShowError: number = 9000;

  change(value: string) {
    if(this.inputConfiguration.IsReadonly) {
      return;
    }

    this.inputConfiguration.Value = value;
    this.onChange.emit(value);
  }

  handleClear(): void {
    this.inputConfiguration.Value = "";
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes['inputConfiguration'].currentValue &&
      changes['inputConfiguration'].currentValue.ErrorMessage != ""
    ) {
      timer(this.timeToShowError).subscribe(() => 
      { 
        this.clearError.emit();
      });
    }
  }
}
