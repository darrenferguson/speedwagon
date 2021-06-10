var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify')

gulp.task('themeCss', function () {
    return gulp.src(
        ['./node_modules/bootstrap/dist/css/bootstrap.min.css',
            '../theme/clean-blog.min.css',
            './node_modules/@fortawesome/fontawesome-free/css/all.min.css']
    ).pipe(concat('theme.min.css'))
    .pipe(gulp.dest('./wwwroot/theme'));
});

gulp.task('themeJs', function () {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/bootstrap/dist/js/bootstrap.bundle.min.js',
        '../theme/clean-blog.min.js'
    ])
        .pipe(concat('theme.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./wwwroot/theme'));
});

gulp.task('img', function () {
    return gulp.src([
        '../theme/**/*.jpg'

    ], { base: '../theme/' })
        .pipe(gulp.dest('./wwwroot/theme'));
});

gulp.task('themeFont', function () {
    return gulp.src([
        './node_modules/@fortawesome/fontawesome-free/webfonts/**/*'

    ], { base: './node_modules/@fortawesome/fontawesome-free/webfonts' })
        .pipe(gulp.dest('./wwwroot/webfonts'));
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

gulp.task('build',
    gulp.series(
        'themeCss',
        'themeJs',
        'img',
        'themeFont',
        'tiny',
        'file-upload'
    )
);