<template>
  <div class="page">
    <div>
      <span class="eyebrow">ТВОИ НАХОДКИ</span>
      <h1 class="page-title">
        Корзина<span class="heading-count">{{ cart.totalCount }}</span>
      </h1>
      <p class="text">Всё, что понравилось, — в одном месте.</p>
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
              ><span aria-live="polite">{{ item.quantity }}</span
              ><button
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
          <span>Количество товаров</span><strong>{{ cart.totalCount }}</strong>
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
        ><button class="button-secondary full" @click="cart.clearCart">
          Очистить корзину
        </button>
      </aside>
    </div>
  </div>
</template>
<script setup>
import { computed } from "vue";
import { useCartStore } from "@/stores/cart";
import { priceLabel } from "@/utils/products";
import ProductImage from "@/components/ProductImage.vue";
import AppIcon from "@/components/AppIcon.vue";
const cart = useCartStore();
const totals = computed(() => {
  const sums = new Map();
  for (const item of cart.items) {
    const currency = item.currency || "RUB";
    sums.set(
      currency,
      (sums.get(currency) || 0) + Number(item.price) * item.quantity,
    );
  }
  return [...sums].map(([currency, price]) => ({ currency, price }));
});
</script>
