<template>
  <div class="page">
    <section class="section-box search-panel search-panel-with-filters">
      <span class="eyebrow">{{ ai ? "ПОИСК ПО СМЫСЛУ" : "НАЙДИ СВОЁ" }}</span>
      <h1 class="page-title">
        {{ ai ? "Просто опиши, что ищешь." : "Что будем искать?" }}
      </h1>
      <p class="text">
        {{
          ai
            ? "Цвет, настроение, материал или повод — начни со своих слов."
            : "Ищи по названию и уточняй результаты с помощью фильтров."
        }}
      </p>
      <nav class="switcher" aria-label="Режим поиска">
        <RouterLink to="/search" :class="{ active: !ai }"
          >По названию</RouterLink
        ><RouterLink to="/ai-search" :class="{ active: ai }"
          ><AppIcon name="spark" /> По смыслу</RouterLink
        >
      </nav>
      <form @submit.prevent="search">
        <div class="search-row">
          <SearchHistoryInput
            ref="historyInput"
            :mode="ai ? 'ai' : 'search'"
            v-model="query"
            :placeholder="
              ai
                ? 'Например, уютный свитер для осенних прогулок'
                : 'Название товара'
            "
          /><button class="button-primary" :disabled="loading">
            <AppIcon name="search" />{{ loading ? "Ищем…" : "Найти" }}
          </button>
        </div>
        <div v-if="!ai" class="filters">
          <h2 class="filters-title">Фильтры</h2>
          <div class="filter-group">
            <span>Категория</span>
            <CategorySelect v-model="category" :categories="categories" />
          </div>
          <div class="price-filters">
          <label class="filter-group"
            >Цена от<input
              v-model="minPrice"
              class="input-base"
              type="number"
              min="0"
              step="any"
              placeholder="От"
          /></label>
          <label class="filter-group"
            >Цена до<input
              v-model="maxPrice"
              class="input-base"
              type="number"
              min="0"
              step="any"
              placeholder="До"
          /></label>
          <button type="button" class="button-secondary button-reset" :disabled="loading || !hasFilters" @click="resetFilters">Сбросить фильтры</button>
          </div>
        </div>
      </form>
      <p v-if="categoryError" class="text">
        Категории пока недоступны. Поиск по названию работает независимо.
      </p>
    </section>
    <div v-if="error" class="error-msg" role="alert">{{ error }}</div>
    <div v-if="loading" class="loading-state" role="status">
      <div class="spinner"></div>
      <p>
        {{ ai ? "Разбираемся в запросе и подбираем товары…" : "Ищем товары…" }}
      </p>
    </div>
    <template v-else-if="searched && !error">
      <section v-if="analysisItems.length" class="section-box">
        <h2>Как мы поняли запрос</h2>
        <div class="analysis-grid">
          <div
            v-for="item in analysisItems"
            :key="item[0]"
            class="analysis-item"
          >
            <strong>{{ item[0] }}</strong
            ><span>{{ item[1] }}</span>
          </div>
        </div>
      </section>
      <div ref="resultsHeading" class="search-results-head">
        <h2>Результаты поиска</h2>
        <div class="results-count"><span>Показано</span><span class="total-badge">{{ paged.length }} из {{ results.length }}</span></div>
      </div>
      <div v-if="!results.length" class="empty-state">
        <AppIcon name="search" />
        <h2>Пока ничего не нашлось</h2>
        <p>Попробуй другие слова или убери часть фильтров.</p>
      </div>
      <div v-else class="grid-products">
        <ProductCard
          v-for="product in paged"
          :key="product.id"
          :product="product"
          @add-to-cart="cart.addToCart"
        />
      </div>
      <nav
        v-if="pages > 1"
        class="pagination"
        aria-label="Страницы результатов"
      >
        <button class="page-btn" :disabled="page === 1" aria-label="Первая страница" @click="changePage(1)">« В начало</button>
        <button v-for="p in visiblePages" :key="p" class="page-num" :class="{ active: page === p }" :aria-current="page === p ? 'page' : undefined" @click="changePage(p)">{{ p }}</button>
        <button class="page-btn" :disabled="page === pages" aria-label="Последняя страница" @click="changePage(pages)">В конец »</button>
      </nav>
      <p v-if="pages > 1" class="page-info">Страница {{ page }} из {{ pages }}</p>
    </template>
    <div v-else-if="!error" class="empty-state">
      <AppIcon :name="ai ? 'spark' : 'search'" />
      <h2>
        {{
          ai ? "Не нужно знать точное название" : "Твоя находка где-то рядом"
        }}
      </h2>
      <p>
        {{
          ai
            ? "Расскажи, каким должен быть товар. Остальное — за поиском."
            : "Введи запрос, чтобы увидеть подходящие товары."
        }}
      </p>
    </div>
  </div>
