# Flexberry.SampleForMetrics

После скачивания репозитория необходимо выполнить следующие команды:
```
git submodule init
git submodule update
```
## Доступ к метрикам

После запуска бэкэнда локально доступны следующие вызовы:
1. http://localhost:6500/health - возвращает показатели работоспособности, также возвращает путь до файла с записанными метриками (добавлен текстовый репортер).
2. http://localhost:6500/ping - простой пинг, возвкащает HttpOk("pong").
3. http://localhost:6500/metrics - возвращает все доступнык метрики (то же само что записано в текстовый файл).
## Развернутое приложение на GitHub Pages

Демо-приложение с БД IndexedDb (в браузере):
https://flexberry-app-sandbox.github.io/Flexberry.SampleForMetrics/

## Запуск приложения в Docker

Для запуска приложения с БД требуется [Docker](https://docker.com).

Последовательность действий:

1. Собрать Docker-образы
```
\src\Docker> .\create-image.cmd
```

2. Запустить Docker-образы
```
\src\Docker> .\start.cmd
```

Приложение будет доступно по адресу http://localhost

3. Остановить выполнение Docker-образов
```
\src\Docker> .\stop.cmd
```

## Ссылки на документацию

Подробнее о сгенерированном фронтенде: https://flexberry.github.io/ru/ef3_landing_page.html  
Подробнее о сгенерированном бекенде: https://flexberry.github.io/ru/fo_orm-odata-service.html
