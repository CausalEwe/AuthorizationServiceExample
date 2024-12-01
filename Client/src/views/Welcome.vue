<template>
  <form @submit.prevent="handleLogout">
    <div>
      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
      <div>Welcome , {{username}}! This page is for authenticated users only</div>
      <button type="submit">Logout</button>
    </div>
  </form>
</template>

<script setup lang="ts">
import {authService, userService} from "@/services";
import {onMounted, ref} from "vue";
import {useRouter} from "vue-router";

const username = ref<string>('');
var router = useRouter();
const errorMessage = ref<string | null>(null);

const handleLogout = async () => {
  var status = await authService.logout();
  if (status === 200) {
    router.push('/login')
  } else {
    errorMessage.value = 'Error during logout'
  }
}

onMounted(async  () => {
  const response = await userService.meUser();
  username.value = response.data;
})
</script>

<style scoped>
.error {
  margin-bottom: 10px;
  color: red;
  margin-top: 10px;
}

</style>