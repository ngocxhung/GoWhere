"use client";

import React, { useState } from 'react';
import {
    RobotOutlined,
    CalendarOutlined,
    SendOutlined,
    DollarOutlined,
    TeamOutlined,
    ClockCircleOutlined,
    EnvironmentOutlined,
    CheckCircleFilled,
    LoadingOutlined
} from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

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
    const [scheduleResult, setScheduleResult] = useState<any>(null);
    const [scheduleLoading, setScheduleLoading] = useState(false);
    const [usageInfo, setUsageInfo] = useState<any>(null);
    const [showPaymentModal, setShowPaymentModal] = useState(false);
    const [paymentQR, setPaymentQR] = useState("");
    const [paymentDetails, setPaymentDetails] = useState<any>(null);
    const [paymentOrder, setPaymentOrder] = useState<any>(null);

    const chatEndRef = React.useRef<HTMLDivElement>(null);

    React.useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [chatMessages]);

    React.useEffect(() => {
        checkUsageLimit();
    }, []);

    const checkUsageLimit = async () => {
        const token = localStorage.getItem('token');
        if (!token) return;

        try {
            const response = await fetch('http://localhost:5191/api/AIChat/CheckUsageLimit', {
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
            const response = await fetch('http://localhost:5191/api/AIChat/Chat', {
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
            alert('Vui lòng nhập điểm đến!');
            return;
        }

        setScheduleLoading(true);
        setScheduleResult(null);

        try {
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

            const response = await fetch('http://localhost:5191/api/AIChat/GenerateSchedule', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    provinceId: 0,
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
                await checkUsageLimit();
            } else {
                const error = await response.json();
                alert(error.message || 'Có lỗi xảy ra khi tạo lịch trình');
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
            const payInfoRes = await fetch('http://localhost:5191/api/Order/GetPaymentInfo');
            const payInfo = await payInfoRes.json();
            setPaymentDetails(payInfo);

            // 2. Create AI Payment Order
            const orderRes = await fetch('http://localhost:5191/api/AIChat/CreateAIPaymentOrder', {
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
                                {usageInfo.requiresPayment ? (
                                    <>
                                        Bạn đã hết lượt tạo lịch trình miễn phí. Thanh toán <strong>{money(usageInfo.paymentAmount)} VND</strong> để sử dụng không giới hạn!
                                    </>
                                ) : (
                                    <>
                                        Bạn còn <strong>{usageInfo.remainingFreeRequests}</strong> lượt tạo lịch trình miễn phí
                                    </>
                                )}
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
                    <div className="payment-modal-overlay" onClick={() => setShowPaymentModal(false)}>
                        <div className="payment-modal" onClick={(e) => e.stopPropagation()}>
                            <div className="pay-header">
                                <h3>Xác nhận thanh toán AI Unlimited</h3>
                                <button className="btn-close" onClick={() => setShowPaymentModal(false)}>✕</button>
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
                                <div className="pay-notice">
                                    <CheckCircleFilled /> Sau khi thanh toán thành công, bạn có thể tạo lịch trình không giới hạn!
                                </div>
                            </div>
                            <div className="pay-footer">
                                <button className="btn-primary full" onClick={() => {
                                    setShowPaymentModal(false);
                                    checkUsageLimit(); // Refresh usage info
                                }}>Tôi đã thanh toán xong</button>
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
                                    {msg.text}
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
                                <input
                                    type="text"
                                    value={scheduleForm.destination}
                                    onChange={(e) => setScheduleForm({ ...scheduleForm, destination: e.target.value })}
                                    placeholder="Vd: Đà Nẵng, Phú Quốc, Hội An..."
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
                                    <><LoadingOutlined /> AI ĐANG TẠO LỊCH TRÌNH...</>
                                ) : (
                                    <><CalendarOutlined /> TẠO LỊCH TRÌNH NGAY</>
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

                            <div className="days-list">
                                {scheduleResult.dailyItinerary?.map((day: any, idx: number) => (
                                    <div key={idx} className="day-item">
                                        <div className="day-header">
                                            <strong>Ngày {day.day}</strong>
                                            <span>{money(day.dayBudget)} VND</span>
                                        </div>
                                        <div className="activities">
                                            {day.activities?.map((act: any, actIdx: number) => (
                                                <div key={actIdx} className="activity">
                                                    <div className="act-time">{act.time}</div>
                                                    <div className="act-details">
                                                        <strong>{act.scenicSpot?.name || act.activity}</strong>
                                                        <p>{act.description}</p>
                                                        {act.scenicSpot?.tips && (
                                                            <div className="act-tips">💡 {act.scenicSpot.tips}</div>
                                                        )}
                                                    </div>
                                                    <div className="act-cost">{money(act.estimatedCost)} VND</div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                ))}
                            </div>

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

            <style jsx>{`
        .ai-page { min-height: 100vh; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); padding-bottom: 80px; }
        
        .ai-hero { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 80px 0; text-align: center; margin-bottom: 60px; }
        .ai-hero h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .ai-hero p { font-size: 20px; opacity: 0.9; }

        .usage-banner { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 20px 30px; border-radius: 16px; margin-bottom: 40px; box-shadow: 0 4px 20px rgba(16, 185, 129, 0.3); display: flex; align-items: center; justify-content: space-between; gap: 20px; }
        .usage-banner.payment-required { background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); box-shadow: 0 4px 20px rgba(245, 158, 11, 0.3); }
        .usage-info { display: flex; align-items: center; gap: 12px; font-size: 16px; }
        .usage-info strong { font-weight: 800; }
        .payment-btn { background: white; color: #f59e0b; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; font-size: 15px; white-space: nowrap; }
        .payment-btn:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(255, 255, 255, 0.3); }

        /* Payment Modal */
        .payment-modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); backdrop-filter: blur(4px); z-index: 10000; display: grid; place-items: center; padding: 20px; animation: fadeIn 0.2s; }
        .payment-modal { background: white; width: 100%; max-width: 440px; border-radius: 24px; overflow: hidden; box-shadow: 0 30px 60px -12px rgba(0,0,0,0.25); animation: zoomIn 0.3s cubic-bezier(0.16, 1, 0.3, 1); }
        .pay-header { padding: 20px 24px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
        .pay-header h3 { margin: 0; font-size: 18px; color: #1e293b; }
        .btn-close { background: none; border: none; font-size: 20px; color: #94a3b8; cursor: pointer; transition: 0.2s; }
        .btn-close:hover { color: #64748b; transform: rotate(90deg); }
        .pay-body { padding: 24px; }
        .qr-box { background: #f8fafc; border-radius: 16px; padding: 20px; text-align: center; border: 1px solid #e2e8f0; margin-bottom: 24px; }
        .qr-box img { width: 240px; height: 240px; background: white; padding: 10px; border-radius: 12px; margin-bottom: 12px; border: 1px solid #f1f5f9; }
        .qr-hint { font-size: 12px; color: #64748b; font-weight: 500; }
        .pay-info-summary { display: flex; flex-direction: column; gap: 10px; margin-bottom: 24px; }
        .p-row { display: flex; justify-content: space-between; font-size: 14px; }
        .p-row span { color: #64748b; }
        .p-row strong { color: #1e293b; }
        .p-amount { color: #f59e0b !important; font-size: 18px; }
        .pay-notice { background: #f0fdf4; color: #16a34a; padding: 12px; border-radius: 12px; font-size: 13px; font-weight: 600; display: flex; gap: 8px; align-items: center; }
        .pay-footer { padding: 0 24px 24px 24px; }
        .btn-primary { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .btn-primary:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
        .btn-primary.full { width: 100%; }

        .ai-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 40px; }
        @media (max-width: 1024px) { .ai-grid { grid-template-columns: 1fr; } }

        .ai-card { background: white; border-radius: 24px; padding: 32px; box-shadow: 0 10px 40px rgba(0,0,0,0.08); border: 1px solid #e5e7eb; }
        .card-header { display: flex; align-items: center; gap: 12px; margin-bottom: 12px; color: #667eea; }
        .card-header h2 { font-size: 24px; font-weight: 800; margin: 0; }
        .card-desc { color: #64748b; margin-bottom: 24px; line-height: 1.6; }

        .chat-messages { height: 400px; overflow-y: auto; border: 1px solid #e5e7eb; border-radius: 16px; padding: 20px; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); margin-bottom: 16px; }
        .chat-msg { padding: 12px 16px; border-radius: 16px; max-width: 85%; margin-bottom: 12px; font-size: 14px; line-height: 1.5; animation: slideIn 0.3s ease-out; }
        .chat-msg.bot { background: white; color: #0f172a; border: 1px solid #e5e7eb; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
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
        .modal-container { background: white; border-radius: 24px; max-width: 900px; width: 100%; max-height: 90vh; overflow: hidden; box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3); position: relative; display: flex; flex-direction: column; animation: slideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1); }
        .modal-close { position: absolute; top: 20px; right: 20px; z-index: 10; background: rgba(0, 0, 0, 0.1); border: none; width: 40px; height: 40px; border-radius: 50%; cursor: pointer; font-size: 20px; font-weight: 700; color: #64748b; transition: 0.2s; display: grid; place-items: center; }
        .modal-close:hover { background: rgba(0, 0, 0, 0.2); transform: rotate(90deg); color: #0f172a; }
        
        .modal-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 32px; display: flex; align-items: center; gap: 16px; }
        .modal-header h2 { font-size: 28px; font-weight: 800; margin: 0; }
        
        .modal-body { flex: 1; overflow-y: auto; padding: 32px; }
        .result-summary { background: #f0f9ff; border: 1px dashed #667eea; border-radius: 12px; padding: 16px; margin-bottom: 24px; color: #0369a1; font-style: italic; line-height: 1.6; }
        .result-cost { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 16px 20px; border-radius: 12px; margin-bottom: 24px; font-size: 16px; font-weight: 700; display: flex; align-items: center; gap: 8px; }
        .result-cost strong { font-size: 20px; }

        .days-list { display: flex; flex-direction: column; gap: 20px; }
        .day-item { background: #f8fafc; border-radius: 16px; padding: 20px; border: 1px solid #e5e7eb; }
        .day-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 2px solid #e5e7eb; }
        .day-header strong { font-size: 18px; color: #667eea; }
        .day-header span { color: #10b981; font-weight: 700; }

        .activities { display: flex; flex-direction: column; gap: 12px; }
        .activity { display: grid; grid-template-columns: 80px 1fr 120px; gap: 16px; align-items: start; background: white; padding: 16px; border-radius: 12px; border: 1px solid #e5e7eb; }
        .act-time { background: #f0f9ff; color: #667eea; font-weight: 800; padding: 8px; border-radius: 8px; text-align: center; font-size: 13px; }
        .act-details strong { display: block; color: #0f172a; margin-bottom: 6px; font-size: 15px; }
        .act-details p { color: #64748b; font-size: 13px; line-height: 1.5; margin: 0; }
        .act-tips { background: #fef3c7; color: #92400e; padding: 8px 12px; border-radius: 8px; font-size: 12px; margin-top: 8px; border: 1px dashed #fbbf24; }
        .act-cost { color: #10b981; font-weight: 700; text-align: right; font-size: 14px; }

        .modal-footer { display: flex; gap: 12px; justify-content: flex-end; padding-top: 24px; border-top: 1px solid #e5e7eb; }
        .btn-secondary { background: white; border: 2px solid #e5e7eb; color: #64748b; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .btn-secondary:hover { border-color: #667eea; color: #667eea; }
        .btn-primary { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; display: flex; align-items: center; gap: 8px; }
        .btn-primary:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }

        @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(40px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes slideIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes bounce { 0%, 80%, 100% { transform: scale(0); } 40% { transform: scale(1); } }
        @keyframes zoomIn { from { opacity: 0; transform: scale(0.95); } to { opacity: 1; transform: scale(1); } }
      `}</style>
        </main>
    );
}
