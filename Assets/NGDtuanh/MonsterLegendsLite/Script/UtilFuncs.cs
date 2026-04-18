using UnityEngine;

namespace NGDtuanh.MonsterLegends {
    public class UtilFuncs : NGDtuanh.Utils.UtilFuncs {
        protected override WaitForSeconds GetWaitForSeconds(float second) => WaitForSecondCache.Get(second);
    }
}