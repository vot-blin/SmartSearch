<template>
  <div class="app-shell">
    <a class="skip-link" href="#main-content">Перейти к содержимому</a>
    <AppHeader />
    <main id="main-content" class="main"><RouterView /></main>
    <footer class="site-footer">
      <BrandMark />
      <p>Опиши словами. Найди по смыслу.</p>
      <span class="footer-slogan">ПОНЯТЬ. НАЙТИ. ВЫБРАТЬ.</span>
    </footer>
  </div>
</template>
<script setup>
import AppHeader from "@/components/AppHeader.vue";
import BrandMark from "@/components/BrandMark.vue";
</script>
