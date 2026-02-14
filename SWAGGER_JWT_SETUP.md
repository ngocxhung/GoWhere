# ✅ ĐÃ CẬP NHẬT - Swagger với JWT Authentication

## 🎉 Thay đổi

### File: `Program.cs`

**Đã thêm:**
1. ✅ Swagger configuration với JWT Bearer Authentication
2. ✅ Security Definition cho Bearer token
3. ✅ Security Requirement cho tất cả endpoints
4. ✅ `UseAuthentication()` middleware

---

## 🔐 Cách sử dụng Swagger với Authentication

### Bước 1: Chạy ứng dụng

```bash
cd backend
dotnet run
```

Swagger UI: `https://localhost:7001/swagger` hoặc `http://localhost:5191/swagger`

---

### Bước 2: Login để lấy Token

**Tìm endpoint:** `POST /api/Auth/Login`

**Click "Try it out"**

**Request Body:**
```json
{
  "username": "adminuser",
  "password": "adminpassword"
}
```

**Hoặc User thường:**
```json
{
  "username": "regularuser",
  "password": "password123"
}
```

**Execute → Nhận response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWludXNlciIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTczODQ5ODkwMCwiZXhwIjoxNzM4NTAyNTAwLCJpYXQiOjE3Mzg0OTg5MDB9.xxx",
  "userId": 2,
  "username": "adminuser",
  "email": "admin@example.com",
  "fullName": "Admin User",
  "roleName": "Admin"
}
```

**Copy token** (phần dài từ "eyJ...")

---

### Bước 3: Authorize trong Swagger

**1. Nhìn lên góc phải Swagger UI**

Bạn sẽ thấy nút **"Authorize"** 🔓

**2. Click "Authorize"**

Modal hiện ra với input "Value"

**3. Nhập token:**

```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

⚠️ **LƯU Ý:** Phải có chữ `Bearer` + **khoảng trắng** + token!

**Hoặc chỉ cần nhập token (không cần "Bearer"):**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**4. Click "Authorize"**

Icon đổi thành 🔒 (có khóa)

**5. Click "Close"**

---

### Bước 4: Test API yêu cầu Authentication

**Ví dụ:** Test Admin endpoint

**Endpoint:** `GET /api/ScenicSpot/Admin/GetAllScenicSpots`

**Click "Try it out" → Execute**

✅ **Success!** Nếu đăng nhập đúng với Admin role

❌ **403 Forbidden** Nếu login với User thường

---

## 🎨 UI Changes

### Before
```
No Authorization button
No way to test protected endpoints
Always get 401 Unauthorized
```

### After
```
✅ "Authorize" button in top-right
✅ Can input JWT token
✅ All requests auto include: Authorization: Bearer {token}
✅ Test protected endpoints successfully
```

---

## 🧪 Test Scenarios

### Test 1: Public Endpoint (No Auth Required)

**Endpoint:** `GET /api/Province`

**Steps:**
1. Không cần Authorize
2. Click "Try it out" → Execute
3. ✅ Success 200 OK

---

### Test 2: User Endpoint (Auth Required)

**Endpoint:** `POST /api/AIChat/GenerateSchedule`

**Steps:**
1. Login as User → Copy token
2. Authorize với token
3. Click "Try it out" → Fill request body → Execute
4. ✅ Success (nếu còn lượt miễn phí)
5. ✅ 402 Payment Required (nếu hết lượt)

---

### Test 3: Admin Endpoint (Admin Role Required)

**Endpoint:** `GET /api/ScenicSpot/Admin/GetAllScenicSpots`

**Login as Admin:**
```json
{
  "username": "adminuser",
  "password": "adminpassword"
}
```

**Steps:**
1. Login as Admin → Copy token
2. Authorize với token
3. Execute endpoint
4. ✅ Success 200 OK (admin có quyền)

**Login as User:**
```json
{
  "username": "regularuser",
  "password": "password123"
}
```

**Steps:**
1. Login as User → Copy token
2. Authorize với token
3. Execute Admin endpoint
4. ❌ 403 Forbidden (user không có quyền)

---

### Test 4: Token Hết Hạn

**Steps:**
1. Login → Copy token
2. Authorize
3. Đợi token hết hạn (mặc định 1 giờ)
4. Execute endpoint
5. ❌ 401 Unauthorized
6. Login lại → Lấy token mới → Authorize lại

---

## 📋 Swagger Features

### Security Definition
```csharp
options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT Authorization header using the Bearer scheme."
});
```

**Tính năng:**
- ✅ Hiển thị nút "Authorize"
- ✅ Input field cho token
- ✅ Tự động thêm header: `Authorization: Bearer {token}`
- ✅ Lock icon khi đã authorize

