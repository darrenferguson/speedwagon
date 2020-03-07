var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    rename = require('gulp-rename');

gulp.task('css', function () {
    return gulp.src(
        ['./node_modules/bootstrap/dist/css/bootstrap.min.css','./wwwroot/speedwagon/css/speedwagon.css']
    ).pipe(concat('site.min.css'))
        .pipe(rename('speedwagon.min.css'))
        .pipe(gulp.dest('./wwwroot/speedwagon/dist'));
});

gulp.task('js', function () {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/bootstrap/dist/js/bootstrap.min.js',
    ])
        .pipe(concat('speedwagon.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./wwwroot/speedwagon/dist'));
});

gulp.task('tiny', function () {
    return gulp.src([
        './node_modules/tinymce/tinymce.min.js',
        './node_modules/tinymce/jquery.tinymce.min.js',
        './node_modules/tinymce/themes/silver/**/*',
        './node_modules/tinymce/plugins/**/*',
        './node_modules/tinymce/skins/**/*'
    ], { base: './node_modules/tinymce/' })
    .pipe(gulp.dest('./wwwroot/speedwagon/lib/tinymce'));
});

gulp.task('file-upload', function () {
    return gulp.src([
        './node_modules/blueimp-file-upload/**/*'
    ], { base: './node_modules/blueimp-file-upload/' })
        .pipe(gulp.dest('./wwwroot/speedwagon/lib/blueimp-file-upload'));
});

gulp.task('build', gulp.series('css', 'js', 'tiny', 'file-upload'));