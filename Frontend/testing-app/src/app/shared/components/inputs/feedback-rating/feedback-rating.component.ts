import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, SimpleChange, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-feedback-rating',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="feedback-rating-main">
      @for (item of ratings; track $index) {
        <div class="feedback-rating-scale"
          (mouseenter)="hoverRating=$index + 1"
          (mouseleave)="handleMouseLeave($index)"
          [ngClass]="{'feedback-rating-scale__active' : $index < hoverRating}"
          (click)="handleClickOnStar($index)">
          <svg xmlns="http://www.w3.org/2000/svg" height="30px" viewBox="0 -960 960 960" width="30px" fill="#fdf7e6"><path d="M333.33-259 480-347l146.67 89-39-166.67 129-112-170-15L480-709l-66.67 156.33-170 15 129 112.34-39 166.33ZM233-120l65-281L80-590l288-25 112-265 112 265 288 25-218 189 65 281-247-149-247 149Zm247-353.33Z"/></svg>
        </div>
      }
    </div>
  `,
  styleUrl: './feedback-rating.component.scss'
})
export class FeedbackRatingComponent {
  @Input() rating:number = 0;
  @Output() change = new EventEmitter<number>();

  ratings: number[] = new Array(10);
  hoverRating:number = 0;

  handleClickOnStar(index: number) {
    this.rating = this.rating == index + 1 ? 0 : index + 1;
    this.change.emit(this.rating);
  }

  handleMouseLeave(index: number) {
    this.hoverRating = this.hoverRating == 0 ? 
      this.hoverRating = 0 : 
      this.hoverRating = index + 1;

    this.hoverRating = this.rating;
  }
}
