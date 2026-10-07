import {
  Component, OnInit, ChangeDetectorRef, Inject, PLATFORM_ID
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import Notiflix from 'notiflix';

import { BungalowsService, BungalowsModel } from '../services/bungalows.service';
import { Header } from '../header/header';

@Component({
  selector: 'app-bungalows',
  standalone: true,
  imports: [CommonModule, FormsModule, Header],
  templateUrl: './bungalows.html',
  styleUrl: './bungalows.css'
})
export class Bungalows implements OnInit {

  bungalowsList: BungalowsModel[] = [];
  editingBungalows: BungalowsModel | null = null;

  isLoading = false;
  isSaving = false;

  constructor(
    private bungalowsService: BungalowsService,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    // Don't call the API during SSR (no token, self-signed cert, no document)
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }
    this.loadBungalows();
  }

  loadBungalows(): void {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.bungalowsService.getAllBungalows().subscribe({
      next: (data) => {
        this.bungalowsList = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Failed to load Bungalows records:', error);
        this.isLoading = false;
        this.cdr.markForCheck();
        Notiflix.Notify.failure('Failed to load Bungalows records.');
      }
    });
  }

  editBungalows(bungalows: BungalowsModel): void {
    this.editingBungalows = { ...bungalows };
  }

  cancelEdit(): void {
    this.editingBungalows = null;
  }

  saveBungalows(): void {
    if (!this.editingBungalows) return;

    if (!this.editingBungalows.bungalowName.trim()) {
      Notiflix.Notify.warning('Bungalows name is required.');
      return;
    }
    if (!this.editingBungalows.bungalowCode.trim()) {
      Notiflix.Notify.warning('Bungalows code is required.');
      return;
    }

    this.isSaving = true;

    const loggedInUser = localStorage.getItem('loggedInUser');
    if (loggedInUser) {
      const loginUser = JSON.parse(loggedInUser);
      this.editingBungalows.updateUser = loginUser.userId ?? loginUser.id;
    }

    this.bungalowsService
      .updateBungalows(this.editingBungalows.bungalowId, this.editingBungalows)
      .subscribe({
        next: () => {
          this.isSaving = false;
          this.editingBungalows = null;   // close the edit panel
          Notiflix.Notify.success('Bungalows updated successfully.');
          this.loadBungalows();           // reload; it manages isLoading itself
          this.cdr.markForCheck();
        },
        error: (error) => {
          console.error('Failed to update Bungalows:', error);
          this.isSaving = false;
          this.cdr.markForCheck();
          Notiflix.Notify.failure(
            error?.error?.message ?? 'Failed to update Bungalows details.'
          );
        }
      });
  }
}