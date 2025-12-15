import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { BillService, FinancialYear } from '../../services/bill.service';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-report',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        CardModule,
        ButtonModule,
        DropdownModule
    ],
    templateUrl: './report.component.html',
    styleUrls: ['./report.component.scss']
})
export class ReportComponent implements OnInit {
    financialYears: FinancialYear[] = [];
    selectedFinancialYear: number = 1;
    loading: boolean = false;

    constructor(
        private reportService: ReportService,
        private billService: BillService
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

    async downloadExcel(): Promise<void> {
        this.loading = true;
        this.reportService.downloadExcelReport(this.selectedFinancialYear).subscribe({
            next: async (blob) => {
                const fyText = this.financialYears.find(f => f.Id === this.selectedFinancialYear)?.FinancialYear || 'Report';
                this.reportService.downloadFile(blob, `BillReport_${fyText}_${new Date().toISOString().split('T')[0]}.xlsx`);
                this.loading = false;
                await Swal.fire({
                    icon: 'success',
                    title: 'Success!',
                    text: 'Excel report downloaded successfully',
                    confirmButtonColor: '#A5D6A7',
                    timer: 2000
                });
            },
            error: async () => {
                this.loading = false;
                await Swal.fire({
                    icon: 'error',
                    title: 'Download Failed',
                    text: 'Failed to download Excel report',
                    confirmButtonColor: '#EF9A9A'
                });
            }
        });
    }

    async downloadPdf(): Promise<void> {
        this.loading = true;
        this.reportService.downloadPdfReport(this.selectedFinancialYear).subscribe({
            next: async (blob) => {
                const fyText = this.financialYears.find(f => f.Id === this.selectedFinancialYear)?.FinancialYear || 'Report';
                this.reportService.downloadFile(blob, `BillReport_${fyText}_${new Date().toISOString().split('T')[0]}.pdf`);
                this.loading = false;
                await Swal.fire({
                    icon: 'success',
                    title: 'Success!',
                    text: 'PDF report downloaded successfully',
                    confirmButtonColor: '#A5D6A7',
                    timer: 2000
                });
            },
            error: async () => {
                this.loading = false;
                await Swal.fire({
                    icon: 'error',
                    title: 'Download Failed',
                    text: 'Failed to download PDF report',
                    confirmButtonColor: '#EF9A9A'
                });
            }
        });
    }
}
