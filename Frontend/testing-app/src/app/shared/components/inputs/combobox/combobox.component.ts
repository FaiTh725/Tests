import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-combobox',
  standalone: true,
  imports: [],
  template: `
    <div class="combobox-main">
      @if (!isOpenItems) {
        <div class="combobox__selected-item"
          (click)="handleOpenItems()">
          @if(selectedItemIndex != null) {
            <div>{{items[selectedItemIndex]}}</div>
          }
          <div>
            <svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 -960 960 960" width="24px" fill="#fdf7e6"><path d="M480-344 240-584l56-56 184 184 184-184 56 56-240 240Z"/></svg>
          </div>
        </div>
      }
      @else if(items.length != 0){
        <div class="combobox-items">
          @for (item of items; track $index) {
            <div class="combobox-item"
              (click)="handleChooseItem($index)">
              {{item}}
            </div>
          }
        </div>
      }
      @else {
        <div class="combobox-items">
          There are no items
        </div>
      }
    </div>
  `,
  styleUrl: './combobox.component.scss'
})
export class ComboboxComponent {
  @Input() items: string[] = [];
  @Input() selectedItemIndex: number | null = null;
  
  @Output() chooseItem = new EventEmitter<number>();

  isOpenItems: boolean  = false;

  handleOpenItems() {
    this.isOpenItems = !this.isOpenItems;
  }

  handleChooseItem(index: number) {
    this.selectedItemIndex = index;
    this.isOpenItems = false;
    this.chooseItem.emit(this.selectedItemIndex);
  }
}
