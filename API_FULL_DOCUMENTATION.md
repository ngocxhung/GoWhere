# 📚 API DOCUMENTATION - GOWHERE API

> **For Frontend Developers**  
> Base URL Development: `https://localhost:7001` hoặc `http://localhost:5191`

---

## ⚠️ QUAN TRỌNG - LƯU Ý

### 🚨 AI Chat Service - Có thể gặp lỗi

**Vấn đề:** Gemini API có thể trả về lỗi do:
- Gemini API key hết hạn hoặc vượt quota
- Gemini service tạm thời không khả dụng
- Network timeout

**Xử lý:**
```javascript
// Frontend nên có fallback UI
try {
  const response = await fetch('/api/AIChat/GenerateSchedule', {...});
  if (response.status === 503) {
    // Hiển thị: "Dịch vụ AI tạm thời không khả dụng, vui lòng thử lại sau"
  }
} catch (error) {
  // Hiển thị error message thân thiện
}
```

**Status Codes:**
- `402` - Hết lượt miễn phí, cần thanh toán
- `503` - AI service không khả dụng (Gemini API lỗi)
- `500` - Lỗi server khác

---

## 🔐 Authentication

### 1. Đăng ký tài khoản
```http
POST /api/Auth/Register
Content-Type: application/json

{
  "username": "string",
  "password": "string",
  "email": "string",
  "fullName": "string",
  "phoneNumber": "string"
}
```

**Response Success (200):**
```json
{
  "message": "User registered successfully"
}
```

**Response Error (400):**
```json
{
  "message": "Username already exists"
}
```

---

### 2. Đăng nhập
```http
POST /api/Auth/Login
Content-Type: application/json

{
  "username": "string",
  "password": "string"
}
```

**Response Success (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "regularuser",
  "email": "user@example.com",
  "fullName": "Nguyen Van A",
  "roleName": "User"
}
```

**Lưu token:** Sử dụng cho tất cả API cần authentication
```javascript
localStorage.setItem('token', response.token);
```

---

## 🤖 AI Chat Service

### ⚠️ Lưu ý quan trọng
- **Miễn phí:** 2 lần đầu tiên
- **Sau đó:** Yêu cầu thanh toán 50,000 VND
- **Có thể lỗi:** Status 503 nếu Gemini API down

---

### 1. Kiểm tra số lượt còn lại

```http
GET /api/AIChat/CheckUsageLimit
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "totalRequests": 0,
  "freeLimit": 2,
  "remainingFreeRequests": 2,
  "requiresPayment": false,
  "paymentAmount": 0,
  "message": "Bạn còn 2 lượt miễn phí."
}
```

**Khi hết lượt:**
```json
{
  "totalRequests": 2,
  "freeLimit": 2,
  "remainingFreeRequests": 0,
  "requiresPayment": true,
  "paymentAmount": 50000,
  "message": "Bạn đã hết lượt tạo lịch trình miễn phí. Vui lòng thanh toán 50,000 VND để tiếp tục."
}
```

---

### 2. Tạo lịch trình du lịch AI

```http
POST /api/AIChat/GenerateSchedule
Authorization: Bearer {token}
Content-Type: application/json

