"use strict";

module.exports = function (configs) {
    var bower = require("gulp-bower");
    var gulp = require("gulp");
    var path = require("path");
    var runSequence = require("run-sequence");

    gulp.task("bower:install", function () {
        return bower({}, "--force" );
    });

    gulp.task("compass:copy", function () {
        return gulp.src(path.join(process.cwd(), configs.bower, "compass-mixins/lib/**"))
                   .pipe(gulp.dest(path.join(process.cwd(), configs.root, configs.styles, "Compass")));
    });

    gulp.task("libraries:install", function (done) {
        runSequence("bower:install", "compass:copy", done);
    });
};