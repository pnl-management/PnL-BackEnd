using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class ParticipantsRoleConst
    {
        public static readonly int INVESTOR = 1;
        public static readonly int STORE_MANAGER = 2;
        public static readonly int ACCOUNTANT = 3;

        public static string GetRole(int? roleId)
        {
            var roleMap = new Dictionary<int?, string>() {
                { INVESTOR, "investor"},
                { STORE_MANAGER, "store-manager"},
                { ACCOUNTANT, "accountant"}
            };

            string result;

            if (!roleMap.TryGetValue(roleId, out result))
            {
                result = null;
            }

            return result;
        }
    }
}
