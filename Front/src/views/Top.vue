<template>
    <div v-if="state === 'home'">
        <Start
            :pos="pos"
            :places="places"
            :state="state"
            :zip="zip"
            @update-option1="updateOption1"
            @update-option2="updateOption2"
            @update-pos="updatePos"
            @update-places="updatePlaces"
            @update-popular="updatePopular"
            @update-state="updateState"
            @update-zip="updateZip"
        ></Start>
    </div>
    <div v-else-if="state === 'popular'">
        <Popular
            :final="final"
            :places="places"
            :popular="popular"
            :state="state"
            :zip="zip"
            @update-final="updateFinal"
            @update-state="updateState"
        ></Popular>
    </div>
    <div v-else-if="state === 'tournament'">
        <Tournament
            :option1="option1"
            :option2="option2"
            :places="places"
            :state="state"
            :zip="zip"
            @update-final="updateFinal"
            @update-state="updateState"
        ></Tournament>
    </div>
    <div v-else-if="state === 'result'">
        <Result :final="final"></Result>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Start from "../components/Start.vue";
import Popular from "../components/Popular.vue";
import Tournament from "../components/Tournament.vue";
import Result from "../components/Result.vue";

export default defineComponent({
    data() {
        return {
            final: {},
            option1: "",
            option2: "",
            places: {},
            popular: {},
            pos: {
                lat: 0.0,
                lng: 0.0,
            },
            state: "home",
            zip: "-1",
        };
    },
    name: "Top",
    components: {
        Start: Start,
        Popular: Popular,
        Tournament: Tournament,
        Result: Result,
    },
    methods: {
        updateFinal(final: any): void {
            this.final = final;
        },
        updateOption1(option1: string): void {
            this.option1 = option1;
        },
        updateOption2(option2: string): void {
            this.option2 = option2;
        },
        updatePlaces(places: any): void {
            this.places = places;
        },
        updatePopular(popular: any): void {
            this.popular = popular;
        },
        updatePos(pos: { lat: number; lng: number }): void {
            this.pos = pos;
        },
        updateState(state: string): void {
            this.state = state;
        },
        updateZip(zip: string): void {
            this.zip = zip;
        },
    },
    //Bind window resize event
    //window.addEventListener("resize", () => {});
});
</script>

<style lang="scss">
$widths: (
    xs: 100%,
    sm: 540px,
    md: 720px,
    lg: 960px,
    xl: 1140px,
    xxl: 1320px,
);

$grid-breakpoints: (
    xs: 0,
    sm: 576px,
    md: 768px,
    lg: 992px,
    xl: 1200px,
    xxl: 1400px,
);

$lavender: #f8f5ff;
.backdrop {
    background-color: $lavender;
    border-radius: 0.25rem;
    padding: 1rem;
    text-align: center;
}

.clean-input {
    border-color: #ced4da;
    border-radius: 0.25rem;
    border-style: solid;
    border-width: 1px;
    display: block;
    padding: 0.375rem 0.75rem;
    transition: 0.15s;
    width: 100%;

    &:focus {
        border-color: #86b7fe;
        box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
        outline: 0;
    }
}

%button-base {
    border-color: transparent;
    border-radius: 0.25rem;
    border-style: solid;
    border-width: 1px;
    color: #ffffff;
    display: inline-block;
    font-size: 1rem;
    padding: 0.375rem 0.75rem;
    text-align: center;
    transition: 0.15s;
    &:not(:disabled) {
        cursor: pointer;
    }
}

@mixin buttonColor($primary, $focus, $active, $shadow) {
    background-color: $primary;
    &:hover {
        background-color: $focus;
    }
    &:active {
        background-color: $active;
        box-shadow: 0px 0px 0px 0.25rem $shadow;
        &:focus {
            background-color: $active;
            box-shadow: 0px 0px 0px 0.25rem $shadow;
        }
    }
    &:focus {
        background-color: $focus;
        box-shadow: 0px 0px 0px 0.25rem $shadow;
    }
}

$indigo: #4800ff;
$indigo-focus: #3c00d5;
$indigo-active: #3900ca;
$indigo-shadow: rgba(115, 59, 255, 0.5);

