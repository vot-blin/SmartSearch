<template>
  <div class="page" :style="{ '--cart-header-height': `${headerHeight}px` }">
    <div>
      <span class="eyebrow">ТВОИ НАХОДКИ</span>
      <h1 class="page-title">
        Корзина<span class="heading-count"><AnimatedCartCount :value="cart.totalCount" auto-width /></span>
      </h1>
      <p class="text">Всё, что понравилось, — будет здесь.</p>
    </div>
    <div v-if="!cart.items.length" class="empty-state">
      <AppIcon name="bag" />
      <h2>Здесь будут твои находки</h2>
      <p>Добавляй понравившиеся товары из каталога или поиска.</p>
      <RouterLink to="/" class="button-primary"
        >Перейти в каталог<AppIcon name="arrow"
      /></RouterLink>
    </div>
    <div v-else class="cart-grid">
      <div class="cart-items">
        <article v-for="item in cart.items" :key="item.id" class="cart-item">
          <RouterLink :to="`/product/${item.id}`"
            ><ProductImage :product="item"
          /></RouterLink>
          <div class="cart-item-info">
            <span class="product-category">{{ item.category }}</span
            ><RouterLink :to="`/product/${item.id}`"
              ><h3>{{ item.title }}</h3></RouterLink
            ><strong>{{ priceLabel(item) }}</strong>
            <div class="quantity-controls">
              <button
                :disabled="item.quantity <= 1"
                :aria-label="`Уменьшить количество: ${item.title}`"
                @click="cart.updateQuantity(item.id, item.quantity - 1)"
              >
                <AppIcon name="minus" /></button
              ><AnimatedCartCount :value="item.quantity" />
              <button
                :aria-label="`Увеличить количество: ${item.title}`"
                @click="cart.updateQuantity(item.id, item.quantity + 1)"
              >
                <AppIcon name="plus" />
              </button>
            </div>
          </div>
          <button
            class="icon-button"
            :aria-label="`Удалить: ${item.title}`"
            @click="cart.removeFromCart(item.id)"
          >
            <AppIcon name="trash" />
          </button>
        </article>
      </div>
      <aside class="summary section-box">
        <h2>Твоя подборка</h2>
        <div class="summary-row">
          <span>Количество товаров</span><strong><AnimatedCartCount :value="cart.totalCount" /></strong>
        </div>
        <div
          v-for="total in totals"
          :key="total.currency"
          class="summary-total"
        >
          <span>Итого</span><strong>{{ priceLabel(total) }}</strong>
        </div>
        <p class="summary-note">
          Корзина сохраняется в этом браузере. Оформление заказа пока
          недоступно.
        </p>
        <RouterLink to="/search" class="button-primary full"
          >Продолжить поиск<AppIcon name="arrow" /></RouterLink
        ><button class="button-secondary button-reset full" @click="cart.clearCart">
          Очистить корзину
        </button>
      </aside>
    </div>
  </div>
</template>
<script setup>
import { computed, ref, onMounted, onBeforeUnmount } from "vue";
import AnimatedCartCount from "@/components/AnimatedCartCount.vue";
import { useCartStore } from "@/stores/cart";
import { priceLabel } from "@/utils/products";
import ProductImage from "@/components/ProductImage.vue";
import AppIcon from "@/components/AppIcon.vue";
const cart = useCartStore();
const headerHeight = ref(88);
let headerObserver;
onMounted(() => {
  const header = document.querySelector('.site-header');
  if (!header) return;
  const measure = () => { headerHeight.value = header.getBoundingClientRect().height; };
  headerObserver = new ResizeObserver(measure);
  headerObserver.observe(header);
  measure();
});
onBeforeUnmount(() => headerObserver?.disconnect());
const totals = computed(() => {
  const sums = new Map();
  for (const item of cart.items) {
    const currency = item.currency || "RUB";
    sums.set(
      currency,
      (sums.get(currency) || 0) + Number(item.price) * item.quantity
    );
  }
  return [...sums].map(([currency, price]) => ({ currency, price }));
});
</script>
<style scoped>
.heading-count {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 32px;
  line-height: 1;
  letter-spacing: 0;
}
.cart-grid { align-items: start; }
.cart-items { align-content: start; }
.summary { top: calc(var(--cart-header-height, 88px) + 20px); }
.quantity-controls button {
  flex-shrink: 0;
  transition: background-color 180ms ease, border-color 180ms ease, color 180ms ease, transform 140ms ease;
}
.quantity-controls button:hover:not(:disabled),
.quantity-controls button:focus-visible:not(:disabled) {
  background: var(--soft);
  border-color: #c8b5fa;
  color: var(--purple);
  transform: translateY(-1px);
}
.quantity-controls button:active:not(:disabled) { transform: scale(.92); }
@media (max-width: 760px) { .summary { position: static; } }
@media (prefers-reduced-motion: reduce) {
  .quantity-controls button { transition: none; transform: none !important; }
}
</style>
