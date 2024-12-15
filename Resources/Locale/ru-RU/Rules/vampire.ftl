## vampire

# Shown at the end of a round of vampire
vampire-round-end-result =
    { $vampireCount ->
        [one] Был один вампир.
       *[other] Было { $vampireCount } вампиров.
    }
vampire-round-end-codewords = Кодовыми словами были: [color=White]{ $codewords }[/color].
# Shown at the end of a round of vampire
vampire-user-was-a-vampire = [color=gray]{ $user }[/color] был(а) вампиром.
vampire-user-was-a-vampire-named = [color=White]{ $name }[/color] ([color=gray]{ $user }[/color]) был(а) вампиром.
vampire-was-a-vampire-named = [color=White]{ $name }[/color] был(а) вампиром.
vampire-user-was-a-vampire-with-objectives = [color=gray]{ $user }[/color] был(а) вампиром со следующими целями:
vampire-user-was-a-vampire-with-objectives-named = [color=White]{ $name }[/color] ([color=gray]{ $user }[/color]) был(а) вампиром со следующими целями:
vampire-was-a-vampire-with-objectives-named = [color=White]{ $name }[/color] был(а) вампиром со следующими целями:
preset-vampire-objective-issuer-syndicate = [color=#87cefa]Синдикат[/color]
# Shown at the end of a round of vampire
vampire-objective-condition-success = { $condition } | [color={ $markupColor }]Успех![/color]
# Shown at the end of a round of vampire
vampire-objective-condition-fail = { $condition } | [color={ $markupColor }]Провал![/color] ({ $progress }%)
vampire-title = Вампиры
vampire-description = Среди нас есть Вампиры...
vampire-not-enough-ready-players = Недостаточно игроков готовы к игре! Из { $minimumPlayers } необходимых игроков готовы { $readyPlayersCount }. Не удалось начать режим Вампиров.
vampire-no-one-ready = Нет готовых игроков! Не удалось начать режим Вампиров.

## vampireDeathMatch

vampire-death-match-title = Бой насмерть вампиров
vampire-death-match-description = Все — Вампиры. Все хотят смерти друг друга.
vampire-death-match-station-is-too-unsafe-announcement = На станции слишком опасно, чтобы продолжать. У вас есть одна минута.
vampire-death-match-end-round-description-first-line = КПК были восстановлены...
vampire-death-match-end-round-description-entry = КПК { $originalName }, с { $tcBalance } ТК

## vampireRole

# vampireRole
vampire-role-greeting =
    Вы - агент Синдиката.
    Ваши цели и кодовые слова перечислены в меню персонажа.
    Воспользуйтесь аплинком, встроенным в ваш КПК, чтобы приобрести всё необходимое для выполнения работы.
    Смерть Nanotrasen!
vampire-role-codewords =
    Кодовые слова следующие:
    { $codewords }
    Кодовые слова можно использовать в обычном разговоре, чтобы незаметно идентифицировать себя для других агентов Синдиката.
    Прислушивайтесь к ним и храните их в тайне.
vampire-role-uplink-code =
    Установите рингтон Вашего КПК на { $code } чтобы заблокировать или разблокировать аплинк.
    Не забудьте заблокировать его и сменить код, иначе любой член экипажа станции сможет открыть аплинк!
# don't need all the flavour text for character menu
vampire-role-codewords-short =
    Кодовые слова:
    { $codewords }.
vampire-role-uplink-code-short = Ваш код аплинка: { $code }.
