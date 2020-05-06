import { catchError } from 'rxjs/operators';
import { AuthService } from './../_services/auth.service';
import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';

import { Message } from './../_models/message';
import { Observable, of } from 'rxjs';

@Injectable()
export class MessageResolver implements Resolve<Message[]> {
  pageNumber = 1;
  pageSize = 5;
  messageContainer = 'Unread';

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private authServer: AuthService,
    private router: Router
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.userService
      .getMessages(
        this.authServer.decodedToken.nameid,
        this.pageNumber,
        this.pageSize,
        this.messageContainer
      )
      .pipe(
        catchError((error) => {
          this.alertify.error('Problem retreving messages.');
          this.router.navigate(['/home']);
          return of(null);
        })
      );
  }
}
