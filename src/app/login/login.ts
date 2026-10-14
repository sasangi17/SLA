import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Notify } from 'notiflix/build/notiflix-notify-aio';

import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  email = '';
  password = '';


  isLoading = signal(false);
  showRoleSelection = signal(false);
  availableRoles = signal<string[]>([]);

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  
  login(): void {
    if (this.isLoading()) {
      return;
    }

    if (!this.email.trim() || !this.password.trim()) {
      Notify.warning('Please enter email and password.');
      return;
    }

    this.sendLogin();
  }

  selectRole(role: string): void {
    if (this.isLoading()) 
    {
      return;
    }

    this.sendLogin(role);
  }

  cancelRoleSelection(): void {
    this.showRoleSelection.set(false);
    this.availableRoles.set([]);
    this.password = '';
  }

  private sendLogin(selectedRole?: string): void {
    this.isLoading.set(true);

    this.authService
      .login({
        email: this.email.trim(),
        password: this.password,
        selectedRole
      })
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);

          const roles: string[] =
            response.roles ?? (response as any).Roles ?? [];

          const requiresRoleSelection: boolean =
            response.requiresRoleSelection ??
            (response as any).RequiresRoleSelection ??
            false;

          const token: string | undefined =
            response.token ?? (response as any).Token;

          // Account has several roles and none chosen yet
          if (requiresRoleSelection || (!token && roles.length > 1)) {
            this.availableRoles.set(roles);
            this.showRoleSelection.set(true);
            return;
          }

          if (!token) {
            Notify.failure('Login failed: no token received.');
            return;
          }

          Notify.success('Login successful.');
          this.router.navigate(['/home']);
        },

        error: (error) => {
          this.isLoading.set(false);
          console.error('Login error:', error);

          if (error.status === 401) {
            Notify.failure('Invalid email or password.');
          } else if (error.status === 403) {
            Notify.failure('You are not assigned to this role.');
          } else if (error.status === 0) {
            Notify.failure('Unable to connect to the backend server.');
          } else {
            Notify.failure('Login failed.');
          }
        }
      });
  }
}