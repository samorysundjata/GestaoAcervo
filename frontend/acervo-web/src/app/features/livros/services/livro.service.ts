import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { LivroViewModel } from '../models/livro.model';
import { LivroDetalheViewModel } from '../models/livro-detalhe.model';
import { CreateLivroDto } from '../models/create-livro.dto';
import { UpdateLivroDto } from '../models/update-livro.dto';

@Injectable({ providedIn: 'root' })
export class LivroService {
  private readonly url = `${environment.apiUrl}/livros`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<LivroViewModel[]>> {
    return this.http.get<ApiResponse<LivroViewModel[]>>(this.url);
  }
  getById(id: string): Observable<ApiResponse<LivroDetalheViewModel>> {
    return this.http.get<ApiResponse<LivroDetalheViewModel>>(`${this.url}/${id}`);
  }
  create(dto: CreateLivroDto): Observable<ApiResponse<LivroViewModel>> {
    return this.http.post<ApiResponse<LivroViewModel>>(this.url, dto);
  }
  update(id: string, dto: UpdateLivroDto): Observable<ApiResponse<LivroViewModel>> {
    return this.http.put<ApiResponse<LivroViewModel>>(`${this.url}/${id}`, dto);
  }
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
