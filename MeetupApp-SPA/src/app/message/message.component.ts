import { AlertifyService } from './../_services/alertify.service';
import { PaginationResult } from './../_models/pagination';
import { AuthService } from './../_services/auth.service';
import { UserService } from './../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css'],
})
export class MessageComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.messages = data.messages.result;
      this.pagination = data.messages.pagination;
    });
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.pagination.itemsPerPage = this.pagination.itemsPerPage;
    this.loadMessages();
  }

  loadMessages() {
    const userId = +this.authService.decodedToken.nameid;
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PaginationResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  deleteMessage(id) {
    this.alertify.confirm('Are you sure want to delete the message?', () => {
      this.userService
        .deleteMessage(id, this.authService.decodedToken.nameid)
        .subscribe((res) => {
          const messageIndex = this.messages.findIndex((m) => m.id === id);
          this.messages.splice(messageIndex, 1);
          this.alertify.success('Message has been deleted successfully.');
        });
    });
  }
}
