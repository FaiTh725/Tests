import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-test-ratings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './test-ratings.component.html',
  styleUrl: './test-ratings.component.scss'
})
export class TestRatingsComponent {
  @Input() rating?: Rating; 
  @Input() maxCount?: number;
  @Input() isSelected: boolean = false;

  @Output() handleClick = new EventEmitter<number>();

  fullProgressWidth = 100;

  getProgressWidth() {
    if(this.rating && this.maxCount) {
      return this.rating.Count / this.maxCount * this.fullProgressWidth;
    }

    return 0;
  }
};

export interface Rating {
  Rating: number,
  Count: number
}
