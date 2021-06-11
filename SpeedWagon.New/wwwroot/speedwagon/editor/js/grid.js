$(function () {

    

    

    $('.grid').each(function () {
        var ta = $(this).parent().find('textarea.value').first();
        $(this).gridEditor({
            new_row_layouts: [[12], [6, 6], [9, 3], [3, 9]],
            row_classes: [],
            source_textarea: ta
        });
    });


    

    $('.save-content').click(function () {

        $('.grid').each(function () {

            var html = $(this).gridEditor('getHtml');
            
            $(this).parent().find('textarea.value').first().html(html);


        });
    });

   

});
