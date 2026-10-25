<template>
  <div class="page">
    <div class="section-box">
      <h1 class="page-title">AI поиск</h1>
      <p class="text">Доступен только авторизованным пользователям.</p>

      <SearchSwitch mode="ai" />

      <div v-if="!auth.isAuthenticated" class="locked">
        Для AI-поиска нужно войти в аккаунт.
      </div>

      <div v-else class="search-row">
        <input 
          v-model="query" 
          @keyup.enter="search" 
          class="input-base" 
          placeholder="Например: зимняя куртка серая с мехом" 
        />
        <button class="button-primary" @click="search" :disabled="loading">
          {{ loading ? 'Поиск...' : 'Найти с ИИ' }}
        </button>
      </div>
    </div>

    <!-- Динамический анализ запроса -->
    <div v-if="analysis" class="section-box">
      <h3>Анализ запроса</h3>
      <div class="analysis-grid">
        <div v-if="analysis.productType" class="analysis-item">
          <strong>Тип товара:</strong> <span>{{ analysis.productType }}</span>
        </div>
        <div v-if="analysis.color" class="analysis-item">
          <strong>Цвет:</strong> <span>{{ analysis.color }}</span>
        </div>
        <div v-if="analysis.size" class="analysis-item">
          <strong>Размер:</strong> <span>{{ analysis.size }}</span>
        </div>
        <div v-if="analysis.gender" class="analysis-item">
          <strong>Пол:</strong> <span>{{ formatGender(analysis.gender) }}</span>
        </div>
        <div v-if="analysis.material" class="analysis-item">
          <strong>Материал:</strong> <span>{{ analysis.material }}</span>
        </div>
        <div v-if="analysis.season" class="analysis-item">
          <strong>Сезон:</strong> <span>{{ analysis.season }}</span>
        </div>
        <div v-if="analysis.features?.length" class="analysis-item">
          <strong>Особенности:</strong> <span>{{ analysis.features.join(', ') }}</span>
        </div>
        <div v-if="analysis.normalizedQueryRu" class="analysis-item">
          <strong>Нормализованный запрос:</strong> <span>{{ analysis.normalizedQueryRu }}</span>
        </div>
        <div v-if="analysis.normalizedQueryEn" class="analysis-item">
          <strong>Поисковый запрос (EN):</strong> <span>{{ analysis.normalizedQueryEn }}</span>
        </div>
      </div>
    </div>

    <!-- Результаты поиска -->
    <div v-if="allResults.length" class="results-section">
      <h3>Результаты поиска ({{ allResults.length }} товаров)</h3>
      <div class="grid-products">
        <ProductCard
          v-for="product in pagedResults"
          :key="product.id"
          :product="product"
          @add-to-cart="cart.addToCart"
        />
      </div>

      <div v-if="aiTotalPages > 1" class="pagination">
        <button
          class="page-btn"
          :disabled="aiCurrentPage === 1"
          @click="aiCurrentPage--"
        >
          ← Назад
        </button>

        <div class="page-numbers">
          <button
            v-for="p in visibleAiPages"
            :key="p"
            class="page-num"
            :class="{ active: p === aiCurrentPage }"
            @click="aiCurrentPage = p"
          >
            {{ p }}
          </button>
        </div>

        <button
          class="page-btn"
          :disabled="aiCurrentPage === aiTotalPages"
          @click="aiCurrentPage++"
        >
          Вперёд →
        </button>
      </div>

      <p class="page-info">
        Страница {{ aiCurrentPage }} из {{ aiTotalPages }}
        (всего {{ allResults.length }} товаров)
      </p>
    </div>

    <!-- Загрузка -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Поиск с ИИ...</p>
    </div>

    <!-- Пустой результат -->
    <div v-if="!loading && searched && allResults.length === 0 && !analysis" class="empty-state">
      Ничего не найдено
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import SearchSwitch from '@/components/SearchSwitch.vue'
import ProductCard from '@/components/ProductCard.vue'
import { aiApi } from '@/api/ai'
import { useCartStore } from '@/stores/cart'
import { useAuthStore } from '@/stores/auth'

const AI_PAGE_SIZE = 20

const query = ref('')
const allResults = ref([])
const analysis = ref(null)
const loading = ref(false)
const searched = ref(false)
const aiCurrentPage = ref(1)
const cart = useCartStore()
const auth = useAuthStore()

const aiTotalPages = computed(() =>
  Math.max(1, Math.ceil(allResults.value.length / AI_PAGE_SIZE))
)

const pagedResults = computed(() => {
  const start = (aiCurrentPage.value - 1) * AI_PAGE_SIZE
  return allResults.value.slice(start, start + AI_PAGE_SIZE)
})

const visibleAiPages = computed(() => {
  const total = aiTotalPages.value
  const current = aiCurrentPage.value
  const pages = []
  const maxButtons = 7
  let start = Math.max(1, current - Math.floor(maxButtons / 2))
  let end = Math.min(total, start + maxButtons - 1)
  start = Math.max(1, end - maxButtons + 1)
  for (let i = start; i <= end; i++) pages.push(i)
  return pages
})

watch(aiTotalPages, () => {
  if (aiCurrentPage.value > aiTotalPages.value) {
    aiCurrentPage.value = 1
  }
})

function formatGender(gender) {
  const genders = {
    male: 'Мужской',
    female: 'Женский',
    unisex: 'Унисекс',
    kids: 'Детский'
  }
  return genders[gender] || gender
}

async function search() {
  if (!query.value.trim()) return
  
  loading.value = true
  searched.value = true
  allResults.value = []
  analysis.value = null
  aiCurrentPage.value = 1
  
  try {
    const res = await aiApi.search(query.value, 500)
    
    console.log('[AI Search] API response:', { productsCount: (res.data.products || []).length, hasAnalysis: !!res.data.analysis, limitSent: 500 })
    
    analysis.value = res.data.analysis || null
    allResults.value = res.data.products || []
    
  } catch (error) {
    console.error('Search error:', error)
    allResults.value = []
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.page {
  display: grid;
  gap: 20px;
}

.text {
  color: #6e6782;
  margin-bottom: 14px;
}

.search-row {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.search-row input {
  flex: 1;
  min-width: 260px;
}

.locked {
  padding: 14px 16px;
  border-radius: 14px;
  background: rgba(124, 58, 237, 0.08);
  color: #7c3aed;
}

.analysis-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 12px;
  margin-top: 16px;
}

.analysis-item {
  padding: 10px 14px;
  background: #f5f3f8;
  border-radius: 10px;
  font-size: 14px;
}

.analysis-item strong {
  color: #4a3a6e;
  margin-right: 8px;
}

.results-section {
  margin-top: 8px;
}

.results-section h3 {
  margin-bottom: 16px;
  color: #3a3550;
}

.grid-products {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 24px;
}

.loading-state {
  text-align: center;
  padding: 40px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e2e8f0;
  border-top-color: #7c3aed;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 12px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #6e6782;
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
