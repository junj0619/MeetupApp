import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Message } from './../../_models/message';
import { AlertifyService } from '../../_services/alertify.service';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css'],
})
export class MemberMessageComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};
  messageForm: FormGroup;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private formBuild: FormBuilder
  ) {}

  createMessageForm() {
    this.messageForm = this.formBuild.group({
      message: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.loadMessageThread();
    this.createMessageForm();
  }

  loadMessageThread() {
    const userId = +this.authService.decodedToken.nameid;
    this.userService
      .getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(
        tap((messages: any) => {
          for (let i = 0; i < messages.length; i++) {
            if (
              messages[i].isRead === false &&
              messages[i].recipientId === userId
            ) {
              this.userService.markMessageRead(userId, messages[i].id);
            }
          }
        })
      )
      .subscribe((res) => {
        this.messages = res;
      });
  }

  sendMessage() {
    if (this.messageForm.invalid) {
      return;
    }

    this.newMessage.content = this.messageForm.value.message;
    this.newMessage.recipientId = this.recipientId;

    this.userService
      .sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe(
        (message: Message) => {
          this.messages.unshift(message);
          this.newMessage.content = '';
          this.messageForm.reset();
        },
        (error) => this.alertify.error(error)
      );
  }
}