{
  "provinceId": 16,
  "budget": 5000000,
  "numberOfDays": 3,
  "travelStyle": "Nghỉ dưỡng",
  "companionType": "Gia đình",
  "additionalRequirements": "Thích ẩm thực địa phương"
}
```

**Request Body:**
- `provinceId` (number, required): ID tỉnh thành (1-63)
- `budget` (number, required): Ngân sách (VND)
- `numberOfDays` (number, required): Số ngày (1-30)
- `travelStyle` (string, required): "Nghỉ dưỡng", "Phiêu lưu", "Ẩm thực", v.v.
- `companionType` (string, required): "Một mình", "Cặp đôi", "Gia đình", "Bạn bè"
- `additionalRequirements` (string, optional): Yêu cầu thêm

**Response Success (200):**
```json
{
  "summary": "Lịch trình 3 ngày tại Hà Nội với ngân sách 5 triệu đồng...",
  "estimatedTotalCost": 4500000,
  "dailyItinerary": [
    {
      "day": 1,
      "date": "Ngày 1",
      "dayBudget": 1500000,
      "activities": [
        {
          "time": "08:00",
          "scenicSpot": {
            "scenicSpotId": 1,
            "name": "Hồ Hoàn Kiếm",
            "description": "Trái tim của thủ đô...",
            "estimatedVisitPrice": 0,
            "tips": "Đi buổi sáng sớm để không bị đông",
            "mediaUrls": ["/uploads/hanoi/hoankiem.jpg"]
          },
          "activity": "Tham quan Hồ Hoàn Kiếm",
          "description": "Đi dạo quanh hồ, chụp ảnh Tháp Rùa...",
          "estimatedCost": 0
        }
      ]
    }
  ],
  "remainingFreeRequests": 1,
  "requiresPayment": false
}
```

**Response Payment Required (402):**
```json
{
  "message": "Bạn đã hết lượt miễn phí. Vui lòng thanh toán 50,000 VND để tiếp tục.",
  "requiresPayment": true,
  "paymentAmount": 50000
}
```

**Response AI Service Error (503):**
```json
{
  "message": "AI service temporarily unavailable",
  "details": "Gemini API error: ..."
}
```

**Frontend handling:**
```javascript
const generateSchedule = async (data) => {
  try {
    const res = await fetch('/api/AIChat/GenerateSchedule', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });

    if (res.status === 402) {
      // Redirect to payment page
      navigate('/payment');
    } else if (res.status === 503) {
      // Show error: AI service unavailable
      showError('Dịch vụ AI tạm thời không khả dụng, vui lòng thử lại sau');
    } else if (res.ok) {
      const schedule = await res.json();
      // Display schedule
    }
  } catch (error) {
    showError('Đã có lỗi xảy ra');
  }
};
```

---

## 💳 Payment Service

### 1. Tạo đơn hàng thanh toán

```http
POST /api/Payment/CreateAIChatPayment
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "success": true,
  "message": "Đơn hàng đã được tạo. Vui lòng quét mã QR để thanh toán.",
  "data": {
    "orderId": 1,
    "totalPrice": 50000,
    "status": "Pending",
    "orderDate": "2026-02-02T10:30:00",
    "qrCodeUrl": "https://img.vietqr.io/image/970422-0123456789-compact2.png?amount=50000&addInfo=AICHAT1&accountName=NGUYEN VAN A",
    "paymentContent": "AICHAT1",
    "bankAccount": "0123456789",
    "bankName": "MB Bank (Ngân hàng TMCP Quân đội)"
  }
}
```

**Frontend display:**
```jsx
<div>
  <img src={data.qrCodeUrl} alt="QR Code" />
  <p>Ngân hàng: {data.bankName}</p>
  <p>Số TK: {data.bankAccount}</p>
  <p>Số tiền: {data.totalPrice.toLocaleString()} VND</p>
  <p>Nội dung: <strong>{data.paymentContent}</strong></p>
</div>
```

**Email notification:** User sẽ nhận email hướng dẫn thanh toán ngay sau khi gọi API này.

---

### 2. Xác nhận thanh toán (Admin only)

```http
POST /api/Payment/ConfirmPayment/{orderId}
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
{
  "success": true,
  "message": "Thanh toán đã được xác nhận thành công.",
  "data": true
}
```

**Email notification:** User sẽ nhận email chúc mừng thanh toán thành công.

---

### 3. Lịch sử thanh toán

```http
GET /api/Payment/PaymentHistory
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "success": true,
  "data": [
    {
      "orderId": 1,
      "totalPrice": 50000,
      "status": "Payed",
      "orderDate": "2026-02-02T10:30:00",
      "qrCodeUrl": "",
      "paymentContent": "Thanh toan AI Chat - Order #1",
      "bankAccount": "",
      "bankName": ""
    }
  ]
}
```

---

## 🗺️ Province (Tỉnh thành)

### 1. Lấy tất cả tỉnh thành

```http
GET /api/Province
```

**Response (200):**
```json
[
  {
    "provinceId": 1,
    "provinceName": "Lào Cai"
  },
  {
    "provinceId": 16,
    "provinceName": "Hà Nội"
  },
  // ... 63 tỉnh thành
]
```

**Frontend usage:**
```jsx
<select name="provinceId">
  {provinces.map(p => (
    <option key={p.provinceId} value={p.provinceId}>
      {p.provinceName}
    </option>
  ))}
