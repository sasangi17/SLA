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

import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { Header } from '../header/header';
import {
  UserService,
  User
} from '../services/user.service';

import {
  ProfileImageService
} from '../services/profile-image.service';

import {
  FaceDetectionService
} from '../services/face-detection.service';
import { Confirm } from 'notiflix/build/notiflix-confirm-aio';
import { Notify } from 'notiflix/build/notiflix-notify-aio';

import IntlTelInput from '@intl-tel-input/angular';
import 'intl-tel-input/styles';


@Component({
  selector: 'app-home',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    Header,
    IntlTelInput
  ],

  templateUrl: './home.html',
  styleUrl: './home.css'
})


export class Home implements OnInit, OnDestroy {

  user: User | null = null;

  profileImage: string | null = null;

  isSaving = false;

  mobilePhoneValid = true;

  telephoneValid = true;

  private imageSubscription?: Subscription;

  loadUtils = () =>
    import('intl-tel-input/utils');


  constructor(

  private userService: UserService,

  private profileImageService:
    ProfileImageService,

  private faceDetectionService:
    FaceDetectionService,

  private router: Router,

  private cdr: ChangeDetectorRef,

  @Inject(PLATFORM_ID)
  private platformId: Object

) {}


  // ==========================================
  // NG ON INIT
  // ==========================================

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


    console.log(
      'HOME STORED USER:',
      storedUser
    );


    if (!storedUser) {

      this.router.navigateByUrl('/');

      return;
    }


    let loginUser: any;


    try {

      loginUser =
        JSON.parse(
          storedUser
        );

    } catch (error) {

      console.error(
        'Invalid logged in user:',
        error
      );


      localStorage.removeItem(
        'loggedInUser'
      );


      this.router.navigateByUrl('/');

      return;
    }


    // ========================================
    // GET USER ID
    // ========================================

    const userId =
      Number(
        loginUser.userId ??
        loginUser.id
      );


    console.log(
      'HOME USER ID:',
      userId
    );


    if (!userId) {

      localStorage.removeItem(
        'loggedInUser'
      );


      this.router.navigateByUrl('/');

      return;
    }


    // ========================================
    // PROFILE IMAGE LISTENER
    // ========================================

    this.imageSubscription =
      this.profileImageService
        .image$
        .subscribe({

          next: (image) => {

            console.log(
              'HOME IMAGE UPDATED:',
              image
            );


            this.profileImage =
              image;


            this.cdr.detectChanges();
          },


          error: (error) => {

            console.error(
              'Image subscription error:',
              error
            );

          }

        });


    // ========================================
    // LOAD USER
    // ========================================

    this.loadUser(userId);


    // ========================================
    // LOAD PROFILE IMAGE
    // ========================================

    this.profileImageService
      .loadImage(userId);

  }


  // ==========================================
  // LOAD USER
  // ==========================================

  private loadUser(
    userId: number
  ): void {

    console.log(
      'REQUESTING USER:',
      userId
    );


    this.userService
      .getUser(userId)
      .subscribe({

        next: (user) => {

          console.log(
            'HOME USER RESPONSE:',
            user
          );


          this.user =
            user;


          // Reset validation

          this.mobilePhoneValid =
            true;

          this.telephoneValid =
            true;


          this.cdr.detectChanges();

        },


        error: (error) => {

          console.error(
            'USER LOAD ERROR:',
            error
          );


          Notify.failure(
            'Failed to load user details.'
          );

        }

      });

  }


  // ==========================================
  // MOBILE VALIDITY
  // ==========================================

  onMobileValidityChange(
    valid: boolean
  ): void {

    this.mobilePhoneValid =
      valid;

  }


  // ==========================================
  // TELEPHONE VALIDITY
  // ==========================================

  onTelephoneValidityChange(
    valid: boolean
  ): void {

    this.telephoneValid =
      valid;

  }


  // ==========================================
  // LINKEDIN VALIDATION
  // ==========================================

  private isValidLinkedInUrl(
    value: string
  ): boolean {

    if (
      !value ||
      !value.trim()
    ) {

      return true;

    }


    const urlPattern =
      /^https?:\/\/(www\.)?linkedin\.com\/(in|company)\/[A-Za-z0-9._%-]+\/?$/i;


    return urlPattern.test(
      value.trim()
    );

  }


  // ==========================================
  // SAVE USER
  // ==========================================

  saveUser(): void {

    if (!this.user) {
      return;
    }


    // ========================================
    // PHONE VALIDATION
    // ========================================

    if (
      this.user.mobilePhone &&
      !this.mobilePhoneValid
    ) {

      Notify.failure(
        'Please enter a valid mobile phone number.'
      );

      return;
    }


    if (
      this.user.telephone &&
      !this.telephoneValid
    ) {

      Notify.failure(
        'Please enter a valid telephone number.'
      );

      return;
    }


    // ========================================
    // LINKEDIN VALIDATION
    // ========================================

    if (
      !this.isValidLinkedInUrl(
        this.user.linkedInLink
      )
    ) {

      Notify.failure(
        'Please enter a valid LinkedIn URL.'
      );

      return;
    }


    // ========================================
    // START SAVING
    // ========================================

    this.isSaving = true;


    this.userService
      .updateUser(
        this.user.id,
        this.user
      )
      .subscribe({

        next: () => {

          this.isSaving = false;


          Notify.success(
            'Profile details updated successfully.'
          );


          // Reload latest data from backend

          this.loadUser(
            this.user!.id
          );

        },


        error: (error) => {

          this.isSaving = false;


          console.error(
            'UPDATE USER ERROR:',
            error
          );


          Notify.failure(
            error.error?.message ??
            'Failed to update profile details.'
          );

        }

      });

  }


