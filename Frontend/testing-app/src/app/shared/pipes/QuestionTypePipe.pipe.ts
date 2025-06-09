import { Pipe, type PipeTransform } from '@angular/core';
import { QuestionType } from '../interfaces/questions/QuestionType';

@Pipe({
  name: 'appQuestionTypePipe',
  standalone: true,
})
export class QuestionTypePipe implements PipeTransform {

  transform(value: number) {
    return value == 0 ? "One answer" : "Many answers";
  }

}
