import { Component, Input } from '@angular/core';
import { Group } from '../../interfaces/groups/Group';
import { GroupTestsComponent } from "../group-tests/group-tests.component";

@Component({
  selector: 'app-readonly-group',
  standalone: true,
  imports: [GroupTestsComponent],
  templateUrl: './readonly-group.component.html',
  styleUrl: './readonly-group.component.scss'
})
export class ReadonlyGroupComponent {
  @Input() group?:Group;
}
