import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-sidebar-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './sidebar-layout.component.html',
  styleUrl: './sidebar-layout.component.scss'
})
export class SidebarLayoutComponent {
  navigationLinks: Navigation[] = [
    {
      Text: "Profile",
      Icon: "/icons/Profile.png",
      Url: "/profile"
    },
    {
      Text: "Create Test",
      Icon: "/icons/AddTest.png",
      Url: "/create-test"
    },
    {
      Text: "Groups",
      Icon: "/icons/group-cats.png",
      Url: "/groups"
    },
  ]

  selectedNavigate = "";

  constructor(private router: Router) {

  }

  handleSelectNavigate(url: string) {
    this.selectedNavigate = url;
    this.router.navigate([this.selectedNavigate]);
  }
}

interface Navigation {
  Icon: string,
  Text: string,
  Url: string
}
