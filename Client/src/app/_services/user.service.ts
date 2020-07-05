import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly url = 'https://localhost:44358/api/';

  constructor(private http: HttpClient) { }

  getUser() {
    return this.http.get(this.url + 'users');
  }


}
