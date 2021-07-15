<template>
    <h1 class="title-wrapper">
        <a href="/" class="title">FoodTourney</a>
    </h1>
    <router-view />
</template>

<script></script>

<style lang="scss">
@import "~@/assets/fonts.css";

*, *::before, *::after {
    box-sizing: border-box;
}

input, button, select, optgroup, textarea {
    font-size: inherit;
    font-family: inherit;
    line-height: inherit;
    margin: 0;
}

ol, ul, dl {
    margin-top: 0;
    margin-bottom: 1rem;
}

ol, ul {
    padding-left: 2rem;
}

h1 {
    font-size: 2.5rem;
    font-weight: 500;
    line-height: 1.2;
    margin-top: 0;
    margin-bottom: 0.5rem;
}

.title-wrapper {
    padding: 0.25rem;
}

$light: #f8f9fa;
$light-hover: #dcdddf;

.title {
    color: $light;
    text-decoration: none;
    &:hover {
        color: $light-hover;
    }
}

html, body {
    color: #212529;
    font-family: "Century Gothic" !important;
    font-size: 1rem !important;
    font-weight: 400;
    height: 100%;
    line-height: 1.5;
    margin: 0;
    padding: 0;
}

/*
Gradient created by generator at: https://cssgradient.io/
*/
#app {
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    background-attachment: fixed;
    background-color: #ffffff;
    background-image: linear-gradient(
        0deg,
        rgba(0, 0, 0, 1) 0%,
        rgba(39, 0, 195, 1) 50%,
        rgba(0, 168, 255, 1) 100%
    );
    min-height: 100%;
    text-align: center;
}
</style>
