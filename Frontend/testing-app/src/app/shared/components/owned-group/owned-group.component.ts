import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { OwnedGroup } from '../../interfaces/groups/OwnedGroup';
import { GroupMemberManagerComponent } from "../group-member-manager/group-member-manager.component";
import { HttpService } from '../../../core/services/Http.service';

@Component({
  selector: 'app-owned-group',
  standalone: true,
  imports: [PrimaryButtonComponent, GroupMemberManagerComponent],
  templateUrl: './owned-group.component.html',
  styleUrl: './owned-group.component.scss'
})
export class OwnedGroupComponent {
  @Input() group?:OwnedGroup;

  @Output() delete = new EventEmitter<number>();
  @Output() groupUpdated = new EventEmitter<number>();

  addMemberError: string =  "";
  isOpenEditSection: boolean = false;

  constructor(
    private httpService: HttpService
  ) {
    
  }

  handleDelete() {
    this.delete.emit(this.group?.Id);
  }

  handleKickMembers(membersIdToDelete: number[]) {
    this.httpService.patchRequest("testing/Group/DeleteMembersGroup", {
      groupId: this.group?.Id,
      membersId: membersIdToDelete
    }).subscribe({
      error: _ => {
        console.error("unknow error");
      },
      complete: () => {
        this.groupUpdated.emit(this.group?.Id);
      }
    });
  }

  handleAddMemmber(newMemberId: number) {
    this.httpService.patchRequest("testing/Group/AddGroupMember", {
      groupId: this.group?.Id,
      memberId: newMemberId
    }).subscribe({
      error: error => {
        if(error.status === 409) {
          this.addMemberError = "Current member already in group";
          setTimeout(() => {
            this.addMemberError = "";
          }, 3000);
        }
        else {
          console.error("unknown error");
        }
      },
      complete: () => {
        this.groupUpdated.emit(this.group?.Id);
      }
    });
  }
}
