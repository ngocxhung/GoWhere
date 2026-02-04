# GoWhere AI Features - Hướng dẫn sử dụng

## 🎯 Tổng quan

GoWhere AI cung cấp 2 tính năng AI chính:

### 1. **AI Chat** - Trợ lý du lịch thông minh
- Chat trực tiếp với AI về bất kỳ câu hỏi nào liên quan đến du lịch Việt Nam
- Hỏi về địa điểm, ẩm thực, văn hóa, lịch sử, tips du lịch...
- AI sẽ trả lời dựa trên kiến thức về du lịch Việt Nam
- **Không giới hạn số lần chat**

### 2. **AI Schedule Generator** - Tạo lịch trình tự động
- Nhập điểm đến, ngân sách, số ngày, phong cách du lịch
- AI sẽ tự động tạo lịch trình chi tiết theo từng ngày
- Bao gồm: địa điểm, thời gian, chi phí dự kiến, tips
- **2 lần miễn phí, từ lần 3 cần thanh toán 50,000 VND**

## 🚀 Cách sử dụng

### Truy cập trang AI
```
http://localhost:3000/ai
```

### AI Chat
1. Nhập câu hỏi vào ô chat
2. Nhấn Enter hoặc click nút gửi
3. Đợi AI trả lời (thường < 3 giây)
4. Tiếp tục hội thoại

**Ví dụ câu hỏi:**
- "Những địa điểm nổi tiếng ở Đà Nẵng là gì?"
- "Món ăn đặc sản Hội An có gì?"
- "Nên đi Phú Quốc vào mùa nào?"
- "Chi phí du lịch Sapa 3 ngày 2 đêm khoảng bao nhiêu?"

### AI Schedule Generator
1. **Nhập thông tin:**
   - Điểm đến: Tên tỉnh/thành phố (VD: Đà Nẵng, Phú Quốc)
   - Ngân sách: Tổng ngân sách cho chuyến đi
   - Số ngày: 1-10 ngày
   - Phong cách: Nghỉ dưỡng / Năng động / Ẩm thực / Văn hóa
   - Đi cùng: Một mình / Cặp đôi / Bạn bè / Gia đình

2. **Click "TẠO LỊCH TRÌNH NGAY"**

3. **Đợi AI xử lý** (10-30 giây)

4. **Xem kết quả:**
   - Tóm tắt lịch trình
   - Chi phí dự kiến
   - Lịch trình chi tiết theo từng ngày
   - Các hoạt động, địa điểm, thời gian, chi phí

## 💡 Lưu ý quan trọng

### Yêu cầu
- ✅ Phải đăng nhập để sử dụng tính năng tạo lịch trình
- ✅ Không cần đăng nhập để chat với AI
- ✅ Cần kết nối internet

### Giới hạn sử dụng
- **AI Chat**: Không giới hạn
- **AI Schedule**: 
  - 2 lần đầu: **MIỄN PHÍ**
  - Từ lần 3: **50,000 VND/lần**

### Tips để có kết quả tốt nhất

#### Khi chat:
- Hỏi câu hỏi cụ thể, rõ ràng
- Có thể hỏi nhiều câu liên tiếp
- AI sẽ nhớ ngữ cảnh cuộc trò chuyện

#### Khi tạo lịch trình:
- Nhập tên điểm đến chính xác (VD: "Đà Nẵng" thay vì "Da Nang")
- Ngân sách nên phù hợp với số ngày (tối thiểu 500k/ngày)
- Chọn phong cách phù hợp với sở thích
- AI sẽ tối ưu lịch trình dựa trên:
  - Ngân sách của bạn
  - Số ngày
  - Phong cách du lịch
  - Số người đi cùng

## 🔧 Cấu hình Backend

### API Endpoints

```
POST /api/AIChat/Chat
- Body: { message: string, conversationId?: string }
- Response: { response: string, conversationId: string, timestamp: datetime }

POST /api/AIChat/GenerateSchedule
- Headers: Authorization: Bearer {token}
- Body: {
    provinceId: number,
    userLocation: string,
    destination: string,
    pax: number,
    budget: number,
    numberOfDays: number,
    travelStyle: string,
    companionType: string,
    additionalRequirements?: string
  }
- Response: {
    summary: string,
    estimatedTotalCost: number,
    dailyItinerary: [...],
    remainingFreeRequests: number,
    requiresPayment: boolean
  }

GET /api/AIChat/CheckUsageLimit
- Headers: Authorization: Bearer {token}
- Response: {
    totalRequests: number,
    freeLimit: number,
    remainingFreeRequests: number,
    requiresPayment: boolean,
    paymentAmount: number,
    message: string
  }
```

### Gemini API Configuration

File: `backend/appsettings.json`

```json
{
  "GeminiAPI": {
    "ApiKey": "YOUR_GEMINI_API_KEY"
  },
  "AISettings": {
    "FreeRequestLimit": 2,
    "PaymentPrice": 50000
  }
}
```

## 🎨 Giao diện

### Màu sắc chủ đạo
- Primary: Gradient Purple (#667eea → #764ba2)
- Success: Green (#10b981)
- Background: Light gradient (#f8f9ff → #ffffff)

### Responsive
- Desktop: 2 cột (Chat | Schedule)
- Mobile: 1 cột (stack)

## 📱 Demo Screenshots

### AI Chat
- Giao diện chat hiện đại với gradient purple
- Typing indicator khi AI đang suy nghĩ
- Auto-scroll đến tin nhắn mới nhất
- Hỗ trợ Enter để gửi tin nhắn

### AI Schedule Generator
- Form nhập liệu trực quan
- Loading state khi AI đang tạo lịch trình
- Hiển thị kết quả theo từng ngày
- Chi tiết hoạt động, thời gian, chi phí
- Tips hữu ích cho từng địa điểm

## 🐛 Troubleshooting

### Lỗi "Không thể kết nối đến AI"
- Kiểm tra backend đang chạy (port 5191)
- Kiểm tra Gemini API key trong appsettings.json
- Kiểm tra kết nối internet

### Lỗi "Không tìm thấy tỉnh/thành phố"
- Nhập tên chính xác (VD: "Đà Nẵng", "Hà Nội")
- Thử tên khác (VD: "Danang" → "Đà Nẵng")
- Kiểm tra database có dữ liệu province

### Lỗi "Bạn đã hết lượt miễn phí"
- Đã sử dụng hết 2 lần miễn phí
- Cần thanh toán 50,000 VND để tiếp tục
- Hoặc tạo tài khoản mới (không khuyến khích)

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra console log (F12)
2. Kiểm tra backend terminal
3. Đọc lại hướng dẫn
4. Liên hệ team support

---

**Chúc bạn có trải nghiệm tuyệt vời với GoWhere AI! 🚀✨**
