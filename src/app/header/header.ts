import {
  Component,
  OnInit,
  OnDestroy,
  ChangeDetectorRef,
  Inject,
  PLATFORM_ID
} from '@angular/core';

import {
  CommonModule,
  isPlatformBrowser
} from '@angular/common';

import {
  Router
} from '@angular/router';

import {
  Subscription
} from 'rxjs';

import {
  ProfileImageService
} from '../services/profile-image.service';

import {
  UserService
} from '../services/user.service';

import {
  AuthService
} from '../services/auth.service';


@Component({
  selector: 'app-header',
  standalone: true,

  imports: [
    CommonModule
  ],

  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header
  implements OnInit, OnDestroy {

  userName = '';

  userEmail = '';

  staffId: number | null = null;

  profileImage:
    string | null = null;

  private imageSubscription?:
    Subscription;


  constructor(

    private userService:
      UserService,

    private profileImageService:
      ProfileImageService,

    private authService:
      AuthService,

    private router:
      Router,

    private cdr:
      ChangeDetectorRef,

    @Inject(PLATFORM_ID)
    private platformId:
      Object

  ) {}


  ngOnInit(): void {

    if (
      !isPlatformBrowser(
        this.platformId
      )
    ) {
      return;
    }


    const storedUser =
      localStorage.getItem(
        'loggedInUser'
      );


    if (!storedUser) {

      this.router.navigateByUrl(
        '/'
      );

      return;
    }


    let loginUser: any;

    try {

      loginUser =
        JSON.parse(storedUser);

    } catch {

      this.authService.logout();

      this.router.navigateByUrl(
        '/'
      );

      return;
    }


    const userId =
      Number(
        loginUser.userId ??
        loginUser.id
      );


    if (!userId) {

      this.authService.logout();

      this.router.navigateByUrl(
        '/'
      );

      return;
    }


    // ==================================
    // LOAD USER
    // ==================================

    this.userService
      .getUser(userId)
      .subscribe({

        next: (user) => {

          console.log(
            'HEADER USER:',
            user
          );


          this.userName =
            user.fullName ||
            user.email ||
            'User';

          this.userEmail =
            user.email ||
            '';

            this.staffId =
            user.staffId;

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'HEADER USER ERROR:',
            error
          );
        }
      });


    // ==================================
    // IMAGE LISTENER
    // ==================================

    this.imageSubscription =
      this.profileImageService
        .image$
        .subscribe({

          next: (image) => {

            console.log(
              'HEADER IMAGE:',
              image
            );

            this.profileImage =
              image;

            this.cdr.detectChanges();
          }
        });


    // ==================================
    // LOAD IMAGE
    // ==================================

    this.profileImageService
      .loadImage(userId);
  }


  // ==================================
  // LOGOUT
  // ==================================

  logout(): void {

    console.log(
      'Logout clicked'
    );


    this.authService.logout();

    this.profileImageService
      .clearImage();


    this.userName = '';

    this.userEmail = '';

    this.staffId = null;

    this.profileImage = null;


    this.cdr.detectChanges();


    // Your login page is the root route
    this.router.navigateByUrl('/');
  }


  ngOnDestroy(): void {

    this.imageSubscription
      ?.unsubscribe();
  }
}