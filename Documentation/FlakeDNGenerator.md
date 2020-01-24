# Flake.NET Generator



## Class: FlakeIdGenerator

This is the actual Id generator. To create one you need to pass it a [GeneratorParameters](GeneratorParameters.md) object.
This controls the structure of the Flake Ids that the generator will generate.

### GeneratorParameters

| Property | Description |
|:---------|:------------|
| NodeBits | The number of bits to allocate to the Node Id in the Id. This controls the number of Id generators that can exist |
| SequenceBits | The number of bits to allocate to the Sequence Number in the Id. This controls the number of Ids that can be generated for every clock tick |
| NodeId | The actual Node Id of this Id Generator. This is encoded in every Id it generates |
| SpinWhenSequenceExhausted | Boolean that controls what to do when the Id generator has run out of sequence numbers for the current clock tick <br> *True*: Keep asking for the time in a tight loop until it changes, then sequence number will reset to 0 <br> *False*: Return an error number (negative) to indicate the sequence number has been exhausted
| TimeSource | Passing in a custom TimeSource allows you to control the source and granularity of the clock used. If this is null, a *SystemClockTimeSource* will be used, which uses Milliseconds since the start of 2020 as its time value.

### Usage

#### Construction

Using the Id Generator is quite simple. Create an instance using a GeneratorParameters. 

You will need to set the Node and Sequence bit lengths in order to control the structure of the Ids.
The number of bits allocated to the timestamp is *63 - (NodeBits + SequenceBits)*

You can also create an instance of an ITimeSource interface. 
This needs to give forward moving values for the time that will be used to generate the Ids.

You will need to set the NodeId which is a unique Id for this generator. 
It needs to be unique across the system, as two generators with the same NodeId will be at risk of collisions.

#### Use

Using the generator simple consists of calling the NewId method.

``` csharp


idGenerator.NewId()
```