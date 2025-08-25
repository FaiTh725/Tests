import { Component, EventEmitter, Input, Output, output, SimpleChanges } from '@angular/core';
import { ClearInputComponent } from "../clear-input/clear-input.component";

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [ClearInputComponent],
  template: `
    <div class="search-main">
      <div class="search-selected-item"
        (click)="openSearch()">
        <p>{{selectedValue || 'Select item'}}</p>
      </div>
      @if (isOpenSearch) {
        <div class="search-search">
          <div class="search-search__input-wrapper">
            <app-clear-input [inputConfiguration]="{
              Value: searchString,
              IsReadonly: false,
              InputName: 'searchString',
              PlaceHolder: 'Search',
              ErrorMessage: ''}"
              (onChange)="executeSearch($event)"
            ></app-clear-input>
          </div>
          <div class="search-search__result">
            @for (item of filteredItems; track $index) {
              <p class="search-search__result-item"
                (click)="handleSelect($index)">
                {{item[displayProperty]}}
              </p>
            }
          </div>
        </div>
      }
    </div>
  `,
  styleUrl: './search.component.scss'
})
export class SearchComponent {
  timeoutToCallSearch:any;
  isOpenSearch = false;
  searchString = "";
  filteredItems: any[] = [];


  @Input() items: any[] = [];
  @Input() displayProperty: string = "";
  @Input() returnProperty: any = null;

  @Input() selectedValue:string | null = null;

  @Output() search = new EventEmitter<string>();
  @Output() select = new EventEmitter<string>();

  ngOnChanges(changes: SimpleChanges) {
    if(changes['items'] && 
      changes['items'].currentValue != this.items) {
      this.items = changes['items'].currentValue;
    }
  }

  openSearch() {
    this.isOpenSearch = true;
  }

  executeSearch(value: string) {
    this.searchString = value;

    if (this.timeoutToCallSearch) {
      clearTimeout(this.timeoutToCallSearch);
    }

    this.timeoutToCallSearch = setTimeout(() => {

      this.filteredItems = this.items
        .filter(item => item[this.displayProperty]
          .toLowerCase()
          .startsWith(value.trim().toLowerCase()));

      this.search.emit(value);
    }, 200);

    if(this.searchString === "") {
      this.search.emit(value);
    }
  }

  handleSelect(indexSelected: number) {
    this.selectedValue = this.filteredItems[indexSelected][this.displayProperty];
    
    this.isOpenSearch = false;
    this.searchString = "";

    this.select.emit(this.filteredItems[indexSelected][this.returnProperty]);
    this.items = [];
  }
}
