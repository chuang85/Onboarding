var gulp = require('gulp');
var durandal = require('gulp-durandal');

gulp.task('default', function() {
    durandal({
            baseDir: 'App',
            main: 'main.js',
            output: 'main-built.js',
            almond: true,
            minify: true
        }).
        pipe(gulp.dest('App'));
});