<template xmlns="http://www.w3.org/1999/html">
  <div class="login-form">
    <form @submit.prevent="handleLogin">
      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
      
      <div class="form-group">
        <label for="login">Username</label>
        <input
            type="text"
            id="login"
            v-model="login"
            required
        />
      </div>
      <div class="form-group">
        <label for="password">Password</label>
        <input
            type="password"
            id="password"
            v-model="password"
            required
        />
      </div>
      <button type="submit">Login</button>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent, ref} from 'vue';
import {useAuthStore} from '@/stores';

export default defineComponent({
  name: 'LoginForm',
  setup() {
    const authStore = useAuthStore();
    
    const login = ref<string>('');
    const password = ref<string>('');
    const errorMessage = ref<string | null>(null);
    
    const handleLogin = async () => {
      try {
        const loginSuccess = await authStore.sendLogin({ login: login.value, password: password.value });
        if (loginSuccess) {
            window.location.href = '/welcome';
        } else {
          errorMessage.value = 'We could not log you in. Please check your username/password and try again.';
          password.value = '';
        }
      } catch (error) {
        errorMessage.value = 'We could not log you in. Please check your username/password and try again.';
        password.value = '';
      }
    };

    return {
      login,
      password,
      errorMessage,
      handleLogin,
    };
  },
});
</script>

<style scoped>
.login-form {
  max-width: 300px;
  margin: 0 auto;
}

.form-group {
  margin-bottom: 15px;
}

label {
  margin-right: 0.8em;
}

.error {
  margin-bottom: 10px;
  color: red;
  margin-top: 10px;
}
</style>
