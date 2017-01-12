
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


## An Example of Multi-Chain Reader

![An Example of MultiChain Reader](https://rawgit.com/samnium/IoTWork.Reader/master/images/IoTWork.Reader.Chain4.png)

## An Example of Multi-Chain Reader Simulation

![Data Sample simulation](https://rawgit.com/samnium/IoTWork.Reader/master/images/IoTWork.Reader.Chain5.png)

## Type of Sensors

We have four type of Sensor:

* Simple: don't produce any data
* Random: produce a random integer
* Custom: custom sensor object implementing IIoTSensor, produce a IIoTSample object.

## Type of Pipes

We have four type of Pipes:

* Simple: do nothing, simply traverse it
* Wait: wait and traverse
* Custom: custom pipe object implementing IIoTPipe, gets a IIoTSample and returns a IIoTSample

## About Custom Chains

A custom Chain usually is composed by:

* A Custom IIoTSensor who produces IIoTSample object
* One or Mode IIoTPipe who transforms a IIoTSample in an other IIoTSample

Interfaces IIoTSample, IIoTSensor, IIotPipe belong to IoTWork.Contracts namespace and dll.

## How to define a Chain

A Chain is defined inside the XML configuration file (




