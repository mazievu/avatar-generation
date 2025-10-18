// FILE: Assets/Scripts/Core/Services/Impl/LocalizationManager.cs

using System.Collections.Generic;

namespace LifeSim.Core.Services
{
    public class LocalizationManager : ILocalization
    {
        private Dictionary<string, string> _table = new Dictionary<string, string>();
        
        // --- BẮT ĐẦU SỬA LỖI ---

        // 1. Triển khai thuộc tính CurrentLanguage từ ILocalization
        public string CurrentLanguage { get; private set; } = "en";

        public void SetLanguage(string lang)
        {
            CurrentLanguage = lang;
            // TODO: Logic tải dữ liệu localization của bạn ở đây (nếu có)
        }

        // 2. Triển khai phương thức T() gốc từ ILocalization
        public string T(string key) => _table.TryGetValue(key, out var v) ? v : key;

        // 3. Triển khai phương thức T(string, params object[]) từ ILocalization
        //    Đây chính là phần sửa lỗi CS0535
        public string T(string key, params object[] args)
        {
            string translation = T(key); // Lấy bản dịch gốc

            // Trường hợp 1: Dùng làm giá trị mặc định
            // Nếu chỉ có 1 tham số và nó là string, thì đó là defaultValue
            if (args.Length == 1 && args[0] is string defaultValue)
            {
                if (translation == key) // Nếu không tìm thấy key
                {
                    return defaultValue; // Trả về giá trị mặc định
                }
            }
            
            // Trường hợp 2: Dùng để thay thế placeholder {0}, {1}, ...
            if (args.Length > 0)
            {
                // Kịch bản đặc biệt: Nếu không tìm thấy key, và tham số đầu tiên là một chuỗi định dạng
                // Ví dụ: T("key_ko_tồn_tại", "Đây là số {0}", 123)
                if (translation == key && args.Length > 1 && args[0] is string formatString)
                {
                     // Bỏ qua tham số đầu tiên (chuỗi định dạng), lấy các tham số còn lại để format
                     var formatArgs = new object[args.Length - 1];
                     System.Array.Copy(args, 1, formatArgs, 0, args.Length - 1);
                     // Trả về chuỗi đã được format
                     try
                     {
                        return string.Format(formatString, formatArgs);
                     }
                     catch (System.FormatException)
                     {
                        return formatString; // Trả về chuỗi gốc nếu format lỗi
                     }
                }
                
                // Kịch bản thông thường: Nếu tìm thấy key, dùng chuỗi dịch để format
                // Ví dụ: key "welcome" có giá trị "Welcome, {0}!" và gọi T("welcome", "John")
                if (translation != key)
                {
                    try
                    {
                        return string.Format(translation, args);
                    }
                    catch (System.FormatException)
                    {
                        return translation; // Trả về chuỗi gốc nếu format lỗi
                    }
                }
            }

            return translation; // Trả về chuỗi gốc nếu không có tham số hoặc không khớp kịch bản nào
        }
        
        // --- KẾT THÚC SỬA LỖI ---
    }
}