# Kế hoạch Triển khai Game "Generations - A Life Sim" (Phiên bản sửa đổi)

## I. Phân tích Hiện trạng (Đã cập nhật)

- **Kiến trúc nền tảng:** Rất tốt. Dự án đã có một kiến trúc vững chắc với `GameEngine` quản lý `GameState`, và `GameUIController` làm cầu nối với giao diện người dùng. Luồng khởi động (`Bootstrap`) và vòng lặp game (`Tick`) đã hoạt động.
- **Mã nguồn hiện tại:** Các thành phần chính đã tồn tại nhưng logic nghiệp vụ chi tiết bên trong còn thiếu. Hầu hết các hàm quan trọng trong `GameEngine` đang ở dạng placeholder (`//...`) hoặc là các phương thức trống.
- **Giao diện người dùng (UI):** Khung sườn của UI đã được dựng trong `GameUIController` với các panel chính. Tuy nhiên, chức năng chi tiết của từng panel và các modal phức tạp chưa được triển khai.
- **Kết luận:** Công việc không phải là xây dựng từ đầu, mà là **lấp đầy các chi tiết gameplay và chức năng UI** vào bộ khung đã có, dựa theo GDD.

---

## II. Lộ trình Triển khai Chi tiết

Lộ trình này tập trung vào việc hoàn thiện các tính năng theo từng giai đoạn một cách hợp lý.

### **Giai đoạn 1: Hoàn thiện Hệ thống Dữ liệu & Logic Cốt lõi**

*Mục tiêu: Nạp dữ liệu tĩnh vào game và triển khai các phép tính toán cơ bản trong `GameEngine`.*

1.  **Định nghĩa và Nạp dữ liệu tĩnh (`Core/Data`):**
    *   Tạo các lớp `ScriptableObject` để chứa dữ liệu từ các file JSON/script: `CareerSO`, `EventSO`, `AssetSO`, `EducationSO`, `BusinessSO`.
    *   Viết lớp `Database.cs` để đọc dữ liệu từ Resources và nạp vào các đối tượng ScriptableObject này khi game khởi động. Điều này giúp tách biệt data khỏi logic.
    *   Hoàn thiện lớp `Character.cs` (`Core/Domain/Characters`) với đầy đủ các chỉ số (`iq`, `eq`, `health`, `happiness`, `skill`) và các thuộc tính khác như trong GDD.

2.  **Triển khai Logic trong `GameEngine.cs` (`Core/Engine`):**
    *   **`UpdateMonthly()`:** Lấp đầy logic tính toán thu nhập và chi phí. Duyệt qua tất cả các nhân vật, tính lương sự nghiệp, lợi nhuận kinh doanh, chi phí sinh hoạt, và cập nhật `familyFund`.
    *   **`UpdateLifeStage()`:** Hoàn thiện logic kiểm tra và thay đổi `LifePhase` của nhân vật dựa trên tuổi.
    *   **`UpdateYearly()`:** Triển khai logic kiểm tra các mốc quyết định quan trọng hàng năm (chọn trường, nghỉ hưu) và đưa các `Pending...Choice` tương ứng vào `GameState`.
    *   Triển khai logic tăng/giảm chỉ số cơ bản (ví dụ: `skill` tăng khi đi làm, `health` giảm khi về già).

### **Giai đoạn 2: Triển khai Hệ thống Sự kiện & Modal**

*Mục tiêu: Làm cho các sự kiện trong game có thể xảy ra và người chơi có thể tương tác với chúng.*

1.  **Hoàn thiện `EventManager` (Logic trong `GameEngine.cs`):**
    *   Triển khai logic trong `MaybeTriggerAmbientEvent()` để lọc và kích hoạt một sự kiện ngẫu nhiên từ `Database` dựa trên các điều kiện (tuổi, trạng thái, cooldown).
    *   Triển khai đầy đủ logic trong `HandleEventChoice(string choiceId)`: áp dụng các hiệu ứng (thay đổi chỉ số, tiền bạc), ghi log, và kích hoạt các sự kiện nối tiếp.

2.  **Xây dựng `ModalManager.cs` và `EventModal`:**
    *   Hoàn thiện `ModalManager.cs` để có thể nhận yêu cầu hiển thị modal từ `GameEngine` (thông qua `OnStateChanged` trong `GameUIController`).
    *   Tạo Prefab và script cho `EventModal`. Script này sẽ nhận một `EventSO`, hiển thị tiêu đề, mô tả, và tạo ra các `ChoiceButton` tương ứng.
    *   Khi một `ChoiceButton` được bấm, nó sẽ gọi lại `engine.HandleEventChoice(choiceId)`.
    *   Triển khai luồng "kết quả" của modal: sau khi chọn, các nút biến mất, modal hiển thị kết quả và nút "OK" để đóng.

