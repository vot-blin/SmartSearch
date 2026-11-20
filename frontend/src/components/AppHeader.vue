<template>
  <header class="site-header">
    <div class="header-inner">
      <RouterLink to="/" class="brand-link" aria-label="SmartSearch — главная"
        ><BrandMark
      /></RouterLink>
      <nav class="main-nav" aria-label="Основная навигация">
        <RouterLink to="/" exact-active-class="selected">Каталог</RouterLink>
        <RouterLink to="/search" active-class="selected">Поиск</RouterLink>
        <RouterLink to="/ai-search" active-class="selected" class="ai-nav"
          ><AppIcon name="spark" /> AI-поиск</RouterLink
        >
      </nav>
      <div class="header-actions">
        <RouterLink
          to="/cart"
          class="cart-link"
          :aria-label="`Корзина: ${cart.totalCount} товаров`"
          ><AppIcon name="bag" /><span class="action-label">Корзина</span
          ><span class="cart-count">{{ cart.totalCount }}</span></RouterLink
        >
        <RouterLink
          :to="auth.isAuthenticated ? '/profile' : '/login'"
          class="account-link"
          :class="{ 'account-avatar-link': auth.isAuthenticated }"
          :aria-label="auth.isAuthenticated ? 'Личный кабинет' : 'Войти'"
          :title="auth.isAuthenticated ? 'Личный кабинет' : 'Войти'"
          ><UserAvatar v-if="auth.isAuthenticated" :user="auth.user" />
          <svg v-if="auth.isAuthenticated" class="account-avatar-ring" viewBox="0 0 44 44" fill="none" aria-hidden="true">
            <path d="M22 43 A21 21 0 0 1 22 1" pathLength="100" />
            <path d="M22 43 A21 21 0 0 0 22 1" pathLength="100" />
          </svg>
          <template v-else><AppIcon name="user" /><span>Войти</span></template></RouterLink
        >
      </div>
    </div>
  </header>
</template>
<script setup>
import BrandMark from "./BrandMark.vue";
import AppIcon from "./AppIcon.vue";
import UserAvatar from "./UserAvatar.vue";
import { useCartStore } from "@/stores/cart";
import { useAuthStore } from "@/stores/auth";
const cart = useCartStore();
const auth = useAuthStore();
</script>
