var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    rename = require('gulp-rename');
    cleanCSS = require('gulp-clean-css');
    del = require('del');
    replace = require('gulp-replace');

gulp.task('clean', function (done) {
    del.sync('./dist');
    del.sync('../SpeedWagon.Web.UI/wwwroot/speedwagon', { force: true });
    del.sync('../SpeedWagon.Web.UI/wwwroot/theme', { force: true });
    done();
});

gulp.task('css', function () {
    return gulp.src([
        './node_modules/bootstrap/dist/css/bootstrap.min.css',
        './node_modules/@fortawesome/fontawesome-free/css/all.min.css',
        './src/speedwagon/speedwagon.css']
    ).pipe(concat('site.min.css'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(rename('speedwagon.min.css'))
        .pipe(gulp.dest('./dist/speedwagon'));
});

gulp.task('js', function () {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/bootstrap/dist/js/bootstrap.min.js',
        './node_modules/feather-icons/dist/feather.min.js',
    ])
        .pipe(concat('speedwagon.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./dist/speedwagon'));
});

gulp.task('copyEditor', function () {
    return gulp.src(['./src/speedwagon/editor/**/*']).pipe(gulp.dest('./dist/speedwagon/editor'));
});
gulp.task('copyLib', function () {
    return gulp.src(['./src/speedwagon/lib/**/*']).pipe(gulp.dest('./dist/speedwagon/lib'));
});

gulp.task('font', function () {
    return gulp.src([
        './node_modules/@fortawesome/fontawesome-free/webfonts/**/*'

    ], { base: './node_modules/@fortawesome/fontawesome-free/webfonts' })
        .pipe(gulp.dest('./dist/speedwagon/font'));
});

gulp.task('themeCss', function () {
    return gulp.src(
        ['./node_modules/bootstrap/dist/css/bootstrap.min.css',
            '../theme/clean-blog.min.css',
            './node_modules/@fortawesome/fontawesome-free/css/all.min.css']
    ).pipe(concat('theme.min.css'))
        .pipe(replace('../webfonts', '/theme/webfonts'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(gulp.dest('./dist/theme'));
});

gulp.task('themeJs', function () {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/bootstrap/dist/js/bootstrap.bundle.min.js',
        '../theme/clean-blog.min.js'
    ])
        .pipe(concat('theme.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./dist/theme'));
});



gulp.task('img', function () {
    return gulp.src([
        './src/theme/**/*.jpg'

    ], { base: './src/theme' })
        .pipe(gulp.dest('./dist/theme'));
});

gulp.task('themeFont', function () {
    return gulp.src([
        './node_modules/@fortawesome/fontawesome-free/webfonts/**/*'

    ], { base: './node_modules/@fortawesome/fontawesome-free/webfonts' })
        .pipe(gulp.dest('./dist/theme/webfonts'));
});

gulp.task('tiny', function () {
    return gulp.src([
        './node_modules/tinymce/tinymce.min.js',
        './node_modules/tinymce/jquery.tinymce.min.js',
        './node_modules/tinymce/themes/silver/**/*',
        './node_modules/tinymce/plugins/**/*',
        './node_modules/tinymce/skins/**/*',
        './node_modules/tinymce/icons/**/*'
    ], { base: './node_modules/tinymce/' })
        .pipe(gulp.dest('./dist/speedwagon/lib/tinymce'));
});

gulp.task('file-upload', function () {
    return gulp.src([
        './node_modules/blueimp-file-upload/**/*'
    ], { base: './node_modules/blueimp-file-upload/' })
        .pipe(gulp.dest('./dist/speedwagon/lib/blueimp-file-upload'));
});

gulp.task('dist', function () {
    return gulp.src(['./dist/**/*']).pipe(gulp.dest('../SpeedWagon.Web.UI/wwwroot'));
});

gulp.task('build',
    gulp.series(
        'clean',
        'css',
        'js',
        'font',
        'themeCss',
        'themeJs',
        'img',
        'themeFont',
        'tiny',
        'file-upload',
        'copyEditor',
        'copyLib',
        'dist')
);