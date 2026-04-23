---
name: gestao-acervo-styling
description: Project-specific styling guidance for Gestão Acervo (acervo-web frontend). Trigger when adding, modifying, or reviewing any UI — new components, pages, layout changes, color tweaks, Material component usage, Tailwind utilities, or theme adjustments. This skill **overrides** the generic `angular-developer/references/tailwind-css.md` recommendations where they conflict with this project's hybrid Material + Tailwind setup.
metadata:
  scope: project
  stack: 'angular-17 material-17 tailwind-v4'
  version: '1.0'
---

# Gestão Acervo — Styling Guidelines for Agents

Before writing or changing any UI code, **read the Design System document**
([`docs/design-system.md`](../../../docs/design-system.md)) — it is the single
source of truth for colors, typography, spacing, component decisions, and
accessibility rules. This SKILL is a short operational guide that must stay
consistent with that document.

## 1. Non-negotiable rules

These rules exist because of specific compatibility constraints. Breaking them
silently breaks the app. **Do not** shortcut them, even if generic Angular /
Tailwind guides suggest otherwise.

| Rule                                                                                          | Why                                                                                                                   |
| --------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| Tailwind imports live in **`src/tailwind.css`**, not in `src/styles.scss`.                    | Tailwind v4's CSS-first directives (`@theme`, layered imports) are handled by the PostCSS plugin; SCSS preprocessing swallows them. |
| Use `@import 'tailwindcss/theme.css' layer(theme);` + `@import 'tailwindcss/utilities.css' layer(utilities);`. **Never** `@import 'tailwindcss';` alone. | Preflight (base reset) would override Angular Material's own resets and break component styling.                      |
| Never create a `tailwind.config.js`.                                                          | This project follows Tailwind **v4** conventions — configuration is CSS-first via `@theme`.                           |
| When installing Tailwind-related packages, pass `--legacy-peer-deps`.                         | Angular 17's `@angular-devkit/build-angular` declares a soft `peerOptional` on Tailwind v2/v3 that conflicts with v4. |
| The Material prebuilt theme is **`purple-green`**. Update both `angular.json` entries (`build` and `test`) if you change it. | Keeping `build` and `test` styles in sync prevents visual test drift.                                                 |
| For Docker builds, the frontend image must use **Node 20+** (`node:20-alpine`).               | Node 18 ships with npm 9, which does not resolve `libc: musl` correctly for Tailwind v4's Oxide native binding.       |

## 2. Decision matrix — Material vs Tailwind

When unsure which tool to reach for, consult the matrix in
[Section 5 of the Design System](../../../docs/design-system.md#5-component-decision-matrix).
The summary:

- **Interactive UI** (buttons, inputs, tables, dialogs, menus, chips) → **Angular Material** (`<mat-*>` + `color="primary|accent|warn"`).
- **Layout, spacing, responsive, typography utilities** → **Tailwind utility classes** on the wrapper.
- **One-off custom visuals** → Tailwind utilities first; fall back to a component-scoped `*.component.scss` only when a utility genuinely does not exist.
- **Never** use `::ng-deep` or write page-scoped SCSS files for layout. Apply Tailwind utilities to the wrapper instead.

## 3. Color usage

Semantic names map directly to Material palettes and Tailwind tokens:

| Role    | Material     | Tailwind token                       |
| ------- | ------------ | ------------------------------------ |
| Primary | `color="primary"` | `bg-brand-700`, `text-brand-700` |
| Accent  | `color="accent"`  | `bg-accent-highlight`, `text-accent-700` (text) |
| Warn    | `color="warn"`    | `bg-warn-500`, `text-warn-700` (small text) |

**Accessibility constraints** (from the Design System):

- `text-brand-700` and `text-brand-900` are safe on white — use them for headings/body.
- `--color-accent-highlight` (Green A200 `#69f0ae`) is **background-only** — never `text-accent-highlight` on white.
- For validation error text at normal sizes, prefer `text-warn-700`, not `text-warn-500`.

Full palette and contrast tables are in the Design System.

## 4. Common patterns — prefer these exact utilities

Keep the repo visually consistent. When creating a new page or section, use
the established patterns:

```html
<!-- Standard page wrapper -->
<section class="mx-auto max-w-[1200px] p-6">
  <!-- Page header / action bar -->
  <div class="mb-4 flex items-center justify-between">
    <h2 class="text-2xl font-medium">Title</h2>
    <button mat-raised-button color="primary">New</button>
  </div>

  <!-- Content -->
</section>
```

Standard utilities already in use across `autor-*`, `genero-*`, `livro-*`
templates:

- Form field full-width: `class="mb-4 w-full"` on `<mat-form-field>`.
- Inline button row: `class="flex gap-2"` (or `gap-4` for wider separation).
- Spacer inside `<mat-toolbar>`: `class="flex-1"` on the spacer span.

## 5. Adding new design tokens

Always edit **`frontend/acervo-web/src/tailwind.css`**:

```css
@theme {
  --color-info-500: #0288d1;
  --color-info-700: #01579b;
}
```

Utilities (`bg-info-500`, `text-info-500`, …) become available automatically.
**Tailwind v4 tree-shakes unused tokens** — a token only appears in the final
`styles.css` once at least one utility references it. To verify your token is
live, grep the built `dist/acervo-web/browser/styles*.css` for the hex value
after `ng build`.

## 6. When this skill conflicts with generic guides

The generic `angular-developer/references/tailwind-css.md` reference, bundled
with the Angular agent skills, describes a **standalone** Tailwind-only setup
(`@import 'tailwindcss';` → Preflight enabled, no Material). **That is not
this project.** When the two disagree, this SKILL wins. Specifically:

| Generic guide says                                   | This project does                                                           |
| ---------------------------------------------------- | --------------------------------------------------------------------------- |
| `ng add tailwindcss`                                 | Manual install with `--legacy-peer-deps` (Angular 17 + Tailwind v4 conflict)|
| `@import 'tailwindcss';` in `src/styles.css`/`.scss` | `@import 'tailwindcss/theme.css' layer(theme);` + utilities, in `src/tailwind.css` |
| Configuration via `tailwind.config.js`               | Configuration via `@theme` in `src/tailwind.css` only                       |

## 7. Verification checklist

Before declaring a styling task done:

- [ ] Design System rule followed (Material for interactive, Tailwind for layout).
- [ ] No new `tailwind.config.js`, no `@tailwind` directives, no `::ng-deep`.
- [ ] New tokens (if any) added to `src/tailwind.css`, never to `styles.scss`.
- [ ] Contrast checked against Section 2.3 of the Design System (especially text colors).
- [ ] `ng build --configuration development` succeeds.
- [ ] If the change is a theme swap, both `architect.build.options.styles[]` and `architect.test.options.styles[]` in `angular.json` were updated.
- [ ] For Docker: image verified with `docker compose up --build --no-deps web` and visible in the browser (hard-refresh to bust cached `styles.css`).
