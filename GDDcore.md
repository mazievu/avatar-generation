# **Tài liệu Thiết kế Game (GDD): Generations - A Life Sim**

## **I. Tổng quan về Game**

*   **Tên game:** Generations - A Life Sim
*   **Thể loại:** Mô phỏng cuộc sống (Life Simulation), nhiều thế hệ (Multi-generational).
*   **Mục tiêu chính:** Người chơi điều khiển một nhân vật, sống cuộc đời của họ từ khi sinh ra đến khi qua đời, đưa ra các quyết định về học tập, sự nghiệp, tình cảm, gia đình, tài chính và cố gắng duy trì và phát triển gia tộc qua nhiều thế hệ. Game kết thúc với chiến thắng khi gia tộc đạt đến thế hệ thứ 6.
*   **Nền tảng mục tiêu (chuyển đổi):** C# với Unity.
*   **Core Loop (Vòng lặp cốt lõi):**
    1.  **Sống:** Thời gian trôi qua mỗi ngày. Nhân vật già đi, các chỉ số thay đổi.
    2.  **Quyết định:** Các sự kiện (event) ngẫu nhiên hoặc theo mốc thời gian (milestone) sẽ xuất hiện, yêu cầu người chơi đưa ra lựa chọn.
    3.  **Hệ quả:** Lựa chọn của người chơi ảnh hưởng trực tiếp đến chỉ số, tài chính, sự nghiệp, và các mối quan hệ của nhân vật.
    4.  **Tiến triển:** Nhân vật trải qua các giai đoạn cuộc đời (sơ sinh, đi học, đi làm, nghỉ hưu), kết hôn, sinh con.
    5.  **Kế thừa:** Khi nhân vật chính qua đời, người chơi có thể chọn một trong những người con để tiếp tục chơi, bắt đầu một thế hệ mới.

***

## **II. Các Hệ Thống Cốt Lõi (Core Game Systems)**

### **1. Vòng Lặp Game & Thời Gian (Game Loop & Time)**

Hệ thống này là trái tim của game, điều khiển mọi sự thay đổi theo thời gian.

*   **Đơn vị thời gian:**
    *   Thời gian trong game được tính bằng `ngày` (day) và `năm` (year).
    *   Mỗi năm có **365 ngày**.
    *   Tốc độ game mặc định là **50 mili giây** cho mỗi ngày trong game trôi qua.
*   **Cơ chế hoạt động:**
    1.  **Cập nhật hàng ngày (Daily Update):**
        *   Ngày hiện tại (`currentDate`) tăng lên 1. Nếu vượt quá 365, ngày reset về 1 và năm tăng lên 1.
        *   Kiểm tra sinh nhật của nhân vật: Nếu ngày hiện tại trùng với ngày sinh, nhân vật tăng 1 tuổi (`age++`).
        *   Khi nhân vật có tuổi mới, kiểm tra xem có chuyển sang giai đoạn sống mới không (`LifePhase`). Nếu có, avatar sẽ được tạo lại ngẫu nhiên để trông già dặn hơn (trừ các avatar tĩnh).
        *   Sức khỏe (`health`) tự động giảm nhẹ mỗi ngày đối với nhân vật trên 50 tuổi và giảm nhiều hơn với nhân vật trên 70 tuổi.
        *   Nếu `health` giảm xuống 0 hoặc thấp hơn, một sự kiện đặc biệt về việc qua đời vì tuổi già (`milestone_death_old_age`) sẽ được đưa vào hàng đợi sự kiện.
    2.  **Cập nhật hàng tháng (Monthly Update):** Diễn ra vào ngày đầu tiên của mỗi 30 ngày.
        *   **Tính toán thu nhập & chi phí:**
            *   **Thu nhập:** Lương từ sự nghiệp, lương hưu, lợi nhuận từ kinh doanh, tiền thực tập, v.v.
            *   **Chi phí:** Chi phí sinh hoạt cơ bản (biến động ngẫu nhiên một chút mỗi năm), chi phí nuôi thú cưng.
        *   **Cập nhật Quỹ Gia Đình (`familyFund`):** `familyFund` = `familyFund` + (Tổng thu nhập - Tổng chi phí).
        *   **Phát triển Kỹ năng & Sự nghiệp:**
            *   Nhân vật đang đi làm (`Working`) sẽ tăng chỉ số Kỹ năng (`skill`) mỗi tháng. Tốc độ tăng phụ thuộc vào IQ và EQ. Nếu làm trái ngành, tốc độ tăng chỉ bằng 50%.
            *   Kiểm tra điều kiện thăng chức.
            *   Nhân vật thất nghiệp (`Unemployed`) quá 12 tháng có 20% cơ hội mỗi tháng để kích hoạt màn hình tìm việc.
    3.  **Cập nhật hàng năm (Yearly Update):** Diễn ra vào ngày đầu tiên của năm mới.
        *   Reset bộ đếm sự kiện của mỗi nhân vật trong năm (`eventsThisYear = 0`).
        *   Kiểm tra và kích hoạt các quyết định quan trọng theo độ tuổi:
            *   **6 tuổi:** Chọn trường cấp 1 (`pendingSchoolChoice`).
            *   **12 tuổi:** Chọn trường cấp 2.
            *   **16 tuổi:** Chọn trường cấp 3.
            *   **19 tuổi:** Quyết định học Đại học hay đi làm (`pendingUniversityChoice`).
            *   **60 tuổi:** Nhân vật về hưu (`Retired`).
        *   Kiểm tra các khoản vay đến hạn. Nếu không đủ tiền trả, game sẽ kết thúc (`gameOverReason = 'debt'`).
        *   Kích hoạt các sự kiện cột mốc (milestone events) nếu đủ điều kiện.

