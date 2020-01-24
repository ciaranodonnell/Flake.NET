# GeneratorParameters

## Properties

| Property | Description |
|---|---|
| NodeBits | The number of bits to allocate to the Node Id in the Id. This controls the number of Id generators that can exist |
| SequenceBits | The number of bits to allocate to the Sequence Number in the Id. This controls the number of Ids that can be generated for every clock tick |
| NodeId | The actual Node Id of this Id Generator. This is encoded in every Id it generates |
| SpinWhenSequenceExhausted | Boolean that controls what to do when the Id generator has run out of sequence numbers for the current clock tick <br> *True*: Keep asking for the time in a tight loop until it changes, then sequence number will reset to 0 <br> *False*: Return an error number (negative) to indicate the sequence number has been exhausted
| TimeSource | Passing in a custom TimeSource allows you to control the source and granularity of the clock used. If this is null, a *SystemClockTimeSource* will be used, which uses Milliseconds since the start of 2020 as its time value.

## Methods

| Name | Parameters | Returns | Description |
|---|---|---|---|
| GetSequenceNumber | Int64 Id | Int64 | Gets the SequenceId from an Id using the properties of the GeneratorParameters |
| GetTimeStamp | Int64 Id | Int64 | Gets the Timestamp component from an Id using the properties of the GeneratorParameters |
| GetNodeId | Int64 Id | Int64 | Gets the Node Id component from an Id using the properties of the GeneratorParameters |
| MaxSequenceNumber | | Int64 | Gets the maximum sequence number of a generator that uses the current parameters |
| GetMaxTimestamp | | Int64 | Gets the maximum sequence number of a generator that uses the current parameters |
