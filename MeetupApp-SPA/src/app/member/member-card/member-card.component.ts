import { AuthService } from './../../_services/auth.service';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private authServer: AuthService
  ) {}

  ngOnInit() {}

  setUserLike(recipientId) {
    this.userService
      .setUserLike(this.authServer.decodedToken.nameid, recipientId)
      .subscribe(
        (response) => {
          this.alertify.success('You have liked ' + this.user.knownAs);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}
