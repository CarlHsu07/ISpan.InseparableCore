$(function () {
    var total = 0;
    var regular = ($('#regularsub').html() == null) ? 0 : parseInt($('#regularsub').html());
    var concession = ($('#concessionsub').html() == null) ? 0 : parseInt($('#concessionsub').html());

    total = regular + concession;
    var product = document.querySelectorAll('#productsub');
    $(product).each(function () {

        var product = ($(this).html() == null) ? 0 : parseInt($(this).html());
        total += product;
    });

    $('#total').html(total);
    $('#money').val(total);
});

function cashpay() {
    $('#form').attr('action', 'CashPay');
    $('#form').submit();
};