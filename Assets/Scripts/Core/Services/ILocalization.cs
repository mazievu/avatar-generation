// FILE: Assets/Scripts/Core/Services/ILocalization.cs
namespace LifeSim.Core.Services
{
    public interface ILocalization
    {
        string CurrentLanguage { get; }
        void SetLanguage(string lang);
        
        // Phương thức T() gốc
        string T(string key);

        // --- THAY THẾ TOÀN BỘ CÁC OVERLOAD CŨ BẰNG MỘT HÀM DUY NHẤT ---
        
        /// <summary>
        /// Dịch một chuỗi và thay thế các placeholder theo định dạng {0}, {1}, ...
        /// Ví dụ: T("key_với_số", 10, "Player") sẽ thay {0} bằng 10, {1} bằng "Player".
        /// Nếu tham số thứ hai là một string và không có tham số nào sau đó, nó sẽ được coi là giá trị mặc định.
        /// </summary>
        string T(string key, params object[] args);
    }
}