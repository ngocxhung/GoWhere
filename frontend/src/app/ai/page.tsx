"use client";

import React, { useState, useEffect } from 'react';
import {
    RobotOutlined,
    CalendarOutlined,
    SendOutlined,
    DollarOutlined,
    TeamOutlined,
    ClockCircleOutlined,
    EnvironmentOutlined,
    CheckCircleFilled,
    LoadingOutlined,
    ArrowRightOutlined
} from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

type Province = { provinceId: number; provinceName: string };

export default function AIPage() {
    // Chat state
    const [chatMessages, setChatMessages] = useState<{ text: string, type: 'bot' | 'user' }[]>([
        { text: "Xin chào! Tôi là trợ lý du lịch AI của GoWhere. Bạn muốn hỏi gì về du lịch Việt Nam?", type: 'bot' }
    ]);
    const [chatInput, setChatInput] = useState('');
    const [chatLoading, setChatLoading] = useState(false);
    const [conversationId, setConversationId] = useState('');

    // Schedule state
    const [scheduleForm, setScheduleForm] = useState({
        destination: '',
        budget: 5000000,
        days: 3,
        style: 'dynamic',
        group: 'friends',
        pax: 2
    });
    const [provinces, setProvinces] = useState<Province[]>([]);
    const [scheduleResult, setScheduleResult] = useState<any>(null);
    const [activeDay, setActiveDay] = useState(0);
    const [scheduleLoading, setScheduleLoading] = useState(false);
    const [usageInfo, setUsageInfo] = useState<any>(null);
    const [showPaymentModal, setShowPaymentModal] = useState(false);
    const [paymentQR, setPaymentQR] = useState("");
    const [paymentDetails, setPaymentDetails] = useState<any>(null);
    const [paymentOrder, setPaymentOrder] = useState<any>(null);
    const [receiptUploaded, setReceiptUploaded] = useState(false);
    const [receiptUploading, setReceiptUploading] = useState(false);
    const [receiptError, setReceiptError] = useState<string | null>(null);
    const [lightboxImage, setLightboxImage] = useState<string | null>(null);

    const chatEndRef = React.useRef<HTMLDivElement>(null);

    const closePaymentModal = () => {
        setShowPaymentModal(false);
        setReceiptUploaded(false);
        setReceiptError(null);
    };

    React.useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [chatMessages]);

    React.useEffect(() => {
        checkUsageLimit();
        fetchProvinces();
    }, []);

    const fetchProvinces = async () => {
        try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Province/GetAllProvinces`);
            if (res.ok) {
                const data = await res.json();
                if (Array.isArray(data)) setProvinces(data);
            }
        } catch (error) {
            console.error('Error fetching provinces:', error);
        }
    };

    const checkUsageLimit = async () => {
        const token = localStorage.getItem('token');
        if (!token) return;

        try {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AIChat/CheckUsageLimit`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                setUsageInfo(data);
            }
        } catch (error) {
            console.error('Failed to check usage limit:', error);
        }
    };

    const sendChatMessage = async () => {
        if (!chatInput.trim() || chatLoading) return;

        const userMessage = chatInput.trim();
        setChatMessages(prev => [...prev, { text: userMessage, type: 'user' }]);
        setChatInput('');
        setChatLoading(true);

        try {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AIChat/Chat`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    message: userMessage,
                    conversationId: conversationId || undefined
                })
            });

            if (response.ok) {
                const data = await response.json();
                setChatMessages(prev => [...prev, { text: data.response, type: 'bot' }]);
                if (data.conversationId && !conversationId) {
                    setConversationId(data.conversationId);
                }
            } else {
                setChatMessages(prev => [...prev, { text: "Xin lỗi, có lỗi xảy ra. Vui lòng thử lại.", type: 'bot' }]);
            }
        } catch (error) {
            setChatMessages(prev => [...prev, { text: "Không thể kết nối đến AI. Vui lòng kiểm tra kết nối.", type: 'bot' }]);
        } finally {
            setChatLoading(false);
        }
    };

    const generateSchedule = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Vui lòng đăng nhập để sử dụng tính năng này');
            return;
        }

        if (!scheduleForm.destination.trim()) {
            alert('Vui lòng chọn điểm đến!');
            return;
        }

        setScheduleLoading(true);
        setScheduleResult(null);

        try {
            // Find provinceId from destination name
            const province = provinces.find(p => p.provinceName === scheduleForm.destination);
            const provinceId = province?.provinceId || 0;

            let userLocation = "Hà Nội, Việt Nam";
            if (navigator.geolocation) {
                try {
                    const pos = await new Promise<GeolocationPosition>((res, rej) =>
                        navigator.geolocation.getCurrentPosition(res, rej)
                    );
                    userLocation = `${pos.coords.latitude},${pos.coords.longitude}`;
                } catch (e) {
                    console.log("Using default location");
                }
            }

            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AIChat/GenerateSchedule`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    provinceId: provinceId,
                    userLocation: userLocation,
                    destination: scheduleForm.destination,
                    pax: scheduleForm.pax,
                    budget: scheduleForm.budget,
                    numberOfDays: scheduleForm.days,
                    travelStyle: scheduleForm.style,
                    companionType: scheduleForm.group,
                    additionalRequirements: ""
                })
            });

            if (response.ok) {
                const data = await response.json();
                setScheduleResult(data);
                setActiveDay(0);
                await checkUsageLimit();
            } else {
                const error = await response.json();
                // Nếu hết lượt, hiển thị modal thanh toán
                if (error.requiresPayment || error.message?.includes('hết lượt') || error.message?.includes('limit')) {
                    await handleAIPayment();
                } else {
                    alert(error.message || 'Có lỗi xảy ra khi tạo lịch trình');
                }
            }
        } catch (error) {
            console.error('Schedule generation error:', error);
            alert('Không thể kết nối đến server');
        } finally {
            setScheduleLoading(false);
        }
    };

    const handleAIPayment = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            alert('Vui lòng đăng nhập để thanh toán');
            return;
        }

        try {
            // 1. Get Payment Info
            const payInfoRes = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/GetPaymentInfo`);
            const payInfo = await payInfoRes.json();
            setPaymentDetails(payInfo);

            // 2. Create AI Payment Order
            const orderRes = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AIChat/CreateAIPaymentOrder`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (orderRes.ok) {
                const orderData = await orderRes.json();
                const orderId = orderData.orderId;
                const amount = orderData.amount;

                // 3. Generate VietQR URL
                const qrUrl = `https://img.vietqr.io/image/${payInfo.bankBin}-${payInfo.accountNo}-${payInfo.template}.png?amount=${amount}&addInfo=${encodeURIComponent("GOWHERE AI " + orderId)}&accountName=${encodeURIComponent(payInfo.accountName)}`;

                setPaymentQR(qrUrl);
                setPaymentOrder({ id: orderId, total: amount });
                setReceiptUploaded(false);
                setReceiptError(null);
                setShowPaymentModal(true);
            } else {
                const error = await orderRes.json();
                alert(error.message || 'Có lỗi xảy ra khi tạo đơn hàng');
            }
        } catch (error) {
            console.error('Payment error:', error);
            alert('Không thể kết nối đến server');
        }
    };

    const handleReceiptUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        const orderId = paymentOrder?.id;
        if (!file || !orderId) return;

        const token = localStorage.getItem('token');
        if (!token) return;

        setReceiptUploading(true);
        setReceiptError(null);
        try {
            const formData = new FormData();
            formData.append('file', file);
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/UploadReceipt/${orderId}`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}` },
                body: formData
            });
            const data = await res.json().catch(() => ({}));
            if (!res.ok) {
                setReceiptError(data.message || 'Upload thất bại');
                return;
            }
            setReceiptUploaded(true);
        } catch {
            setReceiptError('Lỗi kết nối');
        } finally {
            setReceiptUploading(false);
            e.target.value = '';
        }
    };

    return (
        <main className="ai-page">
            <div className="ai-hero">
                <div className="container">
                    <h1><RobotOutlined /> Trợ lý AI du lịch thông minh</h1>
                    <p>Trải nghiệm sức mạnh của AI trong việc lên kế hoạch chuyến đi hoàn hảo</p>
                </div>
            </div>

            <div className="container">
                {usageInfo && (
                    <div className={`usage-banner ${usageInfo.requiresPayment ? 'payment-required' : ''}`}>
                        <div className="usage-info">
                            <CheckCircleFilled style={{ color: usageInfo.requiresPayment ? '#f59e0b' : '#10b981', fontSize: '20px' }} />
                            <span>
                                {usageInfo.message || (usageInfo.requiresPayment ? (
                                    <>
                                        Bạn đã hết lượt tạo lịch trình. Thanh toán <strong>{money(usageInfo.paymentAmount)} VND</strong> để nhận thêm 5 lượt!
                                    </>
                                ) : (
                                    <>
                                        Bạn còn <strong>{usageInfo.remainingFreeRequests}</strong> lượt tạo lịch trình.
                                    </>
                                ))}
                            </span>
                        </div>
                        {usageInfo.requiresPayment && (
                            <button className="payment-btn" onClick={handleAIPayment}>
                                💳 Thanh toán ngay
                            </button>
                        )}
                    </div>
                )}

                {/* Payment Modal */}
                {showPaymentModal && paymentOrder && (
                    <div className="payment-modal-overlay" onClick={closePaymentModal}>
                        <div className="payment-modal" onClick={(e) => e.stopPropagation()}>
                            <div className="pay-header">
                                <h3>Xác nhận thanh toán Gói AI (5 lượt)</h3>
                                <button className="btn-close" onClick={closePaymentModal}>✕</button>
                            </div>
                            <div className="pay-body">
                                <div className="qr-box">
                                    <img src={paymentQR} alt="VietQR Payment" />
                                    <div className="qr-hint">Sử dụng ứng dụng Ngân hàng hoặc Ví điện tử để quét mã QR</div>
                                </div>
                                <div className="pay-info-summary">
                                    <div className="p-row"><span>Mã đơn hàng:</span> <strong>#AI-{paymentOrder.id}</strong></div>
                                    <div className="p-row"><span>Số tiền:</span> <strong className="p-amount">{money(paymentOrder.total)} VND</strong></div>
                                    <div className="p-row"><span>Nội dung:</span> <strong>GOWHERE AI {paymentOrder.id}</strong></div>
                                    <div className="p-row"><span>Chủ tài khoản:</span> <strong>{paymentDetails?.accountName}</strong></div>
                                </div>
                                <div className="receipt-upload-section">
                                    <label className="receipt-upload-label">
                                        <input
                                            type="file"
                                            accept="image/jpeg,image/jpg,image/png"
                                            onChange={handleReceiptUpload}
                                            disabled={receiptUploading || receiptUploaded}
                                        />
                                        {receiptUploaded ? '✓ Đã tải ảnh chứng từ' : receiptUploading ? 'Đang tải...' : 'Chọn ảnh chứng từ chuyển khoản'}
                                    </label>
                                    {receiptError && <span className="receipt-error">{receiptError}</span>}
                                </div>
                                <div className="pay-notice">
                                    <CheckCircleFilled /> Sau khi thanh toán thành công, bạn sẽ có thêm 5 lượt tạo lịch trình AI!
                                </div>
                            </div>
                            <div className="pay-footer">
                                <button
                                    className="btn-primary full"
                                    disabled={!receiptUploaded}
                                    onClick={() => {
                                        closePaymentModal();
                                        checkUsageLimit();
                                    }}
                                >
                                    Tôi đã thanh toán xong
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                <div className="ai-grid">
                    {/* AI Chat Section */}
                    <div className="ai-card chat-card">
                        <div className="card-header">
                            <RobotOutlined style={{ fontSize: '24px' }} />
                            <h2>Chat với AI</h2>
                        </div>
                        <div className="card-desc">
                            Hỏi bất cứ điều gì về du lịch Việt Nam - địa điểm, ẩm thực, văn hóa, lịch sử...
                        </div>

                        <div className="chat-messages">
                            {chatMessages.map((msg, i) => (
                                <div key={i} className={`chat-msg ${msg.type}`}>
                                    <MessageContent text={msg.text} />
                                </div>
                            ))}
                            {chatLoading && (
                                <div className="chat-msg bot typing">
                                    <span></span><span></span><span></span>
                                </div>
                            )}
                            <div ref={chatEndRef} />
                        </div>

                        <div className="chat-input-area">
                            <input
                                type="text"
                                value={chatInput}
                                onChange={(e) => setChatInput(e.target.value)}
                                onKeyPress={(e) => e.key === 'Enter' && sendChatMessage()}
                                placeholder="Nhập câu hỏi của bạn..."
                                disabled={chatLoading}
                            />
                            <button
                                onClick={sendChatMessage}
                                disabled={chatLoading || !chatInput.trim()}
                                className="send-btn"
                            >
                                {chatLoading ? <LoadingOutlined /> : <SendOutlined />}
                            </button>
                        </div>
                    </div>

                    {/* AI Schedule Generator Section */}
                    <div className="ai-card schedule-card">
                        <div className="card-header">
                            <CalendarOutlined style={{ fontSize: '24px' }} />
                            <h2>Tạo lịch trình AI</h2>
                        </div>
                        <div className="card-desc">
                            Để AI tạo lịch trình du lịch hoàn hảo dựa trên sở thích và ngân sách của bạn
                        </div>

                        <div className="schedule-form">
                            <div className="form-group">
                                <label><EnvironmentOutlined /> Điểm đến</label>
                                <CustomSelect
                                    value={scheduleForm.destination}
                                    options={provinces.length > 0
                                        ? provinces.map(p => ({ label: p.provinceName, value: p.provinceName }))
                                        : [{ label: 'Đang tải...', value: '' }]
                                    }
                                    onChange={(v) => setScheduleForm({ ...scheduleForm, destination: v })}
                                />
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label><DollarOutlined /> Ngân sách (VND)</label>
                                    <input
                                        type="number"
                                        value={scheduleForm.budget}
                                        onChange={(e) => setScheduleForm({ ...scheduleForm, budget: parseInt(e.target.value) })}
                                        min="500000"
                                        step="100000"
                                    />
                                </div>

                                <div className="form-group">
                                    <label><ClockCircleOutlined /> Số ngày</label>
                                    <select
                                        value={scheduleForm.days}
                                        onChange={(e) => setScheduleForm({ ...scheduleForm, days: parseInt(e.target.value) })}
                                    >
                                        {[1, 2, 3, 4, 5, 7, 10].map(d => (
                                            <option key={d} value={d}>{d} ngày</option>
                                        ))}
                                    </select>
                                </div>
                            </div>

                            <div className="form-row">
                                <div className="form-group">
                                    <label>Phong cách</label>
                                    <select
                                        value={scheduleForm.style}
                                        onChange={(e) => setScheduleForm({ ...scheduleForm, style: e.target.value })}
                                    >
                                        <option value="lazy">Nghỉ dưỡng</option>
                                        <option value="dynamic">Năng động</option>
                                        <option value="foodie">Ẩm thực</option>
                                        <option value="culture">Văn hóa</option>
                                    </select>
                                </div>

                                <div className="form-group">
                                    <label><TeamOutlined /> Đi cùng</label>
                                    <select
                                        value={scheduleForm.group}
                                        onChange={(e) => {
                                            const group = e.target.value;
                                            let pax = 2;
                                            if (group === 'single') pax = 1;
                                            if (group === 'couple') pax = 2;
                                            if (group === 'friends') pax = 4;
                                            if (group === 'family') pax = 4;
                                            setScheduleForm({ ...scheduleForm, group, pax });
                                        }}
                                    >
                                        <option value="single">Một mình</option>
                                        <option value="couple">Cặp đôi</option>
                                        <option value="friends">Bạn bè</option>
                                        <option value="family">Gia đình</option>
                                    </select>
                                </div>
                            </div>

                            <button
                                className="generate-btn"
                                onClick={generateSchedule}
                                disabled={scheduleLoading}
                            >
                                {scheduleLoading ? (
                                    <><LoadingOutlined /> ĐANG TẠO LỊCH TRÌNH...</>
                                ) : (
                                    <><CalendarOutlined /> TẠO LỊCH TRÌNH AI</>
                                )}
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Schedule Result Modal */}
            {scheduleResult && (
                <div className="modal-overlay" onClick={() => setScheduleResult(null)}>
                    <div className="modal-container" onClick={(e) => e.stopPropagation()}>
                        <button className="modal-close" onClick={() => setScheduleResult(null)}>
                            ✕
                        </button>

                        <div className="modal-header">
                            <CalendarOutlined style={{ fontSize: '28px' }} />
                            <h2>Lịch trình của bạn</h2>
                        </div>

                        <div className="modal-body">
                            <div className="result-summary">{scheduleResult.summary}</div>

                            <div className="result-cost">
                                <DollarOutlined /> Tổng chi phí dự kiến: <strong>{money(scheduleResult.estimatedTotalCost)} VND</strong>
                            </div>

                            {/* Day Tabs */}
                            <div className="day-tabs">
                                {scheduleResult.dailyItinerary?.map((day: any, idx: number) => (
                                    <button
                                        key={idx}
                                        className={`day-tab ${activeDay === idx ? 'active' : ''}`}
                                        onClick={() => setActiveDay(idx)}
                                    >
                                        <span className="day-tab-label">Ngày {day.day}</span>
                                        <span className="day-tab-budget">{money(day.dayBudget)} ₫</span>
                                    </button>
                                ))}
                            </div>

                            {/* Timeline for active day */}
                            {scheduleResult.dailyItinerary?.[activeDay] && (
                                <div className="timeline">
                                    {scheduleResult.dailyItinerary[activeDay].activities?.map((act: any, actIdx: number) => {
                                        const spot = act.scenicSpot;
                                        const heroImg = spot?.suggestedImageUrl || (spot?.mediaUrls && spot.mediaUrls[0]);
                                        const cuisineList: any[] = Array.isArray(spot?.cuisine) ? spot.cuisine : [];
                                        const mealBadgeColor: Record<string, string> = {
                                            BREAKFAST: '#f97316', LUNCH: '#10b981', DINNER: '#8b5cf6', SNACK: '#f59e0b'
                                        };
                                        return (
                                            <div key={actIdx} className="tl-item">
                                                <div className="tl-left">
                                                    <div className="tl-dot" />
                                                    <div className="tl-time">{act.time}</div>
                                                    {actIdx < scheduleResult.dailyItinerary[activeDay].activities.length - 1 && (
                                                        <div className="tl-line" />
                                                    )}
                                                </div>
                                                <div className="tl-content">
                                                    <div className="tl-name-row">
                                                        <h4 className="tl-name">{spot?.name || act.activity}</h4>
                                                        {spot?.googleMapsLink && (
                                                            <a href={spot.googleMapsLink} target="_blank" rel="noopener noreferrer" className="tl-maps-link" title="Xem trên Google Maps">
                                                                <EnvironmentOutlined /> Xem bản đồ
                                                            </a>
                                                        )}
                                                    </div>
                                                    {(spot?.openingHours || spot?.closingHours) && (
                                                        <div className="tl-hours">
                                                            🕐 {spot.openingHours || '--:--'} – {spot.closingHours || '--:--'}
                                                        </div>
                                                    )}
                                                    {heroImg && (
                                                        <img
                                                            src={heroImg}
                                                            alt={spot?.name || ''}
                                                            className="tl-hero-img"
                                                            onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
                                                            onClick={() => setLightboxImage(heroImg)}
                                                        />
                                                    )}
                                                    <p className="tl-desc">{act.description}</p>
                                                    {spot?.tips && (
                                                        <div className="tl-tips">💡 {spot.tips}</div>
                                                    )}
                                                    {spot?.highlights && spot.highlights.length > 0 && (
                                                        <div className="tl-highlights">
                                                            {spot.highlights.map((h: string, hi: number) => (
                                                                <span key={hi} className="tl-badge">✦ {h}</span>
                                                            ))}
                                                        </div>
                                                    )}
                                                    {cuisineList.length > 0 && (
                                                        <div className="tl-cuisine">
                                                            <div className="tl-cuisine-title">🍴 ẨM THỰC</div>
                                                            <div className="tl-food-grid">
                                                                {cuisineList.map((c: any, ci: number) => (
                                                                    <div key={ci} className="food-card">
                                                                        {c.imageUrl && (
                                                                            <img
                                                                                src={c.imageUrl}
                                                                                alt={c.name}
                                                                                className="food-img"
                                                                                onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
                                                                                onClick={() => setLightboxImage(c.imageUrl)}
                                                                            />
                                                                        )}
                                                                        <div className="food-info">
                                                                            <div className="food-name">
                                                                                {c.name}
                                                                                {c.googleMapsUrl && (
                                                                                    <a href={c.googleMapsUrl} target="_blank" rel="noopener noreferrer" className="food-maps-btn" title="Xem quán trên Maps">
                                                                                        <EnvironmentOutlined />
                                                                                    </a>
                                                                                )}
                                                                            </div>
                                                                            {c.restaurant && <div className="food-restaurant">{c.restaurant}</div>}
                                                                            {c.priceRange && <div className="food-price">{c.priceRange}</div>}
                                                                        </div>
                                                                        {c.mealType && (
                                                                            <span
                                                                                className="food-badge"
                                                                                style={{ background: mealBadgeColor[c.mealType] || '#64748b' }}
                                                                            >
                                                                                {c.mealType}
                                                                            </span>
                                                                        )}
                                                                    </div>
                                                                ))}
                                                            </div>
                                                        </div>
                                                    )}
                                                    <div className="tl-cost">{money(act.estimatedCost)} VND</div>
                                                </div>
                                            </div>
                                        );
                                    })}
                                </div>
                            )}

                            <div className="modal-footer">
                                <button className="btn-secondary" onClick={() => setScheduleResult(null)}>
                                    Đóng
                                </button>
                                <button className="btn-primary">
                                    <CalendarOutlined /> Lưu lịch trình
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {lightboxImage && (
                <div className="image-lightbox-overlay" onClick={() => setLightboxImage(null)}>
                    <div className="image-lightbox-inner" onClick={(e) => e.stopPropagation()}>
                        <img src={lightboxImage} alt="Preview" />
                    </div>
                </div>
            )}

            <style jsx>{`
        .ai-page { min-height: 100vh; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); padding-bottom: 80px; }
        
        .ai-hero { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 80px 0; text-align: center; margin-bottom: 60px; }
        .ai-hero h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .ai-hero p { font-size: 20px; opacity: 0.9; }

        .usage-banner { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 20px 30px; border-radius: 16px; margin-bottom: 40px; box-shadow: 0 4px 20px rgba(16, 185, 129, 0.3); display: flex; align-items: center; justify-content: space-between; gap: 20px; flex-wrap: wrap; }
        .usage-banner.payment-required { background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); box-shadow: 0 4px 20px rgba(245, 158, 11, 0.3); }
        @media (max-width: 600px) {
          .usage-banner { padding: 16px 20px; flex-direction: column; text-align: center; }
          .ai-hero h1 { font-size: 28px; }
          .ai-hero p { font-size: 16px; }
        }
        .usage-info { display: flex; align-items: center; gap: 12px; font-size: 16px; }
        .usage-info strong { font-weight: 800; }
        .payment-btn { background: white; color: #f59e0b; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; font-size: 15px; white-space: nowrap; }
        .payment-btn:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(255, 255, 255, 0.3); }

        /* Payment Modal - Responsive & Scrollable */
        .payment-modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); backdrop-filter: blur(4px); z-index: 10000; display: flex; align-items: center; justify-content: center; padding: 16px; overflow-y: auto; animation: fadeIn 0.2s; }
        .payment-modal { background: white; width: 100%; max-width: 440px; max-height: calc(100vh - 32px); border-radius: 24px; overflow: hidden; box-shadow: 0 30px 60px -12px rgba(0,0,0,0.25); animation: zoomIn 0.3s cubic-bezier(0.16, 1, 0.3, 1); display: flex; flex-direction: column; flex-shrink: 0; }
        .pay-header { padding: 16px 20px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; flex-shrink: 0; }
        .pay-header h3 { margin: 0; font-size: 16px; color: #1e293b; line-height: 1.3; }
        .btn-close { background: none; border: none; font-size: 20px; color: #94a3b8; cursor: pointer; transition: 0.2s; flex-shrink: 0; }
        .btn-close:hover { color: #64748b; transform: rotate(90deg); }
        .pay-body { padding: 20px; overflow-y: auto; flex: 1; min-height: 0; -webkit-overflow-scrolling: touch; }
        .qr-box { background: #f8fafc; border-radius: 16px; padding: 16px; text-align: center; border: 1px solid #e2e8f0; margin-bottom: 20px; }
        .qr-box img { width: 200px; height: 200px; max-width: 100%; object-fit: contain; background: white; padding: 10px; border-radius: 12px; margin-bottom: 12px; border: 1px solid #f1f5f9; }
        .qr-hint { font-size: 12px; color: #64748b; font-weight: 500; }
        .pay-info-summary { display: flex; flex-direction: column; gap: 10px; margin-bottom: 24px; }
        .p-row { display: flex; justify-content: space-between; font-size: 14px; }
        .p-row span { color: #64748b; }
        .p-row strong { color: #1e293b; }
        .p-amount { color: #f59e0b !important; font-size: 18px; }
        .pay-notice { background: #f0fdf4; color: #16a34a; padding: 12px; border-radius: 12px; font-size: 13px; font-weight: 600; display: flex; gap: 8px; align-items: center; }
        .receipt-upload-section { margin-bottom: 16px; text-align: center; }
        .receipt-upload-label { display: inline-block; padding: 10px 16px; background: #f0f9ff; border: 1px dashed #667eea; border-radius: 10px; font-size: 13px; color: #4f46e5; cursor: pointer; transition: 0.2s; }
        .receipt-upload-label:hover:not(:has(input:disabled)) { background: #eef2ff; }
        .receipt-upload-label input { display: none; }
        .receipt-error { display: block; margin-top: 8px; font-size: 12px; color: #dc2626; }
        .pay-footer { padding: 16px 20px 20px; flex-shrink: 0; }
        .btn-primary { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        @media (max-width: 480px) {
          .payment-modal-overlay { padding: 12px; align-items: flex-start; }
          .payment-modal { max-height: calc(100vh - 24px); border-radius: 20px; }
          .pay-header h3 { font-size: 14px; }
          .qr-box { padding: 12px; }
          .qr-box img { width: 160px; height: 160px; }
          .pay-info-summary { font-size: 13px; }
          .p-amount { font-size: 16px !important; }
          .receipt-upload-label { font-size: 12px; padding: 8px 12px; }
        }
        .btn-primary:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
        .btn-primary:disabled { background: #94a3b8; cursor: not-allowed; }
        .btn-primary.full { width: 100%; }

        .ai-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 40px; }
        @media (max-width: 1024px) { .ai-grid { grid-template-columns: 1fr; } }

        .ai-card { background: white; border-radius: 24px; padding: 32px; box-shadow: 0 10px 40px rgba(0,0,0,0.08); border: 1px solid #e5e7eb; }
        .card-header { display: flex; align-items: center; gap: 12px; margin-bottom: 12px; color: #667eea; }
        .card-header h2 { font-size: 24px; font-weight: 800; margin: 0; }
        .card-desc { color: #64748b; margin-bottom: 24px; line-height: 1.6; }

        .chat-messages { height: 400px; overflow-y: auto; border: 1px solid #e5e7eb; border-radius: 16px; padding: 20px; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); margin-bottom: 16px; }
        .chat-msg { padding: 12px 16px; border-radius: 16px; max-width: 85%; margin-bottom: 12px; font-size: 14px; line-height: 1.7; animation: slideIn 0.3s ease-out; }
        .chat-msg.bot { background: white; color: #0f172a; border: 1px solid #e5e7eb; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
        .chat-msg.bot :global(strong) { color: #667eea; font-weight: 700; }
        .chat-msg.bot :global(em) { font-style: italic; color: #64748b; }
        .chat-msg.user { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; margin-left: auto; box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3); }
        .chat-msg.typing { display: flex; gap: 6px; padding: 16px; }
        .chat-msg.typing span { width: 8px; height: 8px; background: #cbd5e1; border-radius: 50%; animation: bounce 1.4s infinite ease-in-out both; }
        .chat-msg.typing span:nth-child(1) { animation-delay: -0.32s; }
        .chat-msg.typing span:nth-child(2) { animation-delay: -0.16s; }

        .chat-input-area { display: flex; gap: 12px; }
        .chat-input-area input { flex: 1; border: 1px solid #e5e7eb; padding: 12px 16px; border-radius: 12px; font-size: 14px; outline: none; transition: 0.2s; }
        .chat-input-area input:focus { border-color: #667eea; box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1); }
        .send-btn { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .send-btn:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
        .send-btn:disabled { opacity: 0.5; cursor: not-allowed; }

        .schedule-form { margin-bottom: 24px; }
        .form-group { margin-bottom: 20px; }
        .form-group label { display: block; font-weight: 700; color: #0f172a; margin-bottom: 8px; font-size: 14px; }
        .form-group input, .form-group select { width: 100%; border: 1px solid #e5e7eb; padding: 12px 16px; border-radius: 12px; font-size: 14px; outline: none; transition: 0.2s; }
        .form-group input:focus, .form-group select:focus { border-color: #667eea; box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1); }
        .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }

        .generate-btn { width: 100%; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; padding: 16px; border-radius: 12px; font-size: 16px; font-weight: 800; cursor: pointer; transition: 0.2s; display: flex; align-items: center; justify-content: center; gap: 8px; }
        .generate-btn:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4); }
        .generate-btn:disabled { opacity: 0.6; cursor: not-allowed; }

        /* Modal Styles */
        .modal-overlay { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.7); backdrop-filter: blur(4px); z-index: 9999; display: flex; align-items: center; justify-content: center; padding: 20px; animation: fadeIn 0.2s; }
        .modal-container { background: #f8fafc; border-radius: 24px; max-width: 780px; width: 100%; max-height: 92vh; overflow: hidden; box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3); position: relative; display: flex; flex-direction: column; animation: slideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1); }
        .modal-close { position: absolute; top: 16px; right: 16px; z-index: 10; background: rgba(255,255,255,0.9); border: none; width: 36px; height: 36px; border-radius: 50%; cursor: pointer; font-size: 18px; color: #64748b; transition: 0.2s; display: grid; place-items: center; box-shadow: 0 2px 8px rgba(0,0,0,0.12); }
        .modal-close:hover { background: white; transform: rotate(90deg); color: #0f172a; }
        
        .modal-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 24px 28px; display: flex; align-items: center; gap: 14px; flex-shrink: 0; }
        .modal-header h2 { font-size: 22px; font-weight: 800; margin: 0; }
        
        .modal-body { flex: 1; overflow-y: auto; padding: 24px 28px; }
        .result-summary { background: white; border-left: 4px solid #667eea; border-radius: 0 12px 12px 0; padding: 14px 16px; margin-bottom: 16px; color: #374151; font-style: italic; line-height: 1.6; font-size: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
        .result-cost { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 12px 18px; border-radius: 12px; margin-bottom: 20px; font-size: 15px; font-weight: 700; display: flex; align-items: center; gap: 8px; }
        .result-cost strong { font-size: 18px; }

        /* Day Tabs */
        .day-tabs { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 24px; }
        .day-tab { background: white; border: 2px solid #e5e7eb; border-radius: 12px; padding: 10px 16px; cursor: pointer; transition: 0.2s; display: flex; flex-direction: column; align-items: center; gap: 2px; min-width: 90px; }
        .day-tab:hover { border-color: #667eea; }
        .day-tab.active { border-color: #667eea; background: linear-gradient(135deg, #667eea, #764ba2); color: white; }
        .day-tab-label { font-weight: 800; font-size: 14px; }
        .day-tab-budget { font-size: 11px; opacity: 0.8; }

        /* Timeline */
        .timeline { display: flex; flex-direction: column; }
        .tl-item { display: grid; grid-template-columns: 72px 1fr; gap: 0; }
        .tl-left { display: flex; flex-direction: column; align-items: center; padding-top: 4px; }
        .tl-dot { width: 14px; height: 14px; border-radius: 50%; background: #667eea; border: 3px solid white; box-shadow: 0 0 0 2px #667eea; flex-shrink: 0; }
        .tl-time { font-size: 12px; font-weight: 800; color: #667eea; margin-top: 6px; text-align: center; white-space: nowrap; }
        .tl-line { flex: 1; width: 2px; background: #e5e7eb; margin: 6px 0; min-height: 40px; }
        .tl-content { background: white; border-radius: 16px; padding: 16px 18px; margin-bottom: 16px; border: 1px solid #e5e7eb; box-shadow: 0 1px 6px rgba(0,0,0,0.05); }
        .tl-name-row { display: flex; align-items: center; justify-content: space-between; gap: 8px; margin-bottom: 6px; flex-wrap: wrap; }
        .tl-name { font-size: 17px; font-weight: 800; color: #0f172a; margin: 0; }
        .tl-maps-link { font-size: 12px; color: #667eea; text-decoration: none; font-weight: 600; white-space: nowrap; transition: 0.2s; padding: 3px 8px; background: #f0f9ff; border-radius: 6px; border: 1px solid #e0e7ff; }
        .tl-maps-link:hover { background: #667eea; color: white; }
        .tl-hours { font-size: 12px; color: #667eea; font-weight: 600; margin-bottom: 10px; }
        .tl-hero-img { width: 100%; height: 200px; object-fit: cover; border-radius: 12px; margin-bottom: 12px; border: 1px solid #f1f5f9; cursor: zoom-in; }
        .tl-desc { font-size: 14px; color: #475569; line-height: 1.6; margin: 0 0 10px; }
        .tl-tips { background: #fff7ed; border-left: 3px solid #f97316; padding: 10px 14px; border-radius: 0 10px 10px 0; font-size: 13px; color: #7c2d12; margin-bottom: 10px; line-height: 1.5; }
        .tl-highlights { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 12px; }
        .tl-badge { background: #eff6ff; color: #1d4ed8; font-size: 12px; font-weight: 600; padding: 4px 12px; border-radius: 20px; border: 1px solid #bfdbfe; }
        .tl-cost { font-size: 14px; color: #10b981; font-weight: 700; text-align: right; margin-top: 10px; padding-top: 10px; border-top: 1px dashed #e5e7eb; }

        /* Cuisine section */
        .tl-cuisine { background: #fafafa; border-radius: 12px; padding: 14px; border: 1px solid #f1f5f9; margin-bottom: 8px; }
        .tl-cuisine-title { font-size: 13px; font-weight: 800; color: #374151; margin-bottom: 12px; letter-spacing: 0.5px; display: flex; align-items: center; gap: 6px; }
        .tl-food-grid { display: flex; flex-direction: column; gap: 10px; }
        .food-card { display: flex; align-items: center; gap: 12px; background: white; border-radius: 12px; padding: 10px; border: 1px solid #e5e7eb; position: relative; overflow: hidden; }
        .food-img { width: 60px; height: 60px; object-fit: cover; border-radius: 8px; flex-shrink: 0; cursor: zoom-in; }
        .food-info { flex: 1; min-width: 0; }
        .food-name { font-size: 14px; font-weight: 700; color: #0f172a; margin-bottom: 2px; display: flex; align-items: center; gap: 4px; }
        .food-maps-btn { display: inline-flex; align-items: center; justify-content: center; width: 24px; height: 24px; font-size: 12px; color: #667eea; text-decoration: none; background: #f0f9ff; border: 1px solid #e0e7ff; border-radius: 50%; transition: 0.2s; flex-shrink: 0; }
        .food-maps-btn:hover { background: #667eea; color: white; border-color: #667eea; }
        .food-restaurant { font-size: 12px; color: #64748b; margin-bottom: 3px; }
        .food-price { font-size: 13px; color: #10b981; font-weight: 600; }
        .food-badge { position: absolute; top: 8px; right: 8px; font-size: 10px; font-weight: 800; color: white; padding: 3px 8px; border-radius: 6px; letter-spacing: 0.5px; }

        .modal-footer { display: flex; gap: 12px; justify-content: flex-end; padding: 16px 28px 20px; border-top: 1px solid #e5e7eb; flex-shrink: 0; background: white; }
        .btn-secondary { background: white; border: 2px solid #e5e7eb; color: #64748b; padding: 10px 20px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .btn-secondary:hover { border-color: #667eea; color: #667eea; }
        .btn-primary { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; padding: 10px 20px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; display: flex; align-items: center; gap: 8px; }
        .btn-primary:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }

        /* Image lightbox */
        .image-lightbox-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.8); z-index: 10001; display: flex; align-items: center; justify-content: center; padding: 20px; }
        .image-lightbox-inner img { max-width: 100%; max-height: 100%; object-fit: contain; border-radius: 16px; box-shadow: 0 20px 60px rgba(0,0,0,0.6); }

        @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(40px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes bounce { 0%, 80%, 100% { transform: scale(0); } 40% { transform: scale(1); } }
        @keyframes zoomIn { from { opacity: 0; transform: scale(0.95); } to { opacity: 1; transform: scale(1); } }
      `}</style>
        </main>
    );
}

const MessageContent = ({ text }: { text: string }) => {
    const formatText = (str: string) => {
        // Parse markdown-like formatting
        return str
            // Bold: **text** or ***text***
            .replace(/\*\*\*(.+?)\*\*\*/g, '<strong>$1</strong>')
            .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
            // Italic: *text*
            .replace(/\*(.+?)\*/g, '<em>$1</em>')
            // Line breaks
            .replace(/\n/g, '<br />');
    };

    return <div dangerouslySetInnerHTML={{ __html: formatText(text) }} />;
};

const CustomSelect = ({ value, options, onChange }: { value: any, options: { label: string, value: any }[], onChange: (v: any) => void }) => {
    const [isOpen, setIsOpen] = useState(false);
    const ref = React.useRef<HTMLDivElement>(null);

    useEffect(() => {
        const click = (e: MouseEvent) => { if (ref.current && !ref.current.contains(e.target as Node)) setIsOpen(false); };
        document.addEventListener('mousedown', click);
        return () => document.removeEventListener('mousedown', click);
    }, []);

    const selectedLabel = options.find(o => o.value === value)?.label || value || 'Chọn điểm đến';

    return (
        <div className={`custom-select-ai ${isOpen ? 'is-open' : ''}`} ref={ref} onClick={() => setIsOpen(!isOpen)}>
            <div className="select-trigger-ai">
                <span className="current-value-ai">{selectedLabel}</span>
                <ArrowRightOutlined style={{ transform: `rotate(${isOpen ? -90 : 90}deg)`, fontSize: '12px', transition: '0.3s', color: '#667eea' }} />
            </div>
            {isOpen && (
                <div className="options-dropdown-ai">
                    {options.map(o => (
                        <div key={o.value} className={`option-item-ai ${value === o.value ? 'active' : ''}`} onClick={(e) => { e.stopPropagation(); onChange(o.value); setIsOpen(false); }}>
                            {o.label}
                        </div>
                    ))}
                </div>
            )}
            <style jsx>{`
                .custom-select-ai { 
                    position: relative; 
                    border: 1px solid #e5e7eb; 
                    padding: 12px 16px; 
                    border-radius: 12px; 
                    background: white; 
                    transition: 0.2s; 
                    cursor: pointer; 
                    z-index: 1;
                }
                .custom-select-ai.is-open { z-index: 100; border-color: #667eea; box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1); }
                .custom-select-ai:hover { border-color: #667eea; }
                .select-trigger-ai { display: flex; justify-content: space-between; align-items: center; width: 100%; }
                .current-value-ai { font-size: 14px; color: #0f172a; font-weight: 600; }
                .options-dropdown-ai { 
                    position: absolute; 
                    top: calc(100% + 8px); 
                    left: 0; 
                    right: 0; 
                    background: white; 
                    border-radius: 12px; 
                    box-shadow: 0 10px 40px rgba(0,0,0,0.15); 
                    border: 1px solid #e5e7eb; 
                    z-index: 1100; 
                    padding: 6px; 
                    animation: slideInAI 0.2s ease-out; 
                    max-height: 320px; 
                    overflow-y: auto;
                }
                .options-dropdown-ai::-webkit-scrollbar { width: 8px; }
                .options-dropdown-ai::-webkit-scrollbar-track { background: #f1f5f9; border-radius: 10px; }
                .options-dropdown-ai::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 10px; }
                .options-dropdown-ai::-webkit-scrollbar-thumb:hover { background: #94a3b8; }
                .option-item-ai { 
                    padding: 12px 16px; 
                    font-size: 14px; 
                    font-weight: 600; 
                    color: #0f172a; 
                    border-radius: 10px; 
                    cursor: pointer; 
                    transition: 0.2s; 
                }
                .option-item-ai:hover { background: #f0f9ff; color: #667eea; }
                .option-item-ai.active { background: #667eea; color: white; }
                @keyframes slideInAI { from { opacity: 0; transform: translateY(-10px); } to { opacity: 1; transform: translateY(0); } }
            `}</style>
        </div>
    );
};

