<template>
  <Transition name="scroll-bottom">
    <button v-if="visible" type="button" class="scroll-bottom-button" aria-label="Прокрутить в конец страницы" title="В конец страницы" @click="scrollDown">
      <AppIcon name="arrow" />
    </button>
  </Transition>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue";
import AppIcon from "./AppIcon.vue";

const visible = ref(false);
let observer;
let frame = 0;
function update() {
  frame = 0;
  const root = document.scrollingElement || document.documentElement;
  visible.value = root.scrollHeight - window.innerHeight - window.scrollY > 100;
}
function scheduleUpdate() {
  if (!frame) frame = requestAnimationFrame(update);
}
function scrollDown() {
  window.scrollTo({
    top: (document.scrollingElement || document.documentElement).scrollHeight,
    behavior: window.matchMedia("(prefers-reduced-motion: reduce)").matches ? "instant" : "smooth",
  });
}
onMounted(() => {
  window.addEventListener("scroll", scheduleUpdate, { passive: true });
  window.addEventListener("resize", scheduleUpdate);
  observer = new ResizeObserver(scheduleUpdate);
  observer.observe(document.body);
  update();
});
onBeforeUnmount(() => {
  window.removeEventListener("scroll", scheduleUpdate);
  window.removeEventListener("resize", scheduleUpdate);
  observer?.disconnect();
  cancelAnimationFrame(frame);
});
</script>

<style scoped>
.scroll-bottom-button {
  position: fixed;
  right: max(24px, env(safe-area-inset-right));
  bottom: max(24px, env(safe-area-inset-bottom));
  z-index: 40;
  display: grid;
  place-items: center;
  width: 52px;
  height: 52px;
  padding: 0;
  border: 0;
  border-radius: 50%;
  background: var(--mint);
  color: var(--ink);
  box-shadow: 0 4px 18px rgb(20 125 104 / 22%);
  transition: background-color 200ms ease, transform 200ms ease, box-shadow 200ms ease;
}
.scroll-bottom-button .icon {
  width: 24px;
  height: 24px;
  transform: rotate(90deg);
}
.scroll-bottom-button:hover {
  background: #35d8b6;
  transform: translateY(-2px);
  box-shadow: 0 6px 22px rgb(20 125 104 / 30%);
}
.scroll-bottom-enter-active,
.scroll-bottom-leave-active {
  transition: opacity 200ms ease, transform 200ms ease;
}
.scroll-bottom-enter-from,
.scroll-bottom-leave-to {
  opacity: 0;
  transform: translateY(10px);
  pointer-events: none;
}
@media (max-width: 600px) {
  .scroll-bottom-button {
    right: max(16px, env(safe-area-inset-right));
    bottom: max(16px, env(safe-area-inset-bottom));
    width: 44px;
    height: 44px;
  }
}
@media (prefers-reduced-motion: reduce) {
  .scroll-bottom-button { transition: none; }
}
</style>
