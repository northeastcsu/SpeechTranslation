import { createWebHistory, createRouter } from "vue-router";
import HomeComp from "@/components/Home.vue";
import HostComp from "@/components/Host.vue";
import ConsumerComp from "@/components/Consumer.vue";


const routes = [
  {
    path: "/",
    name: "Home",
    component: HomeComp,
  },
  {
    path: "/host",
    name: "Host",
    component: HostComp,
  },
  {
    path: "/consumer",
    name: "Consumer",
    component: ConsumerComp,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;