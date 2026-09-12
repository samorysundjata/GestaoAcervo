import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LivroService } from './livro.service';
import { environment } from '../../../../environments/environment';
import { CreateLivroDto } from '../models/create-livro.dto';
import { UpdateLivroDto } from '../models/update-livro.dto';

describe('LivroService', () => {
  let service: LivroService;
  let http: HttpTestingController;
  const base = `${environment.apiUrl}/livros`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(LivroService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('getAll should GET /livros', () => {
    service.getAll().subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, message: 'ok', data: [] });
  });

  it('getById should GET /livros/:id', () => {
    service.getById('l1').subscribe();
    const req = http.expectOne(`${base}/l1`);
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, message: 'ok' });
  });

  it('create should POST body to /livros', () => {
    const dto: CreateLivroDto = {
      titulo: 'Mensagem',
      isbn: '9781234567890',
      anoPublicacao: 1934,
      autorId: 'a1',
      generoId: 'g1',
    };
    service.create(dto).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dto);
    req.flush({ success: true, message: 'ok' });
  });

  it('update should PUT body to /livros/:id', () => {
    const dto: UpdateLivroDto = {
      titulo: 'Orpheu',
      isbn: '9781234567890',
      anoPublicacao: 1915,
      autorId: 'a1',
      generoId: 'g1',
    };
    service.update('l1', dto).subscribe();
    const req = http.expectOne(`${base}/l1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush({ success: true, message: 'ok' });
  });

  it('delete should DELETE /livros/:id', () => {
    service.delete('l1').subscribe();
    const req = http.expectOne(`${base}/l1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
