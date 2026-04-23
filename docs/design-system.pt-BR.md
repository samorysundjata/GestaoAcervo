# Gestão Acervo — Design System

**Idiomas:** [English](./design-system.md) • **Português (Brasil)**

> Referência canônica da linguagem visual, regras de uso de componentes e design
> tokens utilizados pelo frontend **acervo-web**. Qualquer decisão de UI deve
> poder ser rastreada de volta a este documento.

---

## 1. Filosofia

O frontend segue uma **estratégia híbrida de estilização** construída sobre
duas tecnologias complementares:

| Papel                                          | Tecnologia                                    |
| ---------------------------------------------- | --------------------------------------------- |
| **Componentes de UI interativos**              | Angular Material 17 (prebuilt `purple-green`) |
| **Utilitários de layout, espaçamento e tipografia** | Tailwind CSS v4 (classes utilitárias)    |

Três princípios regem o sistema:

1. **Não brigue com a "casca" do Material.** Peças interativas e críticas para
   acessibilidade (formulários, dialogs, menus, tabelas, botões com elevação)
   usam Material, para herdarmos de graça navegação por teclado, ARIA e tema.
2. **O Tailwind cuida do canvas.** Primitivos de layout (`flex`, `grid`,
   `gap-*`, `max-w-*`, `p-*`, breakpoints responsivos) são escritos como
   utilitários Tailwind. **Não escrevemos** classes CSS avulsas para layout.
3. **Uma paleta, dois motores.** As mesmas cores alimentam componentes Material
   (via `color="primary|accent|warn"`) e utilitários Tailwind (via tokens
   `@theme`), garantindo consistência visual entre os dois motores.

Para o item 3 funcionar, o **Preflight do Tailwind (reset base) é desativado**
— ele sobrescreveria os resets do próprio Material. O projeto importa apenas
as layers `theme` e `utilities` do Tailwind.

---

## 2. Sistema de cores

### 2.1 Mapeamento Token ↔ Material

| Papel semântico  | Âncora Material       | Token Tailwind              | Hex       |
| ---------------- | --------------------- | --------------------------- | --------- |
| Primary          | Purple 700 (M2)       | `--color-brand-700`         | `#7b1fa2` |
| Primary hover    | Purple 800            | `--color-brand-800`         | `#6a1b9a` |
| Primary suave    | Purple 50             | `--color-brand-50`          | `#f3e5f5` |
| Accent           | Green A200 (M2)       | `--color-accent-highlight`  | `#69f0ae` |
| Escala accent    | Green 50..900         | `--color-accent-50..900`    | —         |
| Warn             | Red 500               | `--color-warn-500`          | `#f44336` |
| Warn forte       | Red 700               | `--color-warn-700`          | `#d32f2f` |

As escalas completas `--color-brand-50..900` e `--color-accent-50..900` estão
definidas em `frontend/acervo-web/src/tailwind.css`. Use tons intermediários
(ex.: `bg-brand-100` para superfícies de hover, `text-brand-900` para texto de
alto contraste) conforme necessário.

### 2.2 Regras de uso

- **Primary (`brand`)** — navegação principal, o `<mat-toolbar color="primary">`,
  CTAs primárias (`<button mat-raised-button color="primary">`). Indica "ação
  padrão" ou "superfície navegacional".
- **Accent (`accent` / `accent-highlight`)** — CTAs de apoio, toggles, chips,
  indicadores de progresso. Indica "ação secundária" ou "atenção sem risco".
- **Warn** — ações destrutivas (excluir, descartar), erros de validação,
  confirmações destrutivas. Nunca use `warn` para avisos meramente cautelares
  (prefira um banner `accent-highlight` ou um tom neutro).

### 2.3 Acessibilidade (WCAG 2.1)

Razões de contraste contra fundo branco (`#ffffff`):

| Token                      | Hex       | Contraste vs branco | Adequado para…                                   |
| -------------------------- | --------- | ------------------- | ------------------------------------------------ |
| `--color-brand-700`        | `#7b1fa2` | ~7,7 : 1            | **AA & AAA** em texto de tamanho normal          |
| `--color-brand-900`        | `#4a148c` | ~11,6 : 1           | **AAA** em texto de tamanho normal               |
| `--color-accent-highlight` | `#69f0ae` | ~1,3 : 1            | ⚠ Apenas fundo / destaque — não use como texto   |
| `--color-accent-700`       | `#388e3c` | ~4,5 : 1            | AA em texto de tamanho normal                    |
| `--color-warn-500`         | `#f44336` | ~3,8 : 1            | AA apenas em texto grande (≥ 18pt ou 14pt bold)  |
| `--color-warn-700`         | `#d32f2f` | ~4,7 : 1            | AA em texto de tamanho normal                    |

**Orientações práticas**

- Use `text-brand-700` (ou mais escuro) para títulos e corpo em fundos brancos.
- Use `accent-highlight` **apenas como fundo** (preenchimento de chip, badge,
  barra de progresso); combine com texto escuro para legibilidade.
- Para texto de erro em tamanhos pequenos (dicas de validação, helpers de
  campo), prefira `text-warn-700` em vez de `text-warn-500`.

---

## 3. Tipografia

| Token            | Valor                                        |
| ---------------- | -------------------------------------------- |
| `--font-sans`    | `'Roboto', 'Helvetica Neue', sans-serif`     |
| Pesos de Roboto  | 300 (light), 400 (regular), 500 (medium)     |
| Material Icons   | carregado globalmente para `<mat-icon>`      |

