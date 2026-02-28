using HorkosAPI.Vote.Models;

namespace HorkosAPI.Reliability.Models
{
    public class Evidence
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public double Gravity { get; set; }
        public double Reliability { get; set; }
        public DateTime Date { get; set; }
        public string LinkedTo { get; set; }
        public Evidence() { }

        public Evidence(VoteDTO vote, DateTime date, string tags)
        {
            Type = tags;
            if (vote.Type.ToLower().Equals("gravity"))
            {
                Gravity = vote.Value;
                Reliability = 0;
            }
            else if (vote.Type.ToLower().Equals("reliability"))
            {
                Reliability = vote.Value;
                Gravity = 0;
            }
            Date = date;
        }
    }
}
