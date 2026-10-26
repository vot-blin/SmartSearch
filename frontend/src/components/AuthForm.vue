<template>
  <div class="auth-page">
    <section class="auth-story">
      <span class="eyebrow">ПОИСК, КОТОРЫЙ ПОНИМАЕТ</span>
      <h1>Хорошие находки начинаются с твоих слов.</h1>
      <p>
        Войди в SmartSearch, чтобы искать товары по смыслу — так, как ты их
        представляешь.
      </p>
      <img src="/brand/smartsearch.svg" alt="" width="220" height="220" /><span
        class="brand-slogan"
        >ПОНЯТЬ. НАЙТИ. ВЫБРАТЬ.</span
      >
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
            ? "Всё готово для твоих новых находок."
            : "Твои идеи. Наш поиск. Отличные находки."
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
