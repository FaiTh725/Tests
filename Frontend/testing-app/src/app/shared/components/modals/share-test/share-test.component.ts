import { Component, Input } from '@angular/core';
import { ClearButtonComponent } from "../../buttons/clear-button/clear-button.component";
import { ModalService } from '../../../../core/services/Modal.service';
import { SearchComponent } from "../../inputs/search/search.component";
import { ComboboxComponent } from "../../inputs/combobox/combobox.component";
import { PrimaryButtonComponent } from "../../buttons/primary-button/primary-button.component";
import { Group } from '../../../interfaces/groups/Group';
import { HttpService } from '../../../../core/services/Http.service';
import { ProfileInfo } from '../../../interfaces/Profile/ProfileInfo';

@Component({
  selector: 'app-share-test',
  standalone: true,
  imports: [ClearButtonComponent, SearchComponent, ComboboxComponent, PrimaryButtonComponent],
  templateUrl: './share-test.component.html',
  styleUrl: './share-test.component.scss'
})
export class ShareTestComponent {
  form: ShareTestForm = {
    TestId: 0,
    TargetEntityId: 0,
    TargetEntity: TargetEntity.Group
  };

  errorMesage = "";

  searchResultGroups: Group[] = [];
  searchResultProfiles: ProfileInfo[] = [];
  
  shareTestEntityTypes = ["Profile", "Group"]

  get displayProperty() {
    return this.form.TargetEntity == TargetEntity.Group ? 
      'Name' : 
      'Email';
  }

  get searchResult() {
    return this.form.TargetEntity == TargetEntity.Group ? 
      this.searchResultGroups : 
      this.searchResultProfiles;
  }

  @Input() testId: number = 0;

  constructor(
    private modalService: ModalService,
    private httpService: HttpService
  ) {
  }
  
  ngOnInit() {
    this.form.TestId = this.testId;
  }

  handleCloseModal() {
    this.modalService.handleClose();
  }

  handleSelectTargetEntity(targetId: string) {
    const targetEntityId = Number(targetId);

    if(isNaN(targetEntityId)){
      console.error("Error with selecet new memberid, value isnt number");
      
      return;
    }

    this.form.TargetEntityId = targetEntityId;
  }

  handleSearch(searchString: string) {
    const requestUrl =  this.form.TargetEntity == TargetEntity.Group ? 
      `testing/Group/GetGroupsByFirstName?GroupName=${searchString}` :
      `testing/Profile/GetProfileByFirstEmail?email=${searchString}`;
    this.httpService.getRequest(requestUrl)
    .subscribe({
      next: (data: any) => {
        if(this.form.TargetEntity == TargetEntity.Group) {
          this.searchResultGroups = data.map((group: any) => ({
            Id: group.id,
            Name: group.name
          }));
        }
        else {
          this.searchResultProfiles = data.map((profile: any) => ({
            Id: profile.id,
            Name: profile.name,
            Email: profile.email
          }));
        }
      },
      error : _ => {
        console.error("unknown error");
      }
    });
  }

  handleShareTest() {
    if(this.form.TargetEntityId === 0) {
      this.errorMesage = "search any entity to share test";
      
      setTimeout(() => {
          this.errorMesage = "";
        }, 3000);

      return;
    }

    const requestUrl = "testing/Test/ProviderTestAccess";
    this.httpService.postRequest(requestUrl, {
      testId: this.form.TestId,
      targetEntityId: this.form.TargetEntityId,
      targetAccessEntityType: this.form.TargetEntityId
    })
    .subscribe({
      complete: () => {
        this.form.TargetEntityId = 0;
      },
      error: error => {
        if(error.status === 400) {
          this.errorMesage = error.detail;

          setTimeout(() => {
            this.errorMesage = "";
          }, 3000);
        }
        else if(error.status === 409) {
          this.errorMesage = "Entity already has access";

          setTimeout(() => {
            this.errorMesage = "";
          }, 3000);
        }
        else {
          console.error("unknown error");
        }
      }
    })
  }
}

interface ShareTestForm {
  TestId: number,
  TargetEntityId: number,
  TargetEntity: TargetEntity
}

enum TargetEntity {
  Profile,
  Group
}
