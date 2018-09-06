const gulp = require('gulp');
const merge = require('merge-stream');

gulp.task("scripts", () => {
	const jquery = gulp.src('node_modules/jquery/dist/**')
		.pipe(gulp.dest('wwwroot/vendor/jquery'));

	const jqueryTypeahead = gulp.src('node_modules/jquery-typeahead/dist/**')
		.pipe(gulp.dest('wwwroot/vendor/jquery-typeahead'));

	return merge(jquery, jqueryTypeahead);
});

gulp.task('default', ['scripts']);