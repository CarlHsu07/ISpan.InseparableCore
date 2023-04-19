$(document).ready(function () {
    $("#citySelect").change(function () {
        if ($(this).val() === '') {
            $("#areaSelect").val('');
        }

        var selectedValue = $("#citySelect option:selected").val();
        GetAreas(selectedValue);
    });

    // 從DB取得區域的內容，並新增選單的選項
    function GetAreas(cityID) {
        var baseUrl = window.location.origin;
        var url = baseUrl + '/Member/GetAreas';

        $.ajax({
            url: url,
            data: { cityID: cityID },
            type: 'get',
            cache: false,
            async: false,
            dataType: 'json',
            success: function (data) {
                $('#areaSelect').empty(); // 清空區域選單的選項
                $('#areaSelect').append($('<option></option>').val('').text('請選擇區域'));
                if (data.length > 0) {
                    // 將回傳的區域資料動態新增到區域選單
                    $.each(data, function (i, item) {
                        $('#areaSelect').append($('<option></option>').val(item.areaID).text(item.areaName));
                    });
                }
            },
            error: function () {
                console.log("取得區域發生錯誤，請稍後再試。");
            }
        });
    }
});
