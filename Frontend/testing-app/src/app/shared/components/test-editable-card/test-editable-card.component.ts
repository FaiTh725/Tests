import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TestInfo } from '../../interfaces/tests/TestInfo';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-test-editable-card',
  standalone: true,
  imports: [DatePipe, PrimaryButtonComponent],
  templateUrl: './test-editable-card.component.html',
  styleUrl: './test-editable-card.component.scss'
})
export class TestEditableCardComponent {
  @Input() test?: TestInfo;

  @Output() delete = new EventEmitter<number>();

  handleDelete() {
    this.delete.emit(this.test?.Id);
  }
}
