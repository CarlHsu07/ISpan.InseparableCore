function deleteConfirmation(function1) {
	Swal.fire({
		title: '你確定要刪除嗎?',
		icon: 'warning',
		showCancelButton: true,
		confirmButtonColor: '#3085d6',
		cancelButtonColor: '#d33',
		confirmButtonText: '確定',
		cancelButtonText: '取消'
	}).then((result) => {
		if (result.isConfirmed) {
			function1();
		}
	});
}