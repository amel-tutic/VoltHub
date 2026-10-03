import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, email, form, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormField, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './login.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly serverError = signal<string | null>(null);
  protected readonly model = signal({ email: '', password: '' });

  // Signal Forms: the model is a signal; the schema lists the rules for each field.
  protected readonly loginForm = form(this.model, path => {
    required(path.email, { message: 'Email is required.' });
    email(path.email, { message: 'Enter a valid email address.' });
    required(path.password, { message: 'Password is required.' });
  });

  protected onSubmit(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    // submit() marks every field touched and only runs the action when the form is valid.
    void submit(this.loginForm, async () => {
      try {
        const { email, password } = this.model();
        await firstValueFrom(this.auth.login(email, password));
        await this.router.navigateByUrl('/stations');
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }
}
