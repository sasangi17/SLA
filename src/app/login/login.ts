import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Notify } from
  'notiflix/build/notiflix-notify-aio';

import { AuthService } from
  '../services/auth.service';

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

  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {

    if (
      !this.email.trim() ||
      !this.password.trim()
    ) {

      Notify.warning(
        'Please enter email and password.'
      );

      return;
    }

    this.isLoading = true;

    this.authService
      .login({
        email: this.email.trim(),
        password: this.password
      })
      .subscribe({

        next: () => {

          this.isLoading = false;

          Notify.success(
            'Login successful.'
          );

          this.router.navigate([
            '/home'
          ]);
        },

        error: (error) => {

          this.isLoading = false;

          console.error(
            'Login error:',
            error
          );

          if (error.status === 401) {

            Notify.failure(
              'Invalid email or password.'
            );

          } else if (error.status === 0) {

            Notify.failure(
              'Unable to connect to the backend server.'
            );

          } else {

            Notify.failure(
              'Login failed.'
            );
          }
        }
      });
  }
}