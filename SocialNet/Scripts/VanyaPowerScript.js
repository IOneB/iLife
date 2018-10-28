function overlayClicked(e){
    var hate = document.documentElement.clientHeight;
    var wild = document.documentElement.clientWidth;
    var x = e.clientX;
    var y = e.clientY;
    if (x < wild / 2.0 && y > hate * 0.1) {
        for (var i = 0; i < 10; i++) {
            if (document.getElementById('modal-im-' + i).getAttribute('hidden') === null) {
                if (i === 0) {
                    for (var j = 9; j >= 0; j--)
                        if (document.getElementById('modal-im-' + j).getAttribute('src') !== null) {
                            next(j);
                            break;
                        }
                }
                else
                    next(i - 1);
                break;
            }
        }
    }
    else
        empty();
}

function addFriend(elem, bulian) {
    var text;
    if (bulian == true) {
        text = "Удалить"
    }
    else
        text = "Отменить запрос";
    elem.innerHTML = text;
    elem.setAttribute("href", "/Home/AddContact?contactName=" + elem.getAttribute('href').split('=')[1]);
    elem.setAttribute("onclick", "removeFriend(this," + bulian + ")");
    if (bulian === true) {
        var id = elem.id.split('-')[1];
        var dialog = document.getElementById("dialog-" + id);
        dialog.hidden = false;
    }
}
function removeFriend(elem, bulian) {
    elem.innerHTML = "Добавить в друзья";
    elem.setAttribute("href", "/Home/CancelRequest?contactName=" + elem.getAttribute('href').split('=')[1]);
    elem.setAttribute("onclick", "addFriend(this," + bulian + ")");
    if (bulian === true) {
        var id = elem.id.split('-')[1];
        var dialog = document.getElementById("dialog-" + id);
        dialog.hidden = true;
    }
}

function removeContact(id) {
    var elem = document.getElementById('friend-first-button-' + id);
    elem.innerHTML = "Отменить удаление";
    document.getElementById('friend-second-button-' + id).hidden = true;
    elem.setAttribute("href", "/Home/CancelRequest?contactName=" + elem.getAttribute('href').split('=')[1]);
    elem.setAttribute("onclick", "cancelRemove(" + id + ")");
}

function cancelRemove(id) {
    var elem = document.getElementById('friend-first-button-' + id);
    elem.innerHTML = "Удалить";
    document.getElementById('friend-second-button-' + id).hidden = false;
    elem.setAttribute("href", "/Home/AddContact?contactName=" + elem.getAttribute('href').split('=')[1]);
    elem.setAttribute("onclick", "removeContact(" + id + ")");
}


function cancelRequest(id) {
    var elem = document.getElementById('contact-' + id);
    elem.remove();
}


function addContact(elem, id) {
    document.getElementById('request-second-button-'+ id).hidden = true;
    elem.innerHTML = "Отменить добавление";
    elem.setAttribute("href", "/Home/AddContact?contactName=" + elem.getAttribute('href').split('=')[1]);
    elem.setAttribute("onclick", "cancelAdd(this," + id + ")");

        //var firstButton = document.getElementById('request-first-button-' + id);
        //var secondButton = document.getElementById('request-second-button-' + id);
        //var user = firstButton.getAttribute('href').split('=')[1];
        //var elem = document.getElementById('request-' + id);
        //document.getElementById('friend-container').appendChild(elem);
        //var number = document.getElementsByClassName('friend').length;
        //elem.id = "friend-" + number;
        //elem.classList.add('friend');
        //firstButton.id = "friend-first-button-" + number;
        //secondButton.id = "friend-second-button-" + number;
        //firstButton.innerHTML = "Удалить";
        //secondButton.innerHTML = "Написать";
        //secondButton.removeAttribute("onclick");
        //secondButton.setAttribute("href", "/Home/Dialog?receiverUser=" + user);
        //secondButton.setAttribute('data-ajax', "false");
        //var el = document.getElementById('alert-danger');
        //el.remove();
        //firstButton.setAttribute("onclick", "removeContact(" + id + ")");
}
function cancelAdd(elem, id) {
    document.getElementById('request-second-button-' + id).hidden = false;
    elem.innerHTML = "Добавить";
    elem.setAttribute("href", "/Home/CancelRequest?contactName=" + elem.getAttribute('href').split('=')[1]);
    elem.setAttribute("onclick", "addFriend(this," + id + ")");
}

function denial(id) {
    var elem = document.getElementById('request-' + id);
    elem.remove();
}


$(document).ready(function () {
    document.getElementById('overlay').addEventListener('click', overlayClicked);
    var addContact = document.getElementById('addContact');
    addContact.addEventListener('click', function () {
        var href = addContact.getAttribute("href");
        if (!addContact.classList.contains("cancel-request")) {
            addContact.setAttribute("href", "/Home/AddContact?contactName=" + href.split('=')[1]);
            addContact.innerHTML = '✔ Запрос отправлен';
            addContact.classList.add("cancel-request");
        }
        else
        {
            addContact.setAttribute("href", "/Home/CancelRequest?contactName=" + href.split('=')[1]);
            addContact.classList.remove("cancel-request");
            addContact.innerHTML = "Добавить в контакты";
        }
    });
});

function bigPicture(id, imgNum, obj) {
    var pictures = $(obj + '-' + id);
    document.getElementById('win').removeAttribute('style');
    document.getElementById('windows').style.overflow = 'hidden';
    for (var i = 0; i < pictures.length; i++) {
        var el = document.getElementById('modal-im-' + i);
        el.setAttribute('src', pictures[i].getAttribute('src'));
        if (imgNum === i) {
            el.removeAttribute('hidden');
            el.classList.add('shown-pic');
        }
    }

}

function empty() {
    document.getElementById('win').style.display = 'none';
    document.getElementById('windows').style.overflow = 'auto';
    for (var i = 0; i < 10; i++) {
        var el = document.getElementById('modal-im-' + i);
        el.classList.remove('shown-pic');
        el.removeAttribute('src');
        el.setAttribute('hidden', 'true');
    }
}

function next(nextImg) {
    var src = document.getElementById('modal-im-' + nextImg).getAttribute('src');
    if (src === null) {
        if (nextImg === 0)
            empty();
        else
            next(0);
    }
    else {
        var current = $('.shown-pic');
        current[0].setAttribute('hidden', 'true');
        current[0].classList.remove('shown-pic');
        document.getElementById('modal-im-' + nextImg).removeAttribute('hidden');
        document.getElementById('modal-im-' + nextImg).classList.add('shown-pic');
    }
}

function Show(id) {
    var el = document.getElementById('com-' + id);
    var classes = el.classList;
    if (classes[1] === 'hide') {
        el.hidden = false;
        el.classList.remove('hide');
        el.classList.add('shown');
    }
    else {
        el.hidden = true;
        el.classList.remove('shown');
        el.classList.add('hide');
    }
}



