import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, email, form, minLength, pattern, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormField, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './register.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Register {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly serverError = signal<string | null>(null);
  protected readonly model = signal({ firstName: '', lastName: '', email: '', password: '' });

  // The same rules the API enforces, repeated here only for instant feedback.
  protected readonly registerForm = form(this.model, path => {
    required(path.firstName, { message: 'First name is required.' });
    required(path.lastName, { message: 'Last name is required.' });
    required(path.email, { message: 'Email is required.' });
    email(path.email, { message: 'Enter a valid email address.' });
    required(path.password, { message: 'Password is required.' });
    minLength(path.password, 8, { message: 'At least 8 characters.' });
    pattern(path.password, /(?=.*[A-Za-z])(?=.*[0-9])/, { message: 'Use at least one letter and one digit.' });
  });

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.registerForm, async () => {
      try {
        const { firstName, lastName, email, password } = this.model();
        await firstValueFrom(this.auth.register(firstName, lastName, email, password));
        await this.router.navigateByUrl('/vehicles');   // a new owner's first step: add a vehicle
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }
}
