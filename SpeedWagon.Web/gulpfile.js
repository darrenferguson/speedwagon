var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    rename = require('gulp-rename');

gulp.task('css', function () {
    return gulp.src(
        ['./node_modules/bootstrap/dist/css/bootstrap.min.css',
            './node_modules/@fortawesome/fontawesome-free/css/all.min.css',
            './Client/speedwagon.css']
    ).pipe(concat('site.min.css'))
        .pipe(rename('speedwagon.min.css'))
        .pipe(gulp.dest('./wwwroot/speedwagon'));
});

gulp.task('js', function () {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/bootstrap/dist/js/bootstrap.min.js',
        './node_modules/feather-icons/dist/feather.min.js',
    ])
        .pipe(concat('speedwagon.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./wwwroot/speedwagon'));
});

gulp.task('font', function () {
    return gulp.src([
        './node_modules/@fortawesome/fontawesome-free/webfonts/**/*'

    ], { base: './node_modules/@fortawesome/fontawesome-free/webfonts' })
        .pipe(gulp.dest('./wwwroot/speedwagon/font'));
});

gulp.task('dist', function() {
    return gulp.src(['./wwwroot/speedwagon/**/*']).pipe(gulp.dest('../SpeedWagon.Web.UI/wwwroot/speedwagon'));
})

gulp.task('build',
    gulp.series('css', 'js', 'font', 'dist')
);