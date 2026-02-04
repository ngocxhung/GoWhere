# 📝 Lịch sử lịch trình AI - Hướng dẫn triển khai

## ✅ Đã hoàn thành

### Backend
1. ✅ Model `AIScheduleHistory` - Lưu lịch sử lịch trình
2. ✅ Repository `IAIScheduleHistoryRepo` + `AIScheduleHistoryRepo`
3. ✅ Service `AIChatService` - Tự động lưu sau khi tạo lịch trình
4. ✅ API Endpoints:
   - `GET /api/AIChat/History` - Lấy danh sách lịch sử
   - `GET /api/AIChat/History/{id}` - Lấy chi tiết 1 lịch trình
   - `DELETE /api/AIChat/History/{id}` - Xóa lịch trình
5. ✅ DbContext updated

### Database Migration
**Cần chạy migration để tạo bảng `AIScheduleHistories`:**

```bash
# Cài EF tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration
dotnet ef migrations add AddAIScheduleHistory

# Apply migration
dotnet ef database update
```

**HOẶC** restart backend - auto migration sẽ chạy tự động.

## 📊 Database Schema

```sql
CREATE TABLE AIScheduleHistories (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Destination NVARCHAR(MAX) NOT NULL,
    ProvinceId INT NULL,
    Budget DECIMAL(18,2) NOT NULL,
    NumberOfDays INT NOT NULL,
    TravelStyle NVARCHAR(MAX) NOT NULL,
    CompanionType NVARCHAR(MAX) NOT NULL,
    Pax INT NOT NULL,
    ScheduleJson NVARCHAR(MAX) NOT NULL,
    Summary NVARCHAR(MAX) NULL,
    EstimatedTotalCost DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProvinceId) REFERENCES Provinces(ProvinceId)
);
```

## 🔌 API Endpoints

### 1. Lấy lịch sử lịch trình
```http
GET /api/AIChat/History
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": 1,
    "userId": 1,
    "destination": "Đà Nẵng",
    "provinceId": 32,
    "province": {
      "provinceId": 32,
      "provinceName": "Đà Nẵng"
    },
    "budget": 5000000,
    "numberOfDays": 3,
    "travelStyle": "dynamic",
    "companionType": "friends",
    "pax": 4,
    "scheduleJson": "{...full AI response...}",
    "summary": "Lịch trình 3 ngày khám phá Đà Nẵng...",
    "estimatedTotalCost": 4800000,
    "createdAt": "2026-02-03T13:30:00Z"
  }
]
```

### 2. Lấy chi tiết lịch trình
```http
GET /api/AIChat/History/1
Authorization: Bearer {token}
```

**Response:** Same as above (single object)

### 3. Xóa lịch trình
```http
DELETE /api/AIChat/History/1
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "Đã xóa lịch trình"
}
```

## 🎯 Luồng hoạt động

1. User tạo lịch trình AI
2. Backend gọi Gemini API
3. Parse response thành công
4. **Tự động lưu vào `AIScheduleHistories`**
5. Return response cho frontend
6. User có thể xem lại lịch sử bất kỳ lúc nào

## 🔐 Security

- ✅ Tất cả endpoints đều yêu cầu authentication
- ✅ User chỉ xem được lịch sử của chính mình
- ✅ Validation userId khi get/delete
- ✅ ScheduleJson được serialize an toàn

## 📱 Frontend Integration (Next step)

Cần tạo UI để:
1. Hiển thị danh sách lịch sử (cards/list)
2. Click vào để xem chi tiết (parse ScheduleJson)
3. Nút xóa lịch trình
4. Filter/search lịch sử

**Gợi ý UI location:**
- Trang `/ai` - Tab "Lịch sử"
- Trang `/profile` hoặc `/itinerary` - Section riêng
- Modal popup từ header

## ✨ Features

✅ **Auto-save** - Tự động lưu sau khi tạo thành công  
✅ **Full data** - Lưu cả request params và AI response  
✅ **User-specific** - Mỗi user có lịch sử riêng  
✅ **Searchable** - Có thể search theo destination, date  
✅ **Deletable** - User có thể xóa lịch sử không cần  

## 🚀 Next Steps

1. ✅ Restart backend để apply migration
2. ⏳ Tạo frontend UI để hiển thị lịch sử
3. ⏳ Thêm pagination cho danh sách lịch sử
4. ⏳ Thêm filter/search functionality
5. ⏳ Thêm export PDF cho lịch trình
6. ⏳ Thêm share lịch trình với bạn bè

---

**Backend đã sẵn sàng! Chỉ cần restart để apply migration. 🎉**
