import { Component, Input } from '@angular/core';
import { PrimaryButtonComponent } from '../buttons/primary-button/primary-button.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [PrimaryButtonComponent],
  templateUrl: './auth-layout.component.html',
  styleUrl: './auth-layout.component.scss'
})
export class AuthLayoutComponent {
  @Input() headerNavigation: HeaderNavigation = {
    BtnName: "",
    NavigateUrl: ""
  };

  constructor(private router: Router) {

  }

  handleHeaderClick(event: MouseEvent) {
    this.router.navigate([this.headerNavigation.NavigateUrl]);
  }
}

export interface HeaderNavigation {
  BtnName: string,
  NavigateUrl: string
}
