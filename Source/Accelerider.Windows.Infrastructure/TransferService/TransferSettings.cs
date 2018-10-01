﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using Polly;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferSettings
    {
        internal TransferSettings() { }

        public IAsyncPolicy BuildPolicy { get; set; }

        public BlockDownloadItemPolicy DownloadPolicy { get; internal set; }

        public int MaxConcurrent { get; set; }
    }

    public enum HandleCommand
    {
        Throw,
        Retry,
        Break
    }

    public delegate HandleCommand BlockDownloadItemExceptionHandler<in TException>(TException exception, int retryCount, BlockTransferContext context) where TException : Exception;

    public class BlockDownloadItemPolicy
    {
        private readonly ConcurrentDictionary<Guid, int> _retryStatistics = new ConcurrentDictionary<Guid, int>();
        private readonly ConcurrentDictionary<Type, BlockDownloadItemExceptionHandler<Exception>> _handlers = new ConcurrentDictionary<Type, BlockDownloadItemExceptionHandler<Exception>>();

        private readonly Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>> _blockDownloadItemFactory;

        public BlockDownloadItemPolicy(Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>> blockDownloadItemFactory)
        {
            _blockDownloadItemFactory = blockDownloadItemFactory;
        }

        public BlockDownloadItemPolicy Catch<TException>(BlockDownloadItemExceptionHandler<TException> handler)
            where TException : Exception
        {
            var type = typeof(TException);
            if (_handlers.ContainsKey(type))
                throw new ArgumentException($"A handler that handles {typeof(TException)} exceptions already exists. ");

            _handlers[type] = (e, rertyCount, context) => handler((TException)e, rertyCount, context);
            return this;
        }

        internal Func<IObservable<(Guid Id, int Bytes)>, IObservable<(Guid Id, int Bytes)>> ToInterceptor()
        {
            return ExceptionInterceptor;
        }

        private IObservable<(Guid Id, int Bytes)> ExceptionInterceptor(IObservable<(Guid Id, int Bytes)> observable)
        {
            return observable.Catch<(Guid Id, int Bytes), BlockTransferException>(e =>
            {
                var handler = _handlers.FirstOrDefault(item => item.Key.IsInstanceOfType(e.InnerException)).Value;
                if (handler != null)
                {
                    switch (handler(e.InnerException, GetRetryCount(e.Context.Id), e.Context))
                    {
                        case HandleCommand.Throw:
                            return Observable.Throw<(Guid Id, int Bytes)>(e.InnerException ?? e);
                        case HandleCommand.Retry:
                            SetRetryCount(e.Context.Id);
                            return _blockDownloadItemFactory.Then(ExceptionInterceptor)(e.Context);
                        case HandleCommand.Break:
                            return Observable.Empty<(Guid Id, int Bytes)>();
                    }
                }

                return Observable.Throw<(Guid Id, int Bytes)>(e.InnerException ?? e);
            });
        }

        private int GetRetryCount(Guid id) => _retryStatistics.ContainsKey(id) ? _retryStatistics[id] : 0;

        private void SetRetryCount(Guid id)
        {
            if (!_retryStatistics.ContainsKey(id))
            {
                _retryStatistics[id] = 0;
            }
            _retryStatistics[id]++;
        }
    }
}
