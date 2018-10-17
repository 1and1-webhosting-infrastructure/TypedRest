using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on an <see cref="IEndpoint"/>.
    /// </summary>
    public interface IEndpointCommand
    {
        /// <summary>
        /// The text input/output device used for user interaction.
        /// </summary>
        IConsole Console { get; set; }

        /// <summary>
        /// Parses command-line arguments and executes the resulting operation.
        /// </summary>
        /// <param name="args">the console arguments.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        Task ExecuteAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default);
    }
}
