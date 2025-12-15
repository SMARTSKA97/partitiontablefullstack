import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface BillCreateRequest {
    BillDate: string;
    TrMasterId: number;
    PaymentMode: number;
    Demand?: string;
    MajorHead?: string;
    SubMajorHead?: string;
    MinorHead?: string;
    GrossAmount: number;
    NetAmount: number;
    BtAmount: number;
    DdoCode?: string;
    TreasuryCode?: string;
    Status: number;
    IsGst: boolean;
    GstAmount?: number;
    BtDetails: BtDetail[];
    GstDetails: GstDetail[];
    EcsDetails: EcsDetail[];
    AllotmentDetails: AllotmentDetail[];
    SubvoucherDetails: SubvoucherDetail[];
}

export interface BtDetail {
    BtSerial?: number;
    Amount: number;
    DdoCode?: string;
}

export interface GstDetail {
    CpinId?: number;
    DdoGstn?: string;
}

export interface EcsDetail {
    PayeeName?: string;
    BankAccountNumber?: string;
    IfscCode?: string;
    Amount: number;
}

export interface AllotmentDetail {
    AllotmentId?: number;
    Amount: number;
    ActiveHoaId: number;
}

export interface SubvoucherDetail {
    SubvoucherNo?: string;
    SubvoucherAmount?: number;
}

export interface BillListResponse {
    Bills: BillListItem[];
    TotalCount: number;
    Page: number;
    PageSize: number;
}

export interface BillListItem {
    BillId: number;
    BillNo?: string;
    BillDate: string;
    DdoCode?: string;
    GrossAmount?: number;
    NetAmount?: number;
    Status: number;
    Remarks?: string;
    FinancialYear: number;
}

export interface BillDetails {
    BillId: number;
    BillNo?: string;
    BillDate: string;
    GrossAmount?: number;
    NetAmount?: number;
    BtAmount?: number;
    IsGst: boolean;
    GstAmount?: number;
    BtDetails: BtDetail[];
    GstDetails: GstDetail[];
    EcsDetails: EcsDetail[];
    AllotmentDetails: AllotmentDetail[];
    SubvoucherDetails: SubvoucherDetail[];
    // ... other fields
}

export interface FinancialYear {
    Id: number;
    FinancialYear: string;
    IsActive: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class BillService {
    private baseUrl = `${environment.apiUrl}/bills`;

    constructor(private http: HttpClient) { }

    createBill(request: BillCreateRequest): Observable<any> {
        return this.http.post(`${this.baseUrl}`, request);
    }

    getBills(financialYear: number, search?: string, page: number = 1, pageSize: number = 20): Observable<BillListResponse> {
        let params = new HttpParams()
            .set('financialYear', financialYear.toString())
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }

        return this.http.get<BillListResponse>(this.baseUrl, { params });
    }

    getBillDetails(billId: number): Observable<BillDetails> {
        return this.http.get<BillDetails>(`${this.baseUrl}/${billId}`);
    }

    getFinancialYears(): Observable<FinancialYear[]> {
        return this.http.get<FinancialYear[]>(`${this.baseUrl}/financial-years`);
    }
}
