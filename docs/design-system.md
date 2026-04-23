# Gestão Acervo — Design System

**Languages:** **English** • [Português (Brasil)](./design-system.pt-BR.md)

> Canonical reference for the visual language, component usage rules, and design
> tokens used by the **acervo-web** frontend. All UI decisions should be traceable
> back to this document.

---

## 1. Philosophy

The frontend follows a **hybrid styling strategy** built on two complementary
technologies:

| Role                           | Technology                                      |
| ------------------------------ | ----------------------------------------------- |
| **Interactive UI components**  | Angular Material 17 (`purple-green` prebuilt)   |
| **Layout, spacing, typography utilities** | Tailwind CSS v4 (utility classes)    |

Three principles govern the system:

1. **Don't fight Material's shell.** Interactive, accessibility-critical pieces
   (forms, dialogs, menus, tables, buttons with elevation) use Material so we
   inherit keyboard navigation, ARIA, and theming for free.
2. **Tailwind owns the canvas.** Layout primitives (`flex`, `grid`, `gap-*`,
   `max-w-*`, `p-*`, responsive breakpoints) are written as Tailwind utilities.
   We do **not** write one-off CSS classes for layout.
3. **One palette, two engines.** The same color values power Material components
   (via `color="primary|accent|warn"`) and Tailwind utilities (via `@theme`
   tokens), so the two engines always look consistent.

To make #3 work, Tailwind's **Preflight (base reset) is skipped** — it would
otherwise override Material's own resets. The project only imports Tailwind's
`theme` and `utilities` layers.

---

## 2. Color system

### 2.1 Token ↔ Material mapping

| Semantic role | Material anchor            | Tailwind token              | Hex       |
| ------------- | -------------------------- | --------------------------- | --------- |
| Primary       | Purple 700 (M2)            | `--color-brand-700`         | `#7b1fa2` |
| Primary hover | Purple 800                 | `--color-brand-800`         | `#6a1b9a` |
| Primary soft  | Purple 50                  | `--color-brand-50`          | `#f3e5f5` |
| Accent        | Green A200 (M2)            | `--color-accent-highlight`  | `#69f0ae` |
| Accent scale  | Green 50..900              | `--color-accent-50..900`    | —         |
| Warn          | Red 500                    | `--color-warn-500`          | `#f44336` |
| Warn strong   | Red 700                    | `--color-warn-700`          | `#d32f2f` |

The full `--color-brand-50..900` and `--color-accent-50..900` scales are
defined in `frontend/acervo-web/src/tailwind.css`. Reach for intermediate
shades (e.g., `bg-brand-100` for hover surfaces, `text-brand-900` for
deep-contrast text) as needed.

### 2.2 Usage rules

- **Primary (`brand`)** — main navigation, the `<mat-toolbar color="primary">`,
  primary CTAs (`<button mat-raised-button color="primary">`). Indicates
  "default action", "navigational surface".
- **Accent (`accent` / `accent-highlight`)** — supporting CTAs, toggles, chips,
  progress indicators. Indicates "secondary action" or "attention without
  risk".
- **Warn** — destructive actions (delete, discard), validation errors,
  destructive confirmations. Never use `warn` for merely cautionary messaging
  (use an accent-highlight banner or a neutral tone).

### 2.3 Accessibility (WCAG 2.1)

Contrast ratios against a white background (`#ffffff`):

| Token                      | Hex       | Ratio vs white | Safe for…                                  |
| -------------------------- | --------- | -------------- | ------------------------------------------ |
| `--color-brand-700`        | `#7b1fa2` | ~7.7 : 1       | **AA & AAA** normal-size text              |
| `--color-brand-900`        | `#4a148c` | ~11.6 : 1      | **AAA** normal-size text                   |
| `--color-accent-highlight` | `#69f0ae` | ~1.3 : 1       | ⚠ Backgrounds / highlights only — not text |
| `--color-accent-700`       | `#388e3c` | ~4.5 : 1       | AA normal-size text                        |
| `--color-warn-500`         | `#f44336` | ~3.8 : 1       | AA large-text only (≥ 18pt or 14pt bold)   |
| `--color-warn-700`         | `#d32f2f` | ~4.7 : 1       | AA normal-size text                        |

**Practical guidance**

- Use `text-brand-700` (or deeper) for headings and body text on white surfaces.
- Use `accent-highlight` **only as a background** (chip fill, pill badge,
  progress bar); pair with dark text for readability.
- For error text in small sizes (validation hints, field helpers), prefer
  `text-warn-700` over `text-warn-500`.

---

## 3. Typography

| Token            | Value                                        |
| ---------------- | -------------------------------------------- |
| `--font-sans`    | `'Roboto', 'Helvetica Neue', sans-serif`     |
| Roboto weights   | 300 (light), 400 (regular), 500 (medium)     |
| Material Icons   | loaded globally for `<mat-icon>`             |

