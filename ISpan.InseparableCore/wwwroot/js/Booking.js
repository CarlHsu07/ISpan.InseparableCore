$(function () {
    var regularprice = $("#regularprice").html();
    var concessionprice = regularprice * 0.9;
    $("#concessionprice").html(concessionprice);

    //網頁重載 session清空
    $.ajax({
        method: 'POST',
        url: 'Clearproduct',
        data: {},
        success: function (data) {
            console.log('pass');
        },
        error: function (error) {
            console.log('fail');
        }
    });
});
function cal() {
    //小計
    var regularprice = $("#regularprice").html();
    var concessionprice = regularprice * 0.9;
    var regularnum = $("#regularnum").val();
    var concessionnum = $("#concessionnum").val();

    $("#regularsubtotal").html(regularnum * regularprice);
    $("#concessionsubtotal").html(concessionnum * concessionprice);

    if (regularnum == 0 & concessionnum == 0) {
        $('#button').attr('disabled', true);
    } else {
        $('#button').attr('disabled', false);
    }
};
function caritem() {
    //更新購物清單

    //票券 
    var regular = $('#regular').html();
    var concession = $('#concession').html();
    var regularprice = $("#regularprice").html();
    var concessionprice = regularprice * 0.9;
    var regularnum = $("#regularnum").val();
    var concessionnum = $("#concessionnum").val();
    var total = 0;
    $("#" + regular).remove();
    $("#" + concession).remove();
    if (regularnum > 0) {
        $("#ticketitem").append(`<li id="${regular}">${regular} <p>x${regularnum} = ${regularnum * regularprice}</p></li>`)
        total += regularnum * regularprice;
    }
    if (concessionnum > 0) {
        $("#ticketitem").append(`<li id="${concession}">${concession} <p>x${concessionnum} = ${concessionnum * concessionprice}</p></li>`);
        total += concessionnum * concessionprice;
    }

    //產品
    var select = document.querySelectorAll('#product');
    $(select).each(function () {
        var name = $(this).attr('name');
        var price = $(this).attr('price');
        var num = $(this).val();
        var id = $(this).attr('pid');

        total += num * price;
    })
    //等ajax完成才能執行 todo有機會改成await
    $("#calculate").html(``);
    $("#calculate").append(`<div class="right border-top"><p>合計：${total}</p></div>`);
}

window.onunload = function () {
    $('select').each(function () {
        $(this).val(0);
    });
};