import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, form, maxLength, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { Charger, PROBLEM_TYPES, PROBLEM_TYPE_LABELS, ProblemType } from '../../core/api/models';
import { FeedbackService } from '../../core/services/feedback.service';

export interface ReportProblemDialogData { charger: Charger; stationName: string; }

// SSA 8.4: a driver reports a problem with one charger. Operators are notified (SSA 7.4).
@Component({
  selector: 'app-report-problem-dialog',
  imports: [FormField, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './report-problem-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReportProblemDialog {
  protected readonly data = inject<ReportProblemDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<ReportProblemDialog, boolean>);
  private readonly feedbackService = inject(FeedbackService);

  protected readonly problemTypes = PROBLEM_TYPES;
  protected readonly typeLabels = PROBLEM_TYPE_LABELS;

  protected readonly model = signal<{ type: ProblemType; description: string }>({ type: 'ChargingStartFailure', description: '' });
  protected readonly reportForm = form(this.model, path => {
    required(path.description, { message: 'Tell the operators what is wrong.' });
    maxLength(path.description, 1000, { message: 'At most 1000 characters.' });
  });
  protected readonly serverError = signal<string | null>(null);

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.reportForm, async () => {
      const { type, description } = this.model();
      try {
        await firstValueFrom(this.feedbackService.reportProblem(this.data.charger.id, type, description));
        this.dialogRef.close(true);
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }
}