async changeProfileImage(
  event: Event
): Promise<void> {

  const input =
    event.target as HTMLInputElement;


  // ==========================================
  // CHECK FILE EXISTS
  // ==========================================

  if (
    !input.files ||
    input.files.length === 0
  ) {
    return;
  }


  // ==========================================
  // CHECK USER
  // ==========================================

  if (!this.user) {

    Notify.warning(
      'User information is not loaded.'
    );

    input.value = '';

    return;
  }


  // ==========================================
  // GET FILE
  // ==========================================

  const file =
    input.files[0];


  // ==========================================
  // ALLOWED IMAGE TYPES
  // ==========================================

  const allowedTypes = [
    'image/jpeg',
    'image/jpg',
    'image/png',
    'image/webp'
  ];


  if (
    !allowedTypes.includes(
      file.type.toLowerCase()
    )
  ) {

    Notify.failure(
      'Only JPG, PNG and WEBP images are allowed.'
    );

    input.value = '';

    return;
  }


  // ==========================================
  // IMAGE SIZE
  // ==========================================

  if (
    file.size >
    5 * 1024 * 1024
  ) {

    Notify.failure(
      'Image must be smaller than 5 MB.'
    );

    input.value = '';

    return;
  }


  // ==========================================
  // FACE DETECTION
  // ==========================================

  Notify.info(
    'Checking the image for a face...'
  );


  try {

    const result =
      await this.faceDetectionService
        .validateSingleFace(file);


    console.log(
      'FACE VALIDATION RESULT:',
      result
    );


    // ========================================
    // INVALID FACE RESULT
    // ========================================

    if (!result.valid) {

      Notify.failure(
        result.message
      );

      input.value = '';

      return;
    }


    // ========================================
    // EXACTLY ONE FACE
    // ========================================

    console.log(
      'One face detected. Uploading image...'
    );


    // ========================================
    // UPLOAD
    // ========================================

    this.profileImageService
      .uploadImage(
        this.user.id,
        file
      )
      .subscribe({

        next: (response) => {

          console.log(
            'UPLOAD SUCCESS:',
            response
          );

          Notify.success(
            'Profile image updated successfully.'
          );

        },

        error: (error) => {

          console.error(
            'UPLOAD ERROR:',
            error
          );

          console.error(
            'STATUS:',
            error.status
          );

          console.error(
            'SERVER:',
            error.error
          );

          Notify.failure(
            error.error?.message ??
            'Failed to upload profile image.'
          );
        }

      });

  } catch (error) {

    console.error(
      'FACE VALIDATION ERROR:',
      error
    );

    Notify.failure(
      'Unable to analyze the image. Please try another image.'
    );

  } finally {

    // Reset file input
    // so the same file can be selected again
    input.value = '';
  }
}


  // ==========================================
  // DELETE PROFILE IMAGE
  // ==========================================

  deleteProfileImage(): void {

    if (!this.user) {
      return;
    }


    Confirm.show(

      'Delete Profile Image',

      'Are you sure you want to delete your profile image?',

      'Yes',

      'Cancel',

      () => {

        this.profileImageService
          .deleteImage(
            this.user!.id
          )
          .subscribe({

            next: () => {

              this.profileImage =
                null;


              Notify.success(
                'Profile image deleted successfully.'
              );


              this.cdr.detectChanges();

            },


            error: (error) => {

              console.error(
                'DELETE IMAGE ERROR:',
                error
              );


              Notify.failure(
                error.error?.message ??
                'Failed to delete profile image.'
              );

            }

          });

      },


      () => {}

    );

  }


  // ==========================================
  // NG ON DESTROY
  // ==========================================

  ngOnDestroy(): void {

    this.imageSubscription
      ?.unsubscribe();

  }

}

