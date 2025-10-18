***

# **Tài liệu Thiết kế Game (GDD): Generations - A Life Sim (Phần 2)**

## **III. Giao diện Người dùng (UI) và Tương tác (UX)**

Phần này mô tả chi tiết cách người chơi tương tác với game thông qua các màn hình, nút bấm, và các yếu tố đồ họa. Toàn bộ UI tuân theo một phong cách thiết kế "comic panel" (khung truyện tranh), sử dụng các modal có đường viền đậm và góc bo tròn, tạo cảm giác thân thiện và độc đáo.

### **1. Cấu trúc Màn hình Chính (`GameUI.tsx`)**

Đây là màn hình bao bọc toàn bộ trải nghiệm chơi game, được chia thành ba khu vực chính:

*   **Thanh Thông tin (Header):**
    *   **Hiển thị Ngày/Tháng/Năm:** Luôn cập nhật thời gian hiện tại trong game.
    *   **Hiển thị Quỹ Gia Đình (`familyFund`):** Hiển thị số tiền hiện có. Con số sẽ đổi màu (xanh lá cho dương, đỏ cho âm).
    *   **Thu nhập/Chi tiêu hàng tháng (`monthlyNetChange`):** Hiển thị sự thay đổi tài chính của tháng gần nhất ngay bên cạnh quỹ gia đình.
*   **Khu vực Nội dung Chính (Main Content):**
    *   Đây là khu vực trung tâm, nơi hiển thị một trong các "Cảnh" (Scene) chính của game. Cảnh được thay đổi thông qua Thanh Điều hướng Dưới.
*   **Thanh Điều hướng Dưới (Bottom Navigation):**
    *   Luôn cố định ở cuối màn hình, chứa 5 nút chính để chuyển đổi giữa các cảnh:
        1.  **Nhật ký (`log`):** Mở màn hình `GameLog`.
        2.  **Tài sản (`assets`):** Mở màn hình `FamilyAssetsPanel`.
        3.  **Cây Gia Phả (`tree`):** Màn hình mặc định, mở `FamilyTree`.
        4.  **Kinh doanh (`business`):** Mở màn hình `BusinessMap`. Nút này sẽ bị vô hiệu hóa (mờ đi) cho đến khi người chơi mở khóa tính năng kinh doanh (khi gia tộc có đủ 10 người con).
        5.  **Con đường Sự sống (`path`):** Mở màn hình `PathOfLifeScreen`.

### **2. Các Cảnh chính (Scenes)**

*   **a. Cây Gia Phả (`FamilyTree.tsx`):**
    *   **Mục đích:** Là màn hình chính và mặc định, hiển thị toàn bộ các thành viên trong gia đình dưới dạng một cây gia phả có thể tương tác.
    *   **Cơ chế hoạt động:**
        1.  **Bố cục (Layout):** Các nhân vật (`CharacterNode`) được tự động sắp xếp theo thế hệ. Các thế hệ được xếp theo chiều dọc. Các nhân vật trong cùng một thế hệ được xếp theo chiều ngang. Thuật toán sẽ cố gắng đặt cha mẹ ở vị trí trung tâm phía trên các con của họ.
        2.  **Tương tác:**
            *   **Kéo (Pan):** Người chơi có thể kéo để di chuyển xung quanh cây gia phả.
            *   **Thu/Phóng (Pinch/Zoom):** Người chơi có thể dùng hai ngón tay để phóng to, thu nhỏ.
        3.  **Đường nối:** Các đường cong (vẽ bằng SVG) sẽ tự động nối cha mẹ với con cái, tạo thành cấu trúc cây.
        4.  **Tối ưu hóa:** Chỉ những `CharacterNode` trong tầm nhìn của người chơi (cộng với một vùng đệm) mới được hiển thị để đảm bảo hiệu năng khi cây gia phả lớn.
        5.  **Tự động canh giữa:** Khi một sự kiện xảy ra, hoặc khi người chơi chọn một nhân vật, cây gia phả sẽ tự động di chuyển và phóng to vào nhân vật đó.
