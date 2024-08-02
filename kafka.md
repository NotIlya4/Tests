## Singleton producer
Singleton producer, 1kb data (no compresssion), 10 partitions

|threads|latency|rate avg|rate total|
| --- | --- | --- | --- |
|1|16ms|62|62|
|20|64ms|15.6|312|
|40|118ms|8.2|340|

Per thread producer

|threads|latency|rate avg|rate total|
| --- | --- | --- | --- |
|1|16ms|62|62|
|40|20ms|50|2000|

2000 is not limit