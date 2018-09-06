$(() => {

	$('[data-marker]').each((index, el) => {
		var data = $(el).data('marker');

		$.typeahead({
			input: '#beginVersion',
			minLength: 0,
			searchOnFocus: true,
			source: {
				groupName: {
					data: data
				}
			}
		});

		$.typeahead({
			input: '#endVersion',
			minLength: 0,
			searchOnFocus: true,
			source: {
				groupName: {
					data: data
				}
			}
		});
	});
});

