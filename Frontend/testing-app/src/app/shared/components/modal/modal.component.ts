import { Component, EventEmitter, Output, ViewChild, ViewContainerRef } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [],
  template: `
    <div class="modal-main">
      <div class="modal-content">
        <ng-template #container></ng-template>
      </div>
    </div>
  `,
  styleUrl: './modal.component.scss'
})
export class ModalComponent {
  @Output() close = new EventEmitter();
  @Output() confirm = new EventEmitter();

  @ViewChild('container', {read: ViewContainerRef, static: true}) viewContainerRef!: ViewContainerRef;

  onClose() {
    this.close.emit();
  }

  onConfirm() {
    this.confirm.emit();
  }
}
