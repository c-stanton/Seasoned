export default defineNuxtPlugin((nuxtApp) => {
  const showTimeout = useState('showSessionTimeout', () => false);
  const isLoggedIn = useState('isLoggedIn');

  nuxtApp.hooks.hook('app:suspense:resolve', () => {
    globalThis.$fetch = $fetch.create({
      onResponseError({ response }) {
        if (response.status === 401 && isLoggedIn.value) {
          isLoggedIn.value = false;
          showTimeout.value = true;
        }
      }
    });
  });
});