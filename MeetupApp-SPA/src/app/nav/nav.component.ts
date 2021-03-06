import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(
    public authService: AuthService,
    private alertifyService: AlertifyService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe((photoUrl) => {
      this.photoUrl = photoUrl;
      console.log(this.photoUrl);
    });
  }

  login() {
    this.authService.login(this.model).subscribe(
      (response) => {
        this.alertifyService.success('logged in successfully');
      },
      (error) => {
        this.alertifyService.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.alertifyService.success('logout successfully.');
    this.router.navigate(['/home']);
  }
}
