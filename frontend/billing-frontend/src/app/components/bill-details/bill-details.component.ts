import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { BillService, BillDetails } from '../../services/bill.service';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';

@Component({
    selector: 'app-bill-details',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        CardModule,
        ButtonModule,
        TableModule,
        TabViewModule
    ],
    templateUrl: './bill-details.component.html',
    styleUrls: ['./bill-details.component.scss']
})
export class BillDetailsComponent implements OnInit {
    bill: BillDetails | null = null;
    loading: boolean = true;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private billService: BillService
    ) { }

    ngOnInit(): void {
        const billId = Number(this.route.snapshot.paramMap.get('id'));
        if (billId) {
            this.loadBill(billId);
        }
    }

    loadBill(billId: number): void {
        this.billService.getBillDetails(billId).subscribe({
            next: (response) => {
                if (response.ApiResponseStatus === 'Success' && response.Result) {
                    this.bill = response.Result;
                } else {
                    console.error('Bill not found:', response.Message);
                    this.router.navigate(['/bills']);
                }
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading bill:', error);
                this.loading = false;
                this.router.navigate(['/bills']);
            }
        });
    }

    goBack(): void {
        this.router.navigate(['/bills']);
    }
}
