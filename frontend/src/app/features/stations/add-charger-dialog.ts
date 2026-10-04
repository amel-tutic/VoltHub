import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, form, max, maxLength, min, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { CONNECTOR_TYPES, CURRENT_TYPES, ChargerInput } from '../../core/api/models';
import { StationsService } from '../../core/services/stations.service';

export interface AddChargerDialogData { stationId: string; stationName: string; }

// SSA 6.2: an operator adds a charger to a station. A new charger starts as Available.
@Component({
  selector: 'app-add-charger-dialog',
  imports: [FormField, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './add-charger-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AddChargerDialog {
  protected readonly data = inject<AddChargerDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<AddChargerDialog, boolean>);
  private readonly stationsService = inject(StationsService);

  protected readonly connectorTypes = CONNECTOR_TYPES;
  protected readonly currentTypes = CURRENT_TYPES;

  protected readonly model = signal<ChargerInput>({ code: '', connectorType: 'CCS', currentType: 'DC', powerKw: 50, pricePerKwh: 35 });
  protected readonly chargerForm = form(this.model, path => {
    required(path.code, { message: 'Give the charger a code, for example A4.' });
    maxLength(path.code, 50, { message: 'At most 50 characters.' });
    min(path.powerKw, 1, { message: 'Power must be at least 1 kW.' });
    max(path.powerKw, 1000, { message: 'Power looks too large.' });
    min(path.pricePerKwh, 0, { message: 'The price cannot be negative.' });
    max(path.pricePerKwh, 1000, { message: 'At most 1000 RSD/kWh.' });
  });
  protected readonly serverError = signal<string | null>(null);

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.chargerForm, async () => {
      try {
        await firstValueFrom(this.stationsService.addCharger(this.data.stationId, this.model()));
        this.dialogRef.close(true);
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));   // e.g. 409: that code already exists at this station
      }
      return undefined;
    });
  }
}
