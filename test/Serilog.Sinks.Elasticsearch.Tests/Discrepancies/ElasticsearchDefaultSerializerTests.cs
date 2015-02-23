using System;
using System.Linq;
using Elasticsearch.Net.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace Serilog.Sinks.Elasticsearch.Tests.Discrepancies
{
    [TestFixture]
    public class ElasticsearchDefaultSerializerTests : ElasticsearchSinkTestsBase
    {
        [Test]
        public void Should_SerializeToExpandedExceptionObjectWhenExceptionIsSet()
        {
            _options.Serializer = new ElasticsearchDefaultSerializer();
            var loggerConfig = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.WithMachineName()
               .WriteTo.ColoredConsole()
               .WriteTo.Elasticsearch(_options);

            var exceptionMessage = "test_with_default_serializer";
            var logger = loggerConfig.CreateLogger();
            using (logger as IDisposable)
            {
                try
                {
                    var innerException = new ApplicationException("inner exception");
                    throw new Exception(exceptionMessage, innerException);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Test exception. Should contain an embedded exception object.");
                }
                logger.Error("Test exception. Should not contain an embedded exception object.");
            }

            var postedEvents = this.GetPostedLogEvents(expectedCount: 2);
            foreach (var post in _seenHttpPosts)
            {
                Console.WriteLine(post);
            }

            var firstEvent = postedEvents[0];
            firstEvent.Exceptions.Should().NotBeNull().And.HaveCount(2);
            firstEvent.Exceptions[0].Message.Should().NotBeNullOrWhiteSpace()
                .And.Be(exceptionMessage);

            var secondEvent = postedEvents[1];
            secondEvent.Exceptions.Should().BeNullOrEmpty();

        }
    }

}
