import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appQuestionTypePipe',
  standalone: true,
})
export class QuestionTypePipe implements PipeTransform {

  transform(value: number) {
    return value == 0 ? "One answer" : "Many answers";
  }

}
