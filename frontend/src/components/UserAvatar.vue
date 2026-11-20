<template>
  <span class="user-avatar" :class="{ 'user-avatar-large': large }" :style="{ backgroundColor: color }" aria-hidden="true">
    {{ initials }}
  </span>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({ user: Object, large: Boolean });
const palette = ["#7040c8", "#147d68", "#3367ad", "#a54168", "#986021", "#526b35"];
const firstCharacter = (word) => {
  if (typeof Intl.Segmenter === "function") {
    return [...new Intl.Segmenter("ru", { granularity: "grapheme" }).segment(word)][0]?.segment || "";
  }
  return Array.from(word)[0] || "";
};
const initials = computed(() => {
  const words = (props.user?.name || "").trim().split(/\s+/).filter(Boolean);
  return words.length ? words.slice(0, 2).map(firstCharacter).join("").toLocaleUpperCase("ru") : "?";
});
const color = computed(() => {
  const identity = String(props.user?.id || props.user?.email || props.user?.name || "user");
  let hash = 0;
  for (const character of identity) hash = (Math.imul(hash, 31) + character.codePointAt(0)) >>> 0;
  return palette[hash % palette.length];
});
</script>

<style scoped>
.user-avatar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: 0 0 auto;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  color: #fff;
  font-size: 13px;
  font-weight: 700;
  line-height: 1;
  user-select: none;
}
.user-avatar-large {
  width: 56px;
  height: 56px;
  font-size: 21px;
}
</style>
