	function search(){
			var keyword=$('#keyword').val();
	$.ajax({
		url:'/Home/Search',
	method:'POST',
	data: {keyword},
	success:function(data){
					if(data==null){
						return;
					}
	var all =JSON.parse(data);

	var cinema=null;
	if(all.cinema!=null){
						var item = JSON.parse(all.cinema);
	cinema = item.$values;
					}
	var movie = null;
	if (all.movie != null) {
						var item = JSON.parse(all.movie);
	movie = item.$values;
					}
	var member = null;
	if (all.member != null) {
						var item = JSON.parse(all.member);
	member = item.$values;
					}
	if (all.articles != null) {
						var item = JSON.parse(all.articles);
	articles = item.$values;
					}
	if (articles.length == 0 && member.length == 0 && movie.length == 0 && cinema.length == 0) {
		alert(`Oopas...查無資料`);
	return;
						//$('#mainbody').append(`<div style="text-align:center; margin-top:20px;"><h4>Oopas...查無資料</h4></div>`)
					}
	$('#mainbody').html('');
	if (member != null) {
		member.forEach(function (value, index) {
			$('#mainbody').append(`<div style="border: solid 1.8px #ffffff00;padding: 5px;margin: 5px;dispaly:flex; border-radius:9px;background-color:#191a1d;">
													<span><a href="/Member/Profile/${value.FId}">
													${value.FPhotoPath == null ?
					`<img src= "/images/nobody.jpg" width = "150";>` :
					`<img src="/images/memberProfilePhotos/${value.FPhotoPath}" width = "150";>`}
													</span>
													<span><h4>${value.FLastName} ${value.FFirstName}</h4>
													${value.FIntroduction == null ? "" :
					`${value.FIntroduction.length > 60 ? `<p>${value.FIntroduction.substring(0, 60)}...</p>` :
						`<p>${value.FIntroduction} < /p>`}`}
													</span></a></div>`)
		});
					}
	if (movie.length != 0) {
		movie.forEach(function (value, index) {
			var transdate = new Date(value.FMovieOnDate)
			var date = (transdate).toLocaleDateString('zh-TW', { year: 'numeric', month: '2-digit', day: '2-digit' });
			$('#mainbody').append(`<div style="border: solid 1.8px #ffffff00;padding: 5px;margin: 5px;display:flex;border-radius:9px;background-color:#191a1d;">
													<span><a href="/TMovies/Details/${value.FMovieId}">
													<img src="/images/${value.FMovieImagePath}" width="200"; style="margin:5px;"></span>
													<span><h4>${value.FMovieName}</h4>
													<p>片長 ： ${value.FMovieLength}</p>
													<p>上映日 ： ${date}</p>
													<p>導演 ： ${value.FMovieDirectors}</p>
													${value.FMovieActors == null ? "" :
					`${value.FMovieActors.length > 80 ? `<p>主要演員 : ${value.FMovieActors.substring(0, 80)}...</p>` :
						`<p>主要演員：${value.FMovieActors} </p>`}`}
													${value.FMovieIntroduction == null ? "" :
					`${value.FMovieIntroduction.length > 150 ? `<p>簡介 : ${value.FMovieIntroduction.substring(0, 150)}...</p>` :
						`<p>簡介 : ${value.FMovieIntroduction} </p>`}`}
													 </span></a></div>`)
		});
					}

	if (cinema.length != 0) {
		cinema.forEach(function (value, index) {
			$('#mainbody').append(`<div style="border: solid 1.8px #ffffff00;padding: 5px;margin: 5px; border-radius:9px;background-color:#191a1d;"><span><a ><h4>${value.FCinemaName}</h4></a>
													<p>${value.FCinemaAddress}</p>
													<p>${value.FCinemaTel}</p>
													</span></div>`)
		});
					}
	if (articles != null) {
		articles.forEach(function (value, index) {
			$('#mainbody').append(`<div style="border: solid 1.8px #ffffff00;padding: 5px;margin: 5px;display:flex; border-radius:9px;background-color:#191a1d;">
													<span><a href="/TArticles/Details/${value.FArticleId}">
													<span><h4>${value.FArticleTitle}</h4>
													${value.FArticleContent == null ? "" :
					`${value.FArticleContent.length > 30 ? `<p>${value.FArticleContent.substring(0, 30)}...</p>` :
						`<p>${value.FArticleContent} < /p>`}`}
													</span></a></div>`)
		});
					}
				},
	error:function(){

	}
			});
		}


	tinymce.init({
		selector: '#textarea', //容器，可使用css选择器
	language: 'zh_TW', //调用放在langs文件夹内的语言包
	toolbar: true, //隐藏工具栏
	menubar: false, //隐藏菜单栏
	//inline: true, //开启内联模式
	plugins: ['quickbars', 'link', 'table'], //选择需加载的插件
	//选中时出现的快捷工具，与插件有依赖关系
	quickbars_selection_toolbar: 'bold italic forecolor | link blockquote quickimage',
	skin: 'oxide-dark',
	content_css: "dark",
		});
