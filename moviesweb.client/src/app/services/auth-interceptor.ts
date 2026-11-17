import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth-service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthService).getAccessToken();
  const isPublicAuthRequest =
    request.url === '/api/auth/login' || request.url === '/api/auth/signup';
  if (!token || !request.url.startsWith('/api/') || isPublicAuthRequest) {
    return next(request);
  }

  return next(request.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  }));
};
