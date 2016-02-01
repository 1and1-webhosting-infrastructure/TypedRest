using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture]
    public class CollectionEndpointTest : EndpointTestBase
    {
        private CollectionEndpoint<MockEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new CollectionEndpoint<MockEntity>(EntryEndpoint, "endpoint");
        }

        [Test, Ignore("Server mock not implemented yet")]
        public async Task TestReadAll()
        {
            //stubFor(get(urlEqualTo("/endpoint/"))
            //    .withHeader("Accept", equalTo(jsonMime))
            //    .willReturn(aResponse()
            //        .withStatus(200)
            //        .withHeader("Content-Type", jsonMime)
            //        .withBody("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]")));

            var result = await _endpoint.ReadAllAsync();
            result.Should().Equal(new MockEntity(5, "test1"), new MockEntity(6, "test2"));
        }

        [Test, Ignore("Server mock not implemented yet")]
        public async Task TestCreate()
        {
            var location = new Uri("/endpoint/new", UriKind.Relative);
            //stubFor(post(
            //    urlEqualTo("/endpoint/"))
            //    .withRequestBody(equalToJson("{\"id\":5,\"name\":\"test\"}"))
            //    .willReturn(aResponse()
            //        .withStatus(201)
            //        .withHeader("Location", location.toASCIIString())));

            var element = await _endpoint.CreateAsync(new MockEntity(5, "test"));
            element.Uri.Should().Be(location);
        }

        [Test]
        public void TestGetByRelativeUri()
        {
            _endpoint[new Uri("1", UriKind.Relative)].Uri
                .Should().Be(new Uri(_endpoint.Uri, "1"));
        }

        [Test]
        public void TestGetByEntity()
        {
            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri(_endpoint.Uri, "1"));
        }

        [Test, Ignore("Server mock not implemented yet")]
        public async Task TestGetByEntityWithLinkHeader()
        {
            //stubFor(get(urlEqualTo("/endpoint/"))
            //    .withHeader("Accept", equalTo(jsonMime))
            //    .willReturn(aResponse()
            //        .withStatus(SC_OK)
            //        .withHeader("Content-Type", jsonMime)
            //        .withHeader("Link", "<children/{id}>; rel=children; templated=true")
            //        .withBody("[]")));

            await _endpoint.ReadAllAsync();

            _endpoint[new MockEntity(1, "test")].Uri
                .Should().Be(new Uri(_endpoint.Uri, "children/1"));
        }
    }
}