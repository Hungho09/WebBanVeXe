# HƯỚNG DẪN THIẾT LẬP VÀ CHẠY HỆ THỐNG WEB ĐẶT VÉ XE

Dự án này là một web application hỗ trợ đặt vé xe khách trực tuyến dành cho cả khách hàng và quản trị viên (Admin). Hệ thống được phát triển theo kiến trúc Clean Architecture.

---

## 🏗️ 1. Kiến trúc Công nghệ (Tech Stack)
- **Backend**: ASP.NET Core 9.0 (C#) sử dụng Entity Framework Core.
- **Frontend**: Angular 19+ (Standalone Components).
- **Database**: Microsoft SQL Server (Hỗ trợ LocalDB).
- **Security**: JWT Authentication kết hợp BCrypt hashing.

---

## 🛠️ 2. Yêu cầu Hệ thống (Prerequisites)
Để hệ thống có thể chạy được, trên máy tính của bạn cần có:
- **.NET SDK 9.0**: [Link tải](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js (v18+) & npm**: [Link tải](https://nodejs.org/)
- **SQL Server**: Phiên bản SQL Server Express hoặc LocalDB (Đi kèm Visual Studio).
- **Angular CLI**: Cài đặt bằng lệnh `npm install -g @angular/cli`.

---

## 🗄️ 3. Thiết lập Cơ sở dữ liệu (Database Setup)

Để triển khai database vào SQL Server theo yêu cầu của thầy/cô, bạn thực hiện theo các bước:

1.  **Mở SQL Server Management Studio (SSMS)** và kết nối tới server của bạn (thường là `(localdb)\mssqllocaldb` hoặc `.\SQLEXPRESS`).
2.  **Mở và chạy file Cấu hình toàn bộ**:
    - Tìm file: `Backend/src/Api/Scripts/Initialize_All_Database.sql` trong mã nguồn.
    - Copy toàn bộ nội dung và thực thi (Execute) tại SSMS. Script này sẽ tự động tạo Database, tạo bảng và nạp đầy đủ dữ liệu mẫu (Admin, Chuyến xe, Tuyến đường, v.v.).

**Lưu ý**: Đảm bảo chuỗi kết nối (Connection String) trong file `Backend/src/Api/appsettings.json` trỏ chính xác vào server của bạn.

---

## 🚀 4. Hướng dẫn Chạy Hệ thống

### Bước 1: Khởi chạy Backend
Mở Terminal tại thư mục gốc của dự án (`Codenhalam-WebBanVeXe/Backend/src/Api`) và chạy:
```bash
dotnet restore
dotnet run
```
Backend sẽ mặc định chạy tại: `http://localhost:5048`

### Bước 2: Khởi chạy Frontend
Mở một Terminal khác tại thư mục `Frontend` và chạy:
```bash
npm install
npm run start
```
Hệ thống sẽ chạy tại: `http://localhost:4200`

---

## 👤 5. Thông tin Tài khoản Mặc định (Login)

Sau khi nạp file `seed_data.sql`, bạn có thể đăng nhập với các tài khoản:

| Quyền | Tên đăng nhập | Mật khẩu | Mô tả |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `Admin@123` | Toàn quyền quản trị, quản lý xe, trip. |
| **Customer** | `customer1` | `Admin@123` | Tài khoản dành cho khách hàng đặt vé. |

---

## 📑 6. Một số tính năng chính
- **Cơ sở dữ liệu tập trung**: Quản lý Tuyến đường, Điểm dừng, Đội xe (VIP, SleepBus).
- **Admin Dashboard**: Thêm mới chuyến đi, cập nhật trạng thái hoạt động của xe.
- **Tìm kiếm thông minh**: Tìm kiếm chuyến đi theo lộ trình Đà Lạt - Sài Gòn,...
- **Đặt vé trực quan**: Chọn ghế ngồi theo sơ đồ tầng 1/tầng 2.

---
*Dự án thuộc bài tập lớn môn Thiết kế Web - GVHD: [Tên thầy/cô].*
