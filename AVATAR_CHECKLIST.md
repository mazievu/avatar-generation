# Checklist Kiểm tra Hệ thống Avatar

Sử dụng checklist này để xác minh rằng hệ thống avatar đã được implement đúng sau khi refactor.

## A. Cấu trúc Code (`AvatarConfig.cs`)

- [ ] File `AvatarConfig.cs` tồn tại ở đúng đường dẫn.
- [ ] Enum `AvatarAge` (Baby, Adult) được định nghĩa.
- [ ] Class `AvatarLayerSet` chứa đúng các trường `Sprite` theo GDD:
    - [ ] `backHair`
    - [ ] `features`
    - [ ] `eyes`
    - [ ] `eyebrows`
    - [ ] `beard`
    - [ ] `mouth`
    - [ ] `frontHair`
    - [ ] `accessory`
- [ ] Class `AvatarConfig` chứa 2 trường `baby` và `adult` kiểu `AvatarLayerSet`.
- [ ] Class `AvatarConfig` chứa đúng các trường `int` cho z-order theo GDD.

## B. Logic Hiển thị (`AvatarPreview.cs`)

- [ ] File `AvatarPreview.cs` tồn tại và không có lỗi biên dịch.
- [ ] Phương thức `Redraw()` đọc đúng các layer từ `AvatarConfig.asset`.
- [ ] `Redraw()` tạo ra các `GameObject` con cho mỗi layer có sprite được gán.
- [ ] Các `GameObject` con được sắp xếp đúng thứ tự `z-order` (kiểm tra trong Hierarchy của Unity).
- [ ] Layer `beard` chỉ được vẽ khi `age` là `Adult`.
- [ ] Gọi `Redraw()` nhiều lần không gây ra lỗi hoặc tạo ra các đối tượng rác (đã dọn dẹp avatar cũ).

## C. Tích hợp và Sử dụng

- [ ] Có thể tạo `AvatarConfig.asset` từ menu `Create > LifeSim > Avatar Config`.
- [ ] Kéo thả sprite vào các trường trong `AvatarConfig.asset` hoạt động bình thường.
- [ ] Gắn `AvatarPreview` vào một `GameObject` và gán `config`, `layerRoot` không gây lỗi.
- [ ] Avatar hiển thị đúng trong Editor khi bật `OnEnable` (nếu có).
- [ ] `Bootstrap.cs` có thể gọi `Redraw()` mà không gây lỗi.

Hoàn thành tất cả các mục trong checklist này đồng nghĩa với việc hệ thống avatar đã sẵn sàng.
