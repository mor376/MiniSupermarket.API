Nguyen Tran Ngoc Thuong  
2124110088  
CCQ2411C
# MiniSupermarket.API

Đồ án môn học **Lập trình ứng dụng C# .NET Core** – Trường Cao đẳng Công Thương TP. Hồ Chí Minh.

- Sinh viên: Nguyễn Trần Ngọc Thương
- MSSV: 2124110088
- Lớp: CCQ2411C
- GVHD: ThS. Nguyễn Cao Thái
- Đề tài: Nghiên cứu kiến trúc RESTful Web API trong xây dựng phần mềm quản lý bán hàng – Ứng dụng Mini Supermarket An Phước

## 1. Giới thiệu

Hệ thống quản lý bán hàng và tồn kho cho siêu thị mini, gồm hai phần:

- **Backend**: ASP.NET Core Web API (.NET 8) cung cấp các endpoint RESTful, xác thực JWT, phân quyền theo vai trò.
- **Frontend**: ứng dụng Windows Forms (`MiniSupermarket.WinForms`) gọi API qua `HttpClient`.

Kiến trúc 3 lớp: **WinForms Client – Web API – SQL Server**.

## 2. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ / nền tảng | C#, .NET 8 |
| Backend | ASP.NET Core Web API, Swagger (Swashbuckle) |
| Truy cập dữ liệu | Entity Framework Core (Code-First, Migrations, LINQ) và ADO.NET (SqlDataReader, SqlParameter, Stored Procedure, SqlTransaction) |
| Cơ sở dữ liệu | Microsoft SQL Server / LocalDB |
| Bảo mật | JWT Bearer, phân quyền `[Authorize(Roles = "...")]` |
| Giao diện | Windows Forms |

## 3. Nội dung đã thực hiện

**Backend**
- Tách Controller riêng: `Auth`, `Categories`, `Products`, `Customers`, `Suppliers`, `Users`, `Roles`, `Checkout`, `Reports`, `AdoProducts`.
- Đường dẫn RESTful dùng danh từ số nhiều (`api/categories`, `api/products`…); dùng đúng GET/POST/PUT/DELETE và mã trạng thái 200/201/204/400/401/403/404/409.
- Entity: `User`, `Category`, `Product`, `Customer`, `Order`, `OrderDetail` (có thêm `Supplier`), kèm ràng buộc dữ liệu; dùng DTO để nhận/trả dữ liệu.
- EF Core Code-First, Migrations, dữ liệu mẫu (7 nhóm hàng, 21 sản phẩm, khách hàng, nhà cung cấp, tài khoản).
- CRUD nhóm hàng/sản phẩm/khách hàng; tìm kiếm bằng LINQ; `.Include()` nạp nhóm hàng; `.AsNoTracking()` cho truy vấn chỉ đọc.
- ADO.NET: tra cứu barcode bằng `SqlDataReader`, tham số hóa chống SQL Injection, Stored Procedure `sp_GetRevenueByCashier`.
- Thanh toán POS trong một `SqlTransaction`: lưu hóa đơn, trừ tồn kho, tích/đổi điểm; hết hàng thì `Rollback`, không lưu hóa đơn rác.
- Đăng nhập cấp JWT; phân quyền Admin / Cashier / Warehouse; Swagger có nút **Authorize**.
- API báo cáo: tổng quan KPI, doanh thu theo ngày, top 5 sản phẩm bán chạy, doanh thu theo thu ngân.

**Frontend (WinForms)**
- Giao diện Shell: menu bên trái, tiêu đề phía trên, vùng làm việc ở giữa; form con nhúng vào panel.
- Đăng nhập lưu token, ẩn/hiện chức năng theo vai trò.
- `ApiClientService` gọi API tập trung, tự gắn Bearer Token.
- Màn hình: Thu ngân (POS), Danh mục sản phẩm (DataGridView), Khách hàng, Báo cáo (KPI, biểu đồ), Quản lý kho.
- Xử lý lỗi mạng/401/403 bằng hộp thoại tiếng Việt.

## 4. Cấu trúc thư mục

