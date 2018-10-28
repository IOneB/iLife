$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    chat.client.addMessage = function (name, message, myName) {
        var text = message.split('<br/>');
        var totaltext = '';
        for (var i = 0; i < text.length; i++) {
            totaltext += htmlEncode(text[i]) + '<br/>';
        }
        var cl;
        if (name == myName)
            cl = 'message-from';
        else
            cl = 'message-to';
        // Добавление сообщений на веб-страницу 
                $('#chatroom').append('<div class="message ' + cl + '"><div class="message-container"><b>' + htmlEncode(name)
            + ':</b><i style="font-size:10px;">  ' + htmlEncode(new Date().toLocaleString()) + '</i><div class=" d-flex flex-row message-row"><img class="mini-avatar img-' + cl + '")"/> <div class="message-person">' + totaltext + '</div></div></div></div>');
    };

    // Открываем соединение
    $.connection.hub.start().done(function () {
        var objDiv = document.getElementById("chatroom");
        objDiv.scrollTop = objDiv.scrollHeight;
        $("#message").keyup(function (event) {
            if (event.keyCode == 13) {
                if (event.ctrlKey) {
                    var str = $("#message").val();
                    $("#message").val(str + '\n');
                }
                else {
                    $("#sendmessage").click();
                }

            }
        });

        $('#sendmessage').click(function () {
            // Вызываем у хаба метод Send
            chat.server.send($('#username').val(), $('#message').val());
            $('#message').val('');
        });

        //Сообщения прочитаны
        $('#message').click(function () {
            Reading('message-to');
        });

        setTimeout(Reading, 7000, 'message-to');

        chat.server.connect($('#username').val());

    });



    // Функция, вызываемая после отправки сообщения
    chat.client.onSended = function () {
        var objDiv = document.getElementById("chatroom");
        objDiv.scrollTop = objDiv.scrollHeight;
        setTimeout(Reading, 4000, 'message-to');
    }
    chat.client.onReaded = function () {
        Reading('message-from');
    }

    //Сообщения прочитаны
    function Reading(str) {
        var elems = document.getElementsByClassName(str);
        var length = elems.length;
        for (var i = 0; i < length; i++) {
            elems[0].classList.remove(str);
        }
        if (length > 0 && str == 'message-to') {
            chat.server.readed($('#username').val());
        }
    }
});

// Кодирование тегов
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
//Добавление нового пользователя
function AddUser(id, name) {

    var userId = $('#hdId').val();

    if (userId != id) {

        $("#chatusers").append('<p id="' + id + '"><b>' + name + '</b></p>');
    }
}