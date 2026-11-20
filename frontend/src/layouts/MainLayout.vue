<template>
  <div class="app-shell">
    <a class="skip-link" href="#main-content">Перейти к содержимому</a>
    <AppHeader />
    <main id="main-content" class="main"><RouterView /></main>
    <footer class="site-footer">
      <BrandMark />
      <p>Опиши словами. Найди по смыслу.</p>
      <nav class="footer-links" aria-label="Ссылки проекта">
        <a href="https://github.com/vot-blin/SmartSearch/tree/main" target="_blank" rel="noopener noreferrer">Код SmartSearch <span aria-hidden="true">↗</span></a>
        <a href="https://smartsearch-landing.pages.dev/" target="_blank" rel="noopener noreferrer">О проекте <span aria-hidden="true">↗</span></a>
        <a href="https://github.com/Luostra/SmartSearchLanding" target="_blank" rel="noopener noreferrer">Код лендинга <span aria-hidden="true">↗</span></a>
      </nav>
    </footer>
    <ScrollToBottom v-if="['home', 'search', 'ai-search'].includes(route.name)" :key="route.name" />
  </div>
</template>
<script setup>
import AppHeader from "@/components/AppHeader.vue";
import BrandMark from "@/components/BrandMark.vue";
import ScrollToBottom from "@/components/ScrollToBottom.vue";
import { useRoute } from "vue-router";
const route = useRoute();
</script>
