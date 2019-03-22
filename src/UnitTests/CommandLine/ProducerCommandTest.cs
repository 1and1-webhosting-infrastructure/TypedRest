using System.Threading.Tasks;
using MorseCode.ITask;
using Xunit;

namespace TypedRest.CommandLine
{
    public class ProducerCommandTest : CommandTestBase<ProducerCommand<MockEntity>, IProducerEndpoint<MockEntity>>
    {
        [Fact]
        public async Task TestInvoke()
        {
            var output = new MockEntity(2, "b");

            EndpointMock.Setup(x => x.InvokeAsync(default))
                        .Returns(Task.FromResult(output).AsITask());
            ConsoleMock.Setup(x => x.Write(output));

            await ExecuteAsync();
        }
    }
}