</select>
```

---

### 2. Lấy tỉnh theo ID

```http
GET /api/Province/{id}
```

**Response (200):**
```json
{
  "provinceId": 16,
  "provinceName": "Hà Nội"
}
```

---

## 🏞️ Scenic Spot (Địa điểm du lịch)

### 1. Lấy tất cả địa điểm

```http
GET /api/ScenicSpot/GetAllScenicSpots
```

**Hỗ trợ OData:** Có thể filter, sort, paginate
```http
GET /api/ScenicSpot/GetAllScenicSpots?$filter=estimatedVisitPrice le 100000&$orderby=scenicSpotName
```

**Response (200):**
```json
[
  {
    "scenicSpotId": 1,
    "scenicSpotName": "Hồ Hoàn Kiếm",
    "description": "Trái tim của thủ đô...",
    "address": "Quận Hoàn Kiếm, Hà Nội",
    "estimatedVisitPrice": 0,
    "provinceId": 16,
    "provinceName": "Hà Nội",
    "medias": [
      {
        "mediaId": 1,
        "mediaUrl": "/uploads/hanoi/hoankiem.jpg"
      }
    ]
  }
]
```

---

### 2. Lấy địa điểm theo ID

```http
GET /api/ScenicSpot/GetScenicSpotById/{id}
```

**Response (200):**
```json
{
  "scenicSpotId": 1,
  "scenicSpotName": "Hồ Hoàn Kiếm",
  "description": "...",
  "estimatedVisitPrice": 0,
  "medias": [...]
}
```

---

### 3. Lấy địa điểm theo tỉnh

```http
GET /api/ScenicSpot/GetScenicSpotsByProvinceId/{provinceId}
```

**Response (200):**
```json
[
  {
    "scenicSpotId": 1,
    "scenicSpotName": "Hồ Hoàn Kiếm",
    "provinceId": 16
  },
  {
    "scenicSpotId": 2,
    "scenicSpotName": "Văn Miếu",
    "provinceId": 16
  }
]
```

---

### 4. Thêm địa điểm (Admin only)

```http
POST /api/ScenicSpot/AddScenicSpot
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

FormData:
- scenicSpotName: string
- description: string
- address: string
- estimatedVisitPrice: number
- provinceId: number
- images: File[] (optional)
```

**Response (201):**
```json
{
  "message": "Create ScenicSpot successful"
}
```

---

## 📅 Customer Schedule (Lịch trình của khách)

### 1. Lấy lịch trình của user hiện tại

```http
GET /api/CustomerSchedule/GetCustomerSchedulesByCustomerId
Authorization: Bearer {token}
```

**Response (200):**
```json
[
  {
    "scheduleId": 1,
    "scheduleName": "Du lịch Hà Nội",
    "scheduleDate": "2026-03-01T00:00:00",
    "description": "Lịch trình 3 ngày 2 đêm",
    "status": "Active",
    "customerId": 1
  }
]
```

---

### 2. Lấy tất cả lịch trình (Admin only)

```http
GET /api/CustomerSchedule/GetAllCustomerSchedules
Authorization: Bearer {admin_token}
```

---

### 3. Tạo lịch trình mới

```http
POST /api/CustomerSchedule/AddNewCustomerSchedule
Authorization: Bearer {token}
Content-Type: application/json

{
  "scheduleName": "Du lịch Hà Nội",
  "scheduleDate": "2026-03-01T00:00:00",
  "description": "Lịch trình 3 ngày 2 đêm"
}
```

**Response (201):**
```json
{
  "message": "Customer schedule created successfully."
}
```

---

### 4. Cập nhật trạng thái lịch trình

```http
PATCH /api/CustomerSchedule/UpdateCustomerScheduleStatus/{scheduleId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Completed"
}
```

---

## 📊 Status Codes Summary

| Code | Ý nghĩa | Khi nào xảy ra |
|------|---------|----------------|
| **200** | OK | Request thành công |
| **201** | Created | Tạo mới thành công |
| **400** | Bad Request | Dữ liệu không hợp lệ, ModelState invalid |
| **401** | Unauthorized | Chưa đăng nhập hoặc token sai |
| **402** | Payment Required | Hết lượt miễn phí AI Chat |
| **403** | Forbidden | Không có quyền (VD: User gọi Admin endpoint) |
| **404** | Not Found | Không tìm thấy resource |
| **500** | Internal Server Error | Lỗi server không xác định |
| **503** | Service Unavailable | AI service (Gemini) không khả dụng |

---

## 🔑 Authorization

### Header Format
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Lưu ý:** 
- Phải có từ `Bearer ` (có space) ở đầu
- Token lấy từ `/api/Auth/Login`

### JavaScript Example
```javascript
const token = localStorage.getItem('token');

