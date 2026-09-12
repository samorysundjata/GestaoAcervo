import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { errorInterceptor } from './error.interceptor';
import { NotificationService } from '../services/notification.service';

describe('errorInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let notification: jasmine.SpyObj<NotificationService>;

  beforeEach(() => {
    notification = jasmine.createSpyObj('NotificationService', ['error', 'success']);
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        { provide: NotificationService, useValue: notification },
      ],
    });
    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should join API errors in the snackbar', () => {
    http.get('/api').subscribe({ error: () => undefined });
    httpMock.expectOne('/api').flush(
      { message: 'ignored', errors: ['E-mail já cadastrado.'] },
      { status: 409, statusText: 'Conflict' },
    );
    expect(notification.error).toHaveBeenCalledWith('E-mail já cadastrado.');
  });

  it('should fall back to error.message', () => {
    http.get('/api').subscribe({ error: () => undefined });
    httpMock.expectOne('/api').flush(
      { message: 'Não foi possível concluir a operação.' },
      { status: 500, statusText: 'Error' },
    );
    expect(notification.error).toHaveBeenCalledWith('Não foi possível concluir a operação.');
  });

  it('should use a default message when the body is empty', () => {
    http.get('/api').subscribe({ error: () => undefined });
    httpMock.expectOne('/api').flush(null, { status: 500, statusText: 'Error' });
    expect(notification.error).toHaveBeenCalledWith('Ocorreu um erro inesperado.');
  });
});
