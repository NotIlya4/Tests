## Postgres итерация #1
Сейчас при нынешней конфигурации я имею 2мс задержку и 16к вставок в секунду при 40 параллельных раннерах при конфигурации 12 цпу со слабым процом и 150 мб/с 11к йопсов нетворк ссд

### Не хватает цпу
Хотя на ноде есть свободные 25% цпу, цпу прежур 50%, у c# приложения и у постгреса прежур по 20-30%, то есть мы упираемся в цпу, хотя цпу есть свободный. Главная гипотеза в том, что при большом колве контекст свитчей (очень много коротких запросов к постгресу) не получается эффективно утилизировать цпу, также доказательством этого может служить, то что если бы между вставками реально проходило 2мс, то должно было быть 20к вставок в сек, а не 16 (по сути на 20% хуже, что и совпадает с прежуром у c# приложения), то есть приложение не успевает обрабатывать все 40 тасок в параллель и некоторые тупят в таск queue в дотнете

### Код медленный
Все что я написал выше объясняет схуяли у нас при 2 мс обработке всего 16к вставок в сек, а не 20к, но это не объясняет откуда берется эта цифра в 2 мс. Я пробовал дебажить perf постгрес бекенд процессы, по ощущениям fdatasync примерно 1мс, иногда процесс тупит на futex, то есть он об чета синхронизируется, futex'ов не было когда я делал однопоточную вставку, значит это реально синхронизация при записи wal или при записи в буфер таблицы (хотя мб и при записи в буфер вала). Я пока не знаю сколько времени от вставки занимает постгрес, надо попробовать запустить менеджед постгрес чтобы можно было пробы для perf делать и чекать трейспоинты постгреса на завершение запроса. Также рабочей темой будет попробовать pg_wait_sampling, будет видно сколько примерно по времени запрос тупит в ожидании синхронизации

### Потенцевал для разгона
Имеет смысл попробовать добавить цпу, в этом случае задержка должна немного увеличиться, а рпс подрости. Также имеет смысл поставить чекпоинт с 5 минут на 30 где-то так как я увидел, при чекпоинтах происходят ебанутые пики на вал (это не чекпоинтер эти пики делает, а бекенд процессы, то есть на пиках сильно вал генерируется), вероятно это fpi ебошит, ~~хотя по факту вставка идет в последнюю страницу, не совсем понимаю почему тут может fpi большим~~ возможно это PK генерит дох fpi, потому что в него как раз происходит отсортированная вставка (я вставлял гуиды, то есть по сути массовая вставка затрагивает все страницы индексе, еще один аргумент в пользу guid v7!)

### Как узнать что постгрес делает
С нынешними метриками я не могу адекватно оценить что происходит в постгресе на вставке, почему при fdatasync в 1мс приложение детектит задержку в 2мс, пока главная гипотеза это cpu прежур у постгреса (хотя у постгри cpu прежур был 30%, то есть потенциально это 0.6мс, все еще не понятно что происходит оставшеися 0.4мс) и бекенд не сразу приступает к обработке запроса, а также возможно дело в блокировках на буфере, так как я видел futex при трейсинге через перф.

Что можно сделать. Можно попробовать задеплоить постгрес на отдельной ноде вне кубер кластера, чтобы была возможность адекватно установить на ноду perf и делать пробы на трейспоинты постгреса, там можно будет засекать сколько по времени обрабатывался запрос (мб это мое приложение тупит, а не постгрес) (вообще мне просто интересно какие у постргри есть трейспоинты и какую инфу тут можно выудить), также можно будет установить pg_wait_sampling и так будет видно сколько времени постгрес тупит на всяких LW. Еще имеет смылс установить sql exporter и экспортить pg_stat_wal, тк postgres exporter это не поддерживает, в частности интересует wal_fpi, чтобы подтвердить гипотезу про пики, ну и просто интересно как растет и какое сред время на вал