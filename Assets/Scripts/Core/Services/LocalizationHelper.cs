// FILE: Assets/Scripts/Core/Services/LocalizationHelper.cs
using System.Collections.Generic;

namespace LifeSim.Core.Services
{
    // ĐỔI TÊN LỚP THÀNH LocalizationHelper VÀ THÊM TỪ KHÓA STATIC
    public static class LocalizationHelper 
    {
        public static Dictionary<string, object> CreateReplacements(params object[] pairs)
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < pairs.Length; i += 2)
            {
                if (i + 1 < pairs.Length && pairs[i] is string key)
                {
                    dict[key] = pairs[i + 1];
                }
            }
            return dict;
        }
    }
}