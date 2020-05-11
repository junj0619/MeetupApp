import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertifyService: AlertifyService
  ) {}

  canActivate(next: ActivatedRouteSnapshot): boolean {
    let canActivate = false;
    const allowedRoles = next.firstChild.data['roles'] as Array<string>;

    if (!this.authService.roleMatch(allowedRoles)) {
      canActivate = false;
    } else {
      canActivate = true;
    }

    if (this.authService.loggedIn()) {
      canActivate = true;
    }

    if (canActivate) {
      return true;
    } else {
      this.alertifyService.error('You are Unauthroized to access the page!');
      this.router.navigate(['/home']);
      return false;
    }
  }
}
