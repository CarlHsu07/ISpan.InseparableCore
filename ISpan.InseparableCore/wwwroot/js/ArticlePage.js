//Article換頁function
function pageChange(page) {
	pageNumber = page
	let key = $("#Key").val()
	let categoryId = $("#CategoryId").find(":selected").val()
	show(key, categoryId, page);
	alert(page + "/" + key + "/" + categoryId)
}

//btnpage換顏色
function pageSelected(event) {
	$('.btn-primary').addClass('btn-secondary');
	$('.btn-secondary').removeClass('btn-primary');

	$(event).removeClass('btn-secondary');
	$(event).addClass('btn-primary');

}
//點頁碼換頁
$(document).on('click', '.btnPage', function () {
	let page = $(this).val()
	pageChange(page)

	pageSelected(this)
});
//上一頁
$(document).on('click', '#prePage', function () {
	let page = parseInt(pageNumber) - 1

	if (page <= 0) {
		page += 1
	}
	$('.btn-primary').addClass('btn-secondary');
	$('.btn-secondary').removeClass('btn-primary');
	$(`#${page}`).removeClass('btn-secondary');
	$(`#${page}`).addClass('btn-primary');
	pageChange(page)
});
//下一頁
$(document).on('click', '#nextPage', function () {
	let page = parseInt(pageNumber) + 1
	if (page > PageCountNumber) {
		page -= 1
	}
	$('.btn-primary').addClass('btn-secondary');
	$('.btn-secondary').removeClass('btn-primary');

	$(`#${page}`).removeClass('btn-secondary');
	$(`#${page}`).addClass('btn-primary');
	pageChange(page)
});
