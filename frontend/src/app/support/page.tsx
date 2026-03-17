'use client';

import React, { useState } from 'react';
import { 
    MailOutlined, 
    PhoneOutlined, 
    FacebookOutlined, 
    MessageOutlined,
    CheckCircleFilled,
    TeamOutlined,
    RocketOutlined,
    HeartOutlined,
    GlobalOutlined
} from '@ant-design/icons';

export default function SupportPage() {
    const [copiedItem, setCopiedItem] = useState<string | null>(null);

    const copyToClipboard = (text: string, type: string) => {
        navigator.clipboard.writeText(text);
        setCopiedItem(type);
        setTimeout(() => setCopiedItem(null), 2000);
    };

    const contactInfo = {
        email: 'gowheretravelwithvibe@gmail.com',
        phone: '+84966478235',
        facebook: 'https://www.facebook.com/profile.php?id=61587150455623',
        messenger: 'https://m.me/961137500418471'
    };

    return (
        <main className="support-page">
            {/* Hero Section */}
            <section className="support-hero">
                <div className="hero-particles">
                    <div className="particle"></div>
                    <div className="particle"></div>
                    <div className="particle"></div>
                </div>
                <div className="container hero-content">
                    <div className="hero-icon">💬</div>
                    <h1>Chúng tôi luôn sẵn sàng hỗ trợ bạn</h1>
                    <p>Đội ngũ GoWhere cam kết mang đến trải nghiệm du lịch tuyệt vời nhất</p>
                </div>
            </section>

            {/* About Section */}
            <section className="about-section">
                <div className="container">
                    <div className="about-grid">
                        <div className="about-card">
                            <div className="card-icon" style={{ background: 'linear-gradient(135deg, #667eea, #764ba2)' }}>
                                <RocketOutlined />
                            </div>
                            <h3>Sứ mệnh</h3>
                            <p>Ứng dụng AI tiên tiến giúp bạn lên kế hoạch du lịch hoàn hảo chỉ trong vài giây</p>
                        </div>
                        <div className="about-card">
                            <div className="card-icon" style={{ background: 'linear-gradient(135deg, #f093fb, #f5576c)' }}>
                                <HeartOutlined />
                            </div>
                            <h3>Tận tâm</h3>
                            <p>Đặt trải nghiệm của bạn lên hàng đầu với dịch vụ hỗ trợ 24/7</p>
                        </div>
                        <div className="about-card">
                            <div className="card-icon" style={{ background: 'linear-gradient(135deg, #4facfe, #00f2fe)' }}>
                                <GlobalOutlined />
                            </div>
                            <h3>Khám phá</h3>
                            <p>Hơn 100+ địa điểm tuyệt vời trên khắp Việt Nam đang chờ bạn</p>
                        </div>
                    </div>
                </div>
            </section>

            {/* Contact Cards Section */}
            <section className="contact-section">
                <div className="container">
                    <div className="section-header">
                        <h2>Liên hệ với chúng tôi</h2>
                        <p>Chọn cách liên hệ phù hợp nhất với bạn</p>
                    </div>

                    <div className="contact-grid">
                        {/* Email Card */}
                        <div className="contact-card email-card">
                            <div className="card-header">
                                <div className="contact-icon">
                                    <MailOutlined />
                                </div>
                                <h3>Email</h3>
                            </div>
                            <div className="contact-value">
                                {contactInfo.email}
                            </div>
                            <div className="card-actions">
                                <button
                                    className="action-btn primary"
                                    onClick={() => window.open(`https://mail.google.com/mail/?view=cm&fs=1&to=${contactInfo.email}`, '_blank')}
                                >
                                    <MailOutlined /> Gửi email
                                </button>
                                <button
                                    className="action-btn secondary"
                                    onClick={() => copyToClipboard(contactInfo.email, 'email')}
                                >
                                    {copiedItem === 'email' ? (
                                        <><CheckCircleFilled /> Đã sao chép</>
                                    ) : (
                                        <>📋 Copy</>
                                    )}
                                </button>
                            </div>
                        </div>

                        {/* Phone Card */}
                        <div className="contact-card phone-card">
                            <div className="card-header">
                                <div className="contact-icon">
                                    <PhoneOutlined />
                                </div>
                                <h3>Điện thoại</h3>
                            </div>
                            <div className="contact-value">
                                {contactInfo.phone}
                            </div>
                            <div className="card-actions">
                                <button
                                    className="action-btn primary"
                                    onClick={() => window.location.href = `tel:${contactInfo.phone}`}
                                >
                                    <PhoneOutlined /> Gọi ngay
                                </button>
                                <button
                                    className="action-btn secondary"
                                    onClick={() => copyToClipboard(contactInfo.phone, 'phone')}
                                >
                                    {copiedItem === 'phone' ? (
                                        <><CheckCircleFilled /> Đã sao chép</>
                                    ) : (
                                        <>📋 Copy</>
                                    )}
                                </button>
                            </div>
                        </div>

                        {/* Facebook Card */}
                        <div className="contact-card facebook-card">
                            <div className="card-header">
                                <div className="contact-icon">
                                    <FacebookOutlined />
                                </div>
                                <h3>Facebook</h3>
                            </div>
                            <div className="contact-value small">
                                GoWhere
                            </div>
                            <div className="card-actions">
                                <button
                                    className="action-btn primary"
                                    onClick={() => window.open(contactInfo.facebook, '_blank')}
                                >
                                    <FacebookOutlined /> Xem trang
                                </button>
                                <button
                                    className="action-btn secondary"
                                    onClick={() => window.open(contactInfo.messenger, '_blank')}
                                >
                                    <MessageOutlined /> Nhắn tin
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Quick Links */}
            <section className="quick-links-section">
                <div className="container">
                    <div className="quick-links-card">
                        <div className="links-header">
                            <TeamOutlined style={{ fontSize: '32px', color: '#667eea' }} />
                            <h3>Kết nối với chúng tôi</h3>
                            <p>Theo dõi để nhận tin tức và ưu đãi mới nhất</p>
                        </div>
                        <div className="social-buttons">
                            <a
                                href={contactInfo.facebook}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="social-btn facebook"
                            >
                                <FacebookOutlined /> Facebook Page
                            </a>
                            <a
                                href={contactInfo.messenger}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="social-btn messenger"
                            >
                                <MessageOutlined /> Messenger
                            </a>
                        </div>
                    </div>
                </div>
            </section>

            {/* FAQ Section */}
            <section className="faq-section">
                <div className="container">
                    <h2>Câu hỏi thường gặp</h2>
                    <div className="faq-grid">
                        <div className="faq-item">
                            <h4>🤖 AI tạo lịch trình hoạt động như thế nào?</h4>
                            <p>Bạn chỉ cần nhập điểm đến, ngân sách và số ngày. AI sẽ phân tích và tạo lịch trình tối ưu dựa trên sở thích của bạn.</p>
                        </div>
                        <div className="faq-item">
                            <h4>💰 Chi phí sử dụng dịch vụ?</h4>
                            <p>Mỗi tài khoản có 2 lượt tạo lịch trình AI miễn phí. Sau đó chỉ 50.000đ cho gói 5 lượt.</p>
                        </div>
                        <div className="faq-item">
                            <h4>📱 Có app mobile không?</h4>
                            <p>Hiện tại chúng tôi đang phát triển. Bạn có thể sử dụng website trên mọi thiết bị.</p>
                        </div>
                        <div className="faq-item">
                            <h4>⏰ Thời gian hỗ trợ?</h4>
                            <p>Chúng tôi hỗ trợ 24/7 qua email và messenger. Thời gian phản hồi trung bình dưới 2 giờ.</p>
                        </div>
                    </div>
                </div>
            </section>

            <style jsx>{`
                .support-page { min-height: 100vh; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); }

                /* Hero */
                .support-hero { 
                    position: relative;
                    padding: 120px 0 80px;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    overflow: hidden;
                }
                .hero-particles { position: absolute; inset: 0; overflow: hidden; }
                .particle { 
                    position: absolute;
                    width: 300px;
                    height: 300px;
                    border-radius: 50%;
                    background: rgba(255,255,255,0.1);
                    animation: float 20s infinite;
                }
                .particle:nth-child(1) { top: 10%; left: 10%; animation-delay: 0s; }
                .particle:nth-child(2) { top: 60%; right: 10%; animation-delay: 7s; }
                .particle:nth-child(3) { bottom: 10%; left: 50%; animation-delay: 14s; }
                @keyframes float {
                    0%, 100% { transform: translate(0, 0) scale(1); }
                    33% { transform: translate(30px, -30px) scale(1.1); }
                    66% { transform: translate(-20px, 20px) scale(0.9); }
                }

                .hero-content { text-align: center; color: white; position: relative; z-index: 1; }
                .hero-icon { font-size: 80px; margin-bottom: 24px; animation: bounce 2s infinite; }
                @keyframes bounce {
                    0%, 100% { transform: translateY(0); }
                    50% { transform: translateY(-10px); }
                }
                .hero-content h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; text-shadow: 0 4px 12px rgba(0,0,0,0.2); }
                .hero-content p { font-size: 20px; opacity: 0.95; }

                /* About Section */
                .about-section { padding: 80px 0; }
                .about-grid { 
                    display: grid; 
                    grid-template-columns: repeat(3, 1fr); 
                    gap: 32px; 
                }
                @media (max-width: 768px) { .about-grid { grid-template-columns: 1fr; } }

                .about-card { 
                    background: white; 
                    padding: 32px; 
                    border-radius: 20px; 
                    text-align: center;
                    box-shadow: 0 4px 16px rgba(0,0,0,0.08);
                    border: 1px solid #f1f5f9;
                    transition: 0.3s;
                }
                .about-card:hover { transform: translateY(-8px); box-shadow: 0 12px 32px rgba(0,0,0,0.15); }
                .card-icon { 
                    width: 80px; 
                    height: 80px; 
                    border-radius: 20px; 
                    margin: 0 auto 20px;
                    display: grid;
                    place-items: center;
                    font-size: 36px;
                    color: white;
                }
                .about-card h3 { font-size: 22px; font-weight: 800; color: #0f172a; margin-bottom: 12px; }
                .about-card p { color: #64748b; line-height: 1.6; }

                /* Contact Section */
                .contact-section { padding: 80px 0; }
                .section-header { text-align: center; margin-bottom: 60px; }
                .section-header h2 { font-size: 40px; font-weight: 800; color: #0f172a; margin-bottom: 12px; }
                .section-header p { color: #64748b; font-size: 18px; }

                .contact-grid { 
                    display: grid; 
                    grid-template-columns: repeat(3, 1fr); 
                    gap: 32px; 
                }
                @media (max-width: 1024px) { .contact-grid { grid-template-columns: 1fr; max-width: 500px; margin: 0 auto; } }

                .contact-card { 
                    background: white; 
                    padding: 32px; 
                    border-radius: 24px;
                    box-shadow: 0 8px 24px rgba(0,0,0,0.1);
                    border: 2px solid #f1f5f9;
                    transition: 0.3s;
                }
                .contact-card:hover { transform: translateY(-4px); box-shadow: 0 16px 40px rgba(0,0,0,0.15); }

                .card-header { margin-bottom: 20px; }
                .contact-icon { 
                    width: 64px; 
                    height: 64px; 
                    border-radius: 16px; 
                    display: grid;
                    place-items: center;
                    font-size: 28px;
                    margin-bottom: 16px;
                    color: white;
                }
                .email-card .contact-icon { background: linear-gradient(135deg, #667eea, #764ba2); }
                .phone-card .contact-icon { background: linear-gradient(135deg, #f093fb, #f5576c); }
                .facebook-card .contact-icon { background: linear-gradient(135deg, #4facfe, #00f2fe); }

                .card-header h3 { font-size: 20px; font-weight: 800; color: #0f172a; }
                .contact-value { 
                    font-size: 18px; 
                    font-weight: 700; 
                    color: #1ea1f2; 
                    margin-bottom: 24px;
                    word-break: break-all;
                    line-height: 1.4;
                }
                .contact-value.small { font-size: 16px; }

                .card-actions { display: flex; gap: 12px; }
                .action-btn { 
                    flex: 1;
                    padding: 12px 20px; 
                    border-radius: 12px; 
                    font-weight: 700;
                    font-size: 14px;
                    cursor: pointer;
                    transition: 0.2s;
                    border: none;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    gap: 8px;
                }
                .action-btn.primary { background: linear-gradient(135deg, #667eea, #764ba2); color: white; }
                .action-btn.primary:hover { transform: scale(1.05); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
                .action-btn.secondary { background: #f1f5f9; color: #64748b; }
                .action-btn.secondary:hover { background: #e2e8f0; }

                /* Quick Links */
                .quick-links-section { padding: 60px 0; }
                .quick-links-card { 
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    padding: 60px 40px; 
                    border-radius: 32px;
                    text-align: center;
                    color: white;
                }
                .links-header h3 { font-size: 32px; font-weight: 800; margin: 16px 0 12px; }
                .links-header p { opacity: 0.9; font-size: 16px; margin-bottom: 32px; }

                .social-buttons { 
                    display: flex; 
                    gap: 20px; 
                    justify-content: center;
                    flex-wrap: wrap;
                }
                .social-btn { 
                    padding: 16px 40px; 
                    border-radius: 16px; 
                    font-weight: 700;
                    font-size: 16px;
                    text-decoration: none;
                    display: inline-flex;
                    align-items: center;
                    gap: 10px;
                    transition: 0.3s;
                }
                .social-btn.facebook { background: #1877f2; color: white; }
                .social-btn.messenger { background: linear-gradient(135deg, #00b2ff, #006aff); color: white; }
                .social-btn:hover { transform: scale(1.05); box-shadow: 0 8px 24px rgba(0,0,0,0.3); }

                /* FAQ */
                .faq-section { padding: 80px 0; }
                .faq-section h2 { text-align: center; font-size: 40px; font-weight: 800; color: #0f172a; margin-bottom: 60px; }
                .faq-grid { 
                    display: grid; 
                    grid-template-columns: repeat(2, 1fr); 
                    gap: 24px; 
                }
                @media (max-width: 768px) { .faq-grid { grid-template-columns: 1fr; } }

                .faq-item { 
                    background: white; 
                    padding: 24px; 
                    border-radius: 16px;
                    border: 1px solid #e5e7eb;
                    transition: 0.2s;
                }
                .faq-item:hover { border-color: #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.1); }
                .faq-item h4 { font-size: 18px; font-weight: 700; color: #0f172a; margin-bottom: 12px; }
                .faq-item p { color: #64748b; line-height: 1.6; margin: 0; }
            `}</style>
        </main>
    );
}