fetch('/api/AIChat/CheckUsageLimit', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
```

---

## 👑 ADMIN MANAGEMENT

> **⚠️ Tất cả endpoints dưới đây yêu cầu RoleId = 2 (Admin)**

---

## 🏞️ Admin - Scenic Spot Management

### ⚡ Thay đổi quan trọng: IsViewed Status

**Giải thích:**
- Mỗi Scenic Spot có cột `IsViewed` (Boolean)
- `IsViewed = true`: Hiển thị cho User thông thường
- `IsViewed = false`: Chỉ Admin thấy (chưa duyệt)

**Workflow:**
1. Admin thêm địa điểm → Mặc định `IsViewed = false`
2. Admin duyệt → Toggle `IsViewed = true`
3. User bắt đầu thấy địa điểm

---

### 1. [ADMIN] Lấy TẤT CẢ scenic spots

```http
GET /api/ScenicSpot/Admin/GetAllScenicSpots
Authorization: Bearer {admin_token}
```

**Khác biệt với endpoint User:**
- User endpoint: Chỉ trả về `IsViewed = true`
- Admin endpoint: Trả về **TẤT CẢ** (bao gồm `IsViewed = false`)

**Response (200):**
```json
[
  {
    "id": 1,
    "scenicSpotName": "Hồ Hoàn Kiếm",
    "description": "...",
    "estimatedVisitPrice": 0,
    "mediaUrls": ["/uploads/hanoi/hoankiem.jpg"],
    "provinceId": 16,
    "isViewed": true  // ← Đã duyệt, user thấy được
  },
  {
    "id": 17,
    "scenicSpotName": "Địa điểm mới chưa duyệt",
    "description": "...",
    "estimatedVisitPrice": 50000,
    "mediaUrls": [],
    "provinceId": 16,
    "isViewed": false  // ← Chưa duyệt, chỉ admin thấy
  }
]
```

**Frontend UI:**
```jsx
<Table>
  {scenicSpots.map(spot => (
    <tr key={spot.id}>
      <td>{spot.scenicSpotName}</td>
      <td>
        <Badge color={spot.isViewed ? 'green' : 'red'}>
          {spot.isViewed ? 'Đã duyệt' : 'Chưa duyệt'}
        </Badge>
      </td>
      <td>
        <Button onClick={() => toggleStatus(spot.id)}>
          {spot.isViewed ? 'Ẩn' : 'Duyệt'}
        </Button>
      </td>
    </tr>
  ))}
</Table>
```

---

### 2. [ADMIN] Thêm scenic spot mới

```http
POST /api/ScenicSpot/AddScenicSpot
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

FormData:
- ScenicSpotName: "Tên địa điểm"
- Description: "Mô tả chi tiết"
- EstimatedVisitPrice: 100000
- ProvinceId: 16
- Medias: [file1.jpg, file2.jpg]  // Tối đa 250MB
```

**Lưu ý:**
- `IsViewed` tự động = `false` (chưa duyệt)
- User thông thường KHÔNG thấy địa điểm này ngay
- Admin cần duyệt bằng Toggle Status

**Response (201):**
```json
{
  "message": "Scenic spot created successfully. Status: Not viewed yet."
}
```

---

### 3. [ADMIN] Cập nhật scenic spot

```http
PUT /api/ScenicSpot/UpdateScenicSpot/{id}
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

FormData:
- ScenicSpotName: "Tên mới"
- Description: "Mô tả mới"
- EstimatedVisitPrice: 150000
- ProvinceId: 16
- Medias: [new_file.jpg]  // Optional
```

**Lưu ý:**
- Không thay đổi `IsViewed` status
- Chỉ update thông tin

**Response (200):**
```json
{
  "message": "Scenic spot updated successfully."
}
```

---

### 4. [ADMIN] Toggle hiển thị (Duyệt/Ẩn)

```http
PATCH /api/ScenicSpot/ToggleStatus/{id}
Authorization: Bearer {admin_token}
```

**Chức năng:**
- Nếu `IsViewed = false` → Chuyển thành `true` (Duyệt)
- Nếu `IsViewed = true` → Chuyển thành `false` (Ẩn)

**Use cases:**
- **Duyệt địa điểm mới:** Admin kiểm tra và bật hiển thị
- **Ẩn địa điểm:** Tạm thời không muốn user thấy (bảo trì, update...)

**Response (200):**
```json
{
  "message": "Scenic spot status toggled successfully."
}
```

**Frontend code:**
```javascript
const toggleStatus = async (spotId) => {
  const res = await fetch(`/api/ScenicSpot/ToggleStatus/${spotId}`, {
    method: 'PATCH',
    headers: {
      'Authorization': `Bearer ${adminToken}`
    }
  });
  
  if (res.ok) {
    // Refresh danh sách
    fetchScenicSpots();
    showSuccess('Trạng thái đã được cập nhật!');
  }
};
```

---

### 5. [ADMIN] Xóa scenic spot

```http
DELETE /api/ScenicSpot/DeleteScenicSpot/{id}
Authorization: Bearer {admin_token}
```

**Chức năng:**
- Xóa scenic spot khỏi database
- Tự động xóa các file media liên quan

**⚠️ Warning:** Không thể undo!

**Response (200):**
```json
{
  "message": "Scenic spot deleted successfully."
}
```

---

## 👥 Admin - User Management

### 1. [ADMIN] Lấy tất cả users

```http
GET /api/AdminUser/GetAllUsers
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
[
  {
    "userId": 1,
    "username": "regularuser",
    "email": "user@example.com",
    "fullName": "Nguyen Van A",
    "phoneNumber": "0123456789",
    "roleId": 1,
    "roleName": "User"
  },
  {
    "userId": 2,
    "username": "adminuser",
    "email": "admin@example.com",
    "fullName": "Admin User",
    "phoneNumber": "0987654321",
    "roleId": 2,
    "roleName": "Admin"
  }
]
```

**Frontend Table:**
```jsx
<UserTable>
  {users.map(user => (
    <tr key={user.userId}>
      <td>{user.username}</td>
      <td>{user.email}</td>
      <td>{user.fullName}</td>
      <td>
        <Badge>{user.roleName}</Badge>
      </td>
      <td>
        <ActionButtons userId={user.userId} />
      </td>
    </tr>
  ))}
</UserTable>
```

---

### 2. [ADMIN] Lấy user theo ID

```http
GET /api/AdminUser/GetUserById/{id}
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
{
  "userId": 1,
  "username": "regularuser",
  "email": "user@example.com",
  "fullName": "Nguyen Van A",
  "phoneNumber": "0123456789",
  "roleId": 1,
  "roleName": "User"
}
```

---

### 3. [ADMIN] Cập nhật user

```http
PUT /api/AdminUser/UpdateUser/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "email": "newemail@example.com",
  "fullName": "New Full Name",
  "phoneNumber": "0999999999",
  "roleId": 1
}
```

**Lưu ý:** Tất cả fields đều optional

**Response (200):**
```json
{
  "message": "User updated successfully."
}
```

---

### 4. [ADMIN] Đổi role (User ↔ Admin)

```http
PATCH /api/AdminUser/ChangeRole/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "roleId": 2  // 1 = User, 2 = Admin
}
```

**Use cases:**
- Promote user thành admin
- Demote admin về user

**Response (200):**
```json
{
  "message": "User role changed successfully to RoleId: 2"
}
```

**Frontend Toggle:**
```jsx
<button onClick={() => changeRole(user.userId, user.roleId === 1 ? 2 : 1)}>
  {user.roleId === 1 ? 'Promote to Admin' : 'Demote to User'}
