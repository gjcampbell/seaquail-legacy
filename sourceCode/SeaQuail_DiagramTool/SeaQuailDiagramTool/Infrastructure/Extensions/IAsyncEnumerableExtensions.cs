using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool
{
    public static class IAsyncEnumerableExtensions
    {
        public static async Task<IEnumerable<T>> ToEnumerable<T>(this IAsyncEnumerable<T> stream, int bufferSize = 100)
        {
            var buffer = new BlockingCollection<T>(bufferSize);
            var results = stream.GetAsyncEnumerator();
            if (await results.MoveNextAsync())
            {
                buffer.Add(results.Current);
#pragma warning disable CS4014
                Task.Run(async () =>
                {
                    await using (results)
                    {
                        while (await results.MoveNextAsync())
                        {
                            buffer.Add(results.Current);
                        }
                        buffer.CompleteAdding();
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            else
            {
                buffer.CompleteAdding();
            }
            return buffer.GetConsumingEnumerable();
        }
    }
}
