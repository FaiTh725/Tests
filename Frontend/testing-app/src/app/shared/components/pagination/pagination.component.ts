import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Pagination } from '../../interfaces/utils/Pagination';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent {
  @Output() next = new EventEmitter();
  @Output() prev = new EventEmitter();
  @Input() paginationConf?: Pagination;
  maxPage: number = 0;

  createArray(length: number): number[] {
    return Array.from({ length }, (_, i) => i);
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes['paginationConf'] && 
      this.paginationConf
    ) {
      this.maxPage = Math.ceil(this.paginationConf.MaxSize / this.paginationConf.PageSize);
    }
  }

  handleNext() {
    if(this.maxPage == this.paginationConf?.Page) {
      return;
    }
    
    this.next.emit();
  }
  
  handlePrev() {
    if(this.paginationConf?.Page === 1) {
      return;
    }

    this.prev.emit();
  }
}