*   **b. Nhật ký (`GameLog.tsx`):**
    *   **Mục đích:** Hiển thị lịch sử các sự kiện và quyết định đã xảy ra trong game, được sắp xếp theo thứ tự thời gian từ mới nhất đến cũ nhất.
    *   **Hiển thị:** Mỗi mục trong nhật ký bao gồm: Năm, Tên nhân vật, Tên sự kiện, và kết quả/lựa chọn của người chơi. Các thay đổi về chỉ số hoặc tiền bạc cũng được hiển thị bằng icon và con số.
*   **c. Tài sản (`FamilyAssetsPanel.tsx`):**
    *   **Mục đích:** Cho phép người chơi xem và mua các loại tài sản.
    *   **Bố cục:** Các tài sản được nhóm theo loại (Nhà cửa, Xe cộ, v.v.). Mỗi tài sản được hiển thị dưới dạng một ô (`AssetSlot`).
    *   **Tương tác:**
        *   Ô tài sản sẽ có màu xanh lá và nhãn "Đã sở hữu" nếu đã được mua.
        *   Khi bấm vào một tài sản, một modal chi tiết (`AssetDetailModal`) sẽ hiện lên, hiển thị hình ảnh, mô tả, giá cả và hiệu ứng cộng chỉ số.
        *   Trong modal chi tiết, người chơi có thể mua tài sản nếu đủ tiền.
*   **d. Bản đồ Kinh doanh (`BusinessMap.tsx`):**
    *   **Mục đích:** Giao diện trực quan để người chơi khám phá, mua và quản lý các doanh nghiệp.
    *   **Cơ chế hoạt động:**
        1.  **Bản đồ tương tác:** Hiển thị một bản đồ thành phố lớn, có thể kéo và thu/phóng.
        2.  **Điểm nóng (Hotspot):** Trên bản đồ có các vị trí tòa nhà tương ứng với các loại hình kinh doanh.
        3.  **Tương tác:**
            *   **Nếu chưa sở hữu:** Bấm vào một tòa nhà sẽ mở `BusinessPurchaseModal`, cho phép người chơi mua doanh nghiệp đó nếu đủ tiền.
            *   **Nếu đã sở hữu:** Tòa nhà sẽ có viền màu xanh lá. Bấm vào sẽ mở `BusinessManagementModal` để quản lý. Nếu sở hữu nhiều doanh nghiệp cùng loại, một modal lựa chọn sẽ hiện ra trước.
        4.  **Hiển thị thông tin nhanh:** Các doanh nghiệp đã sở hữu sẽ hiển thị lợi nhuận/thua lỗ hàng tháng và avatar của các nhân viên đang làm việc ngay trên bản đồ.
*   **e. Con đường Sự sống (`PathOfLifeScreen.tsx`):**
    *   **Mục đích:** Hiển thị tiến trình meta của người chơi, nơi họ mở khóa các tính năng vĩnh viễn và nhận phần thưởng dựa trên tổng số con đã sinh ra.
    *   **Bố cục:** Một con đường thẳng đứng từ dưới lên. Các mốc phần thưởng được đặt dọc theo con đường.
    *   **Cơ chế hoạt động:**
        1.  Con đường sẽ "được tô màu" dần lên dựa trên tổng số con (`totalChildrenBorn`).
        2.  Khi người chơi vượt qua một mốc, họ có thể bấm vào nút "Nhận" (`Claim`) để kích hoạt phần thưởng hoặc mở khóa tính năng.
        3.  Các phần thưởng đã nhận sẽ được đánh dấu. Các phần thưởng chưa đạt tới sẽ bị khóa.

### **3. Các Thành phần Tương tác Chính**

