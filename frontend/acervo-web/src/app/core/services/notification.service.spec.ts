import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(() => {
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);
    TestBed.configureTestingModule({
      providers: [NotificationService, { provide: MatSnackBar, useValue: snackBar }],
    });
    service = TestBed.inject(NotificationService);
  });

  it('should open a success snackbar', () => {
    service.success('ok');
    expect(snackBar.open).toHaveBeenCalledWith('ok', 'Fechar', jasmine.objectContaining({
      duration: 3000,
      panelClass: ['snack-success'],
    }));
  });

  it('should open an error snackbar', () => {
    service.error('fail');
    expect(snackBar.open).toHaveBeenCalledWith('fail', 'Fechar', jasmine.objectContaining({
      duration: 6000,
      panelClass: ['snack-error'],
    }));
  });
});
