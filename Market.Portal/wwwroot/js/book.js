
function Book(event) {

    if (!event)
        throw "invalid event send message";

    var lines = null;
    var sp_quote = null;
    var sel_quotes = null;
    var event_send_msg = event;

    var current_quote = '';

    this.update_msg = function (msg) {

        sp_quote.text(msg.Id);

        for (var i = 0; i < lines.length; i++) {
            var tds = lines.eq(i).find("td");
            tds.eq(0).text(msg.Buy[i].Price);
            tds.eq(1).text(msg.Buy[i].Qtd);
            tds.eq(2).text(msg.Sell[i].Price);
            tds.eq(3).text(msg.Sell[i].Qtd);
        }
    }

    this.start_book = function () {
        sel_quotes.prop('disabled', false);
    };

    var _send_msg = function (quote) {
        if (!quote) {
            _clear_table();
            current_quote = '';
            return;
        }
            
        if (current_quote != quote) {

            if (current_quote) {
                event_send_msg({ CommandType: 1, Stock: current_quote });
            }

            current_quote = quote;

            _clear_table();

            event_send_msg({ CommandType: 0, Stock: current_quote });
        }
    };

    var _clear_table = function () {
        sp_quote.text('-');

        for (var i = 0; i < lines.length; i++) {
            var tds = lines.eq(i).find("td");
            tds.eq(0).text('-');
            tds.eq(1).text('-');
            tds.eq(2).text('-');
            tds.eq(3).text('-');
        }
    };

    var _constructor = function () {
        lines = $('table').find('tr').not(':first');
        sp_quote = $('#sp_quote')
        sel_quotes = $('#sel_quotes');
        sel_quotes.change(function () {
            _send_msg($(this).find(':selected').text());
        });

        _clear_table();
    };

    _constructor();
}