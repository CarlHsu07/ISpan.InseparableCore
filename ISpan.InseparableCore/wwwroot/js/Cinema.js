$(function () {
    var city = "臺北市";
    var url = '/Cinema/City';
    //todo 要先預設選擇台北button???
    getCinema(url, city);
});

//取得電影院
function getCinema(url, name) {
    $.ajax({
        method: 'POST',
        url,
        data: {
            name
        },
        datatype: 'text',
        success: function (data) {
            var cinema = JSON.parse(data);
            cinema.forEach(function (value, index) {
                $('#cinema').append(`<div class="showcinema color" onclick="mapshow(${value.FCinemaId})"><span><a ><h4>${value.FCinemaName}</h4></a>
                                             <p>${value.FCinemaAddress}</p>
                                             <p>${value.FCinemaTel}</p>
                                             </span></div>`)
            })
        },
        error: function () {
        },
    });
}
$('button').click(function () {
    //單選換class
    $('.btn-primary').addClass('btn-outline-primary');
    $('.btn-primary').removeClass('btn-primary');

    $(this).removeClass('btn-outline-primary');
    $(this).addClass('btn-primary');
});

$('#city button').click(function () {
    $('#cinema').html('');
    $('#show h5').html(``);
    $('#show h4').html(``);
    $('#map').html('');
    $('#traffic').html('');
    $('#show').removeClass('show');

    var city = $(this).attr('value');
    var url = '/Cinema/City';
    getCinema(url, city);

});
$('#brand button').click(function () {
    $('#cinema').html('');
    $('#show h5').html(``);
    $('#show h4').html(``);
    $('#map').html('');
    $('#traffic').html('');
    $('#show').removeClass('show');

    var brand = $(this).attr('value');
    var url = '/Cinema/Brand';
    getCinema(url, brand);

});
//map
function sendmapRequest(id) {
    return new Promise(
        function (resolve, reject) {
            $.ajax({
                beforeSend: function () {
                    $('#loading').css("display", "");
                },
                method: 'POST',
                url: '/Cinema/Map',
                data: { id },
                success: function (data) {
                    resolve(data);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    reject(`${textStatus} ${jqXHR.status} ${errorThrown}`);
                },
                complete: function () {
                    setTimeout(function () { $('#loading').css("display", "none"); }, 2000);

                }
            });
        });
}


function mapshow(id) {
    $('#show h5').html(``);
    $('#show h4').html(``);
    $('#show h5').append(`交通資訊：`);
    $('#map').html('');
    $('#traffic').html('');
    $('#show').addClass('show');

    sendmapRequest(id)
        .then(function (data) {
            var cinema = JSON.parse(data);
            $('#show').prepend(`<h4>${cinema.Name}</h4>`);
            traffic = cinema.FTraffic;
            traffic.forEach(function (value, index) {
                $('#traffic').append(`<p style="font-size:18px;">${value}</p>`)
            })
            return cinema;
        })
        .then(function (value) {
            //map
            var lat = value.FLat;
            var lng = value.FLng
            var platform = new H.service.Platform({
                apikey: value.Key
            });

            var pixelRatio = window.devicePixelRatio || 1;
            var defaultLayers = platform.createDefaultLayers({
                tileSize: pixelRatio === 1 ? 256 : 512,
                ppi: pixelRatio === 1 ? undefined : 320
            });
            var map = new H.Map(document.getElementById('map'),
                defaultLayers.vector.normal.map, {
                center: { lat, lng },
                zoom: 4,
                pixelRatio: pixelRatio
            });

            window.addEventListener('resize', () => map.getViewPort().resize());
            var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
            var ui = H.ui.UI.createDefault(map, defaultLayers);
            switchMapLanguage(map, platform);

            moveMapToBerlin(map, lat, lng);
            addMarkersToMap(map, lat, lng);
            //bobble(map, lat, lng, platform);
        });

}
//放大縮小
function moveMapToBerlin(map, lat, lng) {
    map.setCenter({ lat, lng });
    map.setZoom(16);
};

//標記
function addMarkersToMap(map, lat, lng) {
    var parisMarkerIcon = new H.map.Icon('https://img.icons8.com/ios-filled/50/D81111/marker.png');
    var parisMarker = new H.map.Marker({ lat, lng }, { icon: parisMarkerIcon });
    map.addObject(parisMarker);
};

//變中文
function switchMapLanguage(map, platform) {
    let defaultLayers = platform.createDefaultLayers({
        lg: 'zh-tw'
    });

    map.setBaseLayer(defaultLayers.vector.normal.map);
    var ui = H.ui.UI.createDefault(map, defaultLayers, 'zh-CN');
    ui.removeControl('mapsettings');
};

//泡泡 因為會擋到座標所以不放
function bobble(map, lat, lng, platform) {
    var infoBubble = new H.ui.InfoBubble({ lat, lng }, {
        content: '',
    });
    var defaultLayers = platform.createDefaultLayers();
    // 添加 InfoBubble 對象到地圖上
    var ui = H.ui.UI.createDefault(map, defaultLayers);
    ui.addBubble(infoBubble);
};
