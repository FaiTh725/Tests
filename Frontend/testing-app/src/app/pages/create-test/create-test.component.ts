import { Component } from '@angular/core';
import { PrimaryInputComponent } from "../../shared/components/inputs/primary-input/primary-input.component";
import { CheckboxComponent } from "../../shared/components/inputs/checkbox/checkbox.component";
import { ComboboxComponent } from "../../shared/components/inputs/combobox/combobox.component";
import { LargeInputComponent } from "../../shared/components/inputs/large-input/large-input.component";
import { PrimaryButtonComponent } from "../../shared/components/buttons/primary-button/primary-button.component";
import { TestType } from '../../shared/interfaces/tests/TestType';
import { HttpService } from '../../core/services/Http.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-test',
  standalone: true,
  imports: [PrimaryInputComponent, CheckboxComponent, ComboboxComponent, LargeInputComponent, PrimaryButtonComponent],
  templateUrl: './create-test.component.html',
  styleUrl: './create-test.component.scss'
})
export class CreateTestComponent {
  form: CreateTestForm = {
    Name: "",
    Description: "",
    IsPublic: true,
    TestType: TestType.Progressive,
    DurationInMinutes: "",
  }

  formError: CreateTestErrorForm = {
    NameError: "",
    DescriptionError: "",
    IsPublicError: "",
    TestTypeError: "",
    DurationInMinutesError: ""
  }

  testTypes = Object.keys(TestType)
    .filter(key => isNaN(Number(key)));

  constructor(
    private httpService: HttpService,
    private router: Router
  ) {
    
  }

  handleSendForm() {
    var isValidForm = true;

    if(this.form.Name.length < 2 ||
      this.form.Name.length > 100
    ) {
      this.formError.NameError = "Name must be in range from 2 to 100";
      isValidForm = false;
    }
    if(this.form.Description.length > 500) {
      this.formError.DescriptionError = "Error is too long";
      isValidForm = false;
    }
    if(this.form.TestType == 0) {
      const duration = Number(this.form.DurationInMinutes);
      if(isNaN(duration) || duration <= 0) {
        this.formError.DurationInMinutesError = "Duration must be number and greater than zero";
        isValidForm = false;
      }
    }

    if(!isValidForm) {
      return;
    }
    console.log(this.form);
    this.httpService.postRequest("testing/Test/CreateTest", {
      name: this.form.Name,
      description: this.form.Description ?? "",
      isPublic: this.form.IsPublic,
      testType: this.form.TestType,
      durationInMinutes: this.form.TestType == 0 ? 
        Number(this.form.DurationInMinutes) : 
        null
    }).subscribe({
      next: _ => {
        this.router.navigate(["/profile"]);
      },
      error: error => {
        console.log(error)
      }
    });
  }

  handleClearError() {
    this.formError = {
      NameError: "",
      DescriptionError: "",
      IsPublicError: "",
      TestTypeError: "",
      DurationInMinutesError: ""
    }
  }
}

interface CreateTestForm {
  Name: string,
  Description: string,
  IsPublic: boolean,
  TestType: TestType,
  DurationInMinutes: string
}

interface CreateTestErrorForm {
  NameError: string,
  DescriptionError: string,
  IsPublicError: string,
  TestTypeError: string,
  DurationInMinutesError: string
}
