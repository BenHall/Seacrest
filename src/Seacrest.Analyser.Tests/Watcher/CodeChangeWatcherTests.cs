using System.Threading;
using NUnit.Framework;
using System.IO;
using Seacrest.Analyser.Watcher;

namespace Seacrest.Analyser.Tests.Watcher
{
    [TestFixture]
    public class CodeChangeWatcherTests
    {
        string tempFileName;

        [TearDown]
        public void Teardown()
        {
            File.Delete(tempFileName);
        }

        [Test]
        public void When_new_file_added_an_event_is_raised()
        {
            bool eventRaised = false;
            tempFileName = Path.GetTempFileName() + ".cs";

            CodeChangeWatcher watcher = new CodeChangeWatcher();
            watcher.CodeChanged += (s,e) => eventRaised = true;
            watcher.Watch(Path.GetDirectoryName(tempFileName));

            File.WriteAllText(tempFileName, "Some random content");

            for (int i = 0; i < 100; i++)
            {
                if(!eventRaised)
                    Thread.Sleep(10);
            }
            Assert.That(eventRaised, Is.True);
        }
    }
}