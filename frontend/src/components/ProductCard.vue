<template>
  <article class="product-card">
    <RouterLink
      :to="{ name: 'product', params: { id: product.id } }"
      class="product-image-link"
      :aria-label="`Открыть ${product.title}`"
      ><ProductImage :product="product"
    /></RouterLink>
    <div class="product-content">
      <p class="product-category">
        {{ product.subCategory1 || product.category || "Каталог" }}
      </p>
      <RouterLink
        :to="{ name: 'product', params: { id: product.id } }"
        class="product-title"
        >{{ product.title }}</RouterLink
      >
      <div class="product-bottom">
        <strong>{{ priceLabel(product) }}</strong
        ><button
          class="add-button"
          :class="{ added }"
          @click="add"
          :aria-label="`Добавить ${product.title} в корзину`"
        >
          <AppIcon :name="added ? 'check' : 'plus'" /><span>{{
            added ? "Добавлено" : "В корзину"
          }}</span>
        </button>
      </div>
    </div>
  </article>
</template>
<script setup>
import { ref, onUnmounted } from "vue";
import ProductImage from "./ProductImage.vue";
import AppIcon from "./AppIcon.vue";
import { priceLabel } from "@/utils/products";
const props = defineProps({ product: { type: Object, required: true } });
const emit = defineEmits(["add-to-cart"]);
const added = ref(false);
let timer;
function add() {
  emit("add-to-cart", props.product);
  added.value = true;
  clearTimeout(timer);
  timer = setTimeout(() => {
    added.value = false;
  }, 1600);
}
onUnmounted(() => clearTimeout(timer));
</script>
