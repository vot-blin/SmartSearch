<template>
  <svg
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    stroke-width="1.7"
    stroke-linecap="round"
    stroke-linejoin="round"
    aria-hidden="true"
    class="icon"
  >
    <path :d="paths[name] || paths.search" />
  </svg>
</template>
<script setup>
defineProps({ name: String });
const paths = {
  search: "M21 21l-5-5 M10.5 18a7.5 7.5 0 1 0 0-15 7.5 7.5 0 0 0 0 15",
  spark:
    "m12 3 2.5 6.5L21 12l-6.5 2.5L12 21l-2.5-6.5L3 12l6.5-2.5L12 3 M20 2v4 M18 4h4",
  bag: "M5 7h14l1 14H4L5 7 M9 8V6a3 3 0 0 1 6 0v2",
  user: "M20 21v-2a6 6 0 0 0-6-6h-4a6 6 0 0 0-6 6v2 M12 9a4 4 0 1 0 0-8 4 4 0 0 0 0 8",
  arrow: "M5 12h14 m-6-6 6 6-6 6",
  check: "m5 12 4 4L19 6",
  grid: "M3 3h7v7H3z M14 3h7v7h-7z M3 14h7v7H3z M14 14h7v7h-7z",
  close: "m6 6 12 12 M18 6 6 18",
  image: "M3 3h18v18H3z m0 14 5-5 5 5 3-3 5 5 M15 7h.01",
  back: "M19 12H5 m6-6-6 6 6 6",
  plus: "M12 5v14 M5 12h14",
  minus: "M5 12h14",
  trash: "M3 6h18 M9 6V3h6v3 M5 6l1 15h12l1-15 M10 10v7 M14 10v7",
};
</script>
