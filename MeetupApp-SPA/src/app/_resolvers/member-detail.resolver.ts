import { catchError } from 'rxjs/operators';
import { UserService } from './../_services/user.service';
import { AlertifyService } from './../_services/alertify.service';
import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';

@Injectable()
export class MemberDetailResolver implements Resolve<User> {
  constructor(
    private router: Router,
    private alertify: AlertifyService,
    private userService: UserService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User> {
    const id = route.params.id;
    return this.userService.getUser(id).pipe(
      catchError((error) => {
        this.alertify.error(error);
        this.router.navigate(['/members']);
        return of(null);
      })
    );
  }
}
