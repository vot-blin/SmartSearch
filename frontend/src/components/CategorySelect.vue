<template>
  <div ref="root" class="category-select" @focusout="onBlur" @keydown.esc.prevent.stop="close">
    <button ref="trigger" type="button" class="input-base category-trigger" aria-label="Категория" aria-haspopup="listbox" :aria-expanded="open" :aria-controls="id" @click="toggle" @keydown.down.prevent="show" @keydown.up.prevent="show">
      <span>{{ modelValue || 'Все категории' }}</span><span aria-hidden="true">⌄</span>
    </button>
    <div v-if="open" :id="id" class="category-options" role="listbox" aria-label="Категории" @keydown="navigate">
      <button v-for="(option, index) in options" :key="option" type="button" role="option" :aria-selected="option === modelValue" :tabindex="index === active ? 0 : -1" @focus="active = index" @click="choose(option)">{{ option || 'Все категории' }}</button>
    </div>
  </div>
</template>
<script setup>
import { computed, ref, nextTick, useId, onMounted, onBeforeUnmount } from 'vue';
const props = defineProps({ modelValue: String, categories: Array });
const emit = defineEmits(['update:modelValue']);
const root = ref(null), trigger = ref(null), open = ref(false), active = ref(0);
const id = `categories-${useId()}`;
const options = computed(() => ['', ...(props.categories || [])]);
async function focusOption() { await nextTick(); root.value?.querySelectorAll('[role="option"]')[active.value]?.focus(); }
function show() { open.value = true; active.value = Math.max(0, options.value.indexOf(props.modelValue)); focusOption(); }
function toggle() { if (open.value) close(); else show(); }
function close() { open.value = false; trigger.value?.focus(); }
function choose(value) { emit('update:modelValue', value); close(); }
function onBlur(event) { if (!root.value?.contains(event.relatedTarget)) open.value = false; }
function outside(event) { if (!root.value?.contains(event.target)) open.value = false; }
function navigate(event) {
  if (!['ArrowDown', 'ArrowUp', 'Home', 'End'].includes(event.key)) return;
  event.preventDefault();
  active.value = event.key === 'Home' ? 0 : event.key === 'End' ? options.value.length - 1 : (active.value + (event.key === 'ArrowDown' ? 1 : -1) + options.value.length) % options.value.length;
  focusOption();
}
onMounted(() => document.addEventListener('pointerdown', outside));
onBeforeUnmount(() => document.removeEventListener('pointerdown', outside));
</script>
<style scoped>
.category-select { position: relative; min-width: 0; }
.category-trigger { display: flex; justify-content: space-between; align-items: center; gap: 10px; text-align: left; font-weight: 600; }
.category-trigger span:first-child { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.category-options { position: absolute; left: 0; right: 0; top: calc(100% + 8px); z-index: 35; padding: 6px; max-height: 280px; overflow-y: auto; background: rgba(248,247,252,.82); backdrop-filter: blur(18px); -webkit-backdrop-filter: blur(18px); border: 1px solid var(--border); border-radius: 12px; box-shadow: 0 12px 32px #211c2f18; }
.category-options button { display: block; width: 100%; border: 0; padding: 10px 12px; border-radius: 8px; text-align: left; background: transparent; color: var(--ink); font-size: 13px; }
.category-options button:hover, .category-options button:focus-visible, .category-options button[aria-selected="true"] { background: var(--soft); color: var(--purple); outline-offset: -3px; }
</style>
