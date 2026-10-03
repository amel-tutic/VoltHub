// Helpers for <input type="datetime-local">, which works with local "YYYY-MM-DDTHH:mm" strings.

export function nextQuarterHour(from = new Date()): Date {
  const date = new Date(from);
  date.setSeconds(0, 0);
  date.setMinutes(Math.ceil((date.getMinutes() + 1) / 15) * 15);
  return date;
}

export function addMinutes(date: Date, minutes: number): Date {
  return new Date(date.getTime() + minutes * 60_000);
}

export function toLocalInput(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

// <input type="date"> works with local "YYYY-MM-DD" strings.
export function toDateInput(date: Date): string {
  return toLocalInput(date).slice(0, 10);
}

export function addDays(date: Date, days: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
}