</button>
```

---

## 💳 Admin - Order Management

### 1. [ADMIN] Lấy tất cả orders

```http
GET /api/AdminOrder/GetAllOrders
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
[
  {
    "orderId": 1,
    "userId": 1,
    "userName": "regularuser",
    "userEmail": "user@example.com",
    "totalPrice": 50000,
    "status": "Payed",
    "orderDate": "2026-02-02T10:00:00"
  },
  {
    "orderId": 2,
    "userId": 3,
    "userName": "anotheruser",
    "userEmail": "another@example.com",
    "totalPrice": 50000,
    "status": "Pending",
    "orderDate": "2026-02-02T11:00:00"
  }
]
```

**Frontend:**
```jsx
<OrdersTable>
  {orders.map(order => (
    <tr key={order.orderId}>
      <td>#{order.orderId}</td>
      <td>{order.userName}</td>
      <td>{order.totalPrice.toLocaleString()} VND</td>
      <td>
        <StatusBadge status={order.status} />
      </td>
      <td>{new Date(order.orderDate).toLocaleDateString()}</td>
    </tr>
  ))}
</OrdersTable>
```

---

### 2. [ADMIN] Lấy order theo ID

```http
GET /api/AdminOrder/GetOrderById/{id}
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
{
  "orderId": 1,
  "userId": 1,
  "userName": "regularuser",
  "userEmail": "user@example.com",
  "userPhone": "0123456789",
  "totalPrice": 50000,
  "status": "Payed",
  "orderDate": "2026-02-02T10:00:00",
  "items": [
    {
      "orderItemId": 1,
      "itemName": "AI Chat Service",
      "price": 50000,
      "quantity": 1
    }
  ]
}
```

---

### 3. [ADMIN] Lấy orders theo User ID

```http
GET /api/AdminOrder/GetOrdersByUserId/{userId}
Authorization: Bearer {admin_token}
```

**Use case:** Xem lịch sử mua hàng của một user cụ thể

---

### 4. [ADMIN] Lấy orders theo trạng thái

```http
GET /api/AdminOrder/GetOrdersByStatus/{status}
Authorization: Bearer {admin_token}
```

**Status values:**
- `Pending` - Chờ thanh toán
- `Payed` - Đã thanh toán
- `Cancelled` - Đã hủy

**Example:**
```http
GET /api/AdminOrder/GetOrdersByStatus/Pending
```

---

### 5. [ADMIN] Thống kê orders

```http
GET /api/AdminOrder/GetOrderStatistics
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
{
  "totalOrders": 100,
  "pendingOrders": 15,
  "payedOrders": 80,
  "totalRevenue": 4000000,
  "averageOrderValue": 50000,
  "ordersByDate": [
    {
      "date": "2026-02-02",
      "count": 10,
      "totalAmount": 500000
    },
    {
      "date": "2026-02-01",
      "count": 8,
      "totalAmount": 400000
    }
  ]
}
```

**Frontend Dashboard:**
```jsx
<Statistics>
  <Card title="Tổng đơn hàng" value={stats.totalOrders} />
  <Card title="Chờ thanh toán" value={stats.pendingOrders} color="yellow" />
  <Card title="Đã thanh toán" value={stats.payedOrders} color="green" />
  <Card title="Doanh thu" value={`${stats.totalRevenue.toLocaleString()} VND`} />
