<template>
  <div class="home-page">
    <section class="search-hero">
      <div class="hero-copy">
        <span class="eyebrow"
          ><span class="status-dot"></span> ПОИСК, КОТОРЫЙ ПОНИМАЕТ</span
        >
        <h1>Твоя следующая<br />находка — <span>в словах.</span></h1>
        <p class="hero-description">
          Не знаешь, как это называется?<br />Просто опиши вещь. Мы поможем её
          найти.
        </p>
        <form class="hero-search" @submit.prevent="submitSearch()">
          <AppIcon name="search" /><input
            v-model="query"
            aria-label="Что хотите найти?"
            placeholder="Например, тёплый свитер на осень"
            required
          /><button type="submit" aria-label="Найти">
            <span>Найти</span><AppIcon name="arrow" />
          </button>
        </form>
        <div class="hero-examples">
          <span>Попробуй:</span
          ><button
            v-for="example in examples"
            :key="example"
            @click="submitSearch(example)"
          >
            {{ example }} <span>↗</span>
          </button>
        </div>
        <p class="hero-note">
          <AppIcon name="spark" /> Поиск по смыслу доступен после входа
        </p>
      </div>
      <div class="hero-art" aria-label="Пример разбора запроса">
        <span class="art-orbit orbit-one"></span
        ><span class="art-orbit orbit-two"></span>
        <div class="art-label">МЕНЬШЕ ФИЛЬТРОВ. БОЛЬШЕ СМЫСЛА.</div>
        <img
          src="/brand/smartsearch.svg"
          class="hero-symbol"
          alt="Логотип SmartSearch"
          width="300"
          height="300"
        />
        <div class="query-bubble">
          <AppIcon name="search" /><span>«Что-то мягкое и фиолетовое»</span>
        </div>
        <div class="meaning-card">
          <div>
            <span class="meaning-icon"><AppIcon name="spark" /></span
            ><strong>Понимаем детали</strong
            ><span class="demo-tag">пример</span>
          </div>
          <div class="meaning-tags">
            <span>Мягкая фактура</span><span>Фиолетовый</span>
          </div>
        </div>
        <div class="art-foot">
          <span class="status-dot"></span> От твоей идеи — к подходящей вещи
        </div>
      </div>
    </section>
    <section class="catalog-section" aria-labelledby="catalog-title">
      <div class="section-heading">
        <div>
          <span class="eyebrow">ВЫБИРАЙ СВОЁ</span>
          <h2 id="catalog-title">
            Каталог товаров
            <span
              v-if="!products.loading && !products.error"
              class="total-badge"
              >{{ products.totalCount }}</span
            >
          </h2>
        </div>
        <RouterLink to="/search" class="text-link"
          >Поиск с фильтрами <AppIcon name="arrow"
        /></RouterLink>
      </div>
      <div v-if="products.loading" class="empty-state" role="status">
        <span class="spinner"></span> Загружаем каталог…
      </div>
      <div v-else-if="products.error" class="empty-state" role="alert">
        <AppIcon name="bag" />
        <h3>Каталог пока недоступен</h3>
        <p>Не удалось связаться с сервером. Попробуй ещё раз.</p>
        <button class="button-secondary" @click="products.fetchall(true)">
          Повторить
        </button>
      </div>
      <div v-else-if="!products.products.length" class="empty-state">
        <h3>Каталог пока пуст</h3>
        <p>Товары появятся здесь после загрузки на сервер.</p>
      </div>
      <template v-else>
        <div class="grid-products">
          <ProductCard
            v-for="product in products.products"
            :key="product.id"
            :product="product"
            @add-to-cart="cart.addToCart"
          />
        </div>
        <nav
          v-if="products.totalPages > 1"
          class="pagination"
          aria-label="Страницы каталога"
        >
          <button
            class="page-btn"
            :disabled="products.currentPage === 1"
            @click="products.prevPage()"
          >
            ← Назад</button
          ><button
            v-for="p in visiblePages"
            :key="p"
            class="page-num"
            :class="{ active: p === products.currentPage }"
            :aria-current="p === products.currentPage ? 'page' : undefined"
            @click="products.goToPage(p)"
          >
            {{ p }}</button
          ><button
            class="page-btn"
            :disabled="products.currentPage === products.totalPages"
            @click="products.nextPage()"
          >
            Вперёд →
          </button>
        </nav>
        <p class="page-info">
          Страница {{ products.currentPage }} из {{ products.totalPages }}
        </p>
      </template>
    </section>
    <section class="discovery-banner">
      <span class="banner-icon"><AppIcon name="spark" /></span>
      <div>
        <h2>Не ищи точное слово.<br />Найди то самое.</h2>
        <p>Опиши цвет, настроение или детали — начни с того, что важно тебе.</p>
      </div>
      <RouterLink to="/ai-search" class="button-primary"
        >Попробовать AI-поиск <AppIcon name="arrow"
      /></RouterLink>
    </section>
  </div>
</template>
<script setup>
import { computed, onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import ProductCard from "@/components/ProductCard.vue";
import AppIcon from "@/components/AppIcon.vue";
import { useProductsStore } from "@/stores/products";
import { useCartStore } from "@/stores/cart";
const products = useProductsStore();
const cart = useCartStore();
const router = useRouter();
const query = ref("");
const examples = ["Уютный свитер", "Кеды на каждый день", "Сумка для учёбы"];
function submitSearch(text = query.value) {
  if (text.trim())
    router.push({ name: "ai-search", query: { q: text.trim() } });
}
const visiblePages = computed(() => {
  const start = Math.max(
    1,
    Math.min(products.currentPage - 2, products.totalPages - 4),
  );
  return Array.from(
    { length: Math.min(5, products.totalPages) },
    (_, i) => start + i,
  );
});
onMounted(() => products.fetchall());
</script>
