import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import {
  BehaviorSubject,
  Observable
} from 'rxjs';

import {
  tap
} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ProfileImageService {

  private apiUrl =
    'https://localhost:7297/api/profile-image';


  private imageSubject =
    new BehaviorSubject<string | null>(null);


  image$ =
    this.imageSubject.asObservable();


  // Current browser object URL
  private currentObjectUrl:
    string | null = null;


  constructor(
    private http: HttpClient
  ) {}


  // Load image from backend(database)

  loadImage(userId: number): void {

  const imageUrl =
  `${this.apiUrl}/getimage?userId=${userId}`;

  console.log(
    'LOADING IMAGE:',
    imageUrl
  );

  this.http
    .get(
      imageUrl,
      {
        responseType: 'blob'
      }
    )
    .subscribe({

      next: (blob) => {

        console.log(
          'IMAGE RECEIVED:',
          blob.size,
          blob.type
        );


        if (
          !blob ||
          blob.size === 0
        ) {

          this.clearImage();

          return;
        }


        this.revokeCurrentUrl();


        this.currentObjectUrl =
          URL.createObjectURL(blob);


        this.imageSubject.next(
          this.currentObjectUrl
        );
      },

      error: (error) => {

        console.error(
          'IMAGE LOAD ERROR:',
          error
        );

        this.clearImage();
      }
    });
}

  // Image upload

  uploadImage(
    userId: number,
    file: File
  ): Observable<any> {

    const formData =
      new FormData();


    formData.append('file',file,file.name);


    return this.http
    .post(
    `${this.apiUrl}/insertimage?userId=${userId}`,
    formData
  )
      .pipe(

        tap(() => {

          // Automatically load the
          // newly saved image from DB
          this.loadImage(userId);

        })

      );
  }


 
  // Delete image
 

  deleteImage(
    userId: number
  ): Observable<any> {

    return this.http
  .delete(
    `${this.apiUrl}/deleteimage?userId=${userId}`
  )
      .pipe(

        tap(() => {

          // No default image.
          // Home + Header become empty.
          this.clearImage();

        })

      );
  }

  // CLEAR IMAGE
 
  clearImage(): void {

    this.revokeCurrentUrl();

    this.imageSubject.next(null);
  }


  private revokeCurrentUrl(): void {

    if (
      this.currentObjectUrl
    ) {

      URL.revokeObjectURL(
        this.currentObjectUrl
      );

      this.currentObjectUrl =
        null;
    }
  }

}

