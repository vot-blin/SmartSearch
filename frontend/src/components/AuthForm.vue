<template>
  <div class="auth-page">
    <section class="auth-story">
      <h1>{{ register ? "Добро пожаловать в SmartSearch" : "Рады видеть тебя снова" }}</h1>
      <p>
        {{ register ? "Создай аккаунт, чтобы пользоваться поиском по смыслу." : "Войди в аккаунт, чтобы продолжить поиск по смыслу." }}
      </p>
    </section>
    <section class="auth-form-wrap">
      <span class="eyebrow">{{
        register ? "ДАВАЙ ЗНАКОМИТЬСЯ" : "С ВОЗВРАЩЕНИЕМ"
      }}</span>
      <h2 class="page-title">
        {{ register ? "Создать аккаунт" : "Войти в SmartSearch" }}
      </h2>
      <p class="text">
        {{
          register
            ? "Укажи имя, email и придумай пароль."
            : "Введи email и пароль своего аккаунта."
        }}
      </p>
      <form class="form" @submit.prevent="submit">
        <label v-if="register"
          >Имя<input
            v-model="name"
            class="input-base"
            autocomplete="name"
            required
            placeholder="Как к тебе обращаться?"
        /></label>
        <label
          >Email<input
            v-model="email"
            class="input-base"
            type="email"
            autocomplete="email"
            required
            placeholder="you@example.com"
        /></label>
        <label
          >Пароль<input
            v-model="password"
            class="input-base"
            type="password"
            :autocomplete="register ? 'new-password' : 'current-password'"
            :minlength="register ? 6 : undefined"
            required
            :placeholder="register ? 'Не менее 6 символов' : 'Введи пароль'"
        /></label>
        <div v-if="error" class="error-msg" role="alert">{{ error }}</div>
        <button class="button-primary full" :disabled="loading">
          {{ loading ? "Подожди…" : register ? "Зарегистрироваться" : "Войти"
          }}<AppIcon name="arrow" />
        </button>
      </form>
      <p class="links">
        {{ register ? "Уже есть аккаунт?" : "Ещё нет аккаунта?" }}
        <RouterLink
          :to="{ path: register ? '/login' : '/register', query: route.query }"
          >{{ register ? "Войти" : "Зарегистрироваться" }}</RouterLink
        >
      </p>
    </section>
  </div>
</template>
<script setup>
import { ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { authApi } from "@/api/auth";
import { useAuthStore } from "@/stores/auth";
import AppIcon from "./AppIcon.vue";
const props = defineProps({ register: Boolean });
const route = useRoute(),
  router = useRouter(),
  auth = useAuthStore();
const name = ref(""),
  email = ref(""),
  password = ref(""),
  loading = ref(false),
  error = ref("");
async function submit() {
  if (loading.value) return;
  loading.value = true;
  error.value = "";
  try {
    const payload = { email: email.value.trim(), password: password.value };
    const data = props.register
      ? await authApi.register({ ...payload, name: name.value.trim() })
      : await authApi.login(payload);
    auth.login(data);
    const redirect = route.query.redirect;
    router.push(
      typeof redirect === "string" &&
        redirect.startsWith("/") &&
        !redirect.startsWith("//")
        ? redirect
        : "/",
    );
  } catch (err) {
    error.value = err.message || "Не удалось войти. Попробуй ещё раз.";
  } finally {
    loading.value = false;
  }
}
</script>