Roboto and Material Icons are loaded via `<link>` tags in `src/index.html` and
are available immediately after app bootstrap. Angular Material typography
(`mat.typography-config`, heading scale) drives sizing — Tailwind utilities
(`text-sm`, `font-medium`) are used only for one-off, non-heading text.

---

## 4. Layout & spacing

Spacing uses **Tailwind's default scale** (`0.25rem` base), applied through
utilities rather than custom CSS:

| Use case                 | Utility pattern                         |
| ------------------------ | --------------------------------------- |
| Page wrapper             | `mx-auto max-w-[1200px] p-6`            |
| Page header / action bar | `mb-4 flex items-center justify-between`|
| Row of inline controls   | `flex gap-2` (or `gap-4` for wider)     |
| Form field (full-width)  | `mb-4 w-full` on `<mat-form-field>`     |
| Card grid                | `grid grid-cols-1 md:grid-cols-2 gap-4` |

### Breakpoints

Tailwind v4 defaults: `sm (640)`, `md (768)`, `lg (1024)`, `xl (1280)`,
`2xl (1536)`. Use the mobile-first convention (`md:flex`, not `flex md:flex`).

---

## 5. Component decision matrix

When you need…                           | …use                                            | …not
---------------------------------------- | ----------------------------------------------- | -------
A clickable action                       | `<button mat-raised-button color="primary">`    | A styled `<div>`
A destructive action                     | `<button mat-button color="warn">`              | A red Tailwind button
An input field                           | `<mat-form-field>` + `<input matInput>`         | A raw `<input>` with `border-*`
A list of records                        | `<mat-table>` (plus sort/paginator as needed)   | A hand-rolled `<table>`
A modal confirmation                     | `MatDialog`                                     | A custom overlay
Layout wrapping / spacing                | Tailwind utilities                              | New SCSS classes
A chip / badge / status pill             | `<mat-chip>` (interactive) **or** `span.bg-brand-100.text-brand-800.rounded.px-2.py-0.5` (presentational) | Custom CSS
A page container / grid                  | Tailwind utilities on the wrapper               | Page-scoped SCSS

### Wrapping a Material component with Tailwind

Apply Tailwind classes to the **wrapper**, not `::ng-deep` into Material internals:

```html
<!-- Good -->
<section class="mx-auto max-w-[1200px] p-6">
  <div class="mb-4 flex items-center justify-between">
    <h2 class="text-2xl font-medium">Books</h2>
    <button mat-raised-button color="primary">New</button>
  </div>
  <mat-card>…</mat-card>
</section>

<!-- Avoid -->
<section class="books-page">
  <div class="books-header">…</div>
</section>
<!-- plus a companion .scss file defining .books-page / .books-header -->
```

---

## 6. Extending the system

### 6.1 Adding a new brand token

Edit `frontend/acervo-web/src/tailwind.css` **only** (not `styles.scss` — SCSS
preprocessing strips Tailwind v4 directives):

```css
@theme {
  --color-info-500: #0288d1;
  --color-info-700: #01579b;
}
```

The utilities `bg-info-500`, `text-info-500`, etc. become available automatically.
Tailwind v4 tree-shakes tokens, so a token only lands in the final `styles.css`
once at least one utility references it.

### 6.2 Changing the base palette

Two levers, applied together for consistency:

1. Swap the prebuilt in `frontend/acervo-web/angular.json`:
   ```json
   "styles": [
     "@angular/material/prebuilt-themes/<theme>.css",
     …
   ]
   ```
   Available: `indigo-pink`, `deeppurple-amber`, `purple-green`, `pink-bluegrey`.
2. Realign `--color-brand-*` and `--color-accent-*` in
   `frontend/acervo-web/src/tailwind.css` with the new Material palettes so
   `<mat-*[color="primary|accent"]>` and `bg-brand-700` / `text-accent-*`
   stay visually aligned.

For a fully custom, non-prebuilt palette, define a Material theme with
`mat.define-theme()` in `src/styles.scss` (M3 approach) and mirror the hex
values into `@theme` tokens in `tailwind.css`.

---

## 7. File reference

| File                                                | Role                                                           |
| --------------------------------------------------- | -------------------------------------------------------------- |
| `frontend/acervo-web/src/tailwind.css`              | Tailwind imports + `@theme` design tokens (single source)     |
| `frontend/acervo-web/src/styles.scss`               | Angular Material setup, global `html`/`body` resets           |
| `frontend/acervo-web/angular.json`                  | Registers prebuilt Material theme + `tailwind.css` as styles  |
| `frontend/acervo-web/.postcssrc.json`               | Registers `@tailwindcss/postcss` plugin for the Angular CLI   |
| `docs/design-system.md` / `design-system.pt-BR.md`  | This document                                                 |

---

## 8. Changelog

| Date       | Change                                                              |
| ---------- | ------------------------------------------------------------------- |
| 2026-04-23 | Initial Design System document. Palette: Material `purple-green`.   |
