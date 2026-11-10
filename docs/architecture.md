# Архітектура рішення ShopOrders

## 2.1. Шари

| Шар | Проєкт | Що містить | Що йому заборонено |
|---|---|---|---|
| Домен | `ShopOrders.Domain` | сутності `Order`, `OrderLine`, `Customer`, `Product`; перелічення станів і подій; таблиця переходів `OrderTransitions`; правило округлення `Money`; виняток `DomainRuleException` | знати про файли, консоль, JSON та інші шари |
| Прикладний | `ShopOrders.Application` | сервіси сценаріїв `OrderService`, `OrderQueryService`; інтерфейси `IOrderRepository`, `ICatalog`, `IClock`, `IAppLog`; політика знижки `IDiscountPolicy`; барикада `InputParser` | знати про формат зберігання й про консоль |
| Інфраструктура | `ShopOrders.Infrastructure` | `JsonOrderRepository`, `JsonCatalog`, записи файлів, `FileAppLog`, `SystemClock` | містити правила домену |
| Застосунок | `ShopOrders.App` | меню, екрани сценаріїв, форматування виводу, композиційний корінь `Program.cs` | містити розрахунки й правила |

Відповідальність кожного шару одним реченням:

- **Домен** зберігає правила предметної області й не дозволяє привести замовлення в недопустимий стан.
- **Прикладний шар** виконує сценарії використання, з'єднуючи домен зі сховищем, годинником і журналом через інтерфейси.
- **Інфраструктура** реалізує інтерфейси прикладного шару засобами файлової системи.
- **Застосунок** перетворює дії оператора на виклики сценаріїв, а результати — на текст.

## 2.2–2.3. Проєкти й правило залежностей

Обрано варіант із чотирма проєктами: правило залежностей контролює компілятор.

**Рисунок 1 — Схема залежностей проєктів**

```
 ShopOrders.App ──────────► ShopOrders.Application ──────────► ShopOrders.Domain
       │                              ▲
       │                              │
       └──────► ShopOrders.Infrastructure
                (реалізує інтерфейси Application)
```

| Стрілка | Пояснення |
|---|---|
| App → Application | меню викликає сервіси сценаріїв і барикаду валідації, але не містить правил |
| Application → Domain | сервіси керують агрегатом `Order`, а правила переходів і знижки виконує домен |
| Infrastructure → Application | сховища реалізують `IOrderRepository` і `ICatalog`, оголошені в прикладному шарі |
| App → Infrastructure | лише композиційний корінь створює конкретні реалізації |

Жодна стрілка не спрямована всередину домену з боку інфраструктури: `ShopOrders.Domain.csproj`
не має жодного `ProjectReference`, тому використати `File` чи `JsonSerializer` у домені можна,
але звернутися до класів інших шарів неможливо — код не скомпілюється. Додатково правило
зафіксоване тестами `ArchitectureTests.Domain_DoesNotReferenceOtherProjects` і
`ArchitectureTests.Application_ReferencesOnlyDomain`.

## 2.4. Композиційний корінь

Єдине місце, де створюються конкретні реалізації, — `src/ShopOrders.App/Program.cs`:

```csharp
SystemClock clock = new();
FileAppLog log = new(Path.Combine("logs", "shoporders.log"), clock);
try
{
    JsonCatalog catalog = JsonCatalog.Load(dataDirectory);
    JsonOrderRepository orders = JsonOrderRepository.Open(dataDirectory);
    OperationContext context = new(clock, log);
    OrderService service = new(orders, catalog, new TieredDiscountPolicy(), context);
    OrderQueryService queries = new(orders, log);
    ConsoleUi ui = new(Console.In, Console.Out);
    OrderScreens orderScreens = new(ui, service, queries, catalog);
    MainMenu menu = new(ui, orderScreens, new ReportScreens(ui, queries), log);
    return menu.Run();
}
```

Чому тут: решта класів отримує залежності через конструктор і знає лише інтерфейси.
Заміна сховища JSON на інше, годинника на фіксований чи журналу на консольний зачіпає
лише ці рядки. Тести створюють ті самі сервіси з тестовими дублерами замість реалізацій
інфраструктури.

## 2.5. Точки розширення

| Передбачувана зміна | Механізм | Що змінюється |
|---|---|---|
| Новий вид знижки (сезонна, за промокодом) | нова реалізація інтерфейсу `IDiscountPolicy` | один новий клас і один рядок у `Program.cs`; `OrderService` не змінюється |
| Нові ставки або ще одна цінова категорія | таблиця рішень `TieredDiscountPolicy.BuildRates` | рядки таблиці й константа межі; алгоритм `DiscountFor` не змінюється |
| Інше сховище (CSV, база даних) | нова реалізація `IOrderRepository` | новий клас в інфраструктурі й один рядок у `Program.cs` |
| Новий стан або подія замовлення | таблиця переходів `OrderTransitions.TargetOf` | нове значення перелічення й рядок таблиці; методи сервісів не змінюються |
