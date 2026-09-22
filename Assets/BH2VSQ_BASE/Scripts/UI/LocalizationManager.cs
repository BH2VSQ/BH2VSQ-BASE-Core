using UdonSharp;
using TMPro;

namespace BH2VSQ.Base
{
    public class LocalizationManager : UdonSharpBehaviour
    {
        public string[] chinese;

        public int Language() { return 1; }

        public string Get(int key)
        {
            return chinese != null && key >= 0 && key < chinese.Length ? chinese[key] : "?";
        }

        public string RankName(BaseRank rank)
        {
            return Get(rank == BaseRank.Admin ? BaseText.Administrator : rank == BaseRank.Member ? BaseText.Member : BaseText.Visitor);
        }

        public string StateName(FloorState state)
        {
            return Get(state == FloorState.Maintenance ? BaseText.Maintenance : state == FloorState.Reserved ? BaseText.Reserved : BaseText.Open);
        }
    }

}