### **2. Nhân Vật (Character)**

Nhân vật là thực thể trung tâm mà người chơi tương tác. Cấu trúc dữ liệu của nhân vật được định nghĩa trong `core/types.ts`.

*   **Các thuộc tính cơ bản:**
    *   `id`: Mã định danh duy nhất.
    *   `name`, `gender`: Tên và giới tính, được tạo ngẫu nhiên.
    *   `age`, `birthDate`: Tuổi và ngày sinh.
    *   `generation`: Thế hệ của nhân vật (bắt đầu từ 0).
    *   `isAlive`: Trạng thái sống/chết.
    *   `parentsIds`, `partnerId`, `childrenIds`: Lưu trữ mối quan hệ gia đình.
*   **Các chỉ số (Stats):**
    *   `iq` (Trí tuệ): 0-200. Ảnh hưởng đến học tập và tốc độ tăng kỹ năng.
    *   `happiness` (Hạnh phúc): 0-100.
    *   `eq` (Trí tuệ cảm xúc): 0-100. Ảnh hưởng đến tương tác xã hội và sự nghiệp.
    *   `health` (Sức khỏe): 0-100.
    *   `skill` (Kỹ năng): 0-100. Yếu tố chính để thăng tiến trong sự nghiệp.
*   **Vòng Đời & Trạng Thái (Life Cycle & Status):**
    *   **Giai đoạn sống (`LifePhase`):**
        *   `Newborn` (Sơ sinh): 0-5 tuổi
        *   `Elementary School` (Cấp 1): 6-11 tuổi
        *   `Middle School` (Cấp 2): 12-15 tuổi
        *   `High School` (Cấp 3): 16-18 tuổi
        *   `University` (Đại học): 19-22 tuổi
        *   `Working Life` (Đi làm): 23-59 tuổi
        *   `Retired` (Nghỉ hưu): 60+ tuổi
    *   **Trạng thái (`CharacterStatus`):**
        *   `Idle`: Không làm gì cả.
        *   `InEducation`: Đang đi học.
        *   `Working`: Đang đi làm (ăn lương hoặc làm cho doanh nghiệp gia đình).
        *   `Unemployed`: Thất nghiệp.
        *   `Internship`, `VocationalTraining`: Thực tập, học nghề.
        *   `Retired`: Nghỉ hưu.
        *   `Trainee`: Vị trí học việc trước khi vào nghề chính thức.
*   **Cơ chế tử vong:**
    1.  **Tuổi già:** Kích hoạt sự kiện `milestone_death_old_age` khi nhân vật có tuổi thọ cao (trên 85).
    2.  **Sức khỏe/Hạnh phúc thấp:** Nếu `health` hoặc `happiness` dưới 10 trong 2 năm liên tiếp, nhân vật sẽ chết.
    3.  **Sự kiện:** Một số sự kiện tiêu cực có thể dẫn đến cái chết.
