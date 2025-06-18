import { Component, ElementRef, Input, ViewChild, ViewContainerRef } from '@angular/core';
import { TestResult } from '../../interfaces/results/TestResult';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { DatePipe } from '@angular/common';
import { ModalService } from '../../../core/services/Modal.service';
import { ModalComponent } from '../modal/modal.component';
import { TestResultDetailsComponent } from '../test-result-details/test-result-details.component';

@Component({
  selector: 'app-test-result-card',
  standalone: true,
  imports: [PrimaryButtonComponent, DatePipe],
  templateUrl: './test-result-card.component.html',
  styleUrl: './test-result-card.component.scss'
})
export class TestResultCardComponent {
  @Input() result?: TestResult;

  @ViewChild('profileTabs', {static: true, read: ElementRef}) modalView!: ElementRef;
  

  constructor(
    private modalService: ModalService,
    private viewContainerRef: ViewContainerRef
  ) {
    
  }

  handleOpenDetails() {
    this.modalService.openModa(this.viewContainerRef, 
      TestResultDetailsComponent, {
        sessionId: this.result?.Id
      });
  }
}
