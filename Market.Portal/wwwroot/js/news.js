
function News() {
    var _panel = null;

    this.load = function () {

        $.get("/api/News", function (list) {
            _load_list(list);
        });
    };

    this.add = function (news) {

        _add_item(news, _panel.find('.panel').length == 0);
    };

    _add_item = function (news, first) {
        var panel;
        if (first) {
            panel = $('<div class="panel panel-primary"></div>');
        } else {
            panel = $('<div class="panel panel-default"></div>');
        }

        var title = $('<div class="panel-heading"></div>').html(news.description);
        var body = $('<div class="panel-body"></div>').html(news.content);

        panel.append(title).append(body);
        _panel.append(panel);
    };

    _load_list = function (list) {
        if (list && Array.isArray(list)) {

            for (var i = 0; i < list.length; i++) {
                var news = list[i];
                _add_item(news, i == 0);
            }
        }
    };

    var _constructor = function () {
        _panel = $(".panel-group");
    };

    _constructor();
}