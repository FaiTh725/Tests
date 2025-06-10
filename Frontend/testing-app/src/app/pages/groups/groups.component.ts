import { Component, ElementRef, ViewChild } from '@angular/core';
import { PrimaryInputComponent } from "../../shared/components/inputs/primary-input/primary-input.component";
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { HttpService } from '../../core/services/Http.service';
import { AuthService } from '../../core/services/Auth.service';
import { Pagination } from '../../shared/interfaces/utils/Pagination';
import { Group } from '../../shared/interfaces/groups/Group';
import { OwnedGroup } from '../../shared/interfaces/groups/OwnedGroup';
import { OwnedGroupComponent } from "../../shared/components/owned-group/owned-group.component";
import { ReadonlyGroupComponent } from "../../shared/components/readonly-group/readonly-group.component";
import { PaginationComponent } from "../../shared/components/pagination/pagination.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-groups',
  standalone: true,
  imports: [CommonModule, PrimaryInputComponent, PrimaryButtonComponent, OwnedGroupComponent, ReadonlyGroupComponent, PaginationComponent],
  templateUrl: './groups.component.html',
  styleUrl: './groups.component.scss'
})
export class GroupsComponent {
  form: AddGroupForm = {
    Name: ""
  }

  errorForm: AddGroupErrorForm = {
    NameError: ""
  }

  curSelectedGrops = "Created";

  createdGroups: OwnedGroup[] = [];
  joinedGroup: Group[] = []

  paginationCreatedGroups: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 5
  }

  paginationJoinedGroups: Pagination = {
    MaxSize: 0,
    Page: 1,
    PageSize: 5
  }

  @ViewChild('groupSwither', {static: false, read: ElementRef}) groupDiv!: ElementRef;

  constructor(
    private authSerivive: AuthService,
    private httpService: HttpService
  ) {
    
  }

  handleClearErrorForm() {
    this.errorForm = {
      NameError: ""
    }
  }

  handleSendForm() {
    if(this.form.Name.length < 3 || 
      this.form.Name.length > 100
    ) {
      this.errorForm.NameError = "Name mus be in range from 3 to 100";

      return;
    }

    this.httpService.postRequest("testing/Group/CreateGroup", {
      name: this.form.Name
    })
    .subscribe({
      error: _ => {
        console.error("unknown error");
      },
      complete: () => {
        this.getCreatedGroups();

        this.form.Name = "";
      }
    });
  }

  ngOnInit() {
    this.getCreatedGroups();
    this.getJoinedGroups();
  }

  getCreatedGroups() {
    const getCreatedTestsRequest = `testing/Profile/GetProfileCreatedGroups?` + 
    `ProfileEmail=${this.authSerivive.User?.Email}&` + 
    `Page=${this.paginationCreatedGroups.Page}&` + 
    `PageSize=${this.paginationCreatedGroups.PageSize}`;
    this.httpService.getRequest(getCreatedTestsRequest)
    .subscribe({
      next: (data:any) => {
        this.createdGroups = [...data.data.map((group:any) => ({
          Id: group.id,
          Name: group.name,
          Members: [...group.members.map((member:any) => ({
            Id: member.id,
            Email: member.email,
            Name: member.name
          }))]
        }))];

        this.paginationCreatedGroups = {
          MaxSize: data.maxSize,
          Page: data.page,
          PageSize: data.pageSize
        };
      },
      error: _ => {
        console.error("unknow error");
      }
    });
  }

  getJoinedGroups() {
    const getCreatedTestsRequest = `testing/Profile/GetProfileJoinedGroups?` + 
    `ProfileEmail=${this.authSerivive.User?.Email}&` + 
    `Page=${this.paginationCreatedGroups.Page}&` + 
    `PageSize=${this.paginationCreatedGroups.PageSize}`;
    this.httpService.getRequest(getCreatedTestsRequest)
    .subscribe({
      next: (data:any) => {
        this.joinedGroup = [...data.data.map((group:any) => ({
          Id: group.id,
          Name: group.name
        }))];

        this.paginationJoinedGroups = {
          MaxSize: data.maxSize,
          Page: data.page,
          PageSize: data.pageSize
        };
      },
      error: _ => {
        console.error("unknow error");
      }
    });

  }

  handleDeleteGroup(groupId: number) {
    this.httpService.deleteRequest("testing/Group/DeleteGroup", {
      groupId: groupId
    })
    .subscribe({
      next: () => {
        this.getCreatedGroups();
      },
      error: () => {
        console.error("unknown error");
      }
    });
  }

  handleUpdatedGroup(groupId: number) {
    const getGroupRequestUrl = `testing/Group/GetGroupWithMembers?groupId=${groupId}`;
    this.httpService.getRequest(getGroupRequestUrl)
    .subscribe({
      next: (data:any) => {
        var groupToUpdate = this.createdGroups
          .find(group => group.Id == groupId);

        if(groupToUpdate) {
          const updatedGroup = groupToUpdate = {
              Id: data.id,
              Name: data.name,
              Members: data.members.map((member: any) => ({
                Id: member.id,
                Name: member.name,
                Email: member.email,
              }))
            };

          this.createdGroups = this.createdGroups
            .map(group => group.Id === groupId ? updatedGroup : group);
        }
        else {
          console.error("critical error, there are no group by cur id");
        }
      },
      error: _ => {
        console.error("unknown error");
      }
    });
  }

  handleSwith(groupName: string) {
    this.curSelectedGrops = groupName;

    if(this.groupDiv) {
      const scrollTo = groupName == "Created" ? 
        0:
        this.groupDiv.nativeElement.scrollWidth; 
      
      this.groupDiv.nativeElement.scrollTo({
        top: 0,
        left: scrollTo,
        behavior: "smooth",
      });
    }
  }
}

interface AddGroupForm {
  Name: string
}

interface AddGroupErrorForm {
  NameError: string
}
