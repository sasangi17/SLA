import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';


export interface BungalowsModel  {
  bungalowId: number;
  bungalowName: string;
  bungalowCode: string;
  bungalowLocation: string;
  isActive: boolean;
  createUser?: number;
  createDateTime?: string;
  updateUser?: number | null;
  updateDateTime?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class BungalowsService {

  private apiUrl = 'https://localhost:7297/api/bungalows';

  constructor(private http: HttpClient) {}

  getAllBungalows(): Observable<BungalowsModel []> {
    return this.http
      .get<BungalowsModel []>(`${this.apiUrl}/getall`)
      .pipe(
        tap(data => console.log('Bungalows records:', data))
      );
  }

  getBungalows(bungalowsId: number): Observable<BungalowsModel > {
    return this.http
      .get<BungalowsModel >(
        `${this.apiUrl}/get?bungalowId=${bungalowsId}`
      );
  }

   addBungalow(bungalow: BungalowsModel): Observable<any> {
  return this.http.post(
    `${this.apiUrl}/addbungalow`,
    bungalow
  );
}

updateBungalows(
  bungalowId: number,
  bungalows: BungalowsModel
): Observable<any> {
  return this.http.put(
    `${this.apiUrl}/updatebungalow?bungalowId=${bungalowId}`,
    bungalows
  );
}

deleteBungalow(bungalowId: number): Observable<any> {
  return this.http.delete(
    `${this.apiUrl}/deletebungalow?bungalowId=${bungalowId}`
  );
}
}