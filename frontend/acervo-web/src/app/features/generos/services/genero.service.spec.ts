import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GeneroService } from './genero.service';
import { environment } from '../../../../environments/environment';
import { CreateGeneroDto } from '../models/create-genero.dto';
import { UpdateGeneroDto } from '../models/update-genero.dto';

describe('GeneroService', () => {
  let service: GeneroService;
  let http: HttpTestingController;
  const base = `${environment.apiUrl}/generos`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(GeneroService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('getAll should GET /generos', () => {
    service.getAll().subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, message: 'ok', data: [] });
  });

  it('getById should GET /generos/:id', () => {
    service.getById('g1').subscribe();
    const req = http.expectOne(`${base}/g1`);
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, message: 'ok' });
  });

  it('create should POST body to /generos', () => {
    const dto: CreateGeneroDto = { nome: 'Poesia' };
    service.create(dto).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dto);
    req.flush({ success: true, message: 'ok' });
  });

  it('update should PUT body to /generos/:id', () => {
    const dto: UpdateGeneroDto = { nome: 'Romance' };
    service.update('g1', dto).subscribe();
    const req = http.expectOne(`${base}/g1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush({ success: true, message: 'ok' });
  });

  it('delete should DELETE /generos/:id', () => {
    service.delete('g1').subscribe();
    const req = http.expectOne(`${base}/g1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
