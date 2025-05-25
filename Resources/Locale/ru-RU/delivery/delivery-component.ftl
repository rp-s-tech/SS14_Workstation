delivery-recipient-examine = Это предназначено для { $recipient }, { $job }.
delivery-already-opened-examine = Он уже открыт.
delivery-earnings-examine = Станция заработает от доставки этого [color=yellow]{ $spesos }[/color] spesos.
delivery-recipient-no-name = Безымянный
delivery-recipient-no-job = Неизвестный
delivery-unlocked-self = Вы разблокировали { $delivery } своим отпечатком.
delivery-opened-self = Вы открыли { $delivery }.
delivery-unlocked-others = { CAPITALIZE($recipient) } разблокировал { $delivery } с помощью { POSS-ADJ($possadj) } отпечатка.
delivery-opened-others = { CAPITALIZE($recipient) } открыл { $delivery }.
delivery-unlock-verb = Разблокировать
delivery-open-verb = Открыть
delivery-slice-verb = Вскрыть
delivery-teleporter-amount-examine =
    { $amount ->
        [one] Внутри [color=yellow]{ $amount }[/color] доставка.
       *[other] Внутри [color=yellow]{ $amount }[/color] доставок.
    }
delivery-teleporter-empty = { $entity } пуст.
delivery-teleporter-empty-verb = Забрать почту
# modifiers
delivery-priority-examine = Это имеет [color=orange] приоритет { $type }[/color]. У вас осталось [color=orange]{ $time }[/color] чтобы это доставить.
delivery-priority-delivered-examine = Это имеет [color=orange] приоритет { $type }[/color]. Доставлено вовремя.
delivery-priority-expired-examine = Это имеет [color=orange] приоритет { $type }[/color]. Доставка просрочна.
delivery-fragile-examine = Это [color=red] Хрупкий предмет { $type }[/color]. Доставьте аккуратно, чтобы не потерять бонус.
delivery-fragile-broken-examine = Это [color=red] Хрупкий предмет { $type }[/color]. ПОТРАЧЕНО.
