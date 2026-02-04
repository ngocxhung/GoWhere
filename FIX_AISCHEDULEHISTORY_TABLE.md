# Hướng dẫn tạo bảng AIScheduleHistories

## Vấn đề
Lỗi: `Invalid object name 'AIScheduleHistories'` - Bảng AIScheduleHistories chưa tồn tại trong database.

## Giải pháp

### Cách 1: Chạy SQL Script (Khuyến nghị - Nhanh nhất)

1. Mở **SQL Server Management Studio (SSMS)** hoặc **Azure Data Studio**
2. Kết nối đến database của bạn
3. Mở file `backend/Scripts/CreateAIScheduleHistoriesTable.sql`
4. Chạy script (F5 hoặc Execute)
5. Kiểm tra bảng đã được tạo:
   ```sql
   SELECT * FROM INFORMATION_SCHEMA.TABLES 
   WHERE TABLE_NAME = 'AIScheduleHistories'
   ```

### Cách 2: Sử dụng EF Migrations (Nếu dotnet ef hoạt động)

1. Mở terminal trong folder `backend`
2. Chạy lệnh:
   ```bash
   dotnet ef database update
   ```

### Cách 3: Chạy SQL trực tiếp trong code

Nếu không có SSMS, bạn có thể tạo một endpoint tạm thời để chạy migration:

1. Thêm vào `Program.cs` (trước `app.Run()`):
   ```csharp
   app.MapGet("/admin/create-table", async (ApplicationDbContext db) =>
   {
       await db.Database.ExecuteSqlRawAsync(@"
           CREATE TABLE [dbo].[AIScheduleHistories] (
               [Id] INT IDENTITY(1,1) NOT NULL,
               [UserId] INT NOT NULL,
               [Destination] NVARCHAR(MAX) NOT NULL,
               [ProvinceId] INT NULL,
               [Budget] DECIMAL(18, 2) NOT NULL,
               [NumberOfDays] INT NOT NULL,
               [TravelStyle] NVARCHAR(MAX) NOT NULL,
               [CompanionType] NVARCHAR(MAX) NOT NULL,
               [Pax] INT NOT NULL,
               [ScheduleJson] NVARCHAR(MAX) NOT NULL,
               [Summary] NVARCHAR(MAX) NULL,
               [EstimatedTotalCost] DECIMAL(18, 2) NOT NULL,
               [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
               CONSTRAINT [PK_AIScheduleHistories] PRIMARY KEY CLUSTERED ([Id] ASC),
               CONSTRAINT [FK_AIScheduleHistories_Users_UserId] FOREIGN KEY ([UserId]) 
                   REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
               CONSTRAINT [FK_AIScheduleHistories_Provinces_ProvinceId] FOREIGN KEY ([ProvinceId]) 
                   REFERENCES [dbo].[Provinces] ([ProvinceId]) ON DELETE NO ACTION
           );
           
           CREATE NONCLUSTERED INDEX [IX_AIScheduleHistories_UserId] 
               ON [dbo].[AIScheduleHistories] ([UserId] ASC);
           
           CREATE NONCLUSTERED INDEX [IX_AIScheduleHistories_ProvinceId] 
               ON [dbo].[AIScheduleHistories] ([ProvinceId] ASC);
       ");
       return Results.Ok("Table created successfully!");
   });
   ```

2. Restart backend
3. Truy cập: http://localhost:5191/admin/create-table
4. Xóa endpoint này sau khi tạo xong

## Kiểm tra

Sau khi tạo bảng, test lại bằng cách:
1. Refresh trang homepage
2. Tạo lịch trình AI
3. Click "Lưu lịch trình"
4. Không còn lỗi nữa!

## Files liên quan

- `backend/Model/AIScheduleHistory.cs` - Model đã có sẵn ✅
- `backend/Model/ApplicationDbContext.cs` - DbSet đã có sẵn (dòng 24) ✅
- `backend/Scripts/CreateAIScheduleHistoriesTable.sql` - SQL script để tạo bảng
- `backend/Migrations/20260204120515_AddAIScheduleHistoryTable.cs` - Migration file (chưa apply)

## Lưu ý

- Bảng này lưu lịch sử các lịch trình AI mà user đã tạo
- Mỗi lần user click "Lưu lịch trình", một record mới sẽ được thêm vào bảng này
- ScheduleJson column lưu toàn bộ response từ AI dưới dạng JSON