---

### Security Requirement
```csharp
options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }
});
```

**Tính năng:**
- ✅ Tất cả endpoints hiển thị 🔒 icon
- ✅ Endpoints có `[Authorize]` yêu cầu token
- ✅ Public endpoints vẫn hoạt động không cần token

---

## 🎯 API Testing Workflow

### 1. Public APIs (No Auth)
```
GET /api/Province
GET /api/ScenicSpot/GetAllScenicSpots
→ Execute directly, no login needed
```

### 2. User APIs (Auth Required)
```
1. POST /api/Auth/Login → Get token
2. Authorize with token
3. POST /api/AIChat/GenerateSchedule
4. GET /api/Payment/PaymentHistory
→ Success if authenticated
```

### 3. Admin APIs (Admin Role)
```
1. POST /api/Auth/Login (admin credentials)
2. Authorize with admin token
3. GET /api/ScenicSpot/Admin/GetAllScenicSpots
4. PATCH /api/ScenicSpot/ToggleStatus/{id}
5. DELETE /api/ScenicSpot/DeleteScenicSpot/{id}
→ Success only if Admin role
```

---

## 🔍 Troubleshooting

### Issue 1: 401 Unauthorized
**Nguyên nhân:**
- Chưa Authorize
- Token sai
- Token hết hạn

**Giải pháp:**
1. Login lại để lấy token mới
2. Authorize lại
3. Retry request

---

### Issue 2: 403 Forbidden
**Nguyên nhân:**
- Đã login nhưng không đủ quyền
- VD: User gọi Admin endpoint

**Giải pháp:**
1. Kiểm tra endpoint yêu cầu role nào
2. Login với account đúng role
3. Authorize lại với token mới

---

### Issue 3: Token không hoạt động
**Nguyên nhân:**
- Thiếu "Bearer " prefix
- Copy sai token
- Token bị modify

**Giải pháp:**
1. Đảm bảo format: `Bearer {token}`
2. Copy lại token từ đầu đến cuối
3. Không thêm/bớt ký tự nào

---

## 📊 Before vs After

| Feature | Before | After |
|---------|--------|-------|
| **Authorize Button** | ❌ No | ✅ Yes (top-right) |
| **Token Input** | ❌ No | ✅ Yes (modal) |
| **Auto Header** | ❌ No | ✅ Yes (`Authorization: Bearer`) |
| **Test Protected APIs** | ❌ No (401) | ✅ Yes (with token) |
| **Role Testing** | ❌ No | ✅ Yes (User vs Admin) |
| **Lock Icons** | ❌ No | ✅ Yes (on endpoints) |

---

## 🎨 Swagger UI Screenshots

### Authorize Button (Top-Right)
```
┌─────────────────────────────────────────┐
│ GoWhere API v1                  Authorize│
│                                      🔓   │
└─────────────────────────────────────────┘
```

### After Authorize
```
┌─────────────────────────────────────────┐
│ GoWhere API v1                  Authorize│
│                                      🔒   │
└─────────────────────────────────────────┘
```

### Endpoint with Lock
```
POST /api/AIChat/GenerateSchedule 🔒
  Generate AI schedule (requires auth)
```

---

## 💡 Tips

### Quick Login
**Admin:**
```json
{"username":"adminuser","password":"adminpassword"}
```

**User:**
```json
{"username":"regularuser","password":"password123"}
```

### Token Format
**Correct:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Also Correct (Swagger auto adds "Bearer"):**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Wrong:**
```
Bearer: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Bearereyj...
"Bearer eyj..."
```

---

## 📝 Code Changes

### Program.cs

**Added:**
```csharp
// Swagger with JWT
builder.Services.AddSwaggerGen(options =>
{
    // Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {...});
    
    // Security Requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {...});
});

// Middleware
app.UseAuthentication();  // ← MUST be before UseAuthorization
app.UseAuthorization();
```

---

## ✅ Summary

**Changes:**
- ✅ Added Swagger JWT configuration
- ✅ Added "Authorize" button
- ✅ Auto include Authorization header
- ✅ Added `UseAuthentication()` middleware
- ✅ Build successful

**Now you can:**
- ✅ Login in Swagger
- ✅ Authorize with token
- ✅ Test protected endpoints
- ✅ Test role-based access (User vs Admin)
- ✅ See which endpoints require auth (🔒)

---

**Updated:** 02/02/2026  
**Build:** ✅ Success  
**Ready for:** API Testing

**Next:** Test với Postman hoặc Frontend! 🚀
