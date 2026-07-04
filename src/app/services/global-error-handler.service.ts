import { ErrorHandler, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandlerService implements ErrorHandler {
  handleError(error: any): void {
    const chunkFailedMessage = /Loading chunk .* failed/;
    if (chunkFailedMessage.test(error.message || error.toString())) {
      console.warn('Chunk loading failed. Reloading page to fetch the latest assets...');
      window.location.reload();
    } else {
      console.error(error);
    }
  }
}
