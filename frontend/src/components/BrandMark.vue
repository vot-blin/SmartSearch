<template>
  <span class="brand-mark" @mouseenter="logo?.play()"
    ><AnimatedBrand v-if="animated" ref="logo" class="brand-symbol" aria-hidden="true" />
    <img v-else src="/brand/smartsearch.svg" alt="" width="48" height="48" /><span class="brand-wordmark"
      >Smart<span class="brand-accent">Search</span></span
    ></span
  >
</template>
<script setup>
import { ref } from "vue";
import AnimatedBrand from "./AnimatedBrand.vue";
defineProps({ animated: Boolean });
const logo = ref(null);
</script>
