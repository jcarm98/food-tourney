<template>
    <div class="reactive-width backdrop">
        Click the Find Me button to get started. <br />
        Enter your address or zip code, or leave it blank to use your location.
        <br />
        <div class="locate-wrapper">
            <input
                class="clean-input"
                v-model="address"
                placeholder="54321 Hollywood Boulevard"
            />

            <button class="button-indigo" @click="findUser()" v-if="!next">
                Find Me!
            </button>

            <button class="button-orange" @click="getPopular()" v-if="next">
                Next->
            </button>
        </div>
        <div id="map-wrapper">
            <div id="my-map" class="map">
                Something went wrong :( <br />
                Try reloading?
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Request from "../services/Request";

export default defineComponent({
    emits: [
        "update-option1",
        "update-option2",
        "update-places",
        "update-popular",
        "update-pos",
        "update-state",
        "update-zip",
    ],
    name: "Start",
    data() {
        return {
            apiKey: process.env.VUE_APP_GOOGLE_MAPS_API_KEY,
            address: "",
            infoWindow: (window as any).google.maps.InfoWindow,
            map: (window as any).google.maps,
            next: false,
            placesLocal: {},
            posLocal: { lat: 0, lng: 0 },
            request: new Request().request,
            zipLocal: -1,
        };
    },
    props: {
        places: {
            type: Object,
            required: true,
        },
        pos: {
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
    mounted() {
        let map = new (window as any).google.maps.Map(
            document.getElementById("my-map"),
            {
                center: { lat: 33.344, lng: -117.036 },
                zoom: 9,
            }
        );
        this.infoWindow = new (window as any).google.maps.InfoWindow();
        this.map = map;
    },
    computed: {
        safeAddress(): string {
            return this.address.replace(/ /g, "+");
        },
    },
    methods: {
        updateOption1(option1: string): void {
            this.$emit("update-option1", option1);
        },
        updateOption2(option2: string): void {
            this.$emit("update-option2", option2);
        },
        updatePlaces(places: any): void {
            this.placesLocal = places;
            this.$emit("update-places", places);
        },
        updatePopular(popular: any): void {
            this.$emit("update-popular", popular);
        },
        updatePos(pos: { lat: number; lng: number }): void {
            this.posLocal = pos;
            this.$emit("update-pos", pos);
        },
        updateState(state: string): void {
            this.$emit("update-state", state);
        },
        updateZip(zip: number): void {
            this.zipLocal = zip;
            this.$emit("update-zip", zip);
        },
        commitPan(pos: { lat: number; lng: number }): void {
            this.updatePos(pos);
            this.infoWindow.setPosition(pos);
            this.infoWindow.setContent("Location found.");
            this.infoWindow.open(this.map);
            this.map.setCenter(pos);
            this.map.setZoom(13);
            this.next = true;

            this.getPlaces();
        },
        findUser(): void {
            if (this.safeAddress === "") {
                this.panToUser();
                return;
            }
            let url =
                "https://maps.googleapis.com/maps/api/geocode/json?" +
                "address=" +
                this.safeAddress +
                "&key=" +
                this.apiKey;
            this.request("GET", url, "", (response: string) => {
                let res = JSON.parse(response).results;
                this.commitPan(res[0].geometry.location);
            });
        },
        panToUser(): void {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    // GeolocationPosition / Position
                    // TypeScript 4.1       4.0
                    (position: any) => {
                        const pos = {
                            lat: position.coords.latitude,
                            lng: position.coords.longitude,
                        };
                        this.commitPan(pos);
                    },
                    () => {
                        this.handleLocationError(
                            true,
                            this.infoWindow,
                            this.map.getCenter()!
                        );
                    }
                );
            } else {
                this.handleLocationError(
                    false,
                    this.infoWindow,
                    this.map.getCenter()!
                );
            }
        },
        handleLocationError(
            browserHasGeolocation: any,
            infoWindow: any,
            pos: any
        ): void {
            infoWindow.setPosition(pos);
            infoWindow.setContent(
                browserHasGeolocation
                    ? "Error: The Geolocation service failed."
                    : "Error: Your browser doesn't support geolocation."
            );
            infoWindow.open(this.map);
        },
        getPlaces(): void {
            //console.log(process.env.VUE_APP_HOST);
		let url =
                process.env.VUE_APP_HOST + "/places/?lat=" +
                this.posLocal.lat +
                "&lng=" +
                this.posLocal.lng;
            this.request("GET", url, "", (response: string) => {
                this.updatePlaces(JSON.parse(response));
                let keys = Object.keys(this.placesLocal);
                let num1 = Math.trunc(Math.random() * keys.length);
                let num2 = Math.trunc(Math.random() * keys.length);
                while (num2 === num1) {
                    num2 = Math.trunc(Math.random() * keys.length);
                }
                this.updateOption1(keys[num1]);
                this.updateOption2(keys[num2]);
            });
            this.getZIP();
        },
        getZIP(): void {
            if (this.zip != "-1") {
                return;
            }
            let url =
                "https://maps.googleapis.com/maps/api/geocode/json?latlng=" +
                this.posLocal.lat +
                "," +
                this.posLocal.lng +
                "&key=" +
                this.apiKey;
		//console.log(url, process.env.VUE_APP_GOOGLE_MAPS_API_KEY);
            this.request("GET", url, "", (response: string) => {
                var res = JSON.parse(response).results;
                var i;
                for (i = 0; i < Object.keys(res[0].address_components).length; ++i) {
                    if (
                        res[0].address_components[i].types[0] === "postal_code"
                    ) {
                        break;
                    }
                }
                this.updateZip(res[0].address_components[i].short_name);

                //this.getPopular();
            });
        },
        getPopular(): void {
            if (this.zipLocal === -1 || Object.keys(this.placesLocal).length === 0) {
                setTimeout(this.getPopular, 10);
                return;
            }
            let url =
                process.env.VUE_APP_HOST + "/popular?zip=" + this.zipLocal;
            this.request("GET", url, "", (response: string) => {
                let popular = JSON.parse(response);
                if (popular.placeId != -1) {
                    popular.Name = popular.name;
                    popular.PlaceId = popular.placeId;
                    popular.Lat = popular.lat;
                    popular.Lng = popular.lng;
                    popular.Vicinity = popular.vicinity;

                    this.updatePopular(popular);
                    //this.$router.push("/popular");
                    this.updateState("popular");
                } else {
                    //this.$router.push("/tournament");
                    this.updateState("tournament");
                }
            });
        },
    },
});
</script>

