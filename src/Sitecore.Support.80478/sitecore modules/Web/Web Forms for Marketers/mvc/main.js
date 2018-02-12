//WFFM Main Script
//================
function getFxmHost() {
  if (typeof (SCBeacon) != 'undefined') {
    if (typeof (SCBeacon.fxmHost) != 'undefined') {
      return SCBeacon.fxmHost;
    }
  }
  return null;
}

function generateBaseUrl() {
  return getFxmHost() ? getFxmHost() + "/" : "/";
}

if (typeof require == "undefined") {
  //workaround for rendering on FXM side. It allow to load "require" script before setting function for it
  var fileref = document.createElement('script');
  fileref.setAttribute("type", "text/javascript");
  fileref.setAttribute("src", getFxmHost() + "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/require-2.1.15.min.js");
  fileref.onload = configureRequire;
} else {
  configureRequire();
}

function configureRequire() {
  require.config(
  {
    baseUrl: generateBaseUrl(),
    paths: {
      jquery: "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/libs/jquery/jquery-2.1.3.min",
      jquery_ui: "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/libs/jquery/jquery-ui-1.11.3.min",
      jquery_validate: "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/libs/jquery/jquery.validate.min",
      jquery_validate_unobtrusive: "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/libs/jquery/jquery.validate.unobtrusive.min",
      bootstrap: "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/libs/bootstrap/bootstrap.min",
      wffm: "sitecore%20modules/Web/Web%20Forms%20for%20Marketers/mvc/wffm.min"
    },
    waitSeconds: 200,
    shim: {
      "bootstrap": {
        deps: ["jquery"]
      },
      "jquery_ui": {
        deps: ["jquery"]
      },
      "jquery_validate": {
        deps: ["jquery"]
      },
      "jquery_validate_unobtrusive": {
        deps: ["jquery", "jquery_validate"]
      },
      "wffm": {
        deps: ["jquery", "jquery_ui", "jquery_validate", "jquery_validate_unobtrusive", "bootstrap"]
      }
    }
  });

  require(["jquery", "wffm"], function ($scw) {
    $scw(document).ready(function () {
      $scw("form[data-wffm]").each(function () { $scw(this).wffmForm(); });
    });
  });
};