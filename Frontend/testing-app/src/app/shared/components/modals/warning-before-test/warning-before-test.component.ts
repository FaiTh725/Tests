import { Component } from '@angular/core';
import { PrimaryButtonComponent } from "../../buttons/primary-button/primary-button.component";
import { ModalService } from '../../../../core/services/Modal.service';

@Component({
  selector: 'app-warning-before-test',
  standalone: true,
  imports: [PrimaryButtonComponent],
  templateUrl: './warning-before-test.component.html',
  styleUrl: './warning-before-test.component.scss'
})
export class WarningBeforeTestComponent {
  constructor(
    private modalService: ModalService
  ) {
    
  }

  handleConfirm() {
    this.modalService.handleConfirm();
  }

  handleCancel() {
    this.modalService.handleClose();
  }
}
