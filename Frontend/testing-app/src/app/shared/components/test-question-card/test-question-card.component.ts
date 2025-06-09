import { Component, EventEmitter, Input, Output} from '@angular/core';
import { QuestionInfo } from '../../interfaces/questions/QuestionInfo';
import { QuestionTypePipe } from "../../pipes/QuestionTypePipe.pipe";
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { CheckboxComponent } from "../inputs/checkbox/checkbox.component";
import { SquareCheckboxComponent } from "../inputs/square-checkbox/square-checkbox.component";

@Component({
  selector: 'app-test-question-card',
  standalone: true,
  imports: [QuestionTypePipe, PrimaryButtonComponent, SquareCheckboxComponent],
  templateUrl: './test-question-card.component.html',
  styleUrl: './test-question-card.component.scss'
})
export class TestQuestionCardComponent {
  @Input() question?: QuestionInfo;

  @Output() delete = new EventEmitter<number>();

  handleDelete() {
    this.delete.emit(this.question?.Id);
  }
}
