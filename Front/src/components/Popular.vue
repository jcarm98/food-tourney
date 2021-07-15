<template>
    <div class="reactive-width backdrop">
        <div v-if="popular.placeId != -1">
            <div class="top-wrapper">
                <button class="button-indigo" @click="pickPopular()">
                    {{ popular.Name }}
                </button>
                is popular in your area: {{ zip }} <br />
            </div>
            Or go on to the
            <button class="button-indigo" @click="toTournament()">
                Tournament->
            </button>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Request from "../services/Request";

export default defineComponent({
    name: "Popular",
    data() {
        return {
            request: new Request().request,
        };
    },
    props: {
        final: {
            type: Object,
            required: true,
        },
        popular: {
            type: Object,
            required: true,
        },
        places: {
            type: Object,
            required: true,
        },
        state: {
            type: String,
            required: true,
        },
        zip: {
            type: String,
            required: true,
        },
    },
    methods: {
        updateFinal(final: any): void {
            this.$emit("update-final", final);
        },
        updateState(state: string): void {
            this.$emit("update-state", state);
        },
        pickPopular(): void {
            this.updateFinal(this.popular);
            this.updateState("result");
            this.request(
                "POST",
                "https://foodtourney.app:5000/tally/" + this.zip,
                JSON.stringify(this.popular),
                (response: string) => {
                    console.log(response);
                }
            );
        },
        toTournament(): void {
            if (Object.keys(this.places).length != 0) {
                this.updateState("tournament");
            }
        },
    },
});
</script>
