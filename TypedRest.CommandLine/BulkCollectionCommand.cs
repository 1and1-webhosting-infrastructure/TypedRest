﻿namespace TypedRest.CommandLine
{
    /// <summary>
    /// Command operating on a <see cref="BulkCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="BulkCollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class BulkCollectionCommand<TEntity> : BulkCollectionCommandBase<TEntity, BulkCollectionEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST Bulk collection command.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this command operates on.</param>
        public BulkCollectionCommand(BulkCollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(ElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementCommand<TEntity>(elementEndpoint);
        }
    }
}