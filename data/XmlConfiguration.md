<iotreader>
  <device>
    <network>1</network>
    <region>1</region>
    <ring>1</ring>
    <id>1</id>
    <name>1.1.1.1</name>
  </device>
  <manager>
    <client
      type="udp">
      <Uri>udp://localhost:22175</Uri>
    </client>
    <formatter
      type="json" />
    <compressor
      type="gzip" />
    <signer
      type="hmac-sha1" />
    <AliveTimeoutInMilliseconds>5000</AliveTimeoutInMilliseconds>
  </manager>
  <dispatcher>
    <client
      type="udp">
      <Uri>udp://localhost:11175</Uri>
    </client>
    <formatter
      type="json" />
    <compressor
      type="gzip" />
    <signer
      type="hmac-sha1" />
    <throttling
      type="none" />
  </dispatcher>
  <sensors>
    <sensor
      type="custom">
      <UniqueId>1</UniqueId>
      <UniqueName>1.1.1.1.1.s1</UniqueName>
      <TriggerUniqueName>1.1.1.1.1.t1</TriggerUniqueName>
      <ChainUniqueName>1.1.1.1.1</ChainUniqueName>
      <LibraryPath>/iot/iotreader/sensors/Pi.SHat.Sensor.Humidity.mono.dll</LibraryPath>
      <Parameters></Parameters>
    </sensor>
    <sensor
      type="custom">
      <UniqueId>2</UniqueId>
      <UniqueName>1.1.1.1.2.s1</UniqueName>
      <TriggerUniqueName>1.1.1.1.2.t1</TriggerUniqueName>
      <ChainUniqueName>1.1.1.1.2</ChainUniqueName>
      <LibraryPath>/iot/iotreader/sensors/Pi.SHat.Sensor.Pressure.mono.dll</LibraryPath>
      <Parameters></Parameters>
    </sensor>
    <sensor
      type="custom">
      <UniqueId>3</UniqueId>
      <UniqueName>1.1.1.1.3.s1</UniqueName>
      <TriggerUniqueName>1.1.1.1.3.t1</TriggerUniqueName>
      <ChainUniqueName>1.1.1.1.3</ChainUniqueName>
      <LibraryPath>/iot/iotreader/sensors/Pi.SHat.Sensor.Temperature.mono.dll</LibraryPath>
      <Parameters></Parameters>
    </sensor>
  </sensors>
  <triggers>
    <trigger
      type="simple">
      <UniqueId>1</UniqueId>
      <UniqueName>1.1.1.1.1.t1</UniqueName>
      <WithIntervalInMilliseconds>15000</WithIntervalInMilliseconds>
      <RepeatForever>false</RepeatForever>
    </trigger>
    <trigger
      type="simple">
      <UniqueId>2</UniqueId>
      <UniqueName>1.1.1.1.2.t1</UniqueName>
      <WithIntervalInMilliseconds>8000</WithIntervalInMilliseconds>
      <RepeatForever>false</RepeatForever>
    </trigger>
    <trigger
      type="simple">
      <UniqueId>3</UniqueId>
      <UniqueName>1.1.1.1.3.t1</UniqueName>
      <WithIntervalInMilliseconds>12000</WithIntervalInMilliseconds>
      <RepeatForever>false</RepeatForever>
    </trigger>
  </triggers>
  <chains>
    <chain>
      <UniqueId>1</UniqueId>
      <UniqueName>1.1.1.1.1</UniqueName>
      <pipes>
        <pipe
          type="simple"
          stage="1" />
        <pipe stage="2" type="custom">
          <LibraryPath>/iot/iotreader/pipes/Pi.SHat.Sensor.Pipe.Dump.mono.dll</LibraryPath>
          <Class>PiSHatPipe_Dump</Class>
        </pipe>
    </pipes>
    </chain>
    <chain>
      <UniqueId>2</UniqueId>
      <UniqueName>1.1.1.1.2</UniqueName>
      <pipes>
        <pipe
          type="simple"
          stage="1" />
        <pipe stage="2" type="custom">
          <LibraryPath>/iot/iotreader/pipes/Pi.SHat.Sensor.Pipe.Dump.mono.dll</LibraryPath>
          <Class>PiSHatPipe_Dump</Class>
        </pipe>
      </pipes>
    </chain>
    <chain>
      <UniqueId>3</UniqueId>
      <UniqueName>1.1.1.1.3</UniqueName>
      <pipes>
        <pipe
          type="simple"
          stage="1" />
        <pipe stage="2" type="custom">
          <LibraryPath>/iot/iotreader/pipes/Pi.SHat.Sensor.Pipe.Dump.mono.dll</LibraryPath>
          <Class>PiSHatPipe_Dump</Class>
        </pipe>
      </pipes>
    </chain>
  </chains>
</iotreader>
