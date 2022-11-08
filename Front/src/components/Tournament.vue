<template>
    <div class="reactive-width backdrop">
        <div class="top-wrapper">
            Pick the place you prefer until there's only one left.
            <br />
            ( Or pick the place you hate least. )
        </div>
        <div
            class="tournament-view"
            v-if="option1 in places1 && option2 in places1"
        >
            <OptionButton
                :label="places1[option1].Name"
                :clickEvent="updateList"
                first="true"
            ></OptionButton>
            <div class="super-center">or</div>
            <OptionButton
                :label="places1[option2].Name"
                :clickEvent="updateList"
                first="false"
            ></OptionButton>

            <OptionAddress :address="places1[option1].Vicinity"></OptionAddress>
            <div></div>
            <OptionAddress :address="places1[option2].Vicinity"></OptionAddress>

            <OptionRatings
                :rating="places1[option1].Rating"
                :amount="places1[option1].TotalRatings"
            ></OptionRatings>
            <div></div>
            <OptionRatings
                :rating="places1[option2].Rating"
                :amount="places1[option2].TotalRatings"
            ></OptionRatings>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent, h } from "vue";
import Request from "../services/Request";

const OptionButton = defineComponent({
    props: ["label", "clickEvent", "first"],
    render() {
        return h(
            "button",
            {
                class: "button-indigo",
                onClick: (event: any) => {
                    this.clickEvent(this.first);
                    event.target.blur();
                },
            },
            this.label
        );
    },
});

const OptionAddress = defineComponent({
    props: ["address"],
    render() {
        return h("ul", { class: "text-left" }, [
            h("li", [
                h("div", { class: "text-bold" }, ["Address: "]),
                this.address,
            ]),
        ]);
    },
});

const OptionRatings = defineComponent({
    props: ["rating", "amount"],
    render() {
        return h("ul", { class: "text-left" }, [
            h("li", [
                h("div", { class: "text-bold" }, "Rating: "),
                this.rating,
                " with ",
                this.amount,
                " ratings",
            ]),
        ]);
    },
});

export default defineComponent({
    data() {
        return {
            option1: "",
            option2: "",
            request: new Request().request,
            places1: {} as any,
            places2: {} as any,
            finalLocal: {} as any,
        };
    },
    props: {
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
    name: "Tournament",
    beforeMount() {
        this.places1 = JSON.parse(JSON.stringify(this.places));
        this.pickOptions();
    },
    components: {
        OptionButton,
        OptionAddress,
        OptionRatings,
    },
    methods: {
        updateFinal(final: any): void {
            this.finalLocal = final;
            this.$emit("update-final", final);
        },
        updateState(state: string): void {
            this.$emit("update-state", state);
        },
        pickOptions(): void {
            // Pick two new keys
            let keys = Object.keys(this.places1);
            let num1 = Math.trunc(Math.random() * keys.length);
            let num2 = Math.trunc(Math.random() * keys.length);
            while (num2 === num1) {
                num2 = Math.trunc(Math.random() * keys.length);
            }
            this.option1 = keys[num1];
            this.option2 = keys[num2];
        },
        updateList(isFirst: boolean): void {
            // Delete the entry not clicked
            // Store clicked entry
            if (isFirst) {
                delete this.places1[this.option2];
                this.places2[this.option1] = JSON.parse(
                    JSON.stringify(this.places1[this.option1])
                );
                delete this.places1[this.option1];
            } else {
                delete this.places1[this.option1];
                this.places2[this.option2] = JSON.parse(
                    JSON.stringify(this.places1[this.option2])
                );
                delete this.places1[this.option2];
            }

            // Update lists
            if (
                Object.keys(this.places1).length === 0 &&
                Object.keys(this.places2).length > 1
            ) {
                this.places1 = this.places2;
                this.places2 = {};
            } else if (Object.keys(this.places1).length === 1) {
                if (Object.keys(this.places2).length === 1) {
                    this.places1[Object.keys(this.places2)[0]] =
                        this.places2[Object.keys(this.places2)[0]];
                    this.places2 = {};
                } else if (Object.keys(this.places2).length > 1) {
                    this.places2[Object.keys(this.places1)[0]] =
                        this.places1[Object.keys(this.places1)[0]];
                    this.places1 = this.places2;
                    this.places2 = {};
                }
            }

            // Last option remaining is result
            if (
                (Object.keys(this.places1).length === 1 &&
                    Object.keys(this.places2).length === 0) ||
                (Object.keys(this.places1).length === 0 &&
                    Object.keys(this.places2).length === 1)
            ) {
                let finalObject =
                    Object.keys(this.places1).length === 1
                        ? Object.values(this.places1)[0]
                        : Object.values(this.places2)[0];

                this.updateFinal(finalObject);
                this.updateState("result");
                this.request(
                    "POST",
                    process.env.VUE_APP_HOST + "/tally/" + this.zip,
                    JSON.stringify(this.finalLocal),
                    (response: string) => {
                        console.log(response);
                    }
                );
                return;
            }
            this.pickOptions();
        },
    },
});
</script>

<style scoped lang="scss">
.super-center {
    align-items: center;
    display: flex;
    justify-content: center;
}

.text-left {
    text-align: left;
}

.tournament-view {
    display: grid;
    grid-template-columns: 5fr 1fr 5fr;
}
</style>
