import { MessageResolver } from './_resolvers/message.resolver';
import { ListResolver } from './_resolvers/list.resolver';
import { MemberEditComponent } from './member/member-edit/member-edit.component';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessageComponent } from './message/message.component';
import { MemberListComponent } from './member/member-list/member-list.component';
import { HomeComponent } from './home/home.component';

import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';

import { Routes } from '@angular/router';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChangesGuard } from './_guards/preventUnsavedChanges.guard';

export const appRoute: Routes = [
  { path: '', component: HomeComponent },
  {
    // path:'dummy' => access members become localhost:4200/dummymembers
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'members',
        component: MemberListComponent,
        resolve: { users: MemberListResolver },
      },
      {
        path: 'members/:id',
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver },
      },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        canDeactivate: [PreventUnsavedChangesGuard],
        resolve: { user: MemberEditResolver },
      },
      {
        path: 'messages',
        component: MessageComponent,
        resolve: { messages: MessageResolver },
      },
      {
        path: 'lists',
        component: ListsComponent,
        resolve: { users: ListResolver },
      },
    ],
  },
  { path: '**', redirectTo: '', pathMatch: 'full' },
];
