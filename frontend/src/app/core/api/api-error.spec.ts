import { HttpErrorResponse } from '@angular/common/http';
import { apiErrorMessage } from './api-error';

describe('apiErrorMessage', () => {
  it('explains an unreachable server', () => {
    expect(apiErrorMessage(new HttpErrorResponse({ status: 0 }))).toBe('The server is not reachable. Is the API running?');
  });

  it('prefers the first field error of a validation problem', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { title: 'Validation failed', errors: { Password: ['Password must contain a digit.'] } }
    });
    expect(apiErrorMessage(error)).toBe('Password must contain a digit.');
  });

  it('uses the problem detail for business errors', () => {
    const error = new HttpErrorResponse({
      status: 409,
      error: { title: 'Reservation.ChargerSlotTaken', detail: 'The charger is already reserved for an overlapping time slot.' }
    });
    expect(apiErrorMessage(error)).toBe('The charger is already reserved for an overlapping time slot.');
  });

  it('falls back to the status code', () => {
    expect(apiErrorMessage(new HttpErrorResponse({ status: 502 }))).toBe('Request failed (502).');
  });

  it('handles non-HTTP errors', () => {
    expect(apiErrorMessage(new Error('boom'))).toBe('Something went wrong.');
  });
});
