"use strict";

module.exports = function (configs) {
    //require("typescript-require");

    var concat = require("gulp-concat");
    var gulp = require("gulp");
    var merge = require("merge-stream");
    var minifyHTML = require("gulp-minify-html");
    var path = require("path");
    var rename = require("gulp-rename");
    var runSequence = require("run-sequence");
    var templateCache = require("gulp-angular-templatecache");
    var ts = require("gulp-typescript");
    var uglify = require("gulp-uglify");

    var tsCompilingConfig = {
        typescript: require("typescript"),
        target: "ES5",
        noImplicitAny: false,
        removeComments: true
    };

    var sourceDir = path.join(process.cwd(), configs.root, configs.scripts);
    var destinationDir = path.join(process.cwd(), configs.root, configs.publish);
    var tempDir = path.join(process.cwd(), configs.root, "Temp");
    var styleResources = [
        path.join(process.cwd(), configs.root, configs.styles, "customer-setting.{*.map,css,*.css}"),
        path.join(process.cwd(), configs.root, configs.styles, "flexbox-patch.{*.map,css,*.css}")
    ];

    gulp.task("product:scripts:compile", function () {
        var productsConfigs = require(path.join(process.cwd(), configs.gulpDir, configs.configFiles.productsConfig));
        var streams = [];

        productsConfigs.forEach(function (prodConfig) {
            var productTempDir = path.join(tempDir, "product-" + prodConfig.productId);
            var tsSources = [
                path.join(sourceDir, "Core/**/*.ts")
            ];
            var htmSources = [
                path.join(sourceDir, "Core/**/*.htm"),
                path.join(sourceDir, "!customer-setting.htm")
            ];

            if (prodConfig.settingTypes.length > 0) {
                prodConfig.settingTypes.forEach(function (item) {
                    tsSources.push(path.join(sourceDir, "SettingType" + item, "**/*.ts"));
                    htmSources.push(path.join(sourceDir, "SettingType" + item, "**/*.htm"));
                });
            }

            gulp.src(tsSources)
                .pipe(ts(tsCompilingConfig))
                .pipe(concat("1js-" + prodConfig.productId + ".js"))
                .pipe(gulp.dest(productTempDir));
            gulp.src(htmSources)
                .pipe(minifyHTML({ empty: true, quotes: true }))
                .pipe(templateCache({
                    module: "customerSetting"
                }))
                .pipe(concat("2htm-" + prodConfig.productId + ".js"))
                .pipe(gulp.dest(productTempDir));
            streams.push(gulp.src(path.join(productTempDir, "*.js"))
                .pipe(concat("product-" + prodConfig.productId + ".js"))
                .pipe(gulp.dest(tempDir)));
        });

        return merge(streams);
    });

    gulp.task("product:js:combine", ["product:scripts:compile"], function () {
        var productsConfigs = require(path.join(process.cwd(), configs.gulpDir, configs.configFiles.productsConfig));
        var libPaths = [
            path.join(process.cwd(), configs.bower, "angular/angular.js"),
            path.join(process.cwd(), configs.root, "Libraries/**/*.js")
        ];
        var streams = [];

        productsConfigs.forEach(function (prodConfig) {
            var sources = libPaths.concat([
                path.join(tempDir, "product-" + prodConfig.productId + ".js"),
                path.join(sourceDir, "Core/**/*.js")
            ]);
            var destination = path.join(destinationDir, configs.scripts);

            if (prodConfig.settingTypes.length > 0) {
                prodConfig.settingTypes.forEach(function (item) {
                    sources.push(path.join(sourceDir, "SettingType" + item, "**/*.js"));
                });
            }

            streams.push(gulp.src(sources)
                .pipe(concat("product-" + prodConfig.productId + ".js"))
                .pipe(gulp.dest(destination))
                .pipe(uglify())
                .pipe(rename({ suffix: ".min" }))
                .pipe(gulp.dest(destination)));
        });

        return merge(streams);
    });

    gulp.task("template:copy", function () {
        return gulp.src(path.join(sourceDir, "*.htm"))
            .pipe(minifyHTML({ conditionals: true, empty: true, quotes: true }))
            .pipe(gulp.dest(path.join(process.cwd(), configs.templatesDir)));
    });

    gulp.task("scripts:publish", function () {
        runSequence("product:js:combine", "template:copy");
    });

    gulp.task("images:publish", function () {
        return gulp.src(path.join(process.cwd(), configs.root, configs.styles, "Images/*.*"))
            .pipe(gulp.dest(path.join(destinationDir, configs.styles, "Images")));
    });

    gulp.task("styles:publish", ["images:publish"], function () {
        return gulp.src(styleResources)
            .pipe(gulp.dest(path.join(destinationDir, configs.styles)));
    });
};