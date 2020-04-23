import { UserParams } from './../_models/userParams';
import { map } from 'rxjs/operators';
import { PaginationResult } from './../_models/pagination';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Pagination } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  // httpOption = {
  //   headers: new HttpHeaders({
  //     Authorization: 'Bearer ' + localStorage.getItem('token'),
  //   }),
  // };

  baseUrl = environment.apiUrl + 'users/';
  constructor(private http: HttpClient) {}

  getUsers(
    currentPage?,
    itemsPerPage?,
    userParams?,
    likeParam?
  ): Observable<PaginationResult<User[]>> {
    const paginationResult = new PaginationResult<User[]>();

    let params = new HttpParams();
    if (currentPage != null && itemsPerPage != null) {
      params = params.append('pageNumber', currentPage);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('gender', userParams.gender);
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likeParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likeParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this.http
      .get<User[]>(this.baseUrl, { observe: 'response', params })
      .pipe(
        map((response) => {
          paginationResult.result = response.body;
          const pagination = response.headers.get('pagination');
          if (pagination != null) {
            paginationResult.pagination = JSON.parse(pagination);
          }
          return paginationResult;
        })
      );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + id, user);
  }

  setMainPhoto(userId: number, photoId: number) {
    return this.http.post(
      this.baseUrl + userId + '/photos/' + photoId + '/setmain',
      {}
    );
  }

  deletePhoto(userId: number, photoId: number) {
    return this.http.delete(this.baseUrl + userId + '/photos/' + photoId);
  }

  setUserLike(userId: number, recipientId: number) {
    return this.http.post(this.baseUrl + userId + '/like/' + recipientId, {});
  }
}
