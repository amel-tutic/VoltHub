import { ChangeDetectionStrategy, Component, effect, inject, signal, untracked } from '@angular/core';
import { FormField, form, maxLength, minLength, pattern, required, submit, validate } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { AuthService } from '../../core/auth/auth.service';
import { AccountService } from '../../core/services/account.service';
import { NotifyService } from '../../core/ui/notify.service';

const EMPTY_PASSWORDS = { currentPassword: '', newPassword: '', confirmPassword: '' };

// SSA 1.3: every role edits its own name and changes its password.
@Component({
  selector: 'app-profile',
  imports: [FormField, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Profile {
  protected readonly auth = inject(AuthService);
  private readonly accountService = inject(AccountService);
  private readonly notify = inject(NotifyService);

  // ---- Name
  protected readonly profileModel = signal({ firstName: '', lastName: '' });
  protected readonly profileForm = form(this.profileModel, path => {
    required(path.firstName, { message: 'First name is required.' });
    required(path.lastName, { message: 'Last name is required.' });
    maxLength(path.firstName, 100, { message: 'At most 100 characters.' });
    maxLength(path.lastName, 100, { message: 'At most 100 characters.' });
  });
  protected readonly profileError = signal<string | null>(null);

  // ---- Password
  protected readonly passwordModel = signal({ ...EMPTY_PASSWORDS });
  protected readonly passwordForm = form(this.passwordModel, path => {
    required(path.currentPassword, { message: 'Enter your current password.' });
    required(path.newPassword, { message: 'Choose a new password.' });
    minLength(path.newPassword, 8, { message: 'At least 8 characters.' });
    pattern(path.newPassword, /[A-Za-z]/, { message: 'Must contain a letter.' });
    pattern(path.newPassword, /[0-9]/, { message: 'Must contain a digit.' });
    // A rule that compares two fields: valueOf() reads another field of the same form.
    validate(path.confirmPassword, ({ value, valueOf }) =>
      value() === valueOf(path.newPassword) ? null : { kind: 'mismatch', message: 'The passwords do not match.' });
  });
  protected readonly passwordError = signal<string | null>(null);

  constructor() {
    // Fill the name form as soon as the profile is known (after a page reload it is still loading).
    effect(() => {
      const profile = this.auth.profile();
      if (profile) {
        untracked(() => this.profileModel.set({ firstName: profile.firstName, lastName: profile.lastName }));
      }
    });
  }

  protected saveProfile(event: Event): void {
    event.preventDefault();
    this.profileError.set(null);
    void submit(this.profileForm, async () => {
      const { firstName, lastName } = this.profileModel();
      try {
        const profile = await firstValueFrom(this.accountService.updateProfile(firstName, lastName));
        this.auth.profile.set(profile);   // the toolbar shows the new name right away
        this.notify.success('Profile saved.');
      } catch (error) {
        this.profileError.set(apiErrorMessage(error));
      }
      return undefined;
    });
  }

  protected changePassword(event: Event): void {
    event.preventDefault();
    this.passwordError.set(null);
    void submit(this.passwordForm, async () => {
      const { currentPassword, newPassword } = this.passwordModel();
      try {
        const tokens = await firstValueFrom(this.accountService.changePassword(currentPassword, newPassword));
        this.auth.applyTokens(tokens);   // other devices are signed out; this one keeps working
        this.passwordModel.set({ ...EMPTY_PASSWORDS });
        this.passwordForm().reset();
        this.notify.success('Password changed. Your other devices have been signed out.');
      } catch (error) {
        this.passwordError.set(apiErrorMessage(error));   // e.g. "The current password is not correct."
      }
      return undefined;
    });
  }
}