*   **Logic đặc tả (`displayAdjective`):** Mỗi năm, nhân vật có thể nhận một tính từ mô tả (ví dụ: "Thông minh", "Vui vẻ") dựa trên chỉ số cao nhất của họ.

### **3. Kinh Tế (Economy)**

*   **Tiền tệ:** Quỹ Gia Đình (`familyFund`).
*   **Thu nhập:**
    *   **Lương sự nghiệp:** Nhận hàng tháng, dựa trên `careerTrack` và `careerLevel`. Có mức lương trần tuyệt đối là **9,600/năm (800/tháng)**.
    *   **Lương hưu:** **4,200/năm** cho nhân vật đã nghỉ hưu.
    *   **Lương thực tập/học việc:** Mức cố định.
    *   **Lợi nhuận kinh doanh:** Lợi nhuận ròng từ các doanh nghiệp của gia đình.
    *   **Sự kiện:** Một số sự kiện có thể cho tiền (trúng số, được thưởng).
*   **Chi phí:**
    *   **Chi phí sinh hoạt (`Cost of Living`):** Trả hàng tháng, thay đổi theo từng giai đoạn sống (`LifePhase`) và có một khoảng dao động ngẫu nhiên nhỏ.
    *   **Học phí:** Trả một lần khi nhập học (`SCHOOL_OPTIONS`, `UNIVERSITY_MAJORS`).
    *   **Đầu tư:** Mua tài sản (`Assets`), mua/nâng cấp doanh nghiệp (`Businesses`).
    *   **Sự kiện:** Một số sự kiện yêu cầu chi tiền.
*   **Vay nợ (`Loans`):**
    *   Khi `familyFund` < 0, người chơi sẽ được đề nghị vay tiền.
    *   Khoản vay có số tiền và kỳ hạn trả nợ.
    *   Nếu đến hạn không trả được nợ, game sẽ kết thúc.

### **4. Giáo Dục (Education)**

*   **Cấp 1, 2, 3 (`SCHOOL_OPTIONS`):**
    *   Tại các mốc tuổi 6, 12, 16, người chơi phải chọn 1 trong 3 loại trường: Công lập (miễn phí), Tư thục (phí cao), Hoàng gia (phí rất cao).
    *   Mỗi lựa chọn có chi phí và ảnh hưởng đến chỉ số `iq`, `eq` khác nhau. Trường càng đắt thì chỉ số cộng càng nhiều.
*   **Đại học (`UNIVERSITY_MAJORS`):**
    *   Ở tuổi 19, nhân vật có thể chọn vào Đại học hoặc đi làm ngay.
    *   Nếu vào Đại học, người chơi chọn 1 trong 5 chuyên ngành ngẫu nhiên.
    *   Mỗi chuyên ngành có học phí và ảnh hưởng chỉ số khác nhau, và là điều kiện tiên quyết cho các ngành nghề cao cấp.
    *   Thời gian học là 4 năm.
*   **Học nghề (`VOCATIONAL_TRAINING`):**
    *   Là một lựa chọn thay thế cho Đại học, kéo dài 3 năm.
    *   Tốn chi phí, tăng mạnh chỉ số `skill` và `eq`.

### **5. Sự Nghiệp (Careers)**

Hệ thống sự nghiệp được định nghĩa trong `CAREER_LADDER`.

*   **Lộ trình sự nghiệp (`CareerTrack`):**
    *   Có nhiều lộ trình khác nhau: Lao động phổ thông, Kinh doanh, Công nghệ, Y học, v.v.
    *   Mỗi lộ trình yêu cầu `iq`, `eq` và đôi khi là bằng Đại học (`requiredMajor`).
*   **Thăng tiến (`Promotion`):**
    *   Mỗi lộ trình có 4 cấp bậc (`levels`), mỗi cấp bậc có mức lương và yêu cầu về `skill` khác nhau.
    *   Để thăng tiến, `skill` của nhân vật phải đạt mốc yêu cầu của cấp bậc tiếp theo.
    *   **Logic xử phạt:**
        *   Nếu không có bằng cấp (Đại học/Học nghề), yêu cầu `skill` để thăng tiến sẽ tăng 50%.
        *   Nếu làm trái ngành (bằng Đại học không khớp với `requiredMajor`), yêu cầu `skill` cũng tăng 50% và tốc độ tăng `skill` hàng tháng giảm 50%.
    *   Nếu nhân vật không được thăng chức trong 13 tháng, chỉ số `happiness` sẽ bị giảm.
