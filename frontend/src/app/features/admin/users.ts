import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormField, email, form, minLength, pattern, required, submit } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { AdminUser, Role } from '../../core/api/models';
import { AuthService } from '../../core/auth/auth.service';
import { NewOperator, UserFilters, UsersService } from '../../core/services/users.service';
import { NotifyService } from '../../core/ui/notify.service';
import { StatusChip } from '../../core/ui/status-chip';

const EMPTY_OPERATOR: NewOperator = { firstName: '', lastName: '', email: '', password: '' };

// SSA 1.5: the administrator lists accounts, creates operators, and activates or deactivates accounts.
@Component({
  selector: 'app-users',
  imports: [FormField, DatePipe, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule,
            MatIconModule, StatusChip],
  templateUrl: './users.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Users {
  private readonly usersService = inject(UsersService);
  private readonly notify = inject(NotifyService);
  protected readonly auth = inject(AuthService);

  protected readonly roles: Role[] = ['User', 'Operator', 'Admin'];
  protected readonly filters = signal<UserFilters>({ role: '', search: '' });
  protected readonly users = signal<AdminUser[]>([]);
  protected readonly activeCount = computed(() => this.users().filter(u => u.isActive).length);

  protected readonly operatorModel = signal<NewOperator>({ ...EMPTY_OPERATOR });
  protected readonly operatorForm = form(this.operatorModel, path => {
    required(path.firstName, { message: 'First name is required.' });
    required(path.lastName, { message: 'Last name is required.' });
    required(path.email, { message: 'Email is required.' });
    email(path.email, { message: 'Enter a valid email address.' });
    required(path.password, { message: 'Choose a starting password.' });
    minLength(path.password, 8, { message: 'At least 8 characters.' });
    pattern(path.password, /[A-Za-z]/, { message: 'Must contain a letter.' });
    pattern(path.password, /[0-9]/, { message: 'Must contain a digit.' });
  });
  protected readonly serverError = signal<string | null>(null);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.usersService.list(this.filters()).subscribe({
      next: users => this.users.set(users),
      error: error => this.notify.error(error)
    });
  }

  protected setRole(role: Role | ''): void {
    this.filters.update(f => ({ ...f, role }));
    this.load();
  }

  protected setSearch(search: string): void {
    this.filters.update(f => ({ ...f, search }));
    this.load();
  }

  protected roleLabel(role: Role): string {
    return role === 'User' ? 'EV owner' : role;
  }

  protected isMe(user: AdminUser): boolean {
    return user.id === this.auth.profile()?.id;
  }

  protected toggleActive(user: AdminUser): void {
    const activate = !user.isActive;
    if (!activate && !confirm(`Deactivate ${user.firstName} ${user.lastName}? They are signed out and can't log in until reactivated.`)) {
      return;
    }
    this.usersService.setActive(user.id, activate).subscribe({
      next: () => {
        this.notify.success(`${user.firstName} ${user.lastName} ${activate ? 'activated' : 'deactivated'}.`);
        this.load();
      },
      error: error => this.notify.error(error)
    });
  }

  protected createOperator(event: Event): void {
    event.preventDefault();
    this.serverError.set(null);
    void submit(this.operatorForm, async () => {
      try {
        await firstValueFrom(this.usersService.createOperator(this.operatorModel()));
        this.notify.success('Operator account created.');
        this.operatorModel.set({ ...EMPTY_OPERATOR });
        this.operatorForm().reset();
        this.load();
      } catch (error) {
        this.serverError.set(apiErrorMessage(error));   // e.g. 409: the email is already used
      }
      return undefined;
    });
  }
}
