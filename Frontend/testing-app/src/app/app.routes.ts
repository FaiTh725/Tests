import { Routes } from '@angular/router';
import { authUserGuard } from './core/guards/AuthUser.guard';

export const routes: Routes = [
  {path: "authorization", children: [
    {
      path: "sign-up", 
      loadComponent: async () => (await import("./pages/register/register.component"))
      .RegisterComponent
    },
    {
      path: "confirm-email", 
      loadComponent: async () => (await import("./pages/confirm-email/confirm-email.component"))
      .ConfirmEmailComponent
    },
    {
      path: "**", 
      loadComponent: async () => (await import("./pages/login/login.component"))
      .LoginComponent
    }
  ]},
  {path: "", 
    loadComponent: async () => (await import("./shared/components/sidebar-layout/sidebar-layout.component"))
    .SidebarLayoutComponent, 
    canActivateChild:[authUserGuard], children: [
    {
      path: "profile", loadComponent: async () => (await import("./pages/profile/profile.component"))
      .ProfileComponent
    },
    {
      path: "create-test", loadComponent: async () => (await import("./pages/create-test/create-test.component"))
      .CreateTestComponent
    },
    {
      path: "groups", loadComponent: async () => (await import("./pages/groups/groups.component"))
      .GroupsComponent
    },
    {
      path: "**", loadComponent: async () => (await import("./pages/home/home.component"))
      .HomeComponent
    }
  ]}
];
