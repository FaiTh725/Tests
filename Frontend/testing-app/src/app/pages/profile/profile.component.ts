import { Component } from '@angular/core';
import { AuthService } from '../../core/services/Auth.service';
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { Router } from '@angular/router';
import { TestInfo } from '../../shared/interfaces/tests/TestInfo';
import { HttpService } from '../../core/services/Http.service';
import { Pagination } from '../../shared/interfaces/utils/Pagination';
import { PaginationComponent } from "../../shared/components/pagination/pagination.component";
import { TestEditableCardComponent } from "../../shared/components/test-editable-card/test-editable-card.component";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [PrimaryButtonComponent, PaginationComponent, TestEditableCardComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  profileCreatedTests: TestInfo[] = []
  createdTestsPagination: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 5
  }

  constructor(
    public userService: AuthService,
    private router: Router,
    private httpService: HttpService
  ) {
  }

  handleExit() {
    this.userService.Logout();
    this.router.navigate(["/authorization/sign-in"]);
  }

  ngOnInit() {
    this.getTests();
  }

  handleDeleteTest(testId: number) {
    console.log(testId);
    
    this.httpService.deleteRequest(`testing/Test/DeleteTest?testId=${testId}`)
    .subscribe({
      next: _ => {
        this.getTests();
      },
      error: _ => {
        console.log("unknown error");
      }
    });
  }

  executeNextPagination() {
    const maxPages = Math.ceil(this.createdTestsPagination.MaxSize / this.createdTestsPagination.PageSize)
    if(maxPages == this.createdTestsPagination.Page) {
      return;
    }

    this.createdTestsPagination.Page += 1;

    this.getTests();
  }

  executePrevPagination() {
    if(this.createdTestsPagination.Page == 1) {
      return;
    }

    this.createdTestsPagination.Page -= 1;

    this.getTests();
  }

  getTests() {
    var requetsUrl = `testing/Profile/GetProfileTests?` +
      `ProfileEmail=${this.userService.User?.Email}&` +
      `Page=${this.createdTestsPagination.Page}&` +
      `PageCount=${this.createdTestsPagination.PageSize}`;
    this.httpService.getRequest(requetsUrl)
      .subscribe({
        next: (data: any) => {
          this.profileCreatedTests = data.data.map((test: any) => ({
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

          this.createdTestsPagination = {
            MaxSize: data.maxSize,
            Page: data.page,
            PageSize: data.pageSize
          };       
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }
}
