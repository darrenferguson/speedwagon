$(function () {

    $('.upload').fileupload({
        dataType: 'json',
        progressall: function (e, data) {
            $(e.target.parentElement).find('div.progress').show();
            var bar = $(e.target.parentElement).find('div.progress div.bar');
            var progress = parseInt(data.loaded / data.total * 100, 10);

            

            $(bar).css(
                'width', progress + '%'
            );
        },
        done: function (e, data) {

            var items = $(e.target.parentElement).find('ul');
            var input = $(e.target.parentElement).find('input[type=hidden]');
            $.each(data.result.files, function (index, file) {
                $(items).append('<li class="list-group-item">' + file + '</li>');

                var filtered = $(input).val().split(',').filter(function (el) {
                    return el != null && el != '';
                });

                filtered.push(file);

                $(input).val(filtered.join(','));
            });
        }
    });
});
