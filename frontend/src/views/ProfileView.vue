<template>
  <div class="profile-page section-box">
    <span class="eyebrow">ЛИЧНОЕ ПРОСТРАНСТВО</span>
    <h1 class="page-title">Привет, {{ auth.user?.name || "друг" }}.</h1>
    <p class="text">Всё готово для новых находок.</p>
    <div class="fields">
      <div class="field">
        <span>Имя</span><strong>{{ auth.user?.name }}</strong>
      </div>
      <div class="field">
        <span>Email</span><strong>{{ auth.user?.email }}</strong>
      </div>
    </div>
    <RouterLink to="/ai-search" class="button-primary"
      ><AppIcon name="spark" />Попробовать поиск по смыслу</RouterLink
    ><button class="button-secondary" @click="logout">Выйти из аккаунта</button>
  </div>
</template>
<script setup>
import { useAuthStore } from "@/stores/auth";
import { useRouter } from "vue-router";
import AppIcon from "@/components/AppIcon.vue";
const auth = useAuthStore(),
  router = useRouter();
function logout() {
  auth.logout();
  router.push("/login");
}
</script>
