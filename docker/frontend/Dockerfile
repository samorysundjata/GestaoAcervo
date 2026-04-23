# =============================================================================
# Stage 1: Build
# =============================================================================
# Node 20 (npm 10): required because npm 9 (bundled with Node 18) does not
# resolve `libc: musl` in optional deps on Alpine, causing Tailwind v4's Oxide
# native binding (@tailwindcss/oxide-linux-x64-musl) to be silently skipped.
# Angular 17 officially supports Node 20.9+.
FROM node:20-alpine AS build
WORKDIR /app

# Copia manifests de dependências primeiro (cache de layers)
# Copy only package.json (NOT package-lock.json) so npm re-resolves the
# dependency tree from scratch inside the container.
#
# Rationale — Tailwind v4 Oxide native bindings + npm optionalDependencies bug:
# A package-lock.json generated on Windows only records the Windows binding
# (@tailwindcss/oxide-win32-x64-msvc); inside Alpine we need the Linux/musl
# binary, but npm (ci OR install) respects the lockfile and skips missing
# platform entries, causing "Cannot find native binding" at build time.
# See: https://github.com/npm/cli/issues/4828
#
# --legacy-peer-deps: required because Angular 17's @angular-devkit/build-angular
# declares a soft peerOptional on Tailwind v2/v3, while this project uses v4.
# The PostCSS integration is unaffected at runtime.
COPY package.json ./
RUN npm install --silent --legacy-peer-deps --no-audit --no-fund

# Copia o restante do código e compila para produção
COPY . .
RUN npm run build -- --configuration development

# =============================================================================
# Stage 2: Runtime (Nginx para servir a SPA)
# =============================================================================
FROM nginx:1.25-alpine AS runtime

# Remove a configuração padrão do Nginx
RUN rm /etc/nginx/conf.d/default.conf

# Copia configuração customizada para SPA (suporte a HTML5 routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copia os artefatos compilados do Angular
COPY --from=build /app/dist/acervo-web/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
