## Survivor

roles-antag-survivor-name = Выживальщик
# It's a Halo reference
roles-antag-survivor-objective = Текущая цель: Выжить
survivor-role-greeting =
    Вы - Выживший.
    Прежде всего вам нужно вернуться на ЦентКомм живым.
    Соберите столько огнестрельного оружия, сколько необходимо, чтобы гарантировать свое выживание.
    Не доверяйте никому.
survivor-round-end-dead-count =
    { $deadCount ->
        [one] [color=red]{ $deadCount }[/color] выживальщик мёртв.
       *[other] [color=red]{ $deadCount }[/color] выживальщики мёртв.
    }
survivor-round-end-alive-count =
    { $aliveCount ->
        [one] [color=yellow]{ $aliveCount }[/color] выживальщик был брошен на станции.
       *[other] [color=yellow]{ $aliveCount }[/color] выживальщиков был брошено на станции.
    }
survivor-round-end-alive-on-shuttle-count =
    { $aliveCount ->
        [one] [color=green]{ $aliveCount }[/color] сделал это.
       *[other] [color=green]{ $aliveCount }[/color] сделали это.
    }

## Wizard

objective-issuer-swf = [color=turquoise]Федерация Магов[/color]
wizard-title = Волшебник
wizard-description = На станции появился волшебник! Никогда не знаешь, что он может сделать.
roles-antag-wizard-name = Волшебник
roles-antag-wizard-objective = Преподайте им урок, который они никогда не забудут.
wizard-role-greeting =
    ВЫ - ВОЛШЕБНИК!
    Между Федерация Магов и НаноТрейзен возникла напряженность.
    Поэтому Федерация Магов выбрала вас, чтобы нанести визит на станцию.
    Продемонстрируй им свои способности.
    Что вы будете делать, зависит от вас, но помните, что Федерация Магов хочет, чтобы вы выбрались живыми.
wizard-round-end-name = волшебник

## TODO: Wizard Apprentice (Coming sometime post-wizard release)

