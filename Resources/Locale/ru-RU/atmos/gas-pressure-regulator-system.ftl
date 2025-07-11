# Examine Text
gas-pressure-regulator-system-examined =
    The valve is [color={ $statusColor }]{ $open ->
        [true] открыт
       *[false] закрыт
    }[/color].
gas-pressure-regulator-examined-threshold-pressure = Пороговое давление установлено на уровне [color=lightblue]{ $threshold } кПа[/color].
gas-pressure-regulator-examined-flow-rate = Расходомер показывает [color=lightblue]{ $flowRate } л/с[/color].