*   **a. Nút Nhân vật (`CharacterNode.tsx`):**
    *   **Hiển thị:** Đây là avatar tròn của mỗi nhân vật trên cây gia phả.
    *   **Thông tin hiển thị:**
        *   **Avatar:** Sử dụng `AgeAwareAvatarPreview`.
        *   **Tên & Thế hệ:** `displayName` (bao gồm cả tính từ) và (G{số thế hệ}).
        *   **Tuổi:** Hiển thị trong một huy hiệu nhỏ trên góc avatar.
        *   **Thu nhập/Chi tiêu hàng tháng:** Hiển thị bên dưới tên, có màu xanh/đỏ.
    *   **Trạng thái đặc biệt:**
        *   **Nhân vật người chơi (`isPlayerCharacter`):** Có viền màu vàng.
        *   **Đã mất (`isAlive = false`):** Avatar bị làm mờ đi.
    *   **Tương tác:** Bấm vào một `CharacterNode` sẽ mở `CharacterDetailModal`.
*   **b. Hoạt ảnh Thu nhập (`IncomeAnimation.tsx`):**
    *   Một hiệu ứng nhỏ xuất hiện phía trên `CharacterNode` vào mỗi đầu tháng.
    *   Hiển thị số tiền `monthlyNetIncome` (ví dụ: "+$500" hoặc "-$100") và bay mờ dần lên trên.
*   **c. Trình tạo Avatar (`AvatarBuilder.tsx`):**
    *   **Mục đích:** Một màn hình toàn cục cho phép tùy chỉnh chi tiết ngoại hình nhân vật.
    *   **Bố cục:**
        *   **Bên trái:** Khu vực xem trước avatar lớn (`AgeAwareAvatarPreview`), nút "Ngẫu nhiên" và ô nhập "seed" để tạo lại một kết quả ngẫu nhiên cụ thể.
        *   **Bên phải:** Danh sách các lớp (Layer) có thể tùy chỉnh (Tóc, Mắt, Miệng, v.v.).
    *   **Luồng tương tác:**
        1.  Người chơi chọn một lớp (ví dụ: "Tóc").
        2.  Danh sách các tùy chọn cho lớp đó (các kiểu tóc) sẽ hiện ra.
        3.  Người chơi chọn một kiểu tóc. Avatar xem trước cập nhật ngay lập tức.
        4.  Nếu lớp đó có thể tô màu (Tóc, Mắt, Râu), một bảng màu (`AVATAR_COLOR_PALETTE`) sẽ hiện ra bên dưới để người chơi chọn.
    *   **Logic:**
        *   Các tùy chọn sẽ được lọc dựa trên tuổi của nhân vật (ví dụ: `features` cho em bé khác người lớn).
        *   Các lớp bị giới hạn bởi giới tính (Râu cho nam, Tóc sau cho nữ) sẽ tự động bị ẩn/vô hiệu hóa.
    *   **Hành động:**
        *   **Lưu (Pay):** Tốn **10,000** để lưu thay đổi. Nút này bị vô hiệu hóa nếu không đủ tiền.
        *   **Xem Quảng cáo (Watch Ad):** Cho phép lưu miễn phí.
        *   **Hủy (Cancel):** Đóng màn hình và không lưu thay đổi.

### **4. Hệ thống Modal (Pop-ups)**

Tất cả các modal đều được xây dựng trên một component nền tảng là `ComicPanelModal.tsx`, mang lại vẻ ngoài đồng nhất.

*   **a. Modal Sự kiện & Quyết định Cuộc đời:**
    *   **`EventModal.tsx`:**
        *   **Mục đích:** Hiển thị các sự kiện ngẫu nhiên và cột mốc. Đây là modal quan trọng nhất.
        *   **Bố cục:** Avatar và tên nhân vật, tiêu đề sự kiện, mô tả chi tiết, và danh sách các lựa chọn (`ChoiceButton`).
        *   **Luồng tương tác:**
            1.  Modal hiện ra, người chơi đọc và chọn một phương án.
            2.  Sau khi chọn, các nút lựa chọn biến mất. Thay vào đó, modal hiển thị kết quả: một câu log, và các thanh chỉ số (`StatBar`) được cập nhật với hoạt ảnh tăng/giảm. Thay đổi về tiền bạc cũng được hiển thị.
            3.  Một nút "OK" xuất hiện. Bấm "OK" để đóng modal và tiếp tục game.
    *   **Các Modal Lựa chọn Tuần tự (Học vấn & Sự nghiệp):**
        *   `SchoolChoiceModal`, `ClubChoiceModal`, `UniversityChoiceModal`, `UniversityMajorChoiceModal`, `CareerChoiceModal`, `UnderqualifiedChoiceModal`: Các modal này có chung một cấu trúc: hiển thị avatar nhân vật, một câu hỏi/mô tả, và danh sách các lựa chọn. Chúng dẫn dắt người chơi qua các quyết định quan trọng của cuộc đời.
