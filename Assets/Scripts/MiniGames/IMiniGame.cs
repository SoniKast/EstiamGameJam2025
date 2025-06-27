using System;

namespace EstiamGameJam2025
{
    public interface IMiniGame
    {
        void Initialize(int difficulty, Action<bool> onComplete);
        void Reset();
        void ForceComplete();
    }
}