*   **Tìm việc:**
    *   Sau khi tốt nghiệp hoặc quyết định không học ĐH, nhân vật sẽ vào màn hình tìm việc.
    *   Các lựa chọn nghề nghiệp được tạo ra dựa trên bằng cấp và chỉ số.
    *   **Logic xử lý khi không đủ điều kiện:**
        1.  **Thiếu chỉ số (IQ/EQ):** Nhân vật có thể chọn làm `Trainee` (học việc) để tăng dần chỉ số cho đến khi đủ yêu cầu, hoặc chấp nhận vào làm ngay nhưng bị phạt nặng vào tiến độ thăng tiến (`progressionPenalty`).
        2.  **Trái ngành:** Có thể vào làm nhưng bị phạt tiến độ.

### **6. Sự Kiện (Events)**

Hệ thống sự kiện là cốt lõi của phần tường thuật, được định nghĩa trong các file `core/events/*.ts`.

*   **Cấu trúc một sự kiện:**
    *   Bao gồm tiêu đề, mô tả, và các lựa chọn (`choices`).
    *   Mỗi lựa chọn có một hệ quả (`effect`) cụ thể: thay đổi chỉ số, thay đổi tiền, ghi lại nhật ký (`logKey`), và có thể kích hoạt một sự kiện khác (`triggers`).
*   **Phân loại sự kiện:**
    *   **Sự kiện ngẫu nhiên:** Xảy ra ngẫu nhiên trong các giai đoạn sống (`LifePhase`). Một nhân vật chỉ có thể gặp tối đa 2 sự kiện ngẫu nhiên mỗi năm. Có một thời gian chờ (`cooldown`) giữa các sự kiện để tránh spam.
    *   **Sự kiện Cột mốc (`Milestone`):** Xảy ra khi nhân vật đạt một điều kiện cụ thể (ví dụ: kết hôn, sinh con, chết).
    *   **Sự kiện Kích hoạt (`TriggerOnly`):** Chỉ xảy ra khi được một sự kiện khác kích hoạt.
    *   **Sự kiện Ưu tiên (Priority Event):** Sự kiện `decision_children` (quyết định có con) được xử lý riêng và ưu tiên hơn các sự kiện ngẫu nhiên khác để đảm bảo nó xảy ra.
*   **Cơ chế hoạt động:**
    1.  Vòng lặp game liên tục kiểm tra xem có sự kiện nào trong hàng đợi (`eventQueue`) không. Nếu có, nó sẽ được hiển thị.
    2.  Nếu không có, game sẽ kiểm tra các điều kiện để kích hoạt sự kiện ngẫu nhiên mới.
    3.  Khi người chơi chọn một lựa chọn, hàm `handleEventChoice` sẽ được gọi để áp dụng các `effect` tương ứng.

### **7. Mối Quan Hệ & Gia Đình**

*   **Hôn nhân:**
    *   Kích hoạt qua sự kiện `milestone_marriage`.
    *   Một nhân vật NPC mới sẽ được tạo ra làm bạn đời, với các chỉ số và nghề nghiệp được tạo ngẫu nhiên nhưng có liên quan đến nhân vật của người chơi.
*   **Sinh con:**
    *   Là một quy trình 2 bước:
        1.  Sự kiện `decision_children` hỏi người chơi có muốn thử sinh con không.
        2.  Nếu đồng ý, có một tỷ lệ thành công (70%). Nếu thành công, sự kiện `milestone_child_conceived` được kích hoạt.
    *   Một nhân vật con mới được tạo ra. Chỉ số của con được thừa hưởng từ trung bình của bố mẹ, cộng với một yếu tố ngẫu nhiên.
    *   **Sinh đôi/Sinh ba:** Cơ hội sinh đôi/ba sẽ được mở khóa sau khi người chơi đã có tổng số con nhất định trong gia tộc.
        *   Sinh đôi: Yêu cầu 2 con đã được sinh ra.
        *   Sinh ba: Yêu cầu 3 con đã được sinh ra.

