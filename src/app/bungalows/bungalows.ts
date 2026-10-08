import {
  Component,
  OnInit,
  ChangeDetectorRef,
  Inject,
  PLATFORM_ID
} from '@angular/core';

import {
  CommonModule,
  isPlatformBrowser
} from '@angular/common';

import { FormsModule } from '@angular/forms';

import Notiflix from 'notiflix';

import {
  BungalowsService,
  BungalowsModel
} from '../services/bungalows.service';

import { Header } from '../header/header';


@Component({
  selector: 'app-bungalows',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Header
  ],
  templateUrl: './bungalows.html',
  styleUrl: './bungalows.css'
})
export class Bungalows implements OnInit {

  bungalowsList: BungalowsModel[] = [];

  editingBungalows: BungalowsModel | null = null;

  newBungalow: BungalowsModel = this.createEmptyBungalow();

  isLoading = false;
  isSaving = false;
  isAdding = false;
  isDeleting = false;


  constructor(
    private bungalowsService: BungalowsService,
    private cdr: ChangeDetectorRef,

    @Inject(PLATFORM_ID)
    private platformId: Object
  ) {}


  ngOnInit(): void {

    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.loadBungalows();
  }


  // Empty bungalow

  private createEmptyBungalow(): BungalowsModel {

    return {
      bungalowId: 0,
      bungalowName: '',
      bungalowCode: '',
      bungalowLocation: '',
      isActive: true
    };
  }

  loadBungalows(): void {

    this.isLoading = true;

    this.bungalowsService
      .getAllBungalows()
      .subscribe({

        next: (data) => {

          this.bungalowsList = data;

          this.isLoading = false;

          this.cdr.markForCheck();
        },

        error: (error) => {

          console.error(
            'Failed to load Bungalows records:',
            error
          );

          this.isLoading = false;

          this.cdr.markForCheck();

          Notiflix.Notify.failure(
            'Failed to load Bungalows records.'
          );
        }
      });
  }


  // Open Add Bungalow 

  openAddBungalow(): void {

    this.newBungalow =
      this.createEmptyBungalow();

    this.isAdding = true;
  }


  // Cancel bungalow 

  cancelAdd(): void {

    if (this.isSaving) {
      return;
    }

    this.isAdding = false;

    this.newBungalow =
      this.createEmptyBungalow();
  }


  // Add bungalow
    

  addBungalow(): void {

    if (this.isSaving) {
      return;
    }


    if (!this.newBungalow.bungalowName.trim()) {

      Notiflix.Notify.warning(
        'Bungalow name is required.'
      );

      return;
    }


    if (!this.newBungalow.bungalowCode.trim()) {

      Notiflix.Notify.warning(
        'Bungalow code is required.'
      );

      return;
    }


    if (!this.newBungalow.bungalowLocation.trim()) {

      Notiflix.Notify.warning(
        'Bungalow location is required.'
      );

      return;
    }


    const loggedInUser =
      localStorage.getItem('loggedInUser');


    if (loggedInUser) {

      try {

        const loginUser =
          JSON.parse(loggedInUser);

        this.newBungalow.createUser =
          loginUser.userId ??
          loginUser.id;

      } catch (error) {

        console.error(
          'Unable to read logged in user:',
          error
        );
      }
    }


    this.isSaving = true;


    this.bungalowsService
      .addBungalow(this.newBungalow)
      .subscribe({

        next: () => {

          this.isSaving = false;

          this.isAdding = false;

          this.newBungalow =
            this.createEmptyBungalow();

          Notiflix.Notify.success(
            'Bungalow added successfully.'
          );

          this.loadBungalows();
        },


        error: (error) => {

          console.error(
            'Failed to add bungalow:',
            error
          );

          this.isSaving = false;

          this.cdr.markForCheck();

          Notiflix.Notify.failure(
            error?.error?.message ??
            'Failed to add bungalow.'
          );
        }
      });
  }


  // Edit bungalow details

  editBungalows(
    bungalow: BungalowsModel
  ): void {

    this.editingBungalows = {
      ...bungalow
    };
  }


  // Cancel editing

  cancelEdit(): void {

    this.editingBungalows = null;
  }



  // Save edited bungalow details

  saveBungalows(): void {

    if (!this.editingBungalows) {
      return;
    }


    if (!this.editingBungalows.bungalowName.trim()) {

      Notiflix.Notify.warning(
        'Bungalow name is required.'
      );

      return;
    }


    if (!this.editingBungalows.bungalowCode.trim()) {

      Notiflix.Notify.warning(
        'Bungalow code is required.'
      );

      return;
    }


    this.isSaving = true;


    const loggedInUser =
      localStorage.getItem('loggedInUser');


    if (loggedInUser) {

      try {

        const loginUser =
          JSON.parse(loggedInUser);

        this.editingBungalows.updateUser =
          loginUser.userId ??
          loginUser.id;

      } catch (error) {

        console.error(
          'Unable to read logged in user:',
          error
        );
      }
    }


    this.bungalowsService
      .updateBungalows(
        this.editingBungalows.bungalowId,
        this.editingBungalows
      )
      .subscribe({

        next: () => {

          this.isSaving = false;

          this.editingBungalows = null;

          Notiflix.Notify.success(
            'Bungalow updated successfully.'
          );

          this.loadBungalows();
        },


        error: (error) => {

          console.error(
            'Failed to update Bungalow:',
            error
          );

          this.isSaving = false;

          this.cdr.markForCheck();

          Notiflix.Notify.failure(
            error?.error?.message ??
            'Failed to update bungalow details.'
          );
        }
      });
  }


  // Delete 

  deleteBungalow(
    bungalow: BungalowsModel
  ): void {

    if (this.isDeleting) {
      return;
    }


    Notiflix.Confirm.show(
      'Delete Bungalow',
      `Are you sure you want to delete "${bungalow.bungalowName}"?`,
      'Yes, Delete',
      'Cancel',

      () => {

        this.isDeleting = true;

        this.bungalowsService
          .deleteBungalow(
            bungalow.bungalowId
          )
          .subscribe({

            next: () => {

              this.isDeleting = false;

              Notiflix.Notify.success(
                'Bungalow deleted successfully.'
              );

              this.loadBungalows();
            },


            error: (error) => {

              console.error(
                'Failed to delete bungalow:',
                error
              );

              this.isDeleting = false;

              this.cdr.markForCheck();

              Notiflix.Notify.failure(
                error?.error?.message ??
                'Failed to delete bungalow.'
              );
            }
          });
      },

      () => {
      
      }
    );
  }
}