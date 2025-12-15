import { Routes } from '@angular/router';
import { BillListComponent } from './components/bill-list/bill-list.component';
import { BillCreateComponent } from './components/bill-create/bill-create.component';
import { BillDetailsComponent } from './components/bill-details/bill-details.component';
import { ReportComponent } from './components/report/report.component';

export const routes: Routes = [
    { path: '', redirectTo: 'bills', pathMatch: 'full' },
    { path: 'bills', component: BillListComponent },
    { path: 'bills/create', component: BillCreateComponent },
    { path: 'bills/:id', component: BillDetailsComponent },
    { path: 'reports', component: ReportComponent },
    { path: '**', redirectTo: 'bills' }
];
