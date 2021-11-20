using System;
using System.Threading;
using System.Threading.Tasks;

namespace BitMagic.Emulation
{
    public class AsyncBarrierControl
    {
        private readonly int _participantCount;
        private int _remainingParticipants;

        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> _controlTcs = new TaskCompletionSource<bool>();

        public AsyncBarrierControl(int participantCount)
        {
            if (participantCount <= 0) throw new ArgumentOutOfRangeException(nameof(participantCount));
            _remainingParticipants = _participantCount = participantCount;
        }

        // release all waiting threads and block control.
        public Task ControlComplete()
        {
            var tcs = _tcs;
            _remainingParticipants = _participantCount;

            _tcs = new TaskCompletionSource<bool>();
            _controlTcs = new TaskCompletionSource<bool>();

            tcs.SetResult(true);
            return _controlTcs.Task;
        }

        public Task SignalAndWait()
        {
            var tcs = _tcs;
            if (Interlocked.Decrement(ref _remainingParticipants) == 0)
            {
                _controlTcs.SetResult(true);
            }

            return tcs.Task;
        }
    }

}
