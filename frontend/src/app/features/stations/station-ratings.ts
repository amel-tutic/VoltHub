import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject, input, output, signal } from '@angular/core';
import { FormField, form, max, maxLength, min, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { StationRatings } from '../../core/api/models';
import { AuthService } from '../../core/auth/auth.service';
import { FeedbackService } from '../../core/services/feedback.service';
import { NotifyService } from '../../core/ui/notify.service';

// SSA 8.3: everyone sees a station's ratings; an EV owner can rate it once and change the rating later.
// A child component: the station page passes the station id in, and hears back when a rating was saved.
@Component({
  selector: 'app-station-ratings',
  imports: [FormField, DatePipe, DecimalPipe, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './station-ratings.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StationRatingsPanel {
  readonly stationId = input.required<string>();
  readonly rated = output<void>();

  protected readonly auth = inject(AuthService);
  private readonly feedbackService = inject(FeedbackService);
  private readonly notify = inject(NotifyService);

  protected readonly scores = [5, 4, 3, 2, 1];
  protected readonly ratings = signal<StationRatings | null>(null);

  protected readonly model = signal({ score: 5, comment: '' });
  protected readonly ratingForm = form(this.model, path => {
    min(path.score, 1, { message: 'Choose 1 to 5 stars.' });
    max(path.score, 5, { message: 'Choose 1 to 5 stars.' });
    maxLength(path.comment, 1000, { message: 'At most 1000 characters.' });
  });
  protected readonly serverError = signal<string | null>(null);

  constructor() {
    effect(() => this.load(this.stationId()));   // reload when the station changes
  }

  protected stars(score: number): string {
    return '★'.repeat(score) + '☆'.repeat(5 - score);
  }

  protected rounded(average: number): number {
    return Math.round(average);
  }

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.ratingForm, async () => {
      const { score, comment } = this.model();
      try {
        await firstValueFrom(this.feedbackService.rate(this.stationId(), score, comment));
        this.notify.success('Thanks for rating this station.');
        this.load(this.stationId());
        this.rated.emit();   // the station page refreshes its average
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }

  private load(stationId: string): void {
    this.feedbackService.ratings(stationId).subscribe({
      next: ratings => {
        this.ratings.set(ratings);
        if (ratings.mine) {
          this.model.set({ score: ratings.mine.score, comment: ratings.mine.comment ?? '' });   // edit, don't start over
        }
      },
      error: error => this.notify.error(error)
    });
  }
}
