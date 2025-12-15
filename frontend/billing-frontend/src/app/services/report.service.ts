import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ReportService {
    private baseUrl = `${environment.apiUrl}/reports`;

    constructor(private http: HttpClient) { }

    downloadExcelReport(financialYear: number): Observable<Blob> {
        return this.http.get(`${this.baseUrl}/excel?financialYear=${financialYear}`, {
            responseType: 'blob'
        });
    }

    downloadPdfReport(financialYear: number): Observable<Blob> {
        return this.http.get(`${this.baseUrl}/pdf?financialYear=${financialYear}`, {
            responseType: 'blob'
        });
    }

    downloadFile(blob: Blob, filename: string): void {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        link.click();
        window.URL.revokeObjectURL(url);
    }
}
