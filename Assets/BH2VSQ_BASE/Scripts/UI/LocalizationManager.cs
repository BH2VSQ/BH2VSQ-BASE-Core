using UdonSharp;
using TMPro;

namespace BH2VSQ.Base
{
    public class LocalizationManager : UdonSharpBehaviour
    {
        public PlayerDataManager data;
        public string[] english;
        public string[] chinese;

        public int Language() { return data == null ? 0 : data.language; }

        public string Get(int key)
        {
            string[] table = Language() == 1 ? chinese : english;
            return table != null && key >= 0 && key < table.Length ? table[key] : "?";
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
