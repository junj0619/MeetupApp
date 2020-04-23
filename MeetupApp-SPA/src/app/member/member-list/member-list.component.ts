import { PaginationResult, Pagination } from './../../_models/pagination';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserParams } from 'src/app/_models/userParams';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  userParams: any = {};

  pagination: Pagination;
  filterForm: FormGroup;
  genders: any[];

  minAge = 18;
  maxAge = 99;
  gender = this.user.gender === 'male' ? 'female' : 'male';
  orderBy = 'lastActive';

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.users = data.users.result;
      this.pagination = data.users.pagination;
    });

    this.userParams.minAge = this.minAge;
    this.userParams.maxAge = this.maxAge;
    this.userParams.gender = this.gender;
    this.userParams.orderBy = this.orderBy;

    this.genders = [
      { value: 'male', display: 'Males' },
      { value: 'female', display: 'Females' },
    ];

    this.filterForm = this.formBuilder.group({
      minAge: [this.userParams.minAge],
      maxAge: [this.userParams.maxAge],
      gender: [this.userParams.gender],
      orderBy: [this.userParams.orderBy],
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.pagination.itemsPerPage = this.pagination.itemsPerPage;
    this.loadUsers();
  }

  resetFilter() {
    this.filterForm.reset({
      gender: this.gender,
      minAge: this.minAge,
      maxAge: this.maxAge,
      orderBy: this.orderBy,
    });
    this.userParams.minAge = this.minAge;
    this.userParams.maxAge = this.maxAge;
    this.userParams.gender = this.gender;
    this.userParams.orderBy = this.orderBy;

    this.loadUsers();
  }

  applyFilter() {
    this.userParams = Object.assign({}, this.filterForm.value);
    this.loadUsers();
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.userParams
      )
      .subscribe((response: PaginationResult<User[]>) => {
        this.users = response.result;
        this.pagination = response.pagination;
      });
  }
}
