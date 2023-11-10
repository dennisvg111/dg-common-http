using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.TestUtilities
{
    /// <summary>
    /// This class provides functionality to test if ConfigureAwait(false) is used when awaiting asynchronous functions.
    /// </summary>
    public static class ContextAsserts
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Asserts that the given task does not continue on the curent context after awaiting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        public static void AssertContextSwitched<T>(Func<Task<T>> task)
        {
            var context = TestSynchronizationContext.Instance;

            lock (_lock)
            {
                var old = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(context);
                    context.Reset();

                    task().Wait();

                    Assert.True(context.ContextSwitched, $"Task continued on the same context it started, use ConfigureAwait(false) instead.");
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(old);
                }
            }
        }

        /// <summary>
        /// Asserts that the given task does not continue on the curent context after awaiting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        public static void AssertContextSwitched(Func<Task> task)
        {
            AssertContextSwitched(() => task().ContinueWith((t) => Task.FromResult(1)));
        }

        private class TestSynchronizationContext : SynchronizationContext
        {
            private static readonly TestSynchronizationContext _instance = new TestSynchronizationContext();
            public static TestSynchronizationContext Instance => _instance;

            private bool _postCalled;

            public bool ContextSwitched => !_postCalled;

            private TestSynchronizationContext()
            {
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _postCalled = true;
                base.Post(d, state);
            }

            public void Reset()
            {
                _postCalled = false;
            }
        }
    }
}
