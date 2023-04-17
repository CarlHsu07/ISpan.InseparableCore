$(document).ready(function () {
    $("#citySelect").change(function () {
        if ($(this).val() === '') {
            $("#areaSelect").val('');
        }
        var selectedValue = $("#citySelect option:selected").val();
        GetAreas(selectedValue);
        setTimeout(function () {
            $('#Address').val(getAddress());
        }, 100);
    });

    $("#areaSelect").change(function () {
        setTimeout(function () {
            $('#Address').val(getAddress());
        }, 100);
    });

    // 組合縣市+區域的字串，並防止使用者刪到縣市+區域的字串
    $("#Address").on('input', function (e) {
        var address = getAddress();
        if (this.value.indexOf(address) !== 0) {
            var addr2 = this.value.substring(Math.min(address.length, this.value.length));
            this.value = address + addr2;
        }
    });

    // 取得選取的縣市、區域文字
    function getAddress() {
        var $city = $('#citySelect option:selected');
        var $area = $('#areaSelect option:selected');
        var address = '';
        if ($city.val()) {
            address += $city.text();
        }
        if ($area.val()) {
            address += $area.text();
        }
        return address;
    }

    // 從DB取得並顯示區域的內容
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
                        $('#areaSelect').append($('<option></option>').val(item.zipCode).text(item.areaName));
                    });
                }
            },
            error: function () {
                console.log("取得區域發生錯誤，請稍後再試。");
            }
        });
    }
});
