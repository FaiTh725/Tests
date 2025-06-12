import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TestInfo } from '../../interfaces/tests/TestInfo';
import { TestCardComponent } from "../test-card/test-card.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tests-layout',
  standalone: true,
  imports: [TestCardComponent, CommonModule],
  templateUrl: './tests-layout.component.html',
  styleUrl: './tests-layout.component.scss'
})
export class TestsLayoutComponent {
  @Input() tests: TestInfo[] = []

  @Output() clickOnTest = new EventEmitter<number>();

  calculatePadding(index: number) {
    if(Math.trunc(index / 5 ) % 2 === 0) {
      return String(index%5 * 80 + 80) + "px";
    }
    else {
      return String(80 * 5 - index%5 * 80) + "px";
    }  
  }

  handleClickOnTest(testId: number) {
    this.clickOnTest.emit(testId);
  }
}
