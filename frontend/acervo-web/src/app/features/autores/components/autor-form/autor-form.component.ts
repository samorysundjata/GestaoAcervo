import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AutoresActions } from '../../store/autores.actions';
import { AutorService } from '../../services/autor.service';

@Component({
  selector: 'app-autor-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './autor-form.component.html'
})
export class AutorFormComponent implements OnInit {
  form = this.fb.group({
    nome: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
  });
  editId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private router: Router,
    private route: ActivatedRoute,
    private svc: AutorService
  ) {}

  ngOnInit(): void {
    this.editId = this.route.snapshot.paramMap.get('id');
    if (this.editId) {
      this.svc.getById(this.editId).subscribe(res => {
        if (res.data) this.form.patchValue(res.data);
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const val = this.form.value as { nome: string; email: string };
    if (this.editId) {
      this.store.dispatch(AutoresActions.updateAutor({ id: this.editId, dto: val }));
    } else {
      this.store.dispatch(AutoresActions.createAutor({ dto: val }));
    }
    this.router.navigate(['/autores']);
  }
}
