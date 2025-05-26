
function MenuItem(img, text) {
    this.Image = img;
    this.Text = text;
}
MenuItem.prototype.constructor = MenuItem;


var ContextMenu = (function() {
    var cm = {};

    var _CreateMenu = function(items) {
        var res = $('<div class="contextmenu"></div>');
        for (var i in items) {
            var item = $('<a href="javascript:" class="item"><i class="fa fa-' + items[i].Image + '" data-action="' + items[i].Text + '"></i> <span class="text">' + items[i].Text + '</span></a>');
            res.append(item);
        }

        return res;
    }

    var _Menu = null;
    var _RemoveMenu = function() {
        $(document).unbind('click', doc_Click);
        if (_Menu != null)
            _Menu.remove();
        _Menu = null;
    }
    var doc_Click = function() {
        _RemoveMenu();
    }
    var _BindDocClick = function() {
        setTimeout(function() { $(document).click(doc_Click); }, 300);
    }

    cm.Show = function(pos, items, onclick) {
        _RemoveMenu();

        var menu = _CreateMenu(items);
        var onclick = onclick;

        $('body').append(menu);
        var docW = $(document).width();
        var menuW = menu.width();
        var x = pos.X + menuW > docW ? pos.X - menuW : pos.X;
        menu.css({
            top: pos.Y + 'px',
            left: x + 'px'
        });


        $('.item', menu).click(function() {
            onclick({ Text: $('.text', this).text(), Menu: menu, Item: $(this) });
            _RemoveMenu();
        });

        _Menu = menu;
        _BindDocClick();
    }

    return cm;
})();
