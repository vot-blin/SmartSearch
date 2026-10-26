<!--<template>
  <div class="page">
    <div class="hero section-box">
      <div>
        <h1 class="page-title">Каталог товаров</h1>
        <p class="subtitle">Мини-магазин с корзиной, поиском и AI-режимом.</p>
      </div>
    </div>

    <div v-if="products.loading" class="empty-state">Загрузка...</div>
    <div v-else-if="products.error" class="empty-state error">{{ products.error }}</div>
    <div v-else-if="products.products.length === 0" class="empty-state">Товары не найдены</div>

    <div v-else>
      <div class="grid-products">
        <ProductCard
          v-for="product in products.products"
          :key="product.id"
          :product="product"
          @add-to-cart="cart.addToCart"
        />
      </div>

      <div v-if="products.totalPages > 1" class="pagination">
        <button
          class="page-btn"
          :disabled="products.currentPage === 1"
          @click="products.prevPage()"
        >
          ← Назад
        </button>

        <div class="page-numbers">
          <button
            v-for="p in visiblePages"
            :key="p"
            class="page-num"
            :class="{ active: p === products.currentPage }"
            @click="products.goToPage(p)"
          >
            {{ p }}
          </button>
        </div>

        <button
          class="page-btn"
          :disabled="products.currentPage === products.totalPages"
          @click="products.nextPage()"
        >
          Вперёд →
        </button>
      </div>

      <p class="page-info">
        Страница {{ products.currentPage }} из {{ products.totalPages }}
        (всего {{ products.totalCount }} товаров)
      </p>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import ProductCard from '@/components/ProductCard.vue'
import { useProductsStore } from '@/stores/products'
import { useCartStore } from '@/stores/cart'

const products = useProductsStore()
const cart = useCartStore()

const visiblePages = computed(() => {
  const total = products.totalPages
  const current = products.currentPage
  const pages = []
  const maxButtons = 7
  let start = Math.max(1, current - Math.floor(maxButtons / 2))
  let end = Math.min(total, start + maxButtons - 1)
  start = Math.max(1, end - maxButtons + 1)
  for (let i = start; i <= end; i++) pages.push(i)
  return pages
})

onMounted(() => products.fetchall())
</script>

<style scoped>
.page {
  display: grid;
  gap: 20px;
}

.hero {
  padding: 22px;
}

.subtitle {
  margin: 0;
  color: #6e6782;
}

.error {
  color: var(--danger);
  border-color: rgba(239, 68, 68, 0.2);
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  margin-top: 32px;
  flex-wrap: wrap;
}

.page-numbers {
  display: flex;
  gap: 6px;
}

.page-btn {
  padding: 8px 18px;
  border-radius: 10px;
  border: 1px solid #e2e0ea;
  background: #fff;
  color: #4a3a6e;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.page-btn:hover:not(:disabled) {
  background: #f5f3f8;
  border-color: #c4bcd6;
}

.page-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.page-num {
  min-width: 38px;
  height: 38px;
  border-radius: 10px;
  border: 1px solid #e2e0ea;
  background: #fff;
  color: #4a3a6e;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.page-num:hover {
  background: #f5f3f8;
  border-color: #c4bcd6;
}

.page-num.active {
  background: #7c3aed;
  color: #fff;
  border-color: #7c3aed;
}

.page-info {
  text-align: center;
  color: #6e6782;
  font-size: 13px;
  margin-top: 12px;
}
</style>-->


<template>
    <div class="page">
        <div class="hero section-box">
            <div>
                <h1 class="page-title">Каталог товаров</h1>
                <p class="subtitle">Мини-магазин с корзиной, поиском и AI-режимом.</p>
            </div>
        </div>

        <div v-if="products.loading" class="empty-state">Загрузка...</div>
        <div v-else-if="products.error" class="empty-state error">{{ products.error }}</div>
        <div v-else-if="products.products.length === 0" class="empty-state">Товары не найдены</div>

        <div v-else>
            <div class="grid-products">
                <ProductCard v-for="product in products.products"
                             :key="product.id"
                             :product="product"
                             @add-to-cart="cart.addToCart" />
            </div>

            <div v-if="products.totalPages > 1" class="pagination">
                <button class="page-btn"
                        :disabled="products.currentPage === 1"
                        @click="products.prevPage()">
                    ← Назад
                </button>

                <div class="page-numbers">
                    <button v-for="p in visiblePages"
                            :key="p"
                            class="page-num"
                            :class="{ active: p === products.currentPage }"
                            @click="products.goToPage(p)">
                        {{ p }}
                    </button>
                </div>

                <button class="page-btn"
                        :disabled="products.currentPage === products.totalPages"
                        @click="products.nextPage()">
                    Вперёд →
                </button>
            </div>

            <p class="page-info">
                Страница {{ products.currentPage }} из {{ products.totalPages }}
                (всего {{ products.totalCount }} товаров)
            </p>
        </div>
    </div>
</template>

<script setup>
    import { computed, onMounted } from 'vue'
    import ProductCard from '@/components/ProductCard.vue'
    import { useProductsStore } from '@/stores/products'
    import { useCartStore } from '@/stores/cart'

    const products = useProductsStore()
    const cart = useCartStore()

    const visiblePages = computed(() => {
        const total = products.totalPages
        const current = products.currentPage
        const pages = []
        const maxButtons = 7
        let start = Math.max(1, current - Math.floor(maxButtons / 2))
        let end = Math.min(total, start + maxButtons - 1)
        start = Math.max(1, end - maxButtons + 1)
        for (let i = start; i <= end; i++) pages.push(i)
        return pages
    })

    onMounted(() => products.fetchall())
</script>

<style scoped>
    .page {
        display: grid;
        gap: 20px;
    }

    .hero {
        padding: 22px;
    }

    .subtitle {
        margin: 0;
        color: #6e6782;
    }

    .error {
        color: var(--danger);
        border-color: rgba(239, 68, 68, 0.2);
    }

    .pagination {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 16px;
        margin-top: 32px;
        flex-wrap: wrap;
    }

    .page-numbers {
        display: flex;
        gap: 6px;
    }

    .page-btn {
        padding: 8px 18px;
        border-radius: 10px;
        border: 1px solid #e2e0ea;
        background: #fff;
        color: #4a3a6e;
        cursor: pointer;
        font-size: 14px;
        transition: all 0.2s;
    }

        .page-btn:hover:not(:disabled) {
            background: #f5f3f8;
            border-color: #c4bcd6;
        }

        .page-btn:disabled {
            opacity: 0.4;
            cursor: not-allowed;
        }

    .page-num {
        min-width: 38px;
        height: 38px;
        border-radius: 10px;
        border: 1px solid #e2e0ea;
        background: #fff;
        color: #4a3a6e;
        cursor: pointer;
        font-size: 14px;
        transition: all 0.2s;
    }

        .page-num:hover {
            background: #f5f3f8;
            border-color: #c4bcd6;
        }

        .page-num.active {
            background: #7c3aed;
            color: #fff;
            border-color: #7c3aed;
        }

    .page-info {
        text-align: center;
        color: #6e6782;
        font-size: 13px;
        margin-top: 12px;
    }
</style>
