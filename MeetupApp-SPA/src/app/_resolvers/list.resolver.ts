import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ListResolver implements Resolve<User[]> {
  currentPage = 1;
  itemsPerPage = 5;
  likeParam = 'Likers';
  constructor(
    private userService: UserService,
    private router: Router,
    private alertifyService: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    return this.userService
      .getUsers(this.currentPage, this.itemsPerPage, null, this.likeParam)
      .pipe(
        catchError((error) => {
          this.alertifyService.error(error);
          this.router.navigate(['/home']);
          return of(null);
        })
      );
  }
}
