import { User } from './../_models/User';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

// .net core v2.2 then you need this httpOptions otherwise not need.
const httpOptions = {
  headers: new HttpHeaders({'Content-Type':  'application/json'})
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly url = 'https://localhost:44358/api/';
  decodedToken: any;
  jwtHelper = new JwtHelperService();
  isOpen: boolean = true;

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: any) {
    return this.http.post(this.url + 'auth/login', JSON.stringify(credentials), httpOptions).pipe(
      map((response: any) => {
      const result = response;
      if (result) {
        localStorage.setItem('token', result.token);
        // localStorage.setItem('user', result.user);
        this.decodedToken = this.jwtHelper.decodeToken(result.token);
        return true;
      }
      return false;
    }));
  }

  register(credentials) {
    console.log(credentials);
    return this.http.post(this.url + 'auth/register', credentials, httpOptions);
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/']);
  }

  isLoggedIn() {
    let token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  currentUser() {
    let token = localStorage.getItem('token');
    if (!token) return null;

    return new JwtHelperService().decodeToken(token);
  }

  roleMatch(allowedRole): boolean {
    let roleMatch = false;
    let token = localStorage.getItem('token');
    const roles = new JwtHelperService().decodeToken(token).role as Array<string>;
    allowedRole.forEach(element => {
      if (roles.includes(element)) {
        roleMatch = true;
      }
    });
    return roleMatch;
  }
}