</template>
<script setup>
import { ref, computed, nextTick, onMounted } from "vue";
import { useRoute } from "vue-router";
import { productsApi } from "@/api/products";
import { aiApi } from "@/api/ai";
import { useCartStore } from "@/stores/cart";
import ProductCard from "./ProductCard.vue";
import AppIcon from "./AppIcon.vue";
import SearchHistoryInput from "./SearchHistoryInput.vue";
import CategorySelect from "./CategorySelect.vue";
const historyInput = ref(null);
const props = defineProps({ ai: Boolean });
const route = useRoute(),
  cart = useCartStore();
const query = ref(typeof route.query.q === "string" ? route.query.q : "");
const category = ref(""),
  minPrice = ref(""),
  maxPrice = ref(""),
  categories = ref([]),
  categoryError = ref(false);
const loading = ref(false),
  searched = ref(false),
  error = ref(""),
  results = ref([]),
  analysis = ref(null),
  page = ref(1);
const pages = computed(() => Math.max(1, Math.ceil(results.value.length / 20)));
const resultsHeading = ref(null);
const hasFilters = computed(() => category.value !== "" || minPrice.value !== "" || maxPrice.value !== "");
const visiblePages = computed(() => {
  const start = Math.max(1, Math.min(page.value - 2, pages.value - 4));
  return Array.from({ length: Math.min(5, pages.value) }, (_, i) => start + i);
});
async function resetFilters() {
  category.value = "";
  minPrice.value = "";
  maxPrice.value = "";
  error.value = "";
  if (searched.value && query.value.trim()) await search();
}
async function changePage(nextPage) {
  if (nextPage === page.value || nextPage < 1 || nextPage > pages.value) return;
  page.value = nextPage;
  await nextTick();
  if (!resultsHeading.value) return;
  const headerHeight = document.querySelector(".site-header")?.getBoundingClientRect().height || 0;
  window.scrollTo({
    top: Math.max(0, window.scrollY + resultsHeading.value.getBoundingClientRect().top - headerHeight - 20),
    behavior: window.matchMedia("(prefers-reduced-motion: reduce)").matches ? "instant" : "smooth",
  });
}
const paged = computed(() =>
  results.value.slice((page.value - 1) * 20, page.value * 20),
);
const analysisItems = computed(() => {
  const a = analysis.value || {};
  return [
    ["Товар", a.productType],
    ["Цвет", a.color],
    ["Размер", a.size],
    [
      "Для кого",
      {
        male: "Мужской",
        female: "Женский",
        unisex: "Унисекс",
        kids: "Детский",
      }[a.gender] || a.gender,
    ],
    ["Материал", a.material],
    ["Сезон", a.season],
    ["Твой запрос", a.normalizedQueryRu],
    [
      "Особенности",
      Array.isArray(a.features) ? a.features.join(", ") : a.features,
    ],
  ].filter((item) => item[1]);
});
onMounted(async () => {
  if (props.ai) return;
  try {
    const { data } = await productsApi.getCategories();
    categories.value = Array.isArray(data) ? data : data.categories || [];
  } catch {
    categoryError.value = true;
  }
});
async function search() {
  if (loading.value || !query.value.trim()) return;
  error.value = "";
  if (
    !props.ai &&
    minPrice.value !== "" &&
    maxPrice.value !== "" &&
    Number(minPrice.value) > Number(maxPrice.value)
  ) {
    error.value = "Цена «от» не должна быть больше цены «до».";
    return;
  }
  loading.value = true;
  historyInput.value?.remember(query.value);
  searched.value = true;
  results.value = [];
  analysis.value = null;
  page.value = 1;
  try {
    const { data } = props.ai
      ? await aiApi.search(query.value.trim(), 500)
      : await productsApi.search(query.value.trim(), {
          category: category.value || undefined,
          minPrice: minPrice.value === "" ? undefined : Number(minPrice.value),
          maxPrice: maxPrice.value === "" ? undefined : Number(maxPrice.value),
        });
    results.value = Array.isArray(data) ? data : data.products || [];
    analysis.value = data.analysis || null;
  } catch (err) {
    error.value =
      err.response?.status === 401
        ? "Сессия истекла. Войди в аккаунт заново."
        : "Не удалось выполнить поиск. Проверь подключение и попробуй ещё раз.";
  } finally {
    loading.value = false;
  }
}
</script>
<style scoped>
.search-panel {
  max-width: none;
  margin: 0;
  min-height: 350px;
}
.search-panel-with-filters {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 320px;
  grid-template-rows: repeat(4, max-content) minmax(max-content, 1fr);
  column-gap: 40px;
}
.search-panel-with-filters > :not(form) {
  grid-column: 1;
}
.search-panel-with-filters form {
  display: contents;
}
.search-panel-with-filters .search-row {
  grid-column: 1;
  min-width: 0;
  align-self: start;
}
.search-panel .text {
  margin-bottom: 16px;
}
.search-panel .switcher {
  align-self: start;
  padding: 4px;
  margin: 0 0 18px;
  border-radius: 11px;
}
.search-panel .switcher a {
  min-height: 36px;
  padding: 8px 14px;
  border-radius: 8px;
  line-height: 20px;
}
.search-panel .search-row .input-base,
.search-panel .search-row .button-primary {
  min-height: 44px;
  height: 44px;
  padding: 10px 16px;
  border-radius: 11px;
  line-height: 22px;
}
.search-panel .search-row .input-base {
  min-width: 0;
}
.search-panel .search-row .button-primary {
  white-space: nowrap;
}
.search-panel-with-filters .switcher {
  justify-self: start;
}
.search-panel-with-filters .filters {
  grid-column: 2;
  grid-row: 1 / span 5;
  grid-template-columns: minmax(0, 1fr);
  align-content: start;
  gap: 16px;
  border-top: 0;
  border-left: 1px solid var(--border);
  padding: 0 0 0 28px;
  margin: 0;
}
.filters-title {
  font-size: 20px;
  margin: 0;
}
.price-filters {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px 12px;
}
.price-filters > .filter-group {
  grid-column: auto;
  min-width: 0;
}
.price-filters .input-base {
  min-width: 0;
  width: 100%;
}
.price-filters > .button-secondary {
  grid-column: 1 / -1;
}
.search-results-head {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
  margin: 0;
}
.results-count {
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--muted);
  font-size: 12px;
  line-height: 20px;
}
.results-count .total-badge {
  margin: 0;
  font-size: inherit;
  line-height: inherit;
}
.search-results-head .total-badge {
  flex-shrink: 0;
}
@media (max-width: 900px) {
  .search-panel-with-filters {
    grid-template-columns: minmax(0, 1fr);
  }
  .search-panel-with-filters .filters {
    grid-column: 1;
    grid-row: auto;
    border-left: 0;
    border-top: 1px solid var(--border);
    padding: 24px 0 0;
    margin-top: 24px;
  }
}
</style>
