import {
  Injectable,
  Inject,
  PLATFORM_ID
} from '@angular/core';

import {
  isPlatformBrowser
} from '@angular/common';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable,
  tap
} from 'rxjs';


export interface LoginRequest {
  email: string;
  password: string;
}


export interface LoginResponse {
  token: string;
  id: number;
  staffId: number;
  email: string;
  fullName: string;
}


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl =
    'https://localhost:7297/api/auth';

  // Timer for automatic logout
  private logoutTimer:
    ReturnType<typeof setTimeout> | null = null;


  constructor(
    private http: HttpClient,

    @Inject(PLATFORM_ID)
    private platformId: Object
  ) {

    // Check JWT when application starts
    if (
      isPlatformBrowser(
        this.platformId
      )
    ) {
      this.checkTokenExpiration();
    }
  }


  // Login

  login(
    data: LoginRequest
  ): Observable<LoginResponse> {

    return this.http
      .post<LoginResponse>(
        `${this.apiUrl}/login`,
        data
      )
      .pipe(

        tap(response => {

          // Save logged-in user
          const loggedInUser = {

            userId: response.id,
            staffId: response.staffId,  
            email: response.email,
            fullName: response.fullName

          };


          localStorage.setItem(
            'loggedInUser',
            JSON.stringify(
              loggedInUser
            )
          );


          // Save JWT token
          localStorage.setItem(
            'token',
            response.token
          );


          console.log(
            'JWT token saved.'
          );


          // Start automatic logout timer
          this.startLogoutTimer(
            response.token
          );

        })
      );
  }


  // Logout timer

  private startLogoutTimer(
    token: string
  ): void {

    if (
      !isPlatformBrowser(
        this.platformId
      )
    ) {
      return;
    }


    // Clear old timer
    if (
      this.logoutTimer
    ) {

      clearTimeout(
        this.logoutTimer
      );

      this.logoutTimer =
        null;
    }


    try {

      // Decode JWT
      const payload =
        this.decodeToken(
          token
        );


      // JWT must contain expiration
      if (!payload.exp) {

        console.warn(
          'JWT does not contain expiration time.'
        );

        return;
      }


      // JWT exp is seconds
      // Date.now() is milliseconds
      const expirationTime =
        payload.exp * 1000;


      const currentTime =
        Date.now();


      const timeRemaining =
        expirationTime -
        currentTime;


      console.log(
        'JWT expires at:',
        new Date(
          expirationTime
        )
      );


      console.log(
        'JWT expires in:',
        Math.round(
          timeRemaining / 1000
        ),
        'seconds'
      );


      // Already expired
      if (
        timeRemaining <= 0
      ) {

        this.logout();

        window.location.href = '/';

        return;
      }


      // Start timer
      this.logoutTimer =
        setTimeout(() => {

          console.log(
            'JWT expired.'
          );


          // Remove user and token
          this.logout();


          // Redirect to login
          window.location.href = '/';

        }, timeRemaining);

    }
    catch (error) {

      console.error(
        'Invalid JWT:',
        error
      );


      this.logout();

      window.location.href = '/';
    }
  }


  // Check token expiration on app start

  private checkTokenExpiration(): void {

    if (
      !isPlatformBrowser(
        this.platformId
      )
    ) {
      return;
    }


    const token =
      localStorage.getItem(
        'token'
      );


    if (!token) {
      return;
    }


    this.startLogoutTimer(
      token
    );
  }


  // Decode JWT token

  private decodeToken(
    token: string
  ): any {

    const tokenParts =
      token.split('.');


    // JWT format:
    // Header.Payload.Signature
    if (
      tokenParts.length !== 3
    ) {

      throw new Error(
        'Invalid JWT format.'
      );
    }


    const payload =
      tokenParts[1];


    // Base64URL → Base64
    const base64Payload =
      payload
        .replace(/-/g, '+')
        .replace(/_/g, '/');


    // Add padding if necessary
    const paddedPayload =
      base64Payload +
      '='.repeat(
        (
          4 -
          base64Payload.length % 4
        ) % 4
      );


    const decodedPayload =
      atob(
        paddedPayload
      );


    return JSON.parse(
      decodedPayload
    );
  }


  // Logout

  logout(): void {

    console.log(
      'Logging out.'
    );


    // Stop timer
    if (
      this.logoutTimer
    ) {

      clearTimeout(
        this.logoutTimer
      );

      this.logoutTimer =
        null;
    }


    // Remove logged-in user
    if (
      isPlatformBrowser(
        this.platformId
      )
    ) {

      localStorage.removeItem(
        'loggedInUser'
      );


      // Remove JWT
      localStorage.removeItem(
        'token'
      );
    }
  }


  // Get logged-in user

  getLoggedInUser(): any {

    if (
      !isPlatformBrowser(
        this.platformId
      )
    ) {
      return null;
    }

    const user = localStorage.getItem('loggedInUser');

    if (!user) {
      return null;
    }

    try {

      return JSON.parse(
        user
      );

    }
    catch {

      return null;
    }
  }


  // Get user ID 

  getUserId(): number | null {

    const user =
      this.getLoggedInUser();

    return user?.userId ?? null;
  }


  // Get JWT token

  getToken(): string | null {

    if (
      !isPlatformBrowser(
        this.platformId
      )
    ) {
      return null;
    }


    return localStorage.getItem(
      'token'
    );
  }


  // Check login 

  isLoggedIn(): boolean {

    if (
      !isPlatformBrowser(
        this.platformId
      )
    ) {
      return false;
    }


    const token =
      localStorage.getItem(
        'token'
      );


    if (!token) {
      return false;
    }


    try {

      const payload =
        this.decodeToken(
          token
        );


      if (!payload.exp) {
        return false;
      }


      return (
        payload.exp * 1000 >
        Date.now()
      );

    }
    catch {

      return false;
    }
  }
}