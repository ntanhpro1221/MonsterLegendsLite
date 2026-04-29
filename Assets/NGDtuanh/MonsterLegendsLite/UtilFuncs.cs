using System.Globalization;
using UnityEngine;

namespace NGDtuanh.MonsterLegendsLite {
    public class UtilFuncs : NGDtuanh.Utils.UtilFuncs {
        protected override WaitForSeconds GetWaitForSeconds(float second) => WaitForSecondCache.Get(second);

        public string ToStrResource(long num) {
            return num.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
}