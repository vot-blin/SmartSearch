import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

function readSavedCart() {
    try {
        const saved = JSON.parse(localStorage.getItem('cart') || '[]')
        if (!Array.isArray(saved)) return []

        return saved.filter(item =>
            item !== null && typeof item === 'object' &&
            item.id != null &&
            Number.isInteger(item.quantity) && item.quantity > 0 &&
            Number.isFinite(item.price) && item.price >= 0
        )
    } catch {
        return []
    }
}

export const useCartStore = defineStore('cart', () => {
    const items = ref(readSavedCart())

    const totalCount = computed(() => items.value.reduce((sum, item) => sum + item.quantity, 0))

    const totalPrice = computed(() => items.value.reduce((sum, item) => sum + item.price * item.quantity, 0))

    function persist() {
        localStorage.setItem('cart', JSON.stringify(items.value))}

    function addToCart(product) {
        const found = items.value.find(i => i.id === product.id)
        if (found) found.quantity += 1
        else items.value.push({ ...product, quantity: 1})
        persist()}

    function removeFromCart(id) {
        items.value = items.value.filter(item => item.id !== id)
        persist()}
    
    function updateQuantity(id, quantity) {
        const item = items.value.find(i => i.id === id)
        if (item && quantity > 0) {
            item.quantity = quantity
            persist()
        }
    }

    function clearCart() {
        items.value = []
        persist()}

    return { items, totalCount, totalPrice, addToCart, removeFromCart, updateQuantity, clearCart }
})
