
IoTWork.Reader is a C# software component to use as standalone application.
It is mainly focused on reading data from sensors and sending it to the IoTWork.Central.

In the following documentation the words IoTWork.Reader and Reader are interchangeable.

# Technical Issues

Platforms: 

* Linux mono: this repository
* Linux .net core: prototype to be released
* Windows .net: this repository
* Windows .net core: prototype to be released

Language: C#

Note: Current version of IoTWork.Reader is focused on the [Raspberry Pi3](https://www.raspberrypi.org/) and [Sense Hat](https://www.raspberrypi.org/products/sense-hat/) extension board; the operating system used is the [raspbian](https://www.raspbian.org/) Linux and the toolchain used is [mono](http://www.mono-project.com/). We also tested it on Ubuntu 16.04 LTS.

# What's a Reader

A IoTWork.Reader is a **"managed multi-sensors pluggable data sampler and transmitter"**.

It's **multi-sensors** because it can manage up to N sensors at time.
It's **pluggable** because you can write you own dll for Sensor Management, Data Sample Abstraction and Data Manipulation.
It's a **data sampler** because data reading is based on trigger policy (a trigger run each X seconds and read the data from the sensor).
It's a **transmitter** because data read are transmitted to the Central.
It's a **managed** because it has a network interface (tcp, udp, ...) to connect to the application and manage it.

The protocol used to trasmit data from the Reader to the Central is known as **IoTWork.Protocol** and it is documentated here.

# The Reader and Central

Usually interaction between Central and Reader happens as the following:

* Driven from the Central. In this scenario the Central sends commands to the Reader and this one can answer with reply message.
* Pushed from the Reader. In this scenario the Reader sends data collected to the Central





# Components of a Reader

The main components of a reader are:

1. Manager: the network interface used to manage it. Read more.
2. Data Sendere: the network component used to send data to the central. Read more.
3. Chains: a chain is an abstraction used to acquire and manage data from a specific sensor before to send it to the Central. Read more.


# Configuring a Reader

Almost any component of a Reader can be configured using an XML file.
This XML file is hosted inside the iotreader folder.

The XML configuration file include following sections:

1. Base information about the reader
2. Description of the Managent
3. Description of the Data
4. Description of the Sensors, Pipes, Triggers and Chains 

# iotreader folder

The iotreader folder included all components (Sensors, Pipes and your own dll) needed by the reader.
It's located at the path /iot/iotreader/.
The filesystem of the iotreader folder is the following:


The main sub folders are:

* bin: includes the IotWork.Reader binary
* conf: includes configuration files
* modules: includes dll loaded as shared between components (Sensors and Pipes).
* sensors: dll of the sensors
* pipes: dll of the pipes

# Running a Reader

IoTWork.Reader is run from a Linux command line or script using the following syntax:


Configuration and modules are included inside iotreader folder.

During the start process the IoTWork.Reader:

1. read configuration /iot/iotreader/conf/xxxxx
2. load modules listed inside the file /iot/iotreader/modules/modules.txt (dll have to be placed in this same folder)
3. parse the XML configuration file
4. for each sensor it allocate it, after it allocate the trigger, read and allocate the chain by mounting each piece (Sensor, Pipes and Trigger).
5. configure the Management
6. configure the Data
7. start sensor jobs, one for chain
8. start management interface
9. go in listen state and wait for command from the outside

# How a Reader works

The structure of the  iotreader folder is the following

# Commands Managed by a Reader

# Reader internals
