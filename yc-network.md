worker1 (controller, ru-central1-b) to kafka2 (ru-central1-b) between myapp and nginx with 850KB (its not transmit + recive, only one of option)
|runners|latency|rate avg|rate total|traffic|
| --- | --- | --- | --- | --- |
|1|3.6ms|280|280|190 MB/S|
|2|5.1ms|200|400|268 MB/S|
|4|9.1ms|111|444|290 MB/S|
|50|107ms|9.5|470|320 MB/S|
|200|480ms|2.1|420|290 MB/S|

worker1 to kafka1 (ru-central1-a)
|runners|latency|rate avg|rate total|traffic|
| --- | --- | --- | --- | --- |
|1|6.9ms|145|145|101 MB/S|
|2|7.6ms|130|260|182 MB/S|
|4|10.4ms|95|390|267 MB/S|
|50|107ms|9.5|470|323 MB/S|

worker1 to kafka3 (ru-central1-d)
|runners|latency|rate avg|rate total|traffic|
| --- | --- | --- | --- | --- |
|1|10.4ms|96|96|66 MB/S|
|2|10.9ms|91|182|126 MB/S|
|4|12.3ms|81|327|223 MB/S|
|50|100ms|10.2|513|345 MB/S|

kafka1 (myapp) to kafka3, myapp in dedicated not controller node but with less cpu
|runners|latency|rate avg|rate total|traffic|
| --- | --- | --- | --- | --- |
|1|12.6ms|84|84|57 MB/S|
|2|12.3ms|83|164|111 MB/S|
|4|13.5ms|70|285|200 MB/S|
|50|98ms|10.3|510|345 MB/S|