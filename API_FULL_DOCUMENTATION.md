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

| Endpoint | User | Admin |
|----------|------|-------|
| **Auth/** | ✅ | ✅ |
| **AIChat/** | ✅ | ✅ |
| **Payment/** | ✅ | ✅ |
| **Province/GET** | ✅ (public) | ✅ |
| **ScenicSpot/GET** | ✅ (public) | ✅ |
| **ScenicSpot/POST** | ❌ | ✅ Admin only |
| **CustomerSchedule/Own** | ✅ | ✅ |
| **CustomerSchedule/All** | ❌ | ✅ Admin only |

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

### Most Used Endpoints

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

// Get scenic spots by province
GET /api/ScenicSpot/GetScenicSpotsByProvinceId/{provinceId}

// Get user's schedules
GET /api/CustomerSchedule/GetCustomerSchedulesByCustomerId
```

---

## 📚 Additional Resources

- **VietQR Config:** `VIETQR_CONFIG_GUIDE.md`
- **Payment Guide:** `PAYMENT_INTEGRATION_GUIDE.md`
- **Test Examples:** `API_TEST_EXAMPLES.md`
- **Email Config:** `EMAIL_CONFIGURATION_GUIDE.md`

---

**Version:** 2.0  
**Last Updated:** 02/02/2026  
**For:** Frontend Team  
**Contact:** Backend Team

**🚨 Remember:** AI Chat có thể trả về 503 nếu Gemini API lỗi!
