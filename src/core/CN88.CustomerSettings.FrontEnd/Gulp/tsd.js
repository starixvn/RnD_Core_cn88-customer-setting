"use strict";

module.exports = function (configs) {
    var gulp = require("gulp");
    var path = require("path");
    var tsd = require("gulp-tsd");

    gulp.task("tsd:install", function () {
        return gulp.src(path.join(process.cwd(), configs.gulpDir, configs.configFiles.tsd))
                   .pipe(tsd());
    });
};