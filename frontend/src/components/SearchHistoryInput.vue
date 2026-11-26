<template>
  <div ref="root" class="history-field" @focusout="onFocusOut" @keydown.esc.stop.prevent="close">
    <input ref="input" class="input-base" :value="modelValue" :placeholder="placeholder" aria-label="Поисковый запрос" autocomplete="off" required
      :aria-expanded="open && suggestions.length > 0" :aria-controls="listId"
      @input="update" @focus="open = true" @keydown.down.prevent="focusItem(0)" />
    <div v-if="open && suggestions.length" :id="listId" class="history-dropdown">
      <ul aria-label="История поиска">
        <li v-for="(item, index) in suggestions" :key="item">
          <button type="button" class="history-choice" @click="select(item)" @keydown.down.prevent="focusItem(index + 1)" @keydown.up.prevent="focusItem(index - 1)">
            <AppIcon name="search" /><span>{{ item }}</span>
          </button>
          <button type="button" class="history-remove" :aria-label="`Удалить запрос «${item}» из истории`" @click="remove(item)"><AppIcon name="close" /></button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { computed, nextTick, ref, useId, watch } from "vue";
import { useAuthStore } from "@/stores/auth";
import AppIcon from "./AppIcon.vue";
const props = defineProps({ modelValue: String, placeholder: String, mode: String });
const emit = defineEmits(["update:modelValue"]);
const auth = useAuthStore();
const root = ref(null), input = ref(null), open = ref(false), history = ref([]);
const listId = `search-history-${useId()}`;
const storageKey = computed(() => `smartsearch:history:${auth.user?.id || auth.user?.email || "guest"}:${props.mode || "search"}`);
watch(storageKey, (key) => {
  try {
    const saved = JSON.parse(localStorage.getItem(key) || "[]");
    history.value = Array.isArray(saved) ? [...new Set(saved.filter(item => typeof item === "string" && item.trim()))].slice(0, 10) : [];
  } catch { history.value = []; }
  open.value = false;
}, { immediate: true });
const suggestions = computed(() => history.value.filter(item => item.toLocaleLowerCase().includes((props.modelValue || "").trim().toLocaleLowerCase())).slice(0, 10));
function persist() {
  try { localStorage.setItem(storageKey.value, JSON.stringify(history.value)); } catch { /* History remains available for this visit. */ }
}
function remember(value) {
  const text = value.trim();
  if (!text) return;
  history.value = [text, ...history.value.filter(item => item.toLocaleLowerCase() !== text.toLocaleLowerCase())].slice(0, 10);
  persist();
  open.value = false;
}
function update(event) { emit("update:modelValue", event.target.value); open.value = true; }
function select(value) { emit("update:modelValue", value); input.value?.focus(); open.value = false; }
function remove(value) { history.value = history.value.filter(item => item !== value); persist(); input.value?.focus(); }
function close() { input.value?.focus(); open.value = false; }
function onFocusOut(event) { if (!root.value?.contains(event.relatedTarget)) open.value = false; }
function focusItem(index) {
  open.value = true;
  if (index < 0) { input.value?.focus(); return; }
  // Wait for the dropdown to render when opened from the input.
  nextTick(() => root.value?.querySelectorAll(".history-choice")[Math.min(index, suggestions.value.length - 1)]?.focus());
}
defineExpose({ remember });
</script>

<style scoped>
.history-field { position: relative; flex: 1; min-width: 0; }
.history-field > input { width: 100%; height: 44px; min-height: 44px; padding: 10px 16px; border-radius: 11px; line-height: 22px; }
.history-dropdown { position: absolute; top: calc(100% + 8px); left: 0; right: 0; z-index: 30; background: rgba(248, 247, 252, 0.82); -webkit-backdrop-filter: blur(18px); backdrop-filter: blur(18px); border: 1px solid rgba(211, 202, 230, 0.55); border-radius: 14px; padding: 8px; box-shadow: 0 12px 32px #211c2f18; max-height: 360px; overflow-y: auto; }
.history-dropdown { scrollbar-width: none; }
.history-dropdown::-webkit-scrollbar { display: none; }
ul { list-style: none; padding: 0; margin: 0; }
li { display: flex; align-items: center; border-radius: 8px; }
li:hover, li:focus-within { background: var(--soft); }
button { border: 0; background: transparent; color: var(--ink); border-radius: 8px; }
.history-choice { display: flex; align-items: center; gap: 10px; flex: 1; min-width: 0; padding: 10px; text-align: left; font-size: 13px; }
.history-choice span { overflow-wrap: anywhere; }
.history-choice .icon { flex-shrink: 0; color: var(--muted); width: 17px; height: 17px; }
.history-remove { display: grid; place-items: center; width: 36px; height: 36px; flex-shrink: 0; color: var(--muted); }
.history-remove:hover { background: #fff1f2; color: var(--danger); }
.history-remove .icon { width: 16px; height: 16px; }
@media (max-width: 760px) { .history-field { flex-basis: 100%; } }
</style>
