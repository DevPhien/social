/* global process */
import angular from "angular";
import ngAria from "angular-aria";
import ngRoute from "angular-route";
import ngSanitize from "angular-sanitize";
import uiRouter from "angular-ui-router";
import moment from 'moment';
import "lodash";
import "restangular";

import "../styles/main.css";

import config from "app.config";

import appConfig from "./app.config";
import appRoute from "./app.route";

// component
import appComponent from "./app.component";
import socialComponent from "./modules/social/social.component";

export default angular
  .module("social", [ngAria, ngRoute, ngSanitize, "restangular", uiRouter])
  // config
  .config(appConfig)
  .config(appRoute)
  // .config([
  //   "moment",
  //   function (moment) {
  //     moment.locale("en");
  //   },
  // ])
  // constant
  .constant("CONFIG", config)
  .constant("ENVIRONNEMENT", process.env.ENV_NAME)
  .constant("API_CONFIG", "https://localhost:7118")
  .constant('moment', moment)
  // component
  .component("app", appComponent)
  .component("social", socialComponent)
  .directive("fileChange", fileChange).name;

function fileChange() {
  return {
    restrict: "A",
    require: "ngModel",
    scope: {
      fileChange: "&",
    },
    link: function link(scope, element, attrs, ctrl) {
      element.on("change", onChange);

      scope.$on("destroy", function () {
        element.off("change", onChange);
      });

      function onChange() {
        attrs.multiple
          ? ctrl.$setViewValue(element[0].files)
          : ctrl.$setViewValue(element[0].files[0]);
        scope.$apply(function () {
          scope.fileChange();
        });
      }
    },
  };
}
