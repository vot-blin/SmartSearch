<template>
  <span class="animated-cart-count" :class="{ 'auto-width': autoWidth }" aria-live="polite" aria-atomic="true">
    <span class="count-accessible">{{ value }}</span>
    <Transition name="cart-number"><span :key="value" class="cart-number" aria-hidden="true">{{ value }}</span></Transition>
  </span>
</template>
<script setup>
defineProps({ value: { type: Number, required: true }, autoWidth: Boolean });
</script>
<style scoped>
.animated-cart-count {
  display: inline-grid;
  place-items: center;
  width: 5ch;
  height: 1.5em;
  overflow: hidden;
  vertical-align: middle;
  font-variant-numeric: tabular-nums;
  flex-shrink: 0;
}
.count-accessible {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip-path: inset(50%);
  white-space: nowrap;
}
.auto-width {
  width: auto;
  min-width: 1ch;
  height: 1.25em;
  line-height: 1.25;
}
</style>
