import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { WritableSignal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { FieldTree } from '@angular/forms/signals';
import { provideRouter } from '@angular/router';
import { Profile } from './profile';

interface Passwords { currentPassword: string; newPassword: string; confirmPassword: string; }

// The form members are protected (for the template only), so the test reaches them through this shape.
interface ProfileInternals { passwordModel: WritableSignal<Passwords>; passwordForm: FieldTree<Passwords>; }

describe('Profile', () => {
  let profile: ProfileInternals;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [Profile],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
    profile = TestBed.createComponent(Profile).componentInstance as unknown as ProfileInternals;
  });

  it('flags a repeated password that does not match', () => {
    profile.passwordModel.set({ currentPassword: 'OldPass123', newPassword: 'NewPass123', confirmPassword: 'Other4567' });
    expect(profile.passwordForm.confirmPassword().errors().map(e => e.kind)).toContain('mismatch');

    profile.passwordModel.update(p => ({ ...p, confirmPassword: 'NewPass123' }));
    expect(profile.passwordForm.confirmPassword().errors()).toEqual([]);
  });

  it('requires a digit in the new password', () => {
    profile.passwordModel.set({ currentPassword: 'OldPass123', newPassword: 'OnlyLetters', confirmPassword: 'OnlyLetters' });
    expect(profile.passwordForm.newPassword().errors().map(e => e.message)).toContain('Must contain a digit.');
  });
});
