<template>
    <div class="reactive-width backdrop">
        <div v-if="final">
            You picked
            <div class="text-bold">
                {{ final.Name }}
            </div>
            at
            <div class="text-bold">
                {{ final.Vicinity }}
            </div>
            !
            <div id="result-map-wrapper">
                <div class="map" id="result-map">
                    Something went wrong :( <br />
                    Try reloading?
                </div>
            </div>
            Don't like your pick? Click the title at the top to go again.
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
export default defineComponent({
    data() {
        return {
            map: (window as any).google.maps,
        };
    },
    props: {
        final: {
            type: Object,
            required: true,
        },
    },
    name: "Result",
    mounted() {
        let map = new (window as any).google.maps.Map(
            document.getElementById("result-map"),
            {
                center: { lat: this.final.Lat, lng: this.final.Lng },
                zoom: 18,
            }
        );
        this.map = map;
    },
});
</script>
