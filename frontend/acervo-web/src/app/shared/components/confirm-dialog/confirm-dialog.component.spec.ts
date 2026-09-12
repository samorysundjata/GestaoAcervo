import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConfirmDialogComponent } from './confirm-dialog.component';

describe('ConfirmDialogComponent', () => {
  let fixture: ComponentFixture<ConfirmDialogComponent>;
  let dialogRef: jasmine.SpyObj<MatDialogRef<ConfirmDialogComponent>>;

  beforeEach(async () => {
    dialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);
    await TestBed.configureTestingModule({
      imports: [ConfirmDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: dialogRef },
        { provide: MAT_DIALOG_DATA, useValue: { title: 'Excluir', message: 'Confirma?' } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmDialogComponent);
    fixture.detectChanges();
  });

  it('should render title and message', () => {
    expect(fixture.nativeElement.textContent).toContain('Excluir');
    expect(fixture.nativeElement.textContent).toContain('Confirma?');
  });

  it('should close with true on confirm', () => {
    const buttons: HTMLButtonElement[] = Array.from(fixture.nativeElement.querySelectorAll('button'));
    buttons.find(b => b.textContent?.includes('Confirmar'))?.click();
    expect(dialogRef.close).toHaveBeenCalledWith(true);
  });

  it('should close with false on cancel', () => {
    const buttons: HTMLButtonElement[] = Array.from(fixture.nativeElement.querySelectorAll('button'));
    buttons.find(b => b.textContent?.includes('Cancelar'))?.click();
    expect(dialogRef.close).toHaveBeenCalledWith(false);
  });
});
