<template>
  <div class="profile-page section-box">
    <div class="profile-heading">
      <UserAvatar :user="auth.user" class="profile-avatar" large />
      <div class="profile-intro">
        <span class="eyebrow">ЛИЧНЫЙ КАБИНЕТ</span>
        <h1 class="page-title">Привет, <span class="profile-name">{{ auth.user?.name || "друг" }}</span>.</h1>
        <p class="text">Всё готово для новых находок.</p>
      </div>
    </div>
    <div class="fields">
      <div class="field">
        <span>Имя</span><strong>{{ auth.user?.name }}</strong>
      </div>
      <div class="field">
        <span>Email</span><strong>{{ auth.user?.email }}</strong>
      </div>
    </div>
    <div class="profile-actions">
      <RouterLink to="/ai-search" class="button-primary">
        <AppIcon name="spark" />Попробовать поиск по смыслу
      </RouterLink>
      <button type="button" class="button-secondary profile-logout" @click="logout">
        <AppIcon name="logout" />Выйти из аккаунта
      </button>
    </div>
  </div>
</template>
<script setup>
import { useAuthStore } from "@/stores/auth";
import { useRouter } from "vue-router";
import AppIcon from "@/components/AppIcon.vue";
import UserAvatar from "@/components/UserAvatar.vue";
const auth = useAuthStore(),
  router = useRouter();
function logout() {
  auth.logout();
  router.push("/login");
}
</script>
