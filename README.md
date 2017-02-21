# WcfClientStressTest
Stress test volume connections over WCF TCP using a standardized ServiceClientWrapper

Some observations
* Creating new client instances is not inexpensive
* WCF connections is not disposed of peropely by default (Do NOT use USING)
* Must close after each call or the available sockets gets used up
* Exceptions slows the speed down considerably