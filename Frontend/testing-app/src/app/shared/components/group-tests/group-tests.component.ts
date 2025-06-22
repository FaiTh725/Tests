import { Component, Input } from '@angular/core';
import { Pagination } from '../../interfaces/utils/Pagination';
import { TestInfo } from '../../interfaces/tests/TestInfo';
import { ClearButtonComponent } from "../buttons/clear-button/clear-button.component";
import { AvailableTestCardComponent } from "../available-test-card/available-test-card.component";
import { HttpService } from '../../../core/services/Http.service';

@Component({
  selector: 'app-group-tests',
  standalone: true,
  imports: [ClearButtonComponent, AvailableTestCardComponent],
  templateUrl: './group-tests.component.html',
  styleUrl: './group-tests.component.scss'
})
export class GroupTestsComponent {
  @Input() groupId?: number;

  groupTests: TestInfo[] = [];
  sectionIsOpen = false;

  paginationGroupTests: Pagination = {
    Page: 1,
    PageSize: 5,
    MaxSize: 0
  }

  constructor(
    private httpService: HttpService
  ) {
    
  }

  handleLoadModeTests() {
    if((this.paginationGroupTests.Page + 1) * this.paginationGroupTests.PageSize  <= 
      this.paginationGroupTests.MaxSize) {
      this.paginationGroupTests.Page ++;
      this.executeGetGroupTests();
    }
  }

  executeGetGroupTests() {
    const requestUrl = `testing/Group/GetGroupPrivateTests?` + 
    `GroupId=${this.groupId}` + 
    `&Page=${this.paginationGroupTests.Page}` + 
    `&PageSize=${this.paginationGroupTests.PageSize}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.groupTests = [... this.groupTests, ... data.data.map((test: any) => ({
          Id: test.id,
          Name: test.name,
          Description: test.description,
          CreatedTime: new Date(test.createdTime),
          IsPublic: test.isPublic,
          TestType: test.testType == "Timed" ? 0 : 1,
          DurationInMinutes: test.durationInMinutes, 
          Owner: {
            Email: test.owner.email,
            Name: test.owner.name,
            Id: test.owner.id
          }
        }))];

        this.paginationGroupTests.MaxSize = data.maxSize;
        this.paginationGroupTests.Page = data.page;
        this.paginationGroupTests.PageSize = data.pageSize;
      },
      error: _ => {
        console.log("unknown error");
      }
    });
  };

  handleOpen() {
    this.executeGetGroupTests();
    
    this.sectionIsOpen = true;
  }
}