### **Giai đoạn 3: Hoàn thiện các Màn hình UI Chính**

*Mục tiêu: Xây dựng chức năng cho 5 panel chính được điều khiển bởi `GameUIController`.*

1.  **Cây Gia Phả (`panelTree`):**
    *   Tạo script `FamilyTreeController.cs`.
    *   Script này sẽ đọc `familyMembers` từ `GameState` và render các `CharacterNode` (tạo prefab cho node này).
    *   Triển khai chức năng kéo (pan) và thu/phóng (zoom) cho panel.
    *   Vẽ các đường nối giữa các thế hệ.

2.  **Nhật ký (`panelLog`):**
    *   Tạo script `GameLogController.cs`.
    *   Hiển thị một danh sách các mục log được lưu trong `GameState` (cần bổ sung `List<string> gameLog` vào `GameState`).

3.  **Tài sản (`panelAssets`):**
    *   Tạo script `AssetsController.cs`.
    *   Hiển thị danh sách các tài sản từ `Database` và đánh dấu các tài sản đã sở hữu (`GameState.assets`).
    *   Tạo `AssetDetailModal` để hiển thị chi tiết và cho phép mua.

4.  **Con đường Sự sống (`panelPath`):**
    *   Tạo script `PathOfLifeController.cs`.
    *   Hiển thị con đường và các mốc unlock dựa trên `totalChildrenBorn` trong `GameState`.
    *   Xử lý việc nhận (`Claim`) phần thưởng.

### **Giai đoạn 4: Triển khai Hệ thống Kinh doanh (Business)**

*Mục tiêu: Hoàn thiện tính năng kinh doanh, một hệ thống phức tạp cả về logic và UI.*

1.  **Mở rộng Logic Core:**
    *   Cập nhật `BusinessInstance` trong `GameState` để chứa thêm thông tin (cấp bậc, danh sách nhân viên).
    *   Trong `EconomyService` hoặc một `BusinessService` mới, triển khai logic tính toán doanh thu/lợi nhuận chi tiết dựa trên nhân viên, kỹ năng, và các yếu tố khác như trong GDD.

2.  **Xây dựng Giao diện Người dùng:**
    *   Tạo script `BusinessMapController.cs` cho `panelBusiness`.
    *   Thiết kế bản đồ với các "hotspot" cho từng địa điểm kinh doanh.
    *   Tạo các modal `BusinessPurchaseModal` và `BusinessManagementModal` để mua và quản lý doanh nghiệp (gán nhân viên, nâng cấp).

### **Giai đoạn 5: Hoàn thiện và Đánh bóng**

*Mục tiêu: Hoàn thành các phần còn lại và cải thiện trải nghiệm người dùng.*

1.  **Lưu/Tải Game:**
    *   Hoàn thiện `JsonFileStore.cs` (hiện tại chưa có file này, cần tạo trong `Core/Infra`) để thực sự ghi và đọc `GameState` từ một file JSON trong `Application.persistentDataPath`.
    *   Tích hợp vào UI (ví dụ: nút "Continue" ở `StartMenu`).

2.  **Hệ thống Avatar:**
    *   Xây dựng UI cho `AvatarBuilder` theo GDD.
    *   Viết logic để cho phép người chơi tùy chỉnh các lớp của avatar và lưu lại `avatarState` của nhân vật.

3.  **Âm thanh và Hiệu ứng:**
    *   Tạo một `SoundManager` để quản lý việc phát nhạc nền và hiệu ứng âm thanh.
    *   Thêm các hiệu ứng hình ảnh nhỏ (ví dụ: `IncomeAnimation` khi nhận lương).

4.  **Kiểm thử và Cân bằng:**
    *   Chơi thử và điều chỉnh lại các thông số kinh tế, độ khó của sự kiện, và tốc độ tiến triển trong game.

---

## IV. Bước Tiếp theo

**Đề xuất bắt đầu với "Giai đoạn 1", cụ thể là mục 1: "Định nghĩa và Nạp dữ liệu tĩnh".** Điều này bao gồm việc tạo các lớp ScriptableObject và hoàn thiện `Database.cs` để có một nền tảng dữ liệu vững chắc trước khi xây dựng logic phức tạp hơn.

**Câu hỏi:** Bạn có muốn tôi bắt đầu bằng việc tạo các file script cho các `ScriptableObject` (`CareerSO`, `EventSO`, `AssetSO`, v.v.) trong thư mục `Assets/Scripts/Core/Data/SO` không?