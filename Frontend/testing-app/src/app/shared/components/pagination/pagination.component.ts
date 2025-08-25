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
  @Output() paginationChanged = new EventEmitter<Pagination>();
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

  handleMoveToPage(page:number) {
    this.paginationConf!.Page = page;

    this.paginationChanged.emit(this.paginationConf);
  }

  handleNext() {
    if(this.maxPage == this.paginationConf?.Page) {
      return;
    }

    this.paginationConf!.Page = this.paginationConf!.Page + 1;
    
    this.paginationChanged.emit(this.paginationConf);
  }
  
  handlePrev() {
    if(this.paginationConf?.Page === 1) {
      return;
    }

    this.paginationConf!.Page = this.paginationConf!.Page - 1;

    this.paginationChanged.emit(this.paginationConf);
  }
}
