import {createRouter, createWebHistory, RouteRecordRaw} from "vue-router";
import {useAuthStore} from "@/stores/authStore.ts";

const routes: RouteRecordRaw[] = [
    {
        path: '/',
        redirect: '/welcome',
    },
    {
        path: '/login',
        name: 'Login',
        component: () => import('@/views/Login.vue'),
    },
    {
        path: '/welcome',
        name: 'Welcome',
        component: () => import('@/views/Welcome.vue'),
        meta: { requiresAuth: true },
    },
    {
        path: '/users',
        name: 'Users',
        component: () => import('@/views/Users.vue'),
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

router.beforeEach((to, from, next) => {
    const authStore = useAuthStore();
    if (to.meta.requiresAuth && !authStore.isAuthenticated) {
        next({ name: 'Login' });
    } else {
        next();
    }
});

export default router;