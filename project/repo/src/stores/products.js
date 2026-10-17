import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { productsApi } from '@/api/products'

export const useProductsStore = defineStore('products', () => {
    const products = ref([])
    const loading = ref(false)
    const error = ref('')
    const loaded = ref(false)
    const query = ref('')
    const currentPage = ref(1)
    const pageSize = ref(100)
    const totalCount = ref(0)
    const totalPages = ref(1)

    const filteredProducts = computed(() => {
        if(!query.value.trim()) return products.value
        const q = query.value.toLowerCase()
        return products.value.filter(p =>
            (p.title || '').toLowerCase().includes(q) ||
            (p.description || '').toLowerCase().includes(q) ||
            (p.category || '').toLowerCase().includes(q) 
        )
    })

    const getById = computed(() => {
        return (id) => products.value.find(p => String(p.id) === String(id))
    })

    async function fetchPage(page = 1) {
        loading.value = true
        error.value = ''
        try {
            const res = await productsApi.getAll(page, pageSize.value)
            products.value = res.data.products || res.data || []
            totalCount.value = res.data.total || products.value.length
            totalPages.value = res.data.totalPages || 1
            currentPage.value = page
            loaded.value = true
            return products.value
        } catch (e) {
            error.value = 'Не удалось загрузить каталог'
            products.value = []
            return []
        } finally {
            loading.value = false
        }
    }

    async function fetchall(force = false) {
        if (loaded.value && !force) return products.value
        return fetchPage(1)
    }

    async function nextPage() {
        if (currentPage.value < totalPages.value) {
            return fetchPage(currentPage.value + 1)
        }
    }

    async function prevPage() {
        if (currentPage.value > 1) {
            return fetchPage(currentPage.value - 1)
        }
    }

    async function goToPage(page) {
        if (page >= 1 && page <= totalPages.value) {
            return fetchPage(page)
        }
    }

    async function search(q) {
        query.value = q
        loading.value = true
        error.value = ''
        try {
            const res = await productsApi.search(q)
            products.value = res.data.products || res.data || []
            totalCount.value = products.value.length
            totalPages.value = 1
            currentPage.value = 1
            loaded.value = true
            return products.value
        } catch (e) {
            error.value = 'Ошибка поиска'
            products.value = []
        } finally {
            loading.value = false
        }
    }

    function setProducts(list) {
        products.value = Array.isArray(list) ? list : []
        loaded.value = true
    }

    return {
        products,
        loading,
        error,
        loaded,
        query,
        filteredProducts,
        getById,
        fetchall,
        search,
        setProducts,
        currentPage,
        pageSize,
        totalCount,
        totalPages,
        fetchPage,
        nextPage,
        prevPage,
        goToPage,
    }
})