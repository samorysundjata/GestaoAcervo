import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { GeneroViewModel } from '../models/genero.model';
import { CreateGeneroDto } from '../models/create-genero.dto';
import { UpdateGeneroDto } from '../models/update-genero.dto';

@Injectable({ providedIn: 'root' })
export class GeneroService {
  private readonly url = `${environment.apiUrl}/generos`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<GeneroViewModel[]>> {
    return this.http.get<ApiResponse<GeneroViewModel[]>>(this.url);
  }
  getById(id: string): Observable<ApiResponse<GeneroViewModel>> {
    return this.http.get<ApiResponse<GeneroViewModel>>(`${this.url}/${id}`);
  }
  create(dto: CreateGeneroDto): Observable<ApiResponse<GeneroViewModel>> {
    return this.http.post<ApiResponse<GeneroViewModel>>(this.url, dto);
  }
  update(id: string, dto: UpdateGeneroDto): Observable<ApiResponse<GeneroViewModel>> {
    return this.http.put<ApiResponse<GeneroViewModel>>(`${this.url}/${id}`, dto);
  }
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
