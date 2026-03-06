import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { GenerosActions } from '../../store/generos.actions';
import { GeneroService } from '../../services/genero.service';

@Component({
  selector: 'app-genero-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterLink],
  templateUrl: './genero-form.component.html'
})
export class GeneroFormComponent implements OnInit {
  form = this.fb.group({ nome: ['', [Validators.required, Validators.maxLength(100)]] });
  editId: string | null = null;

  constructor(
    private fb: FormBuilder, private store: Store,
    private router: Router, private route: ActivatedRoute, private svc: GeneroService
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
    const val = this.form.value as { nome: string };
    if (this.editId) {
      this.store.dispatch(GenerosActions.updateGenero({ id: this.editId, dto: val }));
    } else {
      this.store.dispatch(GenerosActions.createGenero({ dto: val }));
    }
    this.router.navigate(['/generos']);
  }
}
