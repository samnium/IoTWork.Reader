
# The Chain

The Chain is main entity you find inside a Reader.
Usually a Reader can have more than one Chain.

A Chain has a well defined structure composed by three types of components:

* Sensor: the source of the data sample
* Trigger: the timer who regulate sensor acquisition
* Pipe: the element traversed by the data sample

![Components of a Chain](https://rawgit.com/samnium/IoTWork.Reader/master/images/IoTWork.Reader.Chain1.png)

A Chain has one Sensor, one Trigger and one or more Pipes. 

![An Example Chain](https://rawgit.com/samnium/IoTWork.Reader/master/images/IoTWork.Reader.Chain2.png)

Data Sample is produced by the Sensor and go throughput the Pipes one for time until the end of the Chain.

![Data Sample throughput a Chain](https://rawgit.com/samnium/IoTWork.Reader/master/images/IoTWork.Reader.Chain3.png)

When a Data Sample reaches the end of the Chain then it is enqueued, it waits to be dispatched over the network to Central.


## The Manager

## The Dispatcher


