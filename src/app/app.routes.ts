import { Routes } from '@angular/router';

import { Login } from './login/login';
import {Dashboard} from './dashboard/dashboard';
import { Home } from './home/home';
import { Bungalows } from './bungalows/bungalows';

export const routes: Routes = [

  {
    path: '',
    component: Login
  },

  { 
    path: 'home',
    component: Dashboard 
  },  

  {
    path: 'profile',
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