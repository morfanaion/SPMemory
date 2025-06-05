namespace SPMemory
{
    public interface IMemoryPlayer
    {
        void RegisterCardSeen(int cardIdx, int pairId);

        Task PerformTurn();

        int RegisterCardClick(int cardIdx);
    }
}
