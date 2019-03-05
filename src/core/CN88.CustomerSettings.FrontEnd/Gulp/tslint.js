"use strict";

module.exports = function (configs) {
    var gulp = require("gulp");
    var path = require("path");
    var tslint = require("gulp-tslint");

    gulp.task("tslint:check", function () {
        var sourceDir = [
            path.join(process.cwd(), configs.root, configs.scripts, "Core/**/*.ts"),
            path.join(process.cwd(), configs.root, configs.scripts, "SettingType1/**/*.ts"),
            path.join(process.cwd(), configs.root, configs.scripts, "SettingType2/**/*.ts")
        ];

        return gulp.src(sourceDir)
                   .pipe(tslint({
                       configuration: path.join(process.cwd(), configs.gulpDir, configs.configFiles.tslint)
                   }))
                   .pipe(tslint.report("prose"));
    });
};