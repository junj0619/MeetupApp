import { UserService } from './../_services/user.service';
import { UserParams } from 'src/app/_models/userParams';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { Pagination, PaginationResult } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private userService: UserService
  ) {}
  users: User[];
  userParams: UserParams;
  pagination: Pagination;
  likesParam = 'Likers';

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.users = data.users.result;
      this.pagination = data.users.pagination;
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.pagination.itemsPerPage = this.pagination.itemsPerPage;
    this.loadUsers();
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.userParams,
        this.likesParam
      )
      .subscribe((response: PaginationResult<User[]>) => {
        this.users = response.result;
        this.pagination = response.pagination;
      });
  }
}
