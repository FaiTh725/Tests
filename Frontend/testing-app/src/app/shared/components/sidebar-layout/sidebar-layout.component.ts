import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

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
  ]

  selectedNavigate = "";

  handleSelectNavigate(url: string) {
    this.selectedNavigate = url;
  }
}

interface Navigation {
  Icon: string,
  Text: string,
  Url: string
}
