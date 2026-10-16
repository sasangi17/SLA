import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable,tap} from 'rxjs';

export interface User {
  id: number;
  staffId: number;
  email: string;
  fullName: string;
  position: string;
  department: string;
  mobilePhone: string;
  telephone: string;
  address: string;
  linkedInLink: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl =
    'https://localhost:7297/api/users';

  constructor(
    private http: HttpClient
  ) {}

  getUser(
    userId: number
  ): Observable<User> {

    return this.http
  .get<User>(
    `${this.apiUrl}/getuser?userId=${userId}`
  )
      .pipe(

        tap(user => {

          console.log(
            'User loaded:',
            user
          );

        })
      );
  }

  updateUser(
    userId: number,
    user: User
  ): Observable<any> {

    return this.http.put(
  `${this.apiUrl}/updateuser?userId=${userId}`,
  user
);
  }
}