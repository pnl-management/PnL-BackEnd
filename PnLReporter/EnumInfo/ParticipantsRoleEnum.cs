using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class ParticipantsRoleEnum
    {
        public static string GetRole(int? roleId)
        {
            var roleMap = new Dictionary<int?, string>() {
                { 1, "investor"},
                { 2, "store-manager"},
                { 3, "accountant"}
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