*   **b. Modal Thông tin & Quản lý:**
    *   **`CharacterDetailModal.tsx`:**
        *   **Mục đích:** Hiển thị thông tin chi tiết của một nhân vật khi người chơi bấm vào `CharacterNode`.
        *   **Bố cục:** Giao diện theo tab.
            *   **Tab Chi tiết:** Hiển thị avatar, thông tin cơ bản, các chỉ số dưới dạng thanh tiến trình, học vấn, sự nghiệp. Có nút "Tùy chỉnh" để mở `AvatarBuilder` (nếu đã mở khóa).
            *   **Tab Sự kiện:** Liệt kê tất cả các sự kiện trong đời của nhân vật đó.
    *   **`BusinessManagementModal.tsx`:**
        *   **Mục đích:** Quản lý một doanh nghiệp đã sở hữu.
        *   **Luồng tương tác:**
            1.  Hiển thị danh sách các vị trí nhân viên (`slots`).
            2.  Bấm vào một vị trí sẽ mở một modal con (`AssignmentModal`).
            3.  Trong `AssignmentModal`, người chơi có thể chọn một thành viên gia đình phù hợp, thuê robot, hoặc bỏ trống vị trí đó.
            4.  Modal chính cũng có nút "Nâng cấp" và "Bán".
*   **c. Modal Giao diện Hệ thống:**
    *   `StartMenu`, `WelcomeBackMenu`, `SummaryScreen`: Điều khiển luồng bắt đầu, tiếp tục và kết thúc game.
    *   `SettingsModal`: Cho phép chỉnh tốc độ game, tạm dừng, và thoát game.
    *   `UnlockNotificationModal`: Một thông báo nhỏ hiện lên khi người chơi mở khóa một tính năng mới từ `PathOfLifeScreen`.

***

## **IV. Tổng hợp Tài sản Đồ họa & Âm thanh**

Để xây dựng lại game trên Unity, đội ngũ sẽ cần các tài sản sau, dựa trên phân tích mã nguồn:

1.  **Tài sản Giao diện (UI Assets):**
    *   Các icon cho thanh điều hướng dưới, các chỉ số, các nút (chỉnh sửa, khóa, v.v.).
    *   Hình nền cho các màn hình chính (cây gia phả, bản đồ kinh doanh, con đường sự sống).
    *   Hình ảnh cho các banner, nút bấm có style riêng (ví dụ: nút "Claim").
2.  **Tài sản Avatar:**
    *   Toàn bộ các file ảnh `.webp` cho các bộ phận của avatar (tóc, mắt, miệng, râu, phụ kiện), được tách ra thành các lớp (layer) riêng biệt.
3.  **Tài sản Game:**
    *   Hình ảnh cho các tài sản (nhà, xe).
    *   Hình ảnh bản đồ kinh doanh (`business-map.webp`).
    *   Hình ảnh đại diện tĩnh cho các nhân vật trong kịch bản "Mila" (Mila, Max, Alice, v.v.).
4.  **Âm thanh:**
    *   Âm thanh khi bấm nút (`click sound`). (Mặc dù không có trong code, component `ChoiceButton` có gọi `soundManager.play('click')`, cho thấy yêu cầu này).
    *   Nhạc nền cho các màn hình khác nhau (menu, đang chơi).
    *   Các hiệu ứng âm thanh khác cho sự kiện (thành công, thất bại, nhận tiền).

***