using FileHelpers;

namespace wfaRcptPrg {
   [DelimitedRecord("|")]
   public class RcptnRecords {
      internal string aName = "";
      internal string bIngr = "";
      internal string cPrep = "";
   }
}