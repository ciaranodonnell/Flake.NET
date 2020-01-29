using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace COD.FlakeDN.Generator.Tests
{
    public class GeneratorUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetAnId()
        {
            FlakeIdGenerator g = new FlakeIdGenerator(new GeneratorParameters { NodeId = 111, NodeBits = 12, SequenceBits = 10});

            g.NewId();
        }



        [Test]
        public void Test100000AreSortable()
        {
            int testSize = 100000;
            var p = new GeneratorParameters { NodeId = 111, NodeBits = 12, SequenceBits = 10 };
            FlakeIdGenerator g = new FlakeIdGenerator(p);
            List<Int64> ids = new List<long>(100);
            for (int x = 0; x < testSize; x++)
                ids.Add(g.NewId());

            for (int x = 0; x < (testSize - 1); x++)
            {
                Assert.IsTrue(ids[x] > 0, "Ids is error " + ids.ToString());
                Assert.IsTrue(ids[x] < ids[x + 1], $"{x} id should be less than next {p.GetIdParts(ids[x])} and {p.GetIdParts(ids[x + 1])}");
            }

        }


        [Test]
        public void TestTimeMovingBackward()
        {
            var timesource = new EveryFiveTimeGoesBackwardsTimeSource();

            FlakeIdGenerator g = new FlakeIdGenerator(new GeneratorParameters { NodeId = 111, NodeBits = 12, SequenceBits = 10, TimeSource = timesource });
            List<Int64> ids = new List<long>(100);
            for (int x = 0; x < 6; x++)
                ids.Add(g.NewId());

            Assert.IsTrue(ids[0] > 0, "Ids is error " + ids.ToString());
            Assert.IsTrue(ids[1] > 0, "Ids is error " + ids.ToString());
            Assert.IsTrue(ids[2] > 0, "Ids is error " + ids.ToString());
            Assert.IsTrue(ids[3] > 0, "Ids is error " + ids.ToString());
            Assert.IsTrue(ids[4] == (long)FlakeDN.Client.FlakeIdErrors.TimeMovingBackwards, "Time reversal not detected" + ids.ToString());
            Assert.IsTrue(ids[5] > 0, "Time should be back");

        }


        [Test]
        public void TestSpinOnExhaustion()
        {
            var timesource = new RepeatTimeForXTimeSource(10);
            var gps = new GeneratorParameters
            {
                SpinWhenSequenceExhausted = true,
                NodeId = 111,
                NodeBits = 12,
                SequenceBits = 2,
                TimeSource = timesource
            };

            FlakeIdGenerator g = new FlakeIdGenerator(gps);

            List<long> ids = new List<long>();

            for (int x = 0; x < 5; x++)
                ids.Add(g.NewId());


            Assert.AreEqual(0,gps.GetSequenceNumber(ids[0]) );
            Assert.AreEqual(1,gps.GetSequenceNumber(ids[1]) );
            Assert.AreEqual(2,gps.GetSequenceNumber(ids[2]) );
            Assert.AreEqual(3,gps.GetSequenceNumber(ids[3]) );
            Assert.AreEqual(0,gps.GetSequenceNumber(ids[4]) );
                       
            Assert.AreEqual(10, timesource.Calls);


        }
    }
}