import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { AutorViewModel } from '../models/autor.model';
import { CreateAutorDto } from '../models/create-autor.dto';
import { UpdateAutorDto } from '../models/update-autor.dto';

@Injectable({ providedIn: 'root' })
export class AutorService {
  private readonly url = `${environment.apiUrl}/autores`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<AutorViewModel[]>> {
    return this.http.get<ApiResponse<AutorViewModel[]>>(this.url);
  }
  getById(id: string): Observable<ApiResponse<AutorViewModel>> {
    return this.http.get<ApiResponse<AutorViewModel>>(`${this.url}/${id}`);
  }
  create(dto: CreateAutorDto): Observable<ApiResponse<AutorViewModel>> {
    return this.http.post<ApiResponse<AutorViewModel>>(this.url, dto);
  }
  update(id: string, dto: UpdateAutorDto): Observable<ApiResponse<AutorViewModel>> {
    return this.http.put<ApiResponse<AutorViewModel>>(`${this.url}/${id}`, dto);
  }
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
