$(document).ready(function () {
    var count = 3;

    function updateCountdown() {
        $('#center h4').text(`將於${count}秒後導回訂購畫面...`);
        count--;
    }

    updateCountdown();

    setInterval(updateCountdown, 1000);

    setTimeout(function () {
        $('#form').submit()
    }, 3000);
});