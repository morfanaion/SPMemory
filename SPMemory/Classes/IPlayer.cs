namespace SPMemory.Classes
{
    public interface IPlayer
    {
        int PlayerId { get; set; }
        string Name { get; set; }
        int Score { get; set; }

        void EndTurn();
        void StartTurn();
    }
}