</Statistics>

<Chart data={stats.ordersByDate} type="line" />
```

---

### 6. [ADMIN] Hủy order

```http
PATCH /api/AdminOrder/CancelOrder/{id}
Authorization: Bearer {admin_token}
```

**Lưu ý:** 
- Chỉ có thể hủy order `Pending`
- Không thể hủy order `Payed`

**Response (200):**
```json
{
  "message": "Order cancelled successfully."
}
```

**Response Error (400):**
```json
{
  "message": "Cannot cancel an order that has been paid."
}
```

---

### 7. [ADMIN] Xóa order

```http
DELETE /api/AdminOrder/DeleteOrder/{id}
Authorization: Bearer {admin_token}
```

**⚠️ Warning:** Không thể undo!

**Response (200):**
```json
{
  "message": "Order deleted successfully."
}
```

---

## 📅 Admin - Schedule Management

### 1. [ADMIN] Lấy tất cả schedules

```http
GET /api/AdminSchedule/GetAllSchedules
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
[
  {
    "scheduleId": 1,
    "userId": 1,
    "startTime": "2026-03-01T08:00:00",
    "endTime": "2026-03-03T20:00:00",
    "status": "Active",
    "scenicSpots": [
      {
        "id": 1,
        "scenicSpotName": "Hồ Hoàn Kiếm",
        "description": "...",
        "estimatedVisitPrice": 0,
        "mediaUrls": [],
        "provinceId": 16,
        "isViewed": true
      }
    ],
    "feedback": null
  }
]
```

---

### 2. [ADMIN] Lấy schedule theo ID

```http
GET /api/AdminSchedule/GetScheduleById/{id}
Authorization: Bearer {admin_token}
```

---

### 3. [ADMIN] Lấy schedules theo User ID

```http
GET /api/AdminSchedule/GetSchedulesByUserId/{userId}
Authorization: Bearer {admin_token}
```

**Use case:** Xem tất cả lịch trình của một user cụ thể

---

### 4. [ADMIN] Thống kê schedules

```http
GET /api/AdminSchedule/GetScheduleStatistics
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
{
  "totalSchedules": 50,
  "activeSchedules": 20,
  "completedSchedules": 25,
  "cancelledSchedules": 5,
  "schedulesByUser": [
    {
      "userId": 1,
      "scheduleCount": 10
    },
    {
      "userId": 2,
      "scheduleCount": 8
    }
  ],
  "schedulesWithFeedback": 15
}
```

**Frontend Dashboard:**
```jsx
<ScheduleStats>
  <StatCard title="Tổng lịch trình" value={stats.totalSchedules} />
  <StatCard title="Đang hoạt động" value={stats.activeSchedules} color="blue" />
  <StatCard title="Đã hoàn thành" value={stats.completedSchedules} color="green" />
  <StatCard title="Đã hủy" value={stats.cancelledSchedules} color="red" />
