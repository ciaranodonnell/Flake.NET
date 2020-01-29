using COD.FlakeDN.Client;
using System;
using System.Collections.Generic;

namespace COD.FlakeDN.Generator
{
    public class FlakeIdGenerator : IFlakeIdGenerator
    {
        private long nodeId;
        private int timestampLeftShift;
        private int sequenceBits;
        private long sequenceMask;
        private long nodeMask;
        private bool spinOnSequenceExhaustion;
        private long lastTime=-1;
        private long currentSequence;

        private object LockObject = new object();
        private ITimeSource clock;


        public FlakeIdGenerator(GeneratorParameters parameters)
        {
            Initialize(parameters);
        }

        void Initialize(GeneratorParameters parameters)
        {
            
            this.nodeId = Convert.ToInt64(parameters.NodeId) << parameters.SequenceBits;
            this.timestampLeftShift = parameters.SequenceBits + parameters.NodeBits;
            this.sequenceBits = parameters.SequenceBits;
            this.sequenceMask = -1L ^ (-1L << parameters.SequenceBits);
            this.nodeMask = (-1L ^ (-1L << timestampLeftShift)) ^ sequenceMask;
            this.spinOnSequenceExhaustion = parameters.SpinWhenSequenceExhausted;

            clock = parameters.TimeSource ?? new SystemClockTimeSource(new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        }

        public IEnumerable<long> NewIds(int numberOfIds)
        {

            // this is one potential implementation. You get the ids generated as you consume them from the enumerable, preserving sortablility as best we can.
            // this comes at the cost of re-entering the lock for every Id which has a cost to it too.
            // if that is be avoided we'd need to refactor the inside of the lock to a seperate private method and promote the lock to the public methods. 
            // You'd then want to generate them straight into a list to prevent holding the lock while clients enumerate the enumerable

            for (int x = 0; x<numberOfIds; x++)
                yield return NewId();
        }

        public Int64 NewId()
        {
            lock (LockObject)
            {
                var time = clock.GetCurrentTime();

                if (time < lastTime)
                {
                    return (Int64)FlakeIdErrors.TimeMovingBackwards;
                }


                if (lastTime == time)
                {
                    var nextSequence = currentSequence+1;

                    if ((nextSequence & sequenceMask) < currentSequence)
                    {
                        if (spinOnSequenceExhaustion)
                        {
                            while ((time = clock.GetCurrentTime()) == lastTime) ;
                            currentSequence = 0;
                            lastTime = time;
                        }
                        else
                        {
                            //check if sequence will loop for the current millisecond number. return error
                            return (long)FlakeIdErrors.SequenceExhausted;
                        }
                    }
                    else
                    {
                        ++currentSequence;
                    }
                }
                else
                {
                    currentSequence = 0;
                    lastTime = time;
                }

                long id = (time << timestampLeftShift) | nodeId | currentSequence;

                return id;
            }
        }


 
    }
}
