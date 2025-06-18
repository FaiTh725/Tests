import { CommonModule } from '@angular/common';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';

@Component({
  selector: 'app-image-slider',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="image-slider-main">
      <div class="image-slider-cur-images" 
        #imageSlider>
        @if(imagesUrl.length !== 0) {
          @for (imageUrl of imagesUrl; track $index) {
            <div class="image-slider-cur-images__image"
              [ngStyle]="{'background-image': imagesUrl}">
            </div>
          }
        }
      </div>
      <div class="image-slider-btn image-slider-btn-left"
        (click)="handleScroll('prev')">
        <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#fdf7e6"><path d="M560-280 360-480l200-200v400Z"/></svg>
      </div>
      <div class="image-slider-btn image-slider-btn-right"
        (click)="handleScroll('next')">
        <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#fdf7e6"><path d="M400-280v-400l200 200-200 200Z"/></svg>
      </div>
    </div>
  `,
  styleUrl: './image-slider.component.scss'
})
export class ImageSliderComponent {
  @Input() imagesUrl: string[] = [];

  @ViewChild('imageSlider', {static: false, read: ElementRef}) imageSlider!: ElementRef;

  handleScroll(direction: string) {
    const scrollUnit = this.imageSlider.nativeElement.scrollWidth / this.imagesUrl.length;

    if(direction === "prev") {
      this.imageSlider.nativeElement.scrollBy({
        top: 0,
        left: -scrollUnit,
        behavior: "smooth",
      });
    }
    else if(direction === "next") {
      this.imageSlider.nativeElement.scrollBy({
        top: 0,
        left: scrollUnit,
        behavior: "smooth",
      });
    }
  }
}
