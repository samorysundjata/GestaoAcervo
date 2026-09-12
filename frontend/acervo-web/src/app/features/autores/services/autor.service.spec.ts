import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AutorService } from './autor.service';
import { environment } from '../../../../environments/environment';
import { CreateAutorDto } from '../models/create-autor.dto';
import { UpdateAutorDto } from '../models/update-autor.dto';

describe('AutorService', () => {
  let service: AutorService;
  let http: HttpTestingController;
  const base = `${environment.apiUrl}/autores`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(AutorService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('getAll should GET /autores', () => {
    service.getAll().subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, message: 'ok', data: [] });
  });

  it('getById should GET /autores/:id', () => {
    service.getById('abc').subscribe();
    const req = http.expectOne(`${base}/abc`);
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, message: 'ok' });
  });

  it('create should POST body to /autores', () => {
    const dto: CreateAutorDto = { nome: 'Pessoa', email: 'p@test.com' };
    service.create(dto).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(dto);
    req.flush({ success: true, message: 'ok' });
  });

  it('update should PUT body to /autores/:id', () => {
    const dto: UpdateAutorDto = { nome: 'Novo', email: 'n@test.com' };
    service.update('abc', dto).subscribe();
    const req = http.expectOne(`${base}/abc`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush({ success: true, message: 'ok' });
  });

  it('delete should DELETE /autores/:id', () => {
    service.delete('abc').subscribe();
    const req = http.expectOne(`${base}/abc`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
