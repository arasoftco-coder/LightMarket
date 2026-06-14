# Build stage
FROM node:18-alpine AS build

WORKDIR /app

# Copy package files
COPY src/app/package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY src/app/ ./

# Build the application
RUN npm run build -- --configuration production

# Production stage
FROM nginx:alpine

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built assets from build stage
COPY --from=build /app/dist/browser /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
