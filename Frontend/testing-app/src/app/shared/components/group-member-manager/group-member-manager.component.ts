import { Component, EventEmitter, Input, output, Output, SimpleChanges } from '@angular/core';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { ProfileInfo } from '../../interfaces/Profile/ProfileInfo';
import { SquareCheckboxComponent } from "../inputs/square-checkbox/square-checkbox.component";
import { SearchComponent } from "../inputs/search/search.component";
import { HttpService } from '../../../core/services/Http.service';

@Component({
  selector: 'app-group-member-manager',
  standalone: true,
  imports: [PrimaryButtonComponent, SquareCheckboxComponent, SearchComponent],
  templateUrl: './group-member-manager.component.html',
  styleUrl: './group-member-manager.component.scss'
})
export class GroupMemberManagerComponent {
  newMemberId: number | null = null;

  @Input() members: ProfileInfo[] = [];
  @Input() addMemberError = "";

  @Output() addMember = new EventEmitter<number>();
  @Output() kickMembers = new EventEmitter<number[]>();

  membersIdToDelete: number[] = [];

  searchResultProfiles: ProfileInfo[] = [];

  constructor(
    private httpService: HttpService
  ) {
    
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes['addMemberError']) {
      this.addMemberError = changes['addMemberError'].currentValue;
    }
  }

  handleAddMember() {
    if(this.newMemberId) {
      this.addMember.emit(this.newMemberId);
    }
  }

  handleKickMembers() {
    this.kickMembers.emit(this.membersIdToDelete);
  }

  handleSelectMember(memberId: number) {

    if(this.membersIdToDelete.includes(memberId)) {
      this.membersIdToDelete = this.membersIdToDelete
        .filter(x => x != memberId);
    }
    else {
      this.membersIdToDelete.push(memberId);
    }
  }

  handleSelectNewMembers(memberId: string) {
    const numberMemberId = Number(memberId);

    if(isNaN(numberMemberId)){
      console.error("Error with selecet new memberid, value isnt number");
      
      return;
    }

    this.newMemberId = numberMemberId;
  }

  handleSearch (searchValue: String) {
    if(searchValue === "") {
      return;
    }

    const requestUrl = `testing/Profile/GetProfileByFistEmail?email=${searchValue}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        this.searchResultProfiles = [...data.map((profile:any) => ({
          Id: profile.id,
          Email: profile.email,
          Name: profile.name
        }))];
      },
      error: _ => {
        console.log("unknow error");
      }
    });
  }
}
