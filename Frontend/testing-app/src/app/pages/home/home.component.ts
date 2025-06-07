import { Component } from '@angular/core';
import { Pagination } from '../../shared/interfaces/utils/Pagination';
import { HttpService } from '../../core/services/Http.service';
import { TestInfo } from '../../shared/interfaces/tests/TestInfo';
import { TestsLayoutComponent } from "../../shared/components/tests-layout/tests-layout.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [TestsLayoutComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  pagination: Pagination = {
    Page: 1,
    PageSize: 20,
    MaxSize: 0
  };
  tests: TestInfo[] = [];

  constructor(
    private httpService: HttpService
  ) {
    
  }

  ngOnInit() {
    const requestUrl = `testing/Test/GetTestsPagination?Page=${this.pagination.Page}&PageSize=${this.pagination.PageSize}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.tests = data.data.map((test: any) => ({
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
        }));
        this.tests.push(...this.tests);
        this.tests.push(...this.tests);

        this.pagination.MaxSize = data.maxSize;
      },
      error: error => {
        console.error("Unknow error - " + error);
      }
    });
  }
}
