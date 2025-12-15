import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { BillService, BillListItem, FinancialYear } from '../../services/bill.service';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-bill-list',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        CardModule,
        ButtonModule,
        TableModule,
        InputTextModule,
        DropdownModule,
        TagModule,
        TooltipModule
    ],
    templateUrl: './bill-list.component.html',
    styleUrls: ['./bill-list.component.scss']
})
export class BillListComponent implements OnInit {
    bills: BillListItem[] = [];
    financialYears: FinancialYear[] = [];
    selectedFinancialYear: number = 1;
    searchText: string = '';
    loading: boolean = false;
    totalRecords: number = 0;
    page: number = 1;
    pageSize: number = 20;

    constructor(
        private billService: BillService,
        private router: Router
    ) { }

    ngOnInit(): void {
        this.loadFinancialYears();
    }

    loadFinancialYears(): void {
        this.billService.getFinancialYears().subscribe({
            next: (response) => {
                if (response.ApiResponseStatus === 'Success' && response.Result) {
                    this.financialYears = response.Result;
                    if (this.financialYears.length > 0) {
                        this.selectedFinancialYear = this.financialYears.find(f => f.IsActive)?.Id || this.financialYears[0].Id;
                        this.loadBills();
                    }
                }
            },
            error: async (error) => {
                console.error('Error loading financial years:', error);
                await Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Failed to load financial years',
                    confirmButtonColor: '#EF9A9A'
                });
            }
        });
    }

    loadBills(): void {
        this.loading = true;
        this.billService.getBills(this.selectedFinancialYear, this.searchText, this.page, this.pageSize).subscribe({
            next: (response) => {
                if (response.ApiResponseStatus === 'Success' && response.Result && response.Result.Data) {
                    this.bills = response.Result.Data;
                    this.totalRecords = response.Result.TotalCount;
                } else {
                    this.bills = [];
                    this.totalRecords = 0;
                }
                this.loading = false;
            },
            error: async (error) => {
                console.error('Error loading bills:', error);
                this.bills = [];
                this.totalRecords = 0;
                this.loading = false;
                await Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.error?.Message || 'Failed to load bills',
                    confirmButtonColor: '#EF9A9A'
                });
            }
        });
    }

    onFinancialYearChange(): void {
        this.page = 1;
        this.loadBills();
    }

    onSearch(): void {
        this.page = 1;
        this.loadBills();
    }

    onPageChange(event: any): void {
        this.page = event.first / event.rows + 1;
        this.pageSize = event.rows;
        this.loadBills();
    }

    viewDetails(billId: number): void {
        this.router.navigate(['/bills', billId]);
    }

    getStatusSeverity(status: number): "success" | "info" | "warning" | "danger" | "secondary" {
        switch (status) {
            case 1: return 'success';
            case 2: return 'warning';
            case 3: return 'danger';
            default: return 'info';
        }
    }

    getStatusLabel(status: number): string {
        switch (status) {
            case 1: return 'Active';
            case 2: return 'Pending';
            case 3: return 'Rejected';
            default: return 'Unknown';
        }
    }
}
