$(document).ready(function () {
    $(document).on("click", ".addMore01", function () { //
        debugger;
        var rowCount = $('.memoRows').length + 1;
        var memoRows = '<tr class="memoRows">' +
                        '<td>' +
                            '<input type="text" id="slNo' + rowCount + '" value="' + rowCount + '" name="slNo' + rowCount + '" class="form-control input-sm slNo01" readonly/>' +
                        '</td>' +
                        '<td>' +
                            '<input type="text" id="subject' + rowCount + '" name="MemoSubject" class="form-control input-sm subject01" required/>' +
                        '</td>' +
                        '<td>' +
                            '<select name="department" id="department' + rowCount + '" class="form-control input-sm department01" required>' +
                            '</select>' +
                        '</td>' +
                        '<td>' +
                            '<input type="file" id="Memofile' + rowCount + '" name="Memofile" class="form-control input-sm memofile01"/>' +
                        '</td>' +
                        '<td>' +
                            '<input type="button" id="remove" name="remove" value="Remove" class="form-control btn-danger input-sm remove01" />' +
                        '</td>' +
                    '</tr>';
        //-------------------Contact Type-----------------
        $(function () {
            //$("#department" + rowCount).empty();
            $.ajax({
                type: 'POST',
                url: 'getDepartment',
                dataType: 'json',
                data: {},
                success: function (response) {
                    $("#department" + rowCount).append('<option value="Select">'
                                                + "---Select department---" + '</option>');
                    $.each(response, function (i, response) {
                        $("#department" + rowCount).append('<option value="'
                                                   + response.Value + '">'
                                             + response.Text + '</option>');
                    });
                },
                error: function (ex) {
                    alert('Failed.' + ex);
                }
            });
            $('#memoTable').append(memoRows);
            $("#department" + rowCount).val($("#department" + rowCount + " option:selected").val());
            $("#department" + rowCount).select2();

            //taking local storage start


            $("#subject" + rowCount).focusout(function () {
                localStorage.setItem("subject" + rowCount, $("#subject" + rowCount).val());
            });

            $("#subject" + rowCount).val(localStorage.getItem("subject" + rowCount));

           
            //taking local storage end

        });
    });
    $(document).on("click", ".remove01", function () {
        $(this).closest("tr").remove(); // closest used to remove the respective 'tr' in which I have my controls   
    });
});