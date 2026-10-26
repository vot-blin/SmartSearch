<template>
  <div class="page">
    <section class="section-box search-panel">
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
          <input
            v-model="query"
            class="input-base"
            aria-label="Поисковый запрос"
            :placeholder="
              ai
                ? 'Например, уютный свитер для осенних прогулок'
                : 'Название товара'
            "
            required
          /><button class="button-primary" :disabled="loading">
            <AppIcon name="search" />{{ loading ? "Ищем…" : "Найти" }}
          </button>
        </div>
        <div v-if="!ai" class="filters">
          <label class="filter-group"
            >Категория<select v-model="category" class="input-base">
              <option value="">Все категории</option>
              <option v-for="c in categories" :key="c" :value="c">
                {{ c }}
              </option>
            </select></label
          >
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
      <div class="search-results-head">
        <h2>Результаты поиска</h2>
        <span class="text">Найдено: {{ results.length }}</span>
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
        <button class="page-btn" :disabled="page === 1" @click="page--">
          ← Назад</button
        ><span>{{ page }} / {{ pages }}</span
        ><button class="page-btn" :disabled="page === pages" @click="page++">
          Далее →
        </button>
      </nav>
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
import { ref, computed, onMounted } from "vue";
import { useRoute } from "vue-router";
import { productsApi } from "@/api/products";
import { aiApi } from "@/api/ai";
import { useCartStore } from "@/stores/cart";
import ProductCard from "./ProductCard.vue";
import AppIcon from "./AppIcon.vue";
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
