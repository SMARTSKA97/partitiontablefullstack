import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { BillService, BillCreateRequest } from '../../services/bill.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { CardModule } from 'primeng/card';
import { TooltipModule } from 'primeng/tooltip';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-bill-create',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterModule,
        ButtonModule,
        InputTextModule,
        InputTextareaModule,
        InputNumberModule,
        CalendarModule,
        CheckboxModule,
        CardModule,
        TooltipModule
    ],
    templateUrl: './bill-create.component.html',
    styleUrls: ['./bill-create.component.scss']
})
export class BillCreateComponent implements OnInit {
    billForm!: FormGroup;
    submitted = false;

    constructor(
        private fb: FormBuilder,
        private billService: BillService,
        private router: Router
    ) { }

    ngOnInit(): void {
        this.initForm();
    }

    initForm(): void {
        this.billForm = this.fb.group({
            BillDate: [new Date(), [Validators.required, this.futureDateValidator]],
            TrMasterId: [1, [Validators.required, Validators.min(1)]],
            PaymentMode: [1, Validators.required],
            Demand: ['', Validators.maxLength(100)],
            MajorHead: ['', Validators.maxLength(50)],
            SubMajorHead: ['', Validators.maxLength(50)],
            MinorHead: ['', Validators.maxLength(50)],
            GrossAmount: [0, [Validators.required, Validators.min(1), Validators.max(99999999999.99)]],
            NetAmount: [0, [Validators.required, Validators.min(1), Validators.max(99999999999.99)]],
            BtAmount: [0, [Validators.min(0), Validators.max(99999999999.99)]],
            DdoCode: ['', [Validators.required, Validators.maxLength(50)]],
            TreasuryCode: ['', [Validators.required, Validators.maxLength(50)]],
            Status: [1],
            IsGst: [false],
            GstAmount: [0, [Validators.min(0), Validators.max(99999999999.99)]],
            Remarks: ['', Validators.maxLength(500)],
            BtDetails: this.fb.array([this.createBtDetail()], Validators.required),
            GstDetails: this.fb.array([]),
            EcsDetails: this.fb.array([this.createEcsDetail()], Validators.required),
            AllotmentDetails: this.fb.array([this.createAllotmentDetail()], Validators.required),
            SubvoucherDetails: this.fb.array([])
        }, { validators: [this.grossAmountValidator, this.btDetailsSumValidator, this.ecsAmountValidator, this.allotmentAmountValidator] });
    }

