FROM node:22-alpine AS build
RUN corepack enable
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN pnpm install
COPY . .
RUN pnpm run build

FROM nginx:1.27-alpine
COPY --from=build /app/dist/picochain-frontend/browser /usr/share/nginx/html
EXPOSE 80
