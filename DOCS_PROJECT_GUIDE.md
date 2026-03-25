# 🚌 Codenhalam-WebBanVeXe — Hướng Dẫn Kỹ Thuật & Cài Đặt

Chào mừng bạn đến với dự án Hệ thống Đặt vé Xe khách Trực tuyến. Tài liệu này cung cấp cái nhìn tổng quan về tư duy thiết kế, kiến trúc hệ thống và hướng dẫn chi tiết để một người mới có thể thiết lập dự án từ con số 0.

---

## 🏗️ 1. Tư Duy Thiết Kế & Kiến Trúc (Architecture)

Dự án được xây dựng dựa trên nguyên lý **Clean Architecture**, giúp mã nguồn dễ bảo trì, mở rộng và kiểm thử.

### 🏠 Cơ Cấu Thư Mục (Structure)
- **`backend/src/Domain`**: Chứa các Entity (User, Trip, Seat, Booking...), Enums và Interfaces cơ bản. Không phụ thuộc vào bất kỳ thư viện ngoài nào (Core logic).
- **`backend/src/Application`**: Chứa logic nghiệp vụ (Services), DTOs và Interfaces. Đây là nơi xử lý các quy tắc tính toán (ví dụ: logic giữ ghế 10 phút).
- **`backend/src/Infrastructure`**: Chứa implementation của các interface (Persistence, Notification...). Sử dụng **Entity Framework Core (SQLite)** để quản lý cơ sở dữ liệu.
- **`backend/src/api`**: Tầng giao tiếp (REST API Controllers). Chịu trách nhiệm nhận yêu cầu từ Frontend và trả về kết quả.
- **`Frontend`**: Xây dựng bằng **Angular 19+**, sử dụng kiến trúc **Standalone Components** hiện đại, không cần Module phức tạp.

### 🗝️ Logic Cốt Lõi (Epic 3 & 4)
- **Seat Locking Engine**: Khi khách hàng chọn ghế, hệ thống sẽ giữ ghế (`Status = Locked`) trong 10 phút thông qua `TripService`.
- **Background Worker**: `SeatLockBackgroundService` chạy ngầm để giải phóng các ghế đã hết hạn giữ mà không được thanh toán.
- **Reporting System**: `ReportingService` tổng hợp doanh thu theo ngày/tháng và các tuyến đường phổ biến cho Admin Dashboard.

---

## 🛠️ 2. Yêu Cầu Hệ Thống (Prerequisites)

- **.NET SDK 9.0** trở lên.
- **Node.js v20+** & **npm**.
- **Angular CLI v19+** (Cài đặt bằng: `npm install -g @angular/cli`).

---

## 🚀 3. Hướng Dẫn Cài Đặt Từ Đầu (Clone & Build)

### Bước 1: Clone dự án
```powershell
git clone https://github.com/theanh-512/Codenhalam-WebBanVeXe.git
cd Codenhalam-WebBanVeXe
```

### Bước 2: Thiết lập Backend
1. Di chuyển vào thư mục API:
   ```powershell
   cd backend/src/api
   ```
2. Cài đặt các công cụ Entity Framework (nếu chưa có):
   ```powershell
   dotnet tool install --global dotnet-ef
   ```
3. Khôi phục packages:
   ```powershell
   dotnet restore
   ```
4. **Quan trọng**: Nếu bạn bắt đầu mới hoàn toàn, hãy xóa tệp `vexe.db` cũ (nếu có) để hệ thống tự động khởi tạo lại cấu trúc chuẩn nhất.
5. Chạy Backend:
   ```powershell
   dotnet run
   ```
   *Lưu ý: Hệ thống đã được cấu hình tự động chạy Migration và Seed dữ liệu mẫu (Admin, Tuyến đường, Xe, Chuyến đi) ngay khi khởi động.*

### Bước 3: Thiết lập Frontend
1. Mở terminal mới tại thư mục `frontend`:
   ```powershell
   cd frontend
   ```
2. Cài đặt dependencies:
   ```powershell
   npm install
   ```
3. Chạy Frontend với Proxy:
   ```powershell
   npm run start
   ```
   *Lưu ý: Dự án sử dụng `proxy.conf.json` để chuyển hướng các yêu cầu `/api` từ cổng 4200 sang 5048 của Backend nhằm tránh lỗi CORS.*

---

## 🔑 4. Thông Tin Đăng Nhập Mặc Định

Dữ liệu mẫu (Demo Data) cung cấp tài khoản Admin để truy cập Dashboard:
- **Tài khoản**: `admin`
- **Mật khẩu**: `Admin@123`
- **Vai trò**: Admin

---

## ⚠️ 5. Các Lỗi Thường Gặp & Cách Xử Lý

### 1. Lỗi "Duplicate column" hoặc "Out of sync Migration"
Nếu bạn clone code về mà gặp lỗi database, hãy thực hiện reset triệt để:
1. Xóa nội dung thư mục `backend/src/Infrastructure/Migrations/`.
2. Xóa tệp `backend/src/api/vexe.db`.
3. Chạy lệnh: `dotnet ef migrations add Initial --project backend/src/Infrastructure/Infrastructure.csproj --startup-project backend/src/api/Api.csproj`.
4. Sau đó chạy lại `dotnet run`.

### 2. Lỗi "Process cannot access the file" (Building error)
Lỗi này do tiến trình API cũ chưa tắt hẳn.
- Cách xử lý: Tắt terminal chạy Backend hoặc dùng lệnh: `taskkill /F /IM Api.exe /T`.

---

## 📅 6. Quy Trình Phát Triển (Workflow)

Nếu bạn muốn đóng góp code:
1. Luôn thực hiện `git pull` trước khi bắt đầu.
2. Nếu có thay đổi model ở `Domain`, hãy tạo migration mới và push kèm code.
3. Luôn Build cả 2 đầu Backend & Frontend để đảm bảo tính nhất quán.

---
**Codenhalam Team** — *"Cái gì không làm được thì mình vừa khóc vừa làm"*
