import { TestBed } from '@angular/core/testing';
import { StatusChip } from './status-chip';

describe('StatusChip', () => {
  function render(status: string): HTMLElement {
    const fixture = TestBed.createComponent(StatusChip);
    fixture.componentRef.setInput('status', status);
    fixture.detectChanges();
    return (fixture.nativeElement as HTMLElement).querySelector('.chip')!;
  }

  it('renders a readable label for multi-word statuses', () => {
    expect(render('OutOfOrder').textContent?.trim()).toBe('Out of order');
  });

  it('colors derived charger states differently', () => {
    expect(render('Available').className).toContain('chip--ok');
    expect(render('Reserved').className).toContain('chip--warn');
    expect(render('Occupied').className).toContain('chip--info');
  });
});
