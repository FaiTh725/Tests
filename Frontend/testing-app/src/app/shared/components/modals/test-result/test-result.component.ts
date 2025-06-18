import { Component, Input } from '@angular/core';
import { HttpService } from '../../../../core/services/Http.service';
import { TestResult } from '../../../interfaces/results/TestResult';
import { ClearButtonComponent } from "../../buttons/clear-button/clear-button.component";
import { ModalService } from '../../../../core/services/Modal.service';

@Component({
  selector: 'app-test-result',
  standalone: true,
  imports: [ClearButtonComponent],
  templateUrl: './test-result.component.html',
  styleUrl: './test-result.component.scss'
})
export class TestResultComponent {
  sessionResult?: TestResult;
  
  @Input() sessionId?: number;

  constructor(
    private httpService: HttpService,
    private modalService: ModalService
  ) {
    
  }

  ngOnInit() {
    const requestUrl = `testing/TestSession/GetSessionResult?sessionId=${this.sessionId}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.sessionResult = {
          Id: data.id,
          TestId: data.testId,
          TestName: data.testName,
          ProfileId: data.profileId,
          StartTime: data.startTime,
          EndTime: data.endTime,
          Percent: data.percent
        }
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }

  handleCloseModal() {
    this.modalService.handleClose();
  }
}
