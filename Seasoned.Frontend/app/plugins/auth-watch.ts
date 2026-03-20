export default defineNuxtPlugin((nuxtApp) => {
  const showTimeout = useState('showSessionTimeout', () => false);
  const isLoggedIn = useState('isLoggedIn');

  nuxtApp.hook('app:created', () => {
    const originalFetch = globalThis.$fetch;

    globalThis.$fetch = originalFetch.create({
      onRequest({ options }) {
        if (import.meta.client) {
          const token = localStorage.getItem('auth_token');
          if (token) {
            const headers = new Headers(options.headers);
            headers.set('Authorization', `Bearer ${token}`);
            options.headers = headers;
          }
        }
      },

      onResponseError({ response }) {
        if (response.status === 401) {
          console.warn("Session Interceptor: Caught 401 Unauthorized.");
          
          const route = useRoute();

          if (route.path !== '/login') {
            isLoggedIn.value = false;
            
            if (import.meta.client) {
               localStorage.removeItem('auth_token');
            }
            
            showTimeout.value = true;
          }
        }
      }
    });
  });
});