.button-indigo {
    @extend %button-base;
    @include buttonColor(
        $indigo,
        $indigo-focus,
        $indigo-active,
        $indigo-shadow
    );
}

$orange: #ffba00;
$orange-focus: #eba900;
$orange-active: #e5a300;
$orange-shadow: rgba(255, 213, 51, 0.5);

.button-orange {
    @extend %button-base;
    @include buttonColor(
        $orange,
        $orange-focus,
        $orange-active,
        $orange-shadow
    );
}

.map {
    border-radius: 0.25rem;
    height: 100%;
    width: 100%;
}

.reactive-width {
    margin-left: auto;
    margin-right: auto;
    width: 100%;
}

.locate-wrapper {
    grid-gap: 0.25rem;
    margin: 0.5rem 0 1rem 0;
}

@media only screen and (max-width: map-get($grid-breakpoints, sm)) {
    .locate-wrapper {
        display: grid;
        grid-template-columns: 3fr 1fr;
    }
}

@media only screen and (min-width: map-get($grid-breakpoints, sm)) and (max-width: map-get($grid-breakpoints, md)) {
    .reactive-width {
        max-width: map-get($widths, sm);
    }
    .locate-wrapper {
        display: grid;
        grid-template-columns: 4fr 1fr;
    }
}

@media only screen and (min-width: map-get($grid-breakpoints, md)) and (max-width: map-get($grid-breakpoints, lg)) {
    .reactive-width {
        max-width: map-get($widths, md);
    }
    .locate-wrapper {
        display: grid;
        grid-template-columns: 3fr 1fr 3fr;
    }
}

@media only screen and (min-width: map-get($grid-breakpoints, lg)) and (max-width: map-get($grid-breakpoints, xl)) {
    .reactive-width {
        max-width: map-get($widths, lg);
    }
    .locate-wrapper {
        display: grid;
        grid-template-columns: 4fr 1fr 4fr;
    }
}

@media only screen and (min-width: map-get($grid-breakpoints, xl)) and (max-width: map-get($grid-breakpoints, xxl)) {
    .reactive-width {
        max-width: map-get($widths, xl);
    }
    .locate-wrapper {
        display: grid;
        grid-template-columns: 5fr 1fr 5fr;
    }
}

@media only screen and (min-width: map-get($grid-breakpoints, xxl)) {
    .reactive-width {
        max-width: map-get($widths, xxl);
    }
    .locate-wrapper {
        display: grid;
        grid-template-columns: 6fr 1fr 6fr;
    }
}

$map-breakpoints: (
    sm: 311px,
    md: 436px,
    lg: 726px,
);

.top-wrapper {
    padding-bottom: 0.5rem;
}

.text-bold {
    display: inline-block;
    font-weight: bold;
    white-space: pre;
}

#result-map-wrapper {
    margin: 1rem 0;
}

@media only screen and (min-height: map-get($map-breakpoints, sm)) and (max-height: map-get($map-breakpoints, md)) {
    #map-wrapper {
        height: 30vh;
    }
    #result-map-wrapper {
        height: 32vh;
    }
}

@media only screen and (min-height: map-get($map-breakpoints, md)) and (max-height: map-get($map-breakpoints, lg)) {
    #map-wrapper {
        height: 50vh;
    }
    #result-map-wrapper {
        height: 51.4vh;
    }
}

@media only screen and (min-height: map-get($map-breakpoints, lg)) {
    #map-wrapper {
        height: 70vh;
    }
    #result-map-wrapper {
        height: 70.8vh;
    }
}

/* Unused, from google maps API example*/
.custom-map-control-button {
    background-color: #fff;
    border: 0;
    border-radius: 2px;
    box-shadow: 0 1px 4px -1px rgba(0, 0, 0, 0.3);
    margin: 10px;
    padding: 0 0.5em;
    font: 400 18px Roboto, Arial, sans-serif;
    overflow: hidden;
    height: 40px;
    cursor: pointer;
}

.custom-map-control-button:hover {
    background: #ebebeb;
}
</style>
