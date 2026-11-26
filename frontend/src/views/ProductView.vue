<template>
  <div class="page">
    <RouterLink to="/" class="back-link"
      ><AppIcon name="back" /> В каталог</RouterLink
    >
    <div v-if="loading" class="loading-state" role="status">
      <div class="spinner"></div>
      <p>Загружаем товар…</p>
    </div>
    <div v-else-if="error" class="empty-state" role="alert">
      <h1 class="page-title">{{ error }}</h1>
      <button class="button-primary" @click="load">Попробовать снова</button>
    </div>
    <article v-else-if="product" class="product-detail">
      <ProductImage :product="product" />
      <div>
        <span class="category-tag">{{
          product.category || "Каталог SmartSearch"
        }}</span>
        <h1 class="page-title">{{ product.title }}</h1>
        <p class="detail-price">{{ priceLabel(product) }}</p>
        <p class="detail-description">
          {{
            product.description || "Описание этого товара пока не добавлено."
          }}
        </p>
        <dl v-if="specs.length" class="detail-specs">
          <div v-for="[label, value] in specs" :key="label">
            <dt>{{ label }}</dt>
            <dd>{{ value }}</dd>
          </div>
        </dl>
        <button class="button-primary" @click="add">
          <AppIcon :name="added ? 'check' : 'bag'" />{{
            added ? "Добавлено в корзину" : "В корзину"
          }}
        </button>
        <p class="added-msg" aria-live="polite">
          {{
            added
              ? "Товар сохранён. Ты можешь изменить количество в корзине."
              : ""
          }}
        </p>
      </div>
    </article>
  </div>
</template>
<script setup>
import { ref, computed, watch } from "vue";
import { useRoute } from "vue-router";
import { productsApi } from "@/api/products";
import { useCartStore } from "@/stores/cart";
import { priceLabel } from "@/utils/products";
import ProductImage from "@/components/ProductImage.vue";
import AppIcon from "@/components/AppIcon.vue";
const route = useRoute(),
  cart = useCartStore();
const product = ref(null),
  loading = ref(false),
  error = ref(""),
  added = ref(false);
let request = 0;
const specs = computed(() =>
  [
    ["Категория", product.value?.category],
    ["Подкатегория", product.value?.subCategory1],
  ].filter((item) => item[1]),
);
async function load() {
  const current = ++request;
  loading.value = true;
  error.value = "";
  added.value = false;
  try {
    const { data } = await productsApi.getById(route.params.id);
    if (current === request) product.value = data;
  } catch (err) {
    if (current === request)
      error.value =
        err.response?.status === 404
          ? "Товар не найден"
          : "Не удалось загрузить товар";
  } finally {
    if (current === request) loading.value = false;
  }
}
function add() {
  cart.addToCart(product.value);
  added.value = true;
}
watch(() => route.params.id, load, { immediate: true });
</script>
