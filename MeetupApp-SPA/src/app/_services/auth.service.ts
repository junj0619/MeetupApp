import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'auth/';
  private jwtHelper = new JwtHelperService();
  public decodedToken: any;
  public currentUser: User;
  private photoUrl = new BehaviorSubject('assets/images/default.png');
  public currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.currentUser = user.user;
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.changePhotoUrl(user.user.photoUrl);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  changePhotoUrl(url: string) {
    this.photoUrl.next(url);

    /* update localstorage user photoUrl so that when page got refresh the latest photoUrl will be rendered */
    const user: User = JSON.parse(localStorage.getItem('user'));
    user.photoUrl = url;
    localStorage.setItem('user', JSON.stringify(user));
  }
}
