import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { LivrosActions } from '../../store/livros.actions';
import { LivroService } from '../../services/livro.service';
import { AutorService } from '../../../autores/services/autor.service';
import { GeneroService } from '../../../generos/services/genero.service';
import { AutorViewModel } from '../../../autores/models/autor.model';
import { GeneroViewModel } from '../../../generos/models/genero.model';

@Component({
  selector: 'app-livro-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, RouterLink],
  templateUrl: './livro-form.component.html'
})
export class LivroFormComponent implements OnInit {
  form = this.fb.group({
    titulo: ['', [Validators.required, Validators.maxLength(200)]],
    isbn: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
    anoPublicacao: [null as number | null, [Validators.required, Validators.min(1400), Validators.max(new Date().getFullYear())]],
    autorId: ['', Validators.required],
    generoId: ['', Validators.required],
  });
  editId: string | null = null;
  autores: AutorViewModel[] = [];
  generos: GeneroViewModel[] = [];

  constructor(
    private fb: FormBuilder, private store: Store,
    private router: Router, private route: ActivatedRoute,
    private svc: LivroService,
    private autorSvc: AutorService,
    private generoSvc: GeneroService
  ) {}

  ngOnInit(): void {
    this.autorSvc.getAll().subscribe(res => this.autores = res.data ?? []);
    this.generoSvc.getAll().subscribe(res => this.generos = res.data ?? []);
    this.editId = this.route.snapshot.paramMap.get('id');
    if (this.editId) {
      this.svc.getById(this.editId).subscribe(res => {
        if (res.data) this.form.patchValue(res.data);
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const val = this.form.value as any;
    if (this.editId) {
      this.store.dispatch(LivrosActions.updateLivro({ id: this.editId, dto: val }));
    } else {
      this.store.dispatch(LivrosActions.createLivro({ dto: val }));
    }
    this.router.navigate(['/livros']);
  }
}
