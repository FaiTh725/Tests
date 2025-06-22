import { Component, Input, input } from '@angular/core';
import { TestInfo } from '../../interfaces/tests/TestInfo';
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-available-test-card',
  standalone: true,
  imports: [PrimaryButtonComponent],
  templateUrl: './available-test-card.component.html',
  styleUrl: './available-test-card.component.scss'
})
export class AvailableTestCardComponent {
  @Input() test?: TestInfo;

  constructor(
    private router: Router
  ) {
    
  }

  handleNavigateToTest() {
    console.log(this.test?.Id);
    
    this.router.navigate(
      ["test"], 
      {queryParams: {id: this.test?.Id}});
  }
}
