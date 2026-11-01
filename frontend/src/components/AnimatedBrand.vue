<template>
  <svg
    class="animated-brand"
    :class="{ 'is-playing': playing }"
    @mouseenter="play"
    @animationend="finish"
    role="img"
    aria-label="Логотип SmartSearch"
    width="512"
    height="512"
    viewBox="39.333 56 400 400"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <g>
      <path
        d="M149.123 288.834C84.1333 267.2 82.5333 188.8 120.933 137.6C143.333 105.6 178.533 88 213.733 88H392.933C392.933 145.6 327.973 180.363 264.18 163.271C247.216 158.726 229.33 158.921 212.47 163.835C195.609 168.75 180.42 178.195 168.555 191.143C156.69 204.092 148.604 220.047 145.178 237.271C141.752 254.496 143.116 272.331 149.123 288.834Z"
        class="brand-top"
        fill="#6D3DF5"
        mask="url(#hero-handle-gap)"
      />
      <path
        d="M329.544 223.166C394.533 244.8 396.133 323.2 357.733 374.4C335.333 406.4 300.133 424 264.933 424L85.7332 424C85.7332 366.4 150.693 331.637 214.487 348.729C231.45 353.274 249.336 353.079 266.197 348.165C283.057 343.25 298.247 333.805 310.112 320.857C321.977 307.908 330.062 291.953 333.489 274.729C336.915 257.504 335.55 239.669 329.544 223.166Z"
        class="brand-bottom"
        fill="#20C7A5"
        mask="url(#hero-handle-gap)"
      />

      <path
        d="M239.333 312.667C270.629 312.667 296 287.296 296 256C296 224.704 270.629 199.333 239.333 199.333C208.037 199.333 182.667 224.704 182.667 256C182.667 287.296 208.037 312.667 239.333 312.667Z"
        fill="#211C2F"
      />
      <path
        d="M271 289.333L322.667 344.333"
        stroke="#211C2F"
        stroke-width="33.6"
        stroke-linecap="round"
      />
    </g>
    <defs>
      <mask
        id="hero-handle-gap"
        maskUnits="userSpaceOnUse"
        x="0"
        y="0"
        width="512"
        height="512"
      >
        <rect width="512" height="512" fill="white" />
        <path
          d="M271 289.333L322.667 344.333"
          stroke="black"
          stroke-width="68.8"
          stroke-linecap="round"
        />
      </mask>
      <clipPath id="hero-clip">
        <rect
          width="400"
          height="400"
          fill="white"
          transform="translate(56 56)"
        />
      </clipPath>
    </defs>
  </svg>
</template>
<script setup>
import { ref, onMounted } from "vue";
const playing = ref(false);
function play() {
  playing.value = true;
}
function finish(event) {
  if (event.target.classList.contains("brand-top")) playing.value = false;
}
onMounted(play);
</script>
<style scoped>
.animated-brand {
  overflow: visible;
}
.brand-top,
.brand-bottom {
  transform-box: fill-box;
  transform-origin: center;
}
.is-playing .brand-top {
  animation: brand-open-top 1.8s cubic-bezier(0.45, 0, 0.2, 1) both;
}
.is-playing .brand-bottom {
  animation: brand-open-bottom 1.8s cubic-bezier(0.45, 0, 0.2, 1) both;
}
@keyframes brand-open-top {
  0%,
  100% {
    transform: translate(0, 0) rotate(0);
  }
  42% {
    transform: translate(-14px, -25px) rotate(-5deg);
  }
}
@keyframes brand-open-bottom {
  0%,
  100% {
    transform: translate(0, 0) rotate(0);
  }
  42% {
    transform: translate(14px, 25px) rotate(5deg);
  }
}
@media (prefers-reduced-motion: reduce) {
  .is-playing .brand-top,
  .is-playing .brand-bottom {
    animation: none;
  }
}
</style>
