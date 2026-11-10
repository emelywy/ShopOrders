# Дефекти проєктування коду 

| № | Дефект | Місце | Прояв | Рефакторинг |
|---|---|---|---|---|
| 1 | Магічне число (Magic Number) | `Handle` (рядки 81–105), `Preview` (рядки 146–169) | Числові літерали без пояснення сенсу: знижки `0.15m`, `0.30m`, `0.05m`, ліміт `500m`, поріг доставки `1000m`, вартість `60m`, поріг лояльності `10` | Replace Magic Number with Constant |
| 2 | Вкладені умови (Nested Conditionals) | `Handle`, рядки 45–74 | 4 рівні вкладеності операторів `if` (глибина вкладеності 4); основне тіло зміщене на 20 пробілів | Decompose Conditional, охоронні умови (Guard Clauses) |
| 3 | Тимчасове поле (Temporary Field) | `LegacyOrderProcessor`, рядки 25–26 | Поля `_tmpSum` та `_tmpDiscount` використовуються як проміжні змінні під час роботи методу `Handle` і не зберігають тривалий стан об'єкта | Remove Temporary Field |
| 4 | Довгий список параметрів (Long Parameter List) | `Handle`, рядки 29–41 | Метод приймає 11 параметрів; при цьому 6 параметрів (`clientId`, `clientName`, `clientMail`, `clientKind`, `clientDone`, `createdAt`) описують одну сутність | Introduce Parameter Object |
| 5 | Дублювання коду (Duplicated Code) | `Handle` (рядки 76–105) та `Preview` (рядки 137–169) | Ідентичний розрахунок вартості, знижок і доставки, що повністю дублює 25+ рядків коду в обох методах | Extract Method |
| 6 | Одержимість елементарними типами (Primitive Obsession) | `Handle`, рядки 35–37; `ShopCustomer`, рядок 11 | Стан замовлення передається як `string` (`"new"`, `"paid"`), статус клієнта — як `string` (`"vip"`, `"staff"`), валюта — як рядок замість використання `enum` або value objects | Replace Primitive with Object / Enum |
| 7 | Заздрість до чужих даних (Feature Envy) | `DescribeClient`, рядки 175–192 | Метод класу `LegacyOrderProcessor` працює суто з полями екземпляра `ShopCustomer` (`Name`, `Kind`, `DoneCount`, `Email`, `SinceUtc`), нічого не беручи зі свого класу | Move Method |
| 8 | Довгий метод (Long Method) | `Handle`, рядки 29–130 | Метод містить понад 85 рядків коду та поєднує 5 різних зон відповідальності (валідація, розрахунок суми, нарахування знижки, формування звіту, логування) | Extract Method |
| 9 | Великий клас (Large Class / God Object) | `LegacyOrderProcessor`, рядки 23–204 | Клас порушує принцип SRP: одночасно відповідає за валідацію вхідних даних, ціноутворення, бізнес-правила знижок, текстове форматування звітів і ведення аудиторського журналу | Extract Class |
