import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// API Response wrapper
export interface ServiceResponse<T> {
    Result: T | null;
    ApiResponseStatus: 'Success' | 'Error' | 'ValidationError' | 'NotFound' | 'Unauthorized';
    Message: string;
    ValidationResults: any[];
}

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
    Remarks?: string;
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

export interface BillCreateResponse {
    Success: boolean;
    BillId?: number;
    BillNo?: string;
    Error?: string;
}

export interface PaginatedResponse<T> {
    TotalCount: number;
    PageNumber: number;
    PageSize: number;
    Data: T;
    MetaData: any;
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
    DdoCode?: string;
    TreasuryCode?: string;
    Remarks?: string;
    BtDetails: BtDetail[];
    GstDetails: GstDetail[];
    EcsDetails: EcsDetail[];
    AllotmentDetails: AllotmentDetail[];
    SubvoucherDetails: SubvoucherDetail[];
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

    createBill(request: BillCreateRequest): Observable<ServiceResponse<BillCreateResponse>> {
        return this.http.post<ServiceResponse<BillCreateResponse>>(`${this.baseUrl}`, request);
    }

    getBills(financialYear: number, search?: string, page: number = 1, pageSize: number = 20): Observable<ServiceResponse<PaginatedResponse<BillListItem[]>>> {
        // Use QueryParameters structure for new backend
        const queryParams = {
            globalSearch: search || '',
            filters: [],
            sorts: [{ field: 'CreatedAt', order: 'desc' }],
            pageNumber: page,
            pageSize: pageSize
        };

        return this.http.post<ServiceResponse<PaginatedResponse<BillListItem[]>>>(
            `${this.baseUrl}/query?financialYear=${financialYear}`,
            queryParams
        );
    }

    getBillDetails(billId: number): Observable<ServiceResponse<BillDetails>> {
        return this.http.get<ServiceResponse<BillDetails>>(`${this.baseUrl}/${billId}`);
    }

    getFinancialYears(): Observable<ServiceResponse<FinancialYear[]>> {
        return this.http.get<ServiceResponse<FinancialYear[]>>(`${this.baseUrl}/financial-years`);
    }
}
