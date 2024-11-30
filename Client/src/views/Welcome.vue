<script setup lang="ts">
import {userService} from "@/services";
import {onMounted, ref} from "vue";

const username = ref<string>('');

onMounted(async  () => {
  const response = await userService.meUser();
  username.value = response.data;
})
</script>

<template>
<div>Welcome , {{username}}! This page is for authenticated users only</div>
</template>

<style scoped>

</style>