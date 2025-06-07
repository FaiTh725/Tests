import { Component, Input } from '@angular/core';
import { TestInfo } from '../../interfaces/tests/TestInfo';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-test-card',
  standalone: true,
  imports: [DatePipe, CommonModule],
  templateUrl: './test-card.component.html',
  styleUrl: './test-card.component.scss'
})
export class TestCardComponent {
  @Input() test?: TestInfo;

  mouseIsEnter: boolean = false
}
