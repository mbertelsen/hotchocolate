using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace HotChocolate.DataLoader
{
    public abstract class BatchDataLoader<TKey, TValue>
        : DataLoaderBase<TKey, TValue>
    {
        protected BatchDataLoader(IBatchScheduler batchScheduler)
            : base(batchScheduler)
        { }

        protected sealed override async Task<IReadOnlyList<Result<TValue>>> FetchAsync(
            IReadOnlyList<TKey> keys,
            CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<TKey, TValue> result =
                await LoadBatchAsync(keys, cancellationToken)
                    .ConfigureAwait(false);

            var items = new Result<TValue>[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                if (result.TryGetValue(keys[i], out TValue value))
                {
                    items[i] = value;
                }
            }

            return items;
        }

        protected abstract Task<IReadOnlyDictionary<TKey, TValue>> LoadBatchAsync(
            IReadOnlyList<TKey> keys,
            CancellationToken cancellationToken);
    }
}