### **8. Kinh Doanh (Business)**

Hệ thống này cho phép người chơi mua và quản lý doanh nghiệp.

*   **Định nghĩa (`BUSINESS_DEFINITIONS`):**
    *   Có nhiều loại hình kinh doanh (Y tế, Tài chính, Công nghệ, Nông nghiệp, v.v.).
    *   Mỗi loại có 3 cấp (tier), mỗi cấp có giá mua, doanh thu cơ bản, chi phí, và số lượng vị trí nhân viên (`slots`) khác nhau.
*   **Cơ chế hoạt động (tính toán hàng tháng):**
    1.  **Doanh thu:** Dựa trên `baseRevenue`, được điều chỉnh bởi:
        *   Tỷ lệ lấp đầy nhân viên.
        *   Tổng `skill` trung bình của tất cả nhân viên (bao gồm cả robot). `Skill` càng cao, doanh thu càng lớn.
    2.  **Chi phí:**
        *   Giá vốn hàng bán (`Cost of Goods Sold` - % doanh thu).
        *   Chi phí cố định hàng tháng.
        *   Lương nhân viên (tính dựa trên `skill` của họ, có mức trần).
        *   Chi phí thuê robot (700/tháng/robot).
        *   Chi phí quản lý trên đầu người (250/tháng/nhân viên).
    3.  **Lợi nhuận ròng:** (Doanh thu - Tổng chi phí). Lợi nhuận chủ doanh nghiệp nhận được cũng bị giới hạn bởi mức trần lương (`SALARY_CAP_MONTHLY`).
*   **Quản lý:**
    *   Người chơi có thể chỉ định các thành viên trong gia đình vào các vị trí (`slots`).
    *   Nếu một thành viên đang có việc làm và được chỉ định vào doanh nghiệp, họ sẽ tự động nghỉ việc cũ.
    *   Có thể thuê robot để lấp đầy các vị trí với chi phí cố định. Robot có `skill` là 30.

### **9. Các Hệ Thống Phụ**

*   **Tài sản (`Assets`):**
    *   Người chơi có thể mua các tài sản như nhà cửa, xe cộ, đồ điện tử, tác phẩm nghệ thuật, v.v.
    *   Mỗi tài sản có giá mua và cung cấp một buff chỉ số vĩnh viễn (tăng theo %) cho nhân vật người chơi.
*   **Thú cưng (`Pets`):**
    *   Người chơi có thể nhận nuôi thú cưng qua sự kiện.
    *   Mỗi loại thú cưng có chi phí nuôi hàng tháng và cộng một lượng nhỏ chỉ số (`happiness`, `health`) cho chủ nhân.
*   **Câu lạc bộ (`Clubs`):**
    *   Trong giai đoạn Cấp 2 và Cấp 3, nhân vật có thể tham gia các câu lạc bộ.
    *   Mỗi CLB có điều kiện tham gia (tuổi, chỉ số), cộng chỉ số hàng năm, và ảnh hưởng đến việc lựa chọn chuyên ngành Đại học.
*   **Tùy chỉnh Avatar (`Avatar Customization`):**
    *   Nhân vật có một `avatarState` để lưu trữ các lớp (layer) hình ảnh tạo nên ngoại hình.
    *   Hệ thống avatar bao gồm nhiều lớp như tóc sau, mắt, lông mày, miệng, râu, tóc trước.
    *   Mỗi lớp có thể được tùy chỉnh màu sắc.
    *   Việc tùy chỉnh avatar tốn phí **10,000**.
*   **Hệ thống Mở khóa (`Unlockable Features`):**
    *   Các tính năng lớn như Kinh doanh, Sinh đôi, Sinh ba được mở khóa khi tổng số con được sinh ra trong toàn bộ gia tộc (`totalChildrenBorn`) đạt các mốc nhất định.

### **10. Dữ Liệu & Lưu Trữ**

*   **Lưu game:** Trạng thái game (`GameState`) được lưu vào bộ nhớ cục bộ của thiết bị (`AsyncStorage`) dưới một khóa duy nhất.
*   **Cập nhật phiên bản (`Migrations`):** Hệ thống có cơ chế `applyMigrations` để tự động cập nhật cấu trúc dữ liệu của file save cũ khi có phiên bản game mới, đảm bảo tính tương thích ngược.

***

