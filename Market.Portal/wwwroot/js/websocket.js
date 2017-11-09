
//window.addEventListener("load", init, false);

function WebsocketHB() {

    var websocket = null;

    this.event_create_ws = null;

    this.event_notify_msg = null;

    var instance = this;

    var _getRootUri = function() {

        //var path = document.location.pathname.split('/');

        //var context;

        //if (path[1] != 'testews.html') {
        //    context = path[1];
        //}

        return "ws://localhost:49838/ws";
        //return 'ws://' + (document.location.hostname == '' ? 'localhost' : document.location.hostname) + 
        //        (document.location.port == '' ? '' : ':' + document.location.port) + (context ? '/' + context : '');
    }

    this.create_ws = function() {

        websocket = new WebSocket(_getRootUri());
        websocket.onopen = function (evt) {
            _onOpen(evt)
        };
        websocket.onmessage = function (evt) {
            _onMessage(evt)
        };
        websocket.onerror = function (evt) {
            _onError(evt)
        };
    }

    this.send_message = function(msg) {

        _doSend(msg);
    }

    var _onOpen = function (evt) {
        console.log("Connected to Endpoint!");

        if (instance.event_create_ws) {
            instance.event_create_ws();
        }
    }

    var _onMessage = function(evt) {
        console.log(JSON.parse(evt.data));

        if (instance.event_notify_msg) {
            instance.event_notify_msg(evt.data);
        }
    }

    var _onError = function(evt) {
        console.log(evt.data);

        this.create_ws();
    }

    var _doSend = function (message) {
       
        websocket.send(message);
    }
}