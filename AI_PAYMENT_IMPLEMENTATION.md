# AI Payment Feature Implementation Summary

## Tổng quan
Đã implement tính năng thanh toán 50,000 VND cho unlimited AI schedule generation sau khi người dùng hết 2 lượt miễn phí.

## Backend Changes

### 1. AIChatController.cs
**Endpoint mới:** `POST /api/AIChat/CreateAIPaymentOrder`
- Tạo order thanh toán 50k VND cho AI unlimited
- Kiểm tra nếu user đã thanh toán rồi
- Tạo OrderItem với ServiceType = "AI"
- Link order với AISuggestionRequest của user
- Return OrderId và Amount để frontend tạo QR code

**Logic:**
```csharp
- Check if already paid (Order.Status == "Payed") → Return error
- Check if already has pending order (Order.Status == "WaitPayment") → Return existing order
- Create new Order with Status = "WaitPayment", TotalPrice = 50000
- Create OrderItem: "AI Unlimited - Tạo lịch trình không giới hạn"
- Link AISuggestionRequest.OrderId to new Order
```

## Frontend Changes

### 1. State Management (ai/page.tsx)
Thêm các state mới:
```typescript
const [showPaymentModal, setShowPaymentModal] = useState(false);
const [paymentQR, setPaymentQR] = useState("");
const [paymentDetails, setPaymentDetails] = useState<any>(null);
const [paymentOrder, setPaymentOrder] = useState<any>(null);
```

### 2. Payment Handler Function
**Function:** `handleAIPayment()`
- Gọi API `GetPaymentInfo` để lấy thông tin ngân hàng
- Gọi API `CreateAIPaymentOrder` để tạo order
- Generate VietQR URL với format:
  ```
  https://img.vietqr.io/image/{bankBin}-{accountNo}-{template}.png
    ?amount={amount}
    &addInfo=GOWHERE AI {orderId}
    &accountName={accountName}
  ```
- Hiển thị payment modal

### 3. UI Updates

#### Usage Banner
- **Trước:** Chỉ hiển thị số lượt còn lại
- **Sau:** 
  - Khi `requiresPayment = false`: Banner màu xanh, hiển thị số lượt còn lại
  - Khi `requiresPayment = true`: 
    - Banner màu cam (warning)
    - Hiển thị message: "Bạn đã hết lượt tạo lịch trình miễn phí. Thanh toán 50,000 VND để sử dụng không giới hạn!"
    - Nút "💳 Thanh toán ngay" bên phải

#### Payment Modal
Modal hiển thị:
- QR code VietQR để quét thanh toán
- Mã đơn hàng: #AI-{orderId}
- Số tiền: 50,000 VND (màu cam)
- Nội dung chuyển khoản: GOWHERE AI {orderId}
- Chủ tài khoản
- Notice: "Sau khi thanh toán thành công, bạn có thể tạo lịch trình không giới hạn!"
- Button: "Tôi đã thanh toán xong" → Đóng modal và refresh usage info

### 4. CSS Styling
```css
.usage-banner.payment-required {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  box-shadow: 0 4px 20px rgba(245, 158, 11, 0.3);
}

.payment-btn {
  background: white;
  color: #f59e0b;
  padding: 12px 24px;
  border-radius: 12px;
  font-weight: 700;
}

.payment-modal {
  max-width: 440px;
  border-radius: 24px;
  animation: zoomIn 0.3s;
}
```

## User Flow

1. **Lần đầu sử dụng:** User có 2 lượt miễn phí
   - Banner màu xanh: "Bạn còn 2 lượt tạo lịch trình miễn phí"

2. **Sau khi dùng hết 2 lượt:**
   - Banner chuyển màu cam
   - Hiển thị message cần thanh toán 50k
   - Nút "Thanh toán ngay" xuất hiện

3. **Click "Thanh toán ngay":**
   - Modal mở ra với QR code
   - User quét QR bằng app ngân hàng
   - Chuyển khoản 50,000 VND với nội dung "GOWHERE AI {orderId}"

4. **Sau khi thanh toán:**
   - Admin confirm payment → Update Order.Status = "Payed"
   - User có thể tạo lịch trình unlimited
   - Banner không còn hiển thị warning

## Database Schema

### AISuggestionRequest
```
- RequestId (PK)
- UserId (FK)
- Count (số lần đã dùng)
- Price (50000)
- OrderId (FK) → Link to Order khi thanh toán
```

### Order (AI Payment)
```
- OrderId (PK)
- UserId (FK)
- Status: "WaitPayment" → "Payed"
- TotalPrice: 50000
- OrderItems:
  - ItemName: "AI Unlimited - Tạo lịch trình không giới hạn"
  - ServiceType: "AI"
  - Price: 50000
  - Quantity: 1
```

## Testing Checklist

- [ ] User chưa dùng AI → Banner hiển thị "2 lượt miễn phí"
- [ ] User dùng 1 lần → Banner hiển thị "1 lượt miễn phí"
- [ ] User dùng 2 lần → Banner chuyển màu cam + nút thanh toán
- [ ] Click "Thanh toán ngay" → Modal hiển thị QR code
- [ ] QR code có đúng thông tin (amount, orderId, accountName)
- [ ] Click "Tôi đã thanh toán xong" → Modal đóng, refresh usage info
- [ ] Sau khi admin confirm payment → User có thể tạo unlimited

## Notes

- Payment confirmation vẫn cần admin manual confirm (update Order.Status = "Payed")
- Có thể integrate webhook từ bank để auto-confirm trong tương lai
- VietQR template được config trong appsettings.json
