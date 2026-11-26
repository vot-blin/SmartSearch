<template>
  <div class="product-image">
    <img
      v-if="src && !failed"
      :src="src"
      :alt="product.title || 'Товар'"
      loading="lazy"
      @error="failed = true"
    />
    <div v-else class="image-placeholder">
      <AppIcon name="image" /><span>Фото пока нет</span>
    </div>
  </div>
</template>
<script setup>
import { computed, ref, watch } from "vue";
import AppIcon from "./AppIcon.vue";
import { productImage } from "@/utils/products";
const props = defineProps({ product: { type: Object, required: true } });
const src = computed(() => productImage(props.product));
const failed = ref(false);
watch(src, () => {
  failed.value = false;
});
</script>
