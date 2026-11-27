FROM node:22-alpine AS build
RUN corepack enable
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN echo "package-minimum-release-age=0" > .npmrc && pnpm approve-builds esbuild lmdb msgpackr-extract nice-napi && pnpm install
COPY . .
RUN pnpm install --offline
RUN ./node_modules/.bin/ng build

FROM nginx:1.27-alpine
COPY --from=build /app/dist/picochain-frontend/browser /usr/share/nginx/html
EXPOSE 80
