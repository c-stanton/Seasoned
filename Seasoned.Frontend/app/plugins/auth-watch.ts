export default defineNuxtPlugin((nuxtApp) => {
  const showTimeout = useState('showSessionTimeout', () => false);
  const isLoggedIn = useState('isLoggedIn');

  nuxtApp.hook('app:created', () => {
    
    const originalFetch = globalThis.$fetch;

    globalThis.$fetch = originalFetch.create({
      onResponseError({ response }) {
        
        if (response.status === 401) {
          console.warn("Session Interceptor: Caught 401 Unauthorized.");
        
          const route = useRoute();
          if (route.path !== '/login') {
            isLoggedIn.value = false;
            showTimeout.value = true;
          }
        }
      }
    });
  });
});