# =============================================================================
# Stage 1: Build
# =============================================================================
FROM node:18-alpine AS build
WORKDIR /app

# Copia manifests de dependências primeiro (cache de layers)
COPY package.json package-lock.json ./
RUN npm ci --silent

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