Roboto e Material Icons são carregados via tags `<link>` em `src/index.html` e
ficam disponíveis imediatamente após o bootstrap da aplicação. A tipografia do
Angular Material (`mat.typography-config`, escala de headings) dirige o
dimensionamento — utilitários Tailwind (`text-sm`, `font-medium`) são usados
apenas para textos pontuais, não-heading.

---

## 4. Layout e espaçamento

O espaçamento usa a **escala padrão do Tailwind** (base de `0.25rem`), aplicado
via utilitários em vez de CSS custom:

| Caso de uso                    | Padrão de utilitários                    |
| ------------------------------ | ---------------------------------------- |
| Wrapper de página              | `mx-auto max-w-[1200px] p-6`             |
| Cabeçalho / barra de ações     | `mb-4 flex items-center justify-between` |
| Linha de controles em linha    | `flex gap-2` (ou `gap-4` para mais largo) |
| Campo de form (full-width)     | `mb-4 w-full` no `<mat-form-field>`      |
| Grid de cards                  | `grid grid-cols-1 md:grid-cols-2 gap-4`  |

### Breakpoints

Padrões do Tailwind v4: `sm (640)`, `md (768)`, `lg (1024)`, `xl (1280)`,
`2xl (1536)`. Use a convenção mobile-first (`md:flex`, não `flex md:flex`).

---

## 5. Matriz de decisão de componentes

Quando você precisar de…             | …use                                            | …não use
------------------------------------- | ----------------------------------------------- | ---------
Uma ação clicável                     | `<button mat-raised-button color="primary">`    | Um `<div>` estilizado
Uma ação destrutiva                   | `<button mat-button color="warn">`              | Um botão Tailwind vermelho
Um campo de entrada                   | `<mat-form-field>` + `<input matInput>`         | Um `<input>` cru com `border-*`
Uma lista de registros                | `<mat-table>` (+ sort/paginator quando cabível) | Uma `<table>` feita à mão
Um modal de confirmação               | `MatDialog`                                     | Um overlay customizado
Layout / espaçamento                  | Utilitários Tailwind                            | Novas classes SCSS
Chip / badge / status pill            | `<mat-chip>` (interativo) **ou** `span.bg-brand-100.text-brand-800.rounded.px-2.py-0.5` (apresentação) | CSS customizado
Container / grid de página            | Utilitários Tailwind no wrapper                 | SCSS page-scoped

### Envolvendo um componente Material com Tailwind

Aplique classes Tailwind no **wrapper**, não use `::ng-deep` para atingir
internals do Material:

```html
<!-- Bom -->
<section class="mx-auto max-w-[1200px] p-6">
  <div class="mb-4 flex items-center justify-between">
    <h2 class="text-2xl font-medium">Livros</h2>
    <button mat-raised-button color="primary">Novo</button>
  </div>
  <mat-card>…</mat-card>
</section>

<!-- Evite -->
<section class="books-page">
  <div class="books-header">…</div>
</section>
<!-- acompanhado de um .scss definindo .books-page / .books-header -->
```

---

## 6. Estendendo o sistema

### 6.1 Adicionar um novo token de marca

Edite `frontend/acervo-web/src/tailwind.css` **apenas** (não em `styles.scss` —
o preprocessamento SCSS remove as diretivas do Tailwind v4):

```css
@theme {
  --color-info-500: #0288d1;
  --color-info-700: #01579b;
}
```

Os utilitários `bg-info-500`, `text-info-500` etc. passam a existir
automaticamente. O Tailwind v4 faz tree-shaking de tokens, então um token só
entra no `styles.css` final quando ao menos um utilitário o referencia.

### 6.2 Trocar a paleta base

Duas alavancas, aplicadas juntas para manter consistência:

1. Troque o prebuilt em `frontend/acervo-web/angular.json`:
   ```json
   "styles": [
     "@angular/material/prebuilt-themes/<tema>.css",
     …
   ]
   ```
   Disponíveis: `indigo-pink`, `deeppurple-amber`, `purple-green`, `pink-bluegrey`.
2. Realinhe `--color-brand-*` e `--color-accent-*` em
   `frontend/acervo-web/src/tailwind.css` com as novas paletas Material, para
   que `<mat-*[color="primary|accent"]>` e `bg-brand-700` / `text-accent-*`
   fiquem visualmente alinhados.

Para uma paleta totalmente customizada (fora dos prebuilts), defina um tema
Material com `mat.define-theme()` em `src/styles.scss` (abordagem M3) e espelhe
os valores hex nos tokens `@theme` de `tailwind.css`.

---

## 7. Referência de arquivos

| Arquivo                                             | Papel                                                           |
| --------------------------------------------------- | --------------------------------------------------------------- |
| `frontend/acervo-web/src/tailwind.css`              | Imports do Tailwind + tokens `@theme` (fonte única)             |
| `frontend/acervo-web/src/styles.scss`               | Setup do Angular Material, resets globais `html`/`body`         |
| `frontend/acervo-web/angular.json`                  | Registra o tema Material prebuilt + `tailwind.css` como styles  |
| `frontend/acervo-web/.postcssrc.json`               | Registra o plugin `@tailwindcss/postcss` para o Angular CLI     |
| `docs/design-system.md` / `design-system.pt-BR.md`  | Este documento                                                  |

---

## 8. Changelog

| Data       | Mudança                                                               |
| ---------- | --------------------------------------------------------------------- |
| 2026-04-23 | Documento inicial do Design System. Paleta: Material `purple-green`.  |
