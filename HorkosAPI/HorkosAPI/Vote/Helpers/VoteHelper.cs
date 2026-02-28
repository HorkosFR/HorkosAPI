namespace HorkosAPI.Vote.Helpers
{
    public static class VoteHelper
    {
        public static double UpdateScore(int? voteAmount, double? currentScore, int newScore)
        {
            if (voteAmount == null || voteAmount == 0 || currentScore == null)
            {
                return newScore;
            }
            return (double)((currentScore + newScore) / (voteAmount + 1));
        }
    }
}
