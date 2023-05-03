"use strict";

let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
$("#sendButton").prop("disabled", true);

// 收到訊息
connection.on("ReceiveMessage", function (senderId, message) {
    alert("ReceiveMessage");
    appendSenderMessage(senderId, message);
});

// 啟動連線
// 如果連接成功，將發送按鈕的disabled屬性設置為false，啟用發送按鈕；如果連接失敗，則顯示錯誤訊息在console中。
connection.start().then(function () {
    $("#sendButton").prop("disabled", false);
}).catch(function (err) {
    return console.error(err.toString());
});

// 點sendButton後，送出訊息
$("#sendButton").click(function (event) {
    event.preventDefault();
    sendeMessage();
});

$('#messageInput').on("keydown", function (event) {
    // 判斷按下的是否是enter鍵，並且沒有按下shift鍵（避免換行）
    if (event.keyCode === 13 && !event.shiftKey) {
        event.preventDefault();
        sendeMessage(); // 呼叫送出訊息的函數
    }
});

// 送出訊息
function sendeMessage() {
    console.log("senderId：" + senderId);
    //console.log("receiverId：" + );


    let receiverId = 9;
    let message = $("#messageInput").val();
    if (message.trim() == "") { return; } // 是空字串就不傳送訊息

    connection.invoke("SendMessage", senderId.toString(), receiverId.toString(), message).catch(function (err) {
        return console.error(err.toString());
    });

    $("#messageInput").val("");
    $('#messagesArea').animate({ scrollTop: $('#messagesArea').prop("scrollHeight") }, 1000);
}

// 在網頁上附加訊息的元素
function appendSenderMessage(senderId, message) {
    if (message.trim() == "") {
        return;
    }

    let element = $(`<!-- Reciever Message-->
                <div class="media w-50 ml-auto mb-3">
                    <div class="media-body">
                        <div class="bg-primary rounded py-2 px-3 mb-2">
                            <p class="text-small mb-0 text-white" style="word-wrap: break-word;"></p>
                        </div>
                        <p class="small text-muted">時間</p>
                    </div>
                </div>`);

    // 使用jQuery的text()函數將使用者輸入的訊息當成純文字，不會被當作HTML
    element.find(".text-small").text(message);

    $("#messagesArea").append(element);
}