
$(document).ready(function () {
    var ws = new WebsocketHB();
    var book = new Book(function (json) {
        ws.send_message(JSON.stringify(json))
    });
    ws.event_create_ws = function () {
        book.start_book();
    };

    ws.event_notify_msg = function (msg) {
        book.update_msg(JSON.parse(msg));
    };

    ws.create_ws();

    var news = new News();
    news.load();
});