</ScheduleStats>

<TopUsers data={stats.schedulesByUser} />
```

---

### 5. [ADMIN] Xóa schedule

```http
DELETE /api/AdminSchedule/DeleteSchedule/{id}
Authorization: Bearer {admin_token}
```

**Response (200):**
```json
{
  "message": "Schedule deleted successfully."
}
```

---

## 🎯 Business Rules

### AI Chat Service
1. **2 lượt miễn phí** cho mỗi user mới
2. Sau 2 lượt → Yêu cầu thanh toán 50,000 VND
3. Sau khi thanh toán → **Sử dụng không giới hạn**
4. Nếu AI lỗi → **Không trừ lượt** (fair policy)

### Payment Flow
```
Hết lượt miễn phí
  ↓
CreateAIChatPayment → QR Code + Email hướng dẫn
  ↓
User chuyển khoản (nội dung: AICHAT{orderId})
  ↓
Admin ConfirmPayment → Email chúc mừng
  ↓
Sử dụng AI không giới hạn ✅
```

---

## 🔒 Role-based Access

| Endpoint | User (RoleId=1) | Admin (RoleId=2) |
|----------|-----------------|------------------|
| **Auth/*** | ✅ | ✅ |
| **AIChat/*** | ✅ | ✅ |
| **Payment/*** | ✅ | ✅ |
| **Province/GET** | ✅ (public) | ✅ |
| **ScenicSpot/GetAll** | ✅ (IsViewed=true only) | ✅ (IsViewed=true only) |
| **ScenicSpot/Admin/GetAll** | ❌ | ✅ (All spots) |
| **ScenicSpot/POST,PUT,DELETE** | ❌ | ✅ Admin only |
| **ScenicSpot/ToggleStatus** | ❌ | ✅ Admin only |
| **CustomerSchedule/Own** | ✅ | ✅ |
| **CustomerSchedule/GetAll** | ❌ | ✅ Admin only |
| **AdminUser/*** | ❌ | ✅ Admin only |
| **AdminOrder/*** | ❌ | ✅ Admin only |
| **AdminSchedule/*** | ❌ | ✅ Admin only |

**Tổng Admin endpoints:** +17 endpoints mới

**Lưu ý:**
- Tất cả Admin endpoints yêu cầu: `[Authorize(Roles = "Admin")]`
- Nếu User (RoleId=1) gọi Admin endpoint → `403 Forbidden`

---

## ⚠️ Error Handling Best Practices

### Frontend Example

```javascript
const handleApiCall = async (url, options) => {
  try {
    const response = await fetch(url, options);
    
    // Success
    if (response.ok) {
      return await response.json();
    }
    
    // Handle specific errors
    switch (response.status) {
      case 401:
        // Redirect to login
        window.location.href = '/login';
        break;
        
      case 402:
        // Redirect to payment
        const data = await response.json();
        navigate('/payment', { state: { message: data.message } });
        break;
        
      case 403:
        showError('Bạn không có quyền truy cập');
        break;
        
      case 404:
        showError('Không tìm thấy dữ liệu');
        break;
        
      case 503:
        // AI service down
        showError('Dịch vụ AI tạm thời không khả dụng, vui lòng thử lại sau');
        break;
        
      default:
        showError('Đã có lỗi xảy ra, vui lòng thử lại');
    }
  } catch (error) {
    console.error(error);
    showError('Không thể kết nối đến server');
  }
};
```

---

## 📧 Email Notifications

### Tự động gửi email khi:

1. **Tạo đơn hàng** (`CreateAIChatPayment`)
   - Email: Hướng dẫn thanh toán
   - Chứa: QR code, thông tin chuyển khoản, hướng dẫn

2. **Xác nhận thanh toán** (`ConfirmPayment`)
   - Email: Chúc mừng thanh toán thành công
   - Chứa: Thông tin đơn hàng, quyền lợi không giới hạn

**Note:** Frontend không cần xử lý email, backend tự động gửi.

---

## 🌍 Base URLs

### Development
```
HTTPS: https://localhost:7001
HTTP:  http://localhost:5191
```

### Production
```
TBD (Chưa deploy)
```

---

## 🛠️ Testing với Swagger

```
URL: https://localhost:7001/swagger
```

**Steps:**
1. Gọi `/api/Auth/Login`
2. Copy token từ response
3. Click nút **Authorize** (góc phải)
4. Nhập: `Bearer {token}`
5. Test các API

---

## 📝 Sample Frontend Code

### React Hook cho AI Chat

```jsx
import { useState } from 'react';

