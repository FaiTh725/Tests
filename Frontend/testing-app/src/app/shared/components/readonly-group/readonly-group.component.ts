import { Component, Input } from '@angular/core';
import { Group } from '../../interfaces/groups/Group';

@Component({
  selector: 'app-readonly-group',
  standalone: true,
  imports: [],
  templateUrl: './readonly-group.component.html',
  styleUrl: './readonly-group.component.scss'
})
export class ReadonlyGroupComponent {
  @Input() group?:Group;
}