    futureDateValidator(control: AbstractControl): ValidationErrors | null {
        if (!control.value) return null;
        const selectedDate = new Date(control.value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return selectedDate > today ? { futureDate: true } : null;
    }

    grossAmountValidator(group: AbstractControl): ValidationErrors | null {
        const netAmount = group.get('NetAmount')?.value || 0;
        const btAmount = group.get('BtAmount')?.value || 0;
        const gstAmount = group.get('GstAmount')?.value || 0;
        const grossAmount = group.get('GrossAmount')?.value || 0;
        const isGst = group.get('IsGst')?.value || false;

        const expectedGross = isGst ? netAmount + btAmount + gstAmount : netAmount + btAmount;
        const difference = Math.abs(grossAmount - expectedGross);

        return difference > 0.01 ? { grossAmountIncorrect: { expected: expectedGross, actual: grossAmount, isGst } } : null;
    }

    btDetailsSumValidator(group: AbstractControl): ValidationErrors | null {
        const btAmount = group.get('BtAmount')?.value || 0;
        const btDetails = (group.get('BtDetails') as FormArray)?.value || [];
        const totalBtDetails = btDetails.reduce((sum: number, bt: any) => sum + (bt.Amount || 0), 0);
        const difference = Math.abs(totalBtDetails - btAmount);

        return difference > 0.01 ? { btDetailsSumMismatch: { total: totalBtDetails, expected: btAmount } } : null;
    }

    ecsAmountValidator(group: AbstractControl): ValidationErrors | null {
        const netAmount = group.get('NetAmount')?.value || 0;
        const ecsDetails = (group.get('EcsDetails') as FormArray)?.value || [];
        const totalEcsAmount = ecsDetails.reduce((sum: number, ecs: any) => sum + (ecs.Amount || 0), 0);
        const difference = Math.abs(totalEcsAmount - netAmount);

        return difference > 0.01 ? { ecsAmountMismatch: { totalEcs: totalEcsAmount, netAmount } } : null;
    }

    allotmentAmountValidator(group: AbstractControl): ValidationErrors | null {
        const grossAmount = group.get('GrossAmount')?.value || 0;
        const allotmentDetails = (group.get('AllotmentDetails') as FormArray)?.value || [];
        const totalAllotment = allotmentDetails.reduce((sum: number, allot: any) => sum + (allot.Amount || 0), 0);

        return totalAllotment < grossAmount - 0.01 ? { allotmentInsufficient: { total: totalAllotment, required: grossAmount } } : null;
    }

    ifscCodeValidator(control: AbstractControl): ValidationErrors | null {
        const pattern = /^[A-Z]{4}0[A-Z0-9]{6}$/;
        return control.value && !pattern.test(control.value) ? { invalidIfsc: true } : null;
    }

    gstinValidator(control: AbstractControl): ValidationErrors | null {
        if (!control.value) return null;
        const pattern = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$/;
        return !pattern.test(control.value) ? { invalidGstin: true } : null;
    }

    accountNumberValidator(control: AbstractControl): ValidationErrors | null {
        const pattern = /^\d{9,18}$/;
        return control.value && !pattern.test(control.value) ? { invalidAccount: true } : null;
    }

    createBtDetail(): FormGroup {
        return this.fb.group({
            BtSerial: [1],
            Amount: [0, [Validators.required, Validators.min(1)]],
            DdoCode: ['', Validators.maxLength(50)]
        });
    }

    createGstDetail(): FormGroup {
        return this.fb.group({
            CpinId: [null],
            DdoGstn: ['', this.gstinValidator]
        });
    }

    createEcsDetail(): FormGroup {
        return this.fb.group({
            PayeeName: ['', [Validators.required, Validators.maxLength(200)]],
            BankAccountNumber: ['', [Validators.required, this.accountNumberValidator]],
            IfscCode: ['', [Validators.required, this.ifscCodeValidator]],
            Amount: [0, [Validators.required, Validators.min(1)]]
        });
    }

    createAllotmentDetail(): FormGroup {
        return this.fb.group({
            AllotmentId: [null],
            Amount: [0, [Validators.required, Validators.min(1)]],
            ActiveHoaId: [1, [Validators.required, Validators.min(1)]]
        });
    }

    createSubvoucherDetail(): FormGroup {
        return this.fb.group({
            SubvoucherNo: ['', Validators.maxLength(50)],
            SubvoucherDate: [null],
            SubvoucherAmount: [0, Validators.min(0)],
            Description: ['', Validators.maxLength(500)]
        });
    }

    get btDetails(): FormArray {
        return this.billForm.get('BtDetails') as FormArray;
    }

    get gstDetails(): FormArray {
        return this.billForm.get('GstDetails') as FormArray;
    }

    get ecsDetails(): FormArray {
        return this.billForm.get('EcsDetails') as FormArray;
    }

    get allotmentDetails(): FormArray {
        return this.billForm.get('AllotmentDetails') as FormArray;
    }

    get subvoucherDetails(): FormArray {
        return this.billForm.get('SubvoucherDetails') as FormArray;
    }

    addBtDetail(): void {
        this.btDetails.push(this.createBtDetail());
    }

    addGstDetail(): void {
        this.gstDetails.push(this.createGstDetail());
    }

    addEcsDetail(): void {
        this.ecsDetails.push(this.createEcsDetail());
    }

    addAllotmentDetail(): void {
        this.allotmentDetails.push(this.createAllotmentDetail());
    }

    addSubvoucherDetail(): void {
        this.subvoucherDetails.push(this.createSubvoucherDetail());
    }

    removeBtDetail(index: number): void {
        if (this.btDetails.length > 1) {
            this.btDetails.removeAt(index);
        }
    }

    removeGstDetail(index: number): void {
        this.gstDetails.removeAt(index);
    }

    removeEcsDetail(index: number): void {
        if (this.ecsDetails.length > 1) {
            this.ecsDetails.removeAt(index);
        }
    }

    removeAllotmentDetail(index: number): void {
        if (this.allotmentDetails.length > 1) {
            this.allotmentDetails.removeAt(index);
        }
    }

    removeSubvoucherDetail(index: number): void {
        this.subvoucherDetails.removeAt(index);
    }

    async onSubmit(): Promise<void> {
        this.submitted = true;
        console.log('Form value:', this.billForm.value);
        console.log('Form valid:', this.billForm.valid);
        console.log('Form errors:', this.billForm.errors);
        console.log('Form controls with errors:', Object.keys(this.billForm.controls).filter(key => this.billForm.get(key)?.invalid));

        if (this.billForm.invalid) {
            const errors: string[] = [];

            if (this.billForm.errors?.['grossAmountIncorrect']) {
                const { expected, actual, isGst } = this.billForm.errors['grossAmountIncorrect'];
                const formula = isGst ? 'Gross = Net + BT + GST' : 'Gross = Net + BT';
                errors.push(`Gross amount incorrect. ${formula}. Expected: ₹${expected.toFixed(2)}, Actual: ₹${actual.toFixed(2)}`);
            }
            if (this.billForm.errors?.['btDetailsSumMismatch']) {
                const { total, expected } = this.billForm.errors['btDetailsSumMismatch'];
                errors.push(`Sum of BT details (₹${total.toFixed(2)}) must equal BT amount (₹${expected.toFixed(2)})`);
            }
            if (this.billForm.errors?.['ecsAmountMismatch']) {
                const { totalEcs, netAmount } = this.billForm.errors['ecsAmountMismatch'];
                errors.push(`Sum of ECS amounts (₹${totalEcs.toFixed(2)}) must equal net amount (₹${netAmount.toFixed(2)})`);
            }
            if (this.billForm.errors?.['allotmentInsufficient']) {
                const { total, required } = this.billForm.errors['allotmentInsufficient'];
                errors.push(`Total allotment (₹${total.toFixed(2)}) must be >= gross amount (₹${required.toFixed(2)})`);
            }

            // Collect field-level errors
            Object.keys(this.billForm.controls).forEach(key => {
                const control = this.billForm.get(key);
                if (control && control.invalid && control.errors) {
                    Object.keys(control.errors).forEach(errorKey => {
                        if (errorKey === 'required') {
                            errors.push(`${key} is required`);
                        } else if (errorKey === 'min') {
                            errors.push(`${key} must be at least ${control.errors![errorKey].min}`);
                        } else if (errorKey === 'max') {
                            errors.push(`${key} exceeds maximum value`);
                        } else if (errorKey === 'maxlength') {
                            errors.push(`${key} exceeds maximum length`);
                        }
                    });
                }
            });

            console.log('All validation errors:', errors);

            if (errors.length > 0) {
                await Swal.fire({
                    icon: 'warning',
                    title: 'Validation Errors',
                    html: `<div style="text-align: left; max-height: 400px; overflow-y: auto;"><ul class="mb-0">${errors.map(e => `<li class="mb-2">${e}</li>`).join('')}</ul></div>`,
                    confirmButtonColor: '#EF9A9A',
                    width: '600px'
                });
            } else {
                // Form is invalid but no specific errors captured - show generic message
                await Swal.fire({
                    icon: 'warning',
                    title: 'Invalid Form',
                    text: 'Please check all required fields and correct any errors.',
                    confirmButtonColor: '#EF9A9A'
                });
            }
            this.submitted = false;
            return;
        }

        const formValue = this.billForm.value;

        // Convert empty strings to null for fields that have foreign key constraints
        const cleanValue = {
            ...formValue,
            Demand: formValue.Demand || null,
            MajorHead: formValue.MajorHead || null,
            SubMajorHead: formValue.SubMajorHead || null,
            MinorHead: formValue.MinorHead || null,
            PlanStatus: formValue.PlanStatus || null,
            SchemeHead: formValue.SchemeHead || null,
            DetailHead: formValue.DetailHead || null,
            VotedCharged: formValue.VotedCharged || null,
            SanctionNo: formValue.SanctionNo || null,
            SanctionBy: formValue.SanctionBy || null,
            ReferenceNo: formValue.ReferenceNo || null,
            Remarks: formValue.Remarks || null
        };

        const request: BillCreateRequest = {
            ...cleanValue,
            BillDate: this.formatDate(cleanValue.BillDate),
            SubvoucherDetails: cleanValue.SubvoucherDetails.map((sv: any) => ({
                ...sv,
                SubvoucherDate: sv.SubvoucherDate ? this.formatDate(sv.SubvoucherDate) : null,
                Description: sv.Description || null
            })),
            BtDetails: cleanValue.BtDetails.map((bt: any) => ({
                ...bt,
                DdoCode: bt.DdoCode || null,
                TreasuryCode: bt.TreasuryCode || null
            })),
            EcsDetails: cleanValue.EcsDetails.map((ecs: any) => ({
                ...ecs,
                BeneficiaryId: ecs.BeneficiaryId || null,
                PanNo: ecs.PanNo || null,
                ContactNumber: ecs.ContactNumber || null,
                Address: ecs.Address || null,
                Email: ecs.Email || null,
                BankName: ecs.BankName || null
            })),
            AllotmentDetails: cleanValue.AllotmentDetails.map((allot: any) => ({
                ...allot,
                AllotmentId: allot.AllotmentId || null,
                DdoCode: allot.DdoCode || null,
                TreasuryCode: allot.TreasuryCode || null
            })),
            GstDetails: cleanValue.GstDetails.map((gst: any) => ({
                ...gst,
                DdoGstn: gst.DdoGstn || null,
                DdoCode: gst.DdoCode || null
            }))
        };
        console.log(request);


        this.billService.createBill(request).subscribe({
            next: async (response) => {
                console.log('API Response:', response);

                if (response.ApiResponseStatus === 'Success' && response.Result) {
                    // Check both PascalCase and snake_case for compatibility
                    const result: any = response.Result;
                    const isSuccess = result.Success === true || result.success === true;

                    if (isSuccess) {
                        const billNo = result.BillNo || result.bill_no || 'Unknown';

                        await Swal.fire({
                            icon: 'success',
                            title: 'Bill Created Successfully!',
                            html: `
                <div class="text-center">
                  <p class="mb-3">Your bill has been created and saved.</p>
                  <div class="p-3 surface-100 border-round">
                    <strong class="text-primary">Bill Number:</strong><br>
                    <span style="font-size: 1.5em; color: #9FA8DA; font-weight: bold;">${billNo.trim()}</span>
                  </div>
                </div>
              `,
                            confirmButtonColor: '#A5D6A7',
                            timer: 4000,
                            timerProgressBar: true
                        });
                        this.router.navigate(['/bills']);
                    } else {
                        await Swal.fire({
                            icon: 'error',
                            title: 'Bill Creation Failed',
                            text: response.Result.Error || 'Failed to create bill',
                            confirmButtonColor: '#EF9A9A'
                        });
                        this.submitted = false;
                    }
                } else if (response.ApiResponseStatus === 'ValidationError') {
                    const validationErrors = response.ValidationResults?.map((v: any) =>
                        `<li><strong>${v.Field}:</strong> ${v.Message}</li>`
                    ).join('') || '';

                    await Swal.fire({
                        icon: 'error',
                        title: 'Validation Errors',
                        html: `<div style="text-align: left;"><ul>${validationErrors}</ul></div>`,
                        confirmButtonColor: '#EF9A9A'
                    });
                    this.submitted = false;
                } else {
                    await Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.Message || 'Failed to create bill',
                        confirmButtonColor: '#EF9A9A'
                    });
                    this.submitted = false;
                }
            },
            error: async (error) => {
                console.error('Bill creation error:', error);
                await Swal.fire({
                    icon: 'error',
                    title: 'Server Error',
                    html: `<p>${error.error?.Message || error.error?.message || 'An error occurred'}</p>`,
                    confirmButtonColor: '#EF9A9A'
                });
                this.submitted = false;
            }
        });
    }

    formatDate(date: Date): string {
        if (!date) return '';
        const d = new Date(date);
        return d.toISOString().split('T')[0];
    }
}