```
MiniSupermarket.API
├── Controllers/              Các controller của API
├── Models/                   Entity, DTO
├── Filters/                  Bộ lọc đo thời gian request
├── Properties/               launchSettings.json
├── MiniSupermarket.WinForms/ Ứng dụng Windows Forms (Client)
├── appsettings.json          Chuỗi kết nối, cấu hình JWT
├── Program.cs                Đăng ký dịch vụ, middleware, Swagger, JWT
└── MiniSupermarket.API.sln
```

## 5. Yêu cầu môi trường

- Windows 10/11
- Visual Studio 2022 (workload *ASP.NET and web development* và *.NET desktop development*) hoặc .NET 8 SDK
- SQL Server hoặc SQL Server LocalDB
- (Tùy chọn) SQL Server Management Studio để xem dữ liệu

## 6. Hướng dẫn chạy Backend (Web API)

1. Clone mã nguồn:
   ```bash
   git clone https://github.com/mor376/MiniSupermarket.API.git
   cd MiniSupermarket.API
   ```
2. Mở `appsettings.json`, chỉnh chuỗi kết nối `DefaultConnection` cho đúng SQL Server của máy, ví dụ:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```
3. Tạo cơ sở dữ liệu (nếu chưa tự tạo khi chạy). Trong Package Manager Console:
   ```powershell
   Update-Database
   ```
   hoặc dùng terminal: `dotnet ef database update`.
4. Chạy API: bấm **F5** trong Visual Studio hoặc:
   ```bash
   dotnet run
   ```
5. Mở Swagger để kiểm thử: `https://localhost:7046/swagger` (số cổng xem trong `Properties/launchSettings.json`).

**Kiểm thử API bảo mật trên Swagger**
1. Gọi `POST /api/Auth/login` với tài khoản demo bên dưới để nhận `token`.
2. Bấm nút **Authorize** (ổ khóa), dán token vào rồi xác nhận.
3. Gọi các endpoint cần đăng nhập, ví dụ `POST /api/checkout`.

## 7. Hướng dẫn chạy Frontend (WinForms)

1. Giữ cho API đang chạy.
2. Mở solution trong Visual Studio, chuột phải project `MiniSupermarket.WinForms` → **Set as Startup Project** → **F5**. (Hoặc chạy `dotnet run --project MiniSupermarket.WinForms`.)
3. Đăng nhập bằng một trong các tài khoản demo.

> Nếu địa chỉ API khác `https://localhost:7046`, sửa địa chỉ gốc trong `ApiClientService` của project WinForms.

## 8. Tài khoản demo

| Tài khoản | Vai trò | Mật khẩu |
|---|---|---|
| `admin01` | Admin | `123456` |
| `cashier01` | Thu ngân (Cashier) | `123456` |
| `ware01` | Thủ kho (Warehouse) | `123456` |

Chỉ dùng cho mục đích học tập, không dùng trong môi trường thật.

## 9. Danh sách API chính

| Phương thức | Đường dẫn | Mô tả |
|---|---|---|
| POST | `/api/auth/login` | Đăng nhập, nhận JWT |
| GET/POST | `/api/categories` | Danh sách / thêm nhóm hàng |
| GET/PUT/DELETE | `/api/categories/{id}` | Xem / sửa / xóa nhóm hàng |
| GET | `/api/categories/search` | Tìm nhóm hàng |
| GET/POST | `/api/products` | Danh sách / thêm sản phẩm |
| GET/PUT/DELETE | `/api/products/{id}` | Xem / sửa / xóa sản phẩm |
| GET | `/api/ado-products/barcode/{barcode}` | Tra cứu barcode bằng ADO.NET |
| POST | `/api/checkout` | Thanh toán POS (Admin, Cashier) |
| DELETE | `/api/checkout/cancel/{orderId}` | Hủy hóa đơn (Admin) |
| GET | `/api/reports/dashboard-summary` | KPI tổng quan |
| GET | `/api/reports/revenue-by-cashier` | Doanh thu theo thu ngân (Admin) |

Danh sách đầy đủ xem trên Swagger hoặc trong báo cáo đồ án.

## 10. Hạn chế và hướng phát triển

- Phân trang phía server (`Skip`/`Take`) cho danh sách sản phẩm.
- Gắn `[Authorize]` cho toàn bộ API CRUD.
- Đổi cách lưu mật khẩu sang thuật toán băm có salt (BCrypt/PBKDF2) và đưa khóa JWT ra User Secrets hoặc biến môi trường.
- Tích hợp thanh toán điện tử và in hóa đơn.
