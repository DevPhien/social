/**
 * Application route definition.
 */
export default function ($stateProvider, $urlRouterProvider) {
  "ngInject";

  $stateProvider
    .state("app", {
      url: "/",
      component: "app",
    })
    .state("social", {
      url: "/social",
      component: "social",
    });

  $urlRouterProvider.otherwise("/");
}
