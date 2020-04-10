import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { environment } from './../../../environments/environment';
import { Photo } from './../../_models/photo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.css'],
})
export class PhotoEditComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter();
  currentMainPhoto: Photo;

  baseUrl = environment.apiUrl;
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertifyService: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      maxFileSize: 10 * 1024 * 1024, // 10MB
      autoUpload: false,
      removeAfterUpload: true,
    });

    /* Fix fileupload adding crendential issue
          Access to XMLHttpRequest at 'http://localhost:5000/api/users/1/photos'
          from origin 'http://localhost:4200' has been blocked by CORS policy:
          Response to preflight request doesn't pass access control check:
          The value of the 'Access-Control-Allow-Origin' header in the response must not be the wildcard '*'
          when the request's credentials mode is 'include'.
          The credentials mode of requests initiated by the XMLHttpRequest is controlled by the withCredentials attribute.
    */
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    /* After upload success show the uploaded photo on the page */
    this.uploader.onSuccessItem = (item, response, status) => {
      const res = JSON.parse(response);
      const photo: Photo = {
        id: res.id,
        url: res.url,
        dateAdded: res.dateAdded,
        description: res.description,
        isMain: res.isMain,
      };

      this.photos.push(photo);
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.currentMainPhoto = this.photos.filter(
            (p) => p.isMain === true
          )[0];
          this.currentMainPhoto.isMain = false;
          photo.isMain = true;
          this.authService.changePhotoUrl(photo.url);
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  deletePhoto(photoId) {
    this.alertifyService.confirm('Are you sure to delete the photo?', () => {
      this.userService
        .deletePhoto(this.authService.decodedToken.nameid, photoId)
        .subscribe(
          () => {
            this.photos.splice(
              this.photos.findIndex((p) => p.id === photoId),
              1
            );
            this.alertifyService.success('The photo has been deleted.');
          },
          (error) => {
            this.alertifyService.error(error);
          }
        );
    });
  }
}
