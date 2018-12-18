/// <vs SolutionOpened='modules:compile, clean' />
(function (require) {
    "use strict";

    var gulp = require("gulp");
    var path = require("path");
    var rimraf = require("rimraf");
    var runSequence = require("run-sequence");
    var util = require("gulp-util");
    var wrench = require("wrench");

    var configs = {
        gulpDir: "Gulp",
        bower: "bower_components",
        root: "CN88",
        scripts: "Scripts",
        styles: "Styles",
        publish: "Publish",
        templatesDir: "../CN88.CustomerSetting.WebApi/App_Data/Templates",
        staticResourceProject: "../CN88.CustomerSetting.StaticResources/",
        configFiles: {
            bower: "bower.json",
            tslint: "tslint.json",
            tsd: "gulp-tsd.json",
            productsConfig: "products-config.json",
            karmaConfig: "karma-config.js"
        },
        errorHandler: function (title) {
            return function (error) {
                util.log(util.colors.red("[" + title + "]"), error.toString());
                this.emit("end");
            };
        }
    };

    wrench.readdirSyncRecursive(configs.gulpDir).filter(function (file) {
        return (/\.js$/i).test(file);
    }).map(function (file) {
        require(path.join(process.cwd(), configs.gulpDir, file))(configs);
    });

    gulp.task("clean", function (callback) {
        rimraf(path.join(process.cwd(), configs.root, configs.publish), callback);
    });

    gulp.task("install", ["libraries:install", "tsd:install"]);

    gulp.task("copy-packages", function () {
        return gulp.src(path.join(process.cwd(), configs.root, configs.publish, "**/**"))
            .pipe(gulp.dest(configs.staticResourceProject));
    });

    gulp.task("publish", ["clean"], function (callback) {
        runSequence("styles:publish", "scripts:publish", "copy-packages", callback);
    });

    gulp.task("default", ["install"], function () {
        gulp.start("publish");
    });
})(require);