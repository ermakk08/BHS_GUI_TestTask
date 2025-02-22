# BHS_GUI_TestTask

## Задание № 1 Оконное приложение

[Скачать решение](https://github.com/ermakk08/BHS_GUI_TestTask/releases/download/release/win64.zip)

---

## Задание № 2 Code Review

[Код](https://pastebin.com/QZfAaxdn)

---

### 1. Потокобезопасность (RegisterPassenger)
**Проблема:**
- Неатомарная операция `_currentNumber++` в многопоточной среде.
- Прямое присваивание в словарь без потокобезопасных операций.

**Решение:**
```csharp
public void RegisterPassenger(Person p)
{
    int num = Interlocked.Increment(ref _currentNumber);
    Passengers.TryAdd(num, p);
}
```

**Обоснование:**
- `Interlocked.Increment` гарантирует атомарность инкремента.
- `ConcurrentDictionary.TryAdd()` потокобезопасен.

### 2. Ошибка в статическом методе
**Проблема:**
```csharp
public static string GetAllPassengersNames()
{
    foreach (var passenger in Passengers) // Обращение к нестатическому полю
}
```

**Решение:**
- Убрать `static` из сигнатуры метода.
- Добавить параметр для передачи коллекции:

```csharp
public string GetAllPassengersNames()
{
    var sb = new StringBuilder();
    foreach (var passenger in Passengers.Values)
        sb.Append(passenger.FirstName);
    return sb.ToString();
}
```

### 3. Управление блокировками (CheckCoordinate)
**Проблема:**
- Риск `SynchronizationLockException` при вызове `Monitor.Exit` без захвата.
- Отсутствие гарантированного освобождения блокировки.

**Решение:**
```csharp
public async Task CheckCoordinate(string coord)
{
    lock(_sync)
    {
        // Логика
    }
}
```
- Или с `SemaphoreSlim` для async:
```csharp
private readonly SemaphoreSlim _semaphore = new(1);

public async Task CheckCoordinate(string coord)
{
    await _semaphore.WaitAsync();
    try 
    {
        // Логика
    }
    finally 
    {
        _semaphore.Release();
    }
}
```
### 4. Валидация зависимостей
**Проблема:**
- Возможность передачи `null` в зависимости через конструктор.

**Решение:**
```csharp
public Ship(
    Person p, 
    int id, 
    IPlaceService placeService, 
    ISailDb sailDb, 
    IPassengersDb passengersDb)
{
    PlaceService = placeService ?? throw new ArgumentNullException(nameof(placeService));
    SailDb = sailDb ?? throw new ArgumentNullException(nameof(sailDb));
    PassengersDb = passengersDb ?? throw new ArgumentNullException(nameof(passengersDb));
    // Остальная инициализация
}
```

### 5. Инкапсуляция коллекции
**Проблема:**
- Публичный сеттер позволяет произвольную модификацию коллекции.

**Решение:**
```csharp
public IReadOnlyDictionary<int, Person> Passengers { get; private set; }

// Инициализация:
Passengers = new ConcurrentDictionary<int, Person>().AsReadOnly();
```

### 6. Неиспользуемый параметр
**Проблема:**
```csharp
public void ChangeCaptain(string fn, string ln, string doc)
{
    // Параметр doc не используется
}
```

**Решение:**
- Удалить параметр или добавить валидацию:
```csharp
public void ChangeCaptain(string fn, string ln, string passportId)
{
    if (!Captain.PassportId.Equals(passportId))
        throw new InvalidOperationException("Invalid document");
    // ...
}
```

### 7. Опасность async void
**Проблема:**
```csharp
public async void SetSail() // Некорректная обработка исключений
```

**Решение:**
```csharp
public async Task SetSailAsync()
{
    try 
    {
        // Логика
    }
    catch (Exception ex)
    {
        // Логирование
        throw;
    }
}
```

### 8. Оптимизация работы со строками
**Проблема:**
- Конкатенация в цикле создает множество промежуточных строк.

**Решение:**
```csharp
var sb = new StringBuilder(Passengers.Count * 20); // Предвыделение памяти
foreach (var passenger in Passengers.Values)
    sb.Append(passenger.FirstName);
return sb.ToString();
```

### 9. Обработка координат
**Проблема:**
- Риск `FormatException` при парсинге.
- Возможный `NullReferenceException`.

**Решение

```csharp
if (!long.TryParse(coord, out long parsedCoord))
    throw new ArgumentException("Invalid coordinate format");

if (coordinate is null)
    coordinate = string.Empty;

if (!coordinate.Equals(tmp, StringComparison.Ordinal))
    coordinate = tmp;
```

### 10. Стиль кода
**Исправления:**
- Поля: `VoyageId → voyageId`.
- `_sync` сделать `readonly`:
```csharp
private static readonly object _sync = new();
```

### 11. Неизменяемость данных (Person)
**Решение:**
```csharp
public class Person
{
    public string PassportId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public Person(string passportId, string firstName, string lastName)
    {
        PassportId = passportId;
        FirstName = firstName;
        LastName = lastName;
    }
}
```

### 12. Атомарность операций сохранения
**Решение:**
```csharp
public async Task SetSailAsync()
{
    using var transaction = new TransactionScope();
    try 
    {
        await SailDb.SaveAsync(...);
        await PassengersDb.SaveAsync(...);
        transaction.Complete();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Save failed");
        throw;
    }
}
```

### 13. Оптимизация сериализации
**Решение:**
```csharp
var passengersData = Passengers.Values
    .Select(p => new { p.FirstName, p.LastName })
    .ToList();

await SailDb.SaveAsync(JsonConvert.SerializeObject(passengersData));
```

### 14. Документация
**Пример:**
```csharp
/// <summary>
/// Корабль для управления рейсами
/// </summary>
/// <remarks>
/// Требует обязательной инициализации зависимостей через конструктор
/// </remarks>
public class Ship
{
    /// <summary>
    /// Регистрация пассажира на рейс
    /// </summary>
    /// <param name="p">Данные пассажира</param>
    /// <exception cref="ArgumentNullException">Если пассажир не указан</exception>
    public void RegisterPassenger(Person p)
    {
        // ...
    }
}
```

---

## Задание № 3 Вспомогательный метод
Исходный код:

```csharp
public static class HashSetExtensions
{
   public static IReadOnlyList<T> ConvertToList<T>(this HashSet<T> hashSet)
   {
       var result = new List<T>();


       foreach (var value in hashSet)
       {
           result.Add(value);
       }
      
       return result;
   }
}
```

---


### 1. Использование AddRange вместо цикла
Цикл `foreach` с пошаговым добавлением элементов через `Add` может быть неоптимальным для больших коллекций. Можно заменить цикл на `AddRange`, который внутренне оптимизирован:

```csharp
var result = new List<T>(hashSet.Count);
result.AddRange(hashSet);
```

### 2. Предварительное выделение памяти для списка
Создание `List<T>` без указания ёмкости приводит к многократному копированию массива. Можно инициализировать список с ёмкостью, равной размеру `HashSet`:

```csharp
var result = new List<T>(hashSet.Count);
result.AddRange(hashSet);
```

### 3. Возврат неизменяемого списка
Возвращаемый `IReadOnlyList<T>` может быть приведен к изменяемому `List<T>`. Можно использовать `AsReadOnly()` для гарантированной защиты от изменений:

```csharp
public static IReadOnlyList<T> ConvertToList<T>(this HashSet<T> hashSet)
{
    var list = new List<T>(hashSet.Count);
    list.AddRange(hashSet);
    return list.AsReadOnly();
}
```

### 4. Обработка пустого HashSet
Создание списка для пустого `HashSet` избыточно. Можно возвращать `Array.Empty<T>()` или кэшированный пустой список:

```csharp
if (hashSet.Count == 0)
    return Array.Empty<T>();
```

### 5. Использование связного списка
Для использования списка на основе массива необходимо, чтобы свободная память шла 'подряд', что может быть затруднительно при большом числе элементов. В таком случае можно использовать связный список.

### 6. Ленивый прокси-список (опционально)
**Идея:** Отложенное копирование элементов до первого обращения по индексу.  
**Проблемы:**
- `HashSet` не гарантирует порядок элементов, что может нарушить логику индексации.
- Изменения в `HashSet` после создания прокси приведут к неконсистентности данных.

Реализация возможна, но требует осторожности. Пример наивной реализации:

```csharp
public class LazyHashSetProxy<T> : IReadOnlyList<T>
{
    private readonly HashSet<T> _hashSet;
    private List<T>? _list;

    public LazyHashSetProxy(HashSet<T> hashSet) => _hashSet = hashSet;

    public T this[int index]
    {
        get
        {
            _list ??= new List<T>(_hashSet);
            return _list[index];
        }
    }

    public int Count => _hashSet.Count;
    public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();
}
```

**Использование:**

```csharp
public static IReadOnlyList<T> ConvertToList<T>(this HashSet<T> hashSet) 
    => new LazyHashSetProxy<T>(hashSet);
```
