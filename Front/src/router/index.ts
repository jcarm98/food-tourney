import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Top from "../views/Top.vue";

const routes: Array<RouteRecordRaw> = [
    {
        path: "/",
        name: "Top",
        component: Top,
    },
    {
        path: "/:pathMatch(.*)*",
        name: "Wildcard",
        component: Top,
    },
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes,
});

export default router;
