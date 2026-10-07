import { Routes } from '@angular/router';

import { Login } from './login/login';
import { Home } from './home/home';
import { Bungalows } from './bungalows/bungalows';

export const routes: Routes = [

  {
    path: '',
    component: Login
  },

  {
    path: 'home',
    component: Home
  },

  {
    path: 'bungalows',
    component: Bungalows
  },

  {
    path: '**',
    redirectTo: ''
  }

];