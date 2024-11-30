<template>
  <div class="users-page">
    <h1>Users Administration</h1>
    <div v-if="usersStore.modifiedUsers.length > 0" class="save-panel">
      One or more users have been modified.
      <button @click="saveChanges">Save</button>
    </div>
    <div class="users-list">
      <div class="user-header">
        <span>Username</span>
        <span>Active</span>
      </div>
      <UserRow v-for="user in usersStore.users" :key="user.id" :user="user"/>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {onMounted} from 'vue';
import {useUsersStore} from '@/stores/usersStore';
import UserRow from '@/components/UserRow.vue';

const usersStore = useUsersStore();

onMounted(async () => {
  await usersStore.fetchUsers();
});

const saveChanges = () => {
  usersStore.saveChanges();
};
</script>

<style scoped>
.users-page {
  max-width: 600px;
  margin: 0 auto;
}

.save-panel {
  padding: 10px;
  margin-bottom: 15px;
  border: 1px solid #ddd;
  display: flex;
  justify-content: space-around;
  align-items: center;
}

.users-list {
  display: flex;
  flex-direction: column;
}
.user-header {
  display: flex;
  justify-content: space-between;
  font-weight: bold;
  margin-bottom: 5px;
}
</style>