const useAIChat = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  const checkUsageLimit = async () => {
    setLoading(true);
    try {
      const res = await fetch('/api/AIChat/CheckUsageLimit', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      
      if (!res.ok) throw new Error('Failed');
      return await res.json();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };
  
  const generateSchedule = async (data) => {
    setLoading(true);
    setError(null);
    
    try {
      const res = await fetch('/api/AIChat/GenerateSchedule', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
      });
      
      if (res.status === 402) {
        // Payment required
        return { requiresPayment: true };
      }
      
      if (res.status === 503) {
        throw new Error('Dịch vụ AI tạm thời không khả dụng');
      }
      
      if (!res.ok) {
        throw new Error('Failed to generate schedule');
      }
      
      return await res.json();
    } catch (err) {
      setError(err.message);
      return null;
    } finally {
      setLoading(false);
    }
  };
  
  return { checkUsageLimit, generateSchedule, loading, error };
};
```

---

## 🎯 Quick Reference

### Most Used Endpoints (User)

```javascript
// Login
POST /api/Auth/Login

// Check AI usage
GET /api/AIChat/CheckUsageLimit

// Generate schedule
POST /api/AIChat/GenerateSchedule

// Create payment
POST /api/Payment/CreateAIChatPayment

// Get provinces (for dropdown)
GET /api/Province

// Get scenic spots by province (IsViewed=true only)
GET /api/ScenicSpot/GetScenicSpotsByProvinceId/{provinceId}

// Get user's schedules
GET /api/CustomerSchedule/GetCustomerSchedulesByCustomerId
```

### Most Used Endpoints (Admin)

```javascript
// Admin - Scenic Spot Management
GET /api/ScenicSpot/Admin/GetAllScenicSpots  // All spots (bao gồm IsViewed=false)
PATCH /api/ScenicSpot/ToggleStatus/{id}      // Duyệt/Ẩn địa điểm
DELETE /api/ScenicSpot/DeleteScenicSpot/{id} // Xóa địa điểm

// Admin - User Management
GET /api/AdminUser/GetAllUsers               // Tất cả users
PATCH /api/AdminUser/ChangeRole/{id}         // Đổi role

// Admin - Order Management
GET /api/AdminOrder/GetAllOrders             // Tất cả đơn hàng
GET /api/AdminOrder/GetOrderStatistics       // Thống kê
PATCH /api/AdminOrder/CancelOrder/{id}       // Hủy đơn

// Admin - Schedule Management
GET /api/AdminSchedule/GetAllSchedules       // Tất cả lịch trình
GET /api/AdminSchedule/GetScheduleStatistics // Thống kê
```

---

## 📚 Additional Resources

- **Admin Management Guide:** `ADMIN_MANAGEMENT_GUIDE.md`
- **Admin Features Summary:** `ADMIN_FEATURES_SUMMARY.md`
- **VietQR Config:** `VIETQR_CONFIG_GUIDE.md`
- **Payment Guide:** `PAYMENT_INTEGRATION_GUIDE.md`
- **Test Examples:** `API_TEST_EXAMPLES.md`
- **Email Config:** `EMAIL_CONFIGURATION_GUIDE.md`

---

**Version:** 3.0  
**Last Updated:** 02/02/2026  
**For:** Frontend Team  
**Contact:** Backend Team

**🚨 Remember:** 
- AI Chat có thể trả về 503 nếu Gemini API lỗi!
- Admin endpoints yêu cầu RoleId = 2
- User chỉ thấy ScenicSpots có IsViewed = true
