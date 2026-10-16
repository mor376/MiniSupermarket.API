# Quản lý bán hàng - Bách hóa Xanh An Phước

**Đề tài:** Nghiên cứu kiến trúc RESTful Web API trong xây dựng phần mềm quản lý bán hàng  
**Sinh viên:** Nguyen Tran Ngoc Thuong  
**Mã sinh viên:** 2124110088  
**Lớp:** CCQ2411C

## Dữ liệu demo

Cơ sở dữ liệu triển khai đủ 6 bảng: `Users`, `Categories`, `Products`, `Customers`, `Orders` và `OrderDetails`. Dữ liệu mẫu của cửa hàng gồm 7 nhóm hàng, 21 sản phẩm, 16 khách hàng, 3 hóa đơn và 6 dòng chi tiết hóa đơn; các dữ liệu này được giữ nguyên khi bổ sung giao diện và phân quyền.

## Kiểm thử phân quyền giao diện

API demo chấp nhận các tài khoản `admin01`, `admin02`, `admin_backup`, `supervisor`; `cashier01` đến `cashier06`; và `ware01` đến `ware05`. Mật khẩu ban đầu cho các tài khoản kiểm thử là `123456`. Đây là tài khoản demo xác thực trong bộ nhớ của API, không phải dữ liệu đã lưu trong bảng `Users`; không dùng cấu hình này cho môi trường thật. Tài khoản cũ `admin` và `cashier` cũng được giữ tương thích.

## Buổi 5: ADO.NET và POS

- `GET /api/ado-products/barcode/{barcode}`: tra cứu mã vạch bằng `SqlDataReader`.
- `GET /api/ado-products/disconnected-cache`: tải danh sách vào `DataTable` bằng `SqlDataAdapter`.
- `GET /api/reports/revenue-by-cashier`: báo cáo Stored Procedure, chỉ Admin.
- `POST /api/checkout`: thanh toán đa bảng trong `SqlTransaction`, chỉ Admin/Cashier. Giá bán lấy từ database; giá gửi từ client không được dùng để tính hóa đơn.
- `DELETE /api/checkout/cancel/{orderId}`: hủy hóa đơn, hoàn kho và điểm trong transaction, chỉ Admin.

Sau khi cấu hình `DefaultConnection`, khởi chạy API để tự động áp dụng migration, tạo Stored Procedure và nạp dữ liệu mẫu. Có thể áp dụng migration thủ công bằng lệnh `dotnet ef database update`. Dữ liệu cửa hàng mẫu được giữ nguyên; các thay đổi tồn kho, hóa đơn và điểm khách chỉ phát sinh khi thực hiện thanh toán/hủy hóa đơn.

## Giao diện WinForms

Client sử dụng một cửa sổ Dashboard với Sidebar, breadcrumb/header và màn hình con được nhúng vào vùng làm việc. Màn POS có bố cục giỏ hàng/thanh toán 7:3; danh mục sản phẩm hiển thị đủ mã SP, mã vạch, tên, mã ngành hàng, giá vốn, giá bán, tồn kho và đơn vị, kèm lọc, CRUD, menu chuột phải và phím F2. Màn khách hàng có tìm kiếm, KPI tổng khách/điểm/hạng VIP và CRUD hồ sơ; màn báo cáo hiển thị tổng doanh thu theo thu ngân từ Stored Procedure.

API checkout hiện chỉ hoàn tất thanh toán theo quy trình POS; phương thức chuyển khoản/thẻ và in hóa đơn chưa được lưu/xử lý bởi backend. Quản lý tài khoản WinForms cũng chưa được nối CRUD với bảng `Users`: tài khoản đăng nhập hiện vẫn là danh sách demo trong bộ nhớ của API.
