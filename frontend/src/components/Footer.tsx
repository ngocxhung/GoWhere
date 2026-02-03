"use client";
import React from 'react';

const Footer = () => {
    return (
        <footer className="footer-v2">
            <div className="container footer-content">
                <div className="footer-brand">
                    <img src="/media/logo.png" alt="GoWhere Logo" className="logo-img-small" />
                    <span className="brand-text-small">Go<span>Where</span> AI</span>
                </div>
                <div className="footer-links">
                    <a href="#">Về chúng tôi</a>
                    <a href="#">Hợp tác</a>
                    <a href="#">Điều khoản</a>
                    <a href="#">Chính sách bảo mật</a>
                </div>
                <p>© 2025 GoWhere AI • Proudly built for next-gen travel.</p>
            </div>
            <style jsx>{`
        .footer-v2 { padding: 80px 0; background: #0f172a; color: white; text-align: center; margin-top: 100px; }
        .footer-content { display: flex; flex-direction: column; align-items: center; gap: 32px; }
        .footer-brand { display: flex; align-items: center; justify-content: center; gap: 12px; margin-bottom: 12px; }
        .logo-img-small { height: 32px; width: auto; object-fit: contain; }
        .brand-text-small { font-family: 'Outfit', sans-serif; font-size: 20px; font-weight: 800; letter-spacing: -0.5px; color: var(--brand); }
        .brand-text-small span { color: white; }
        .footer-links { display: flex; gap: 30px; }
        .footer-links a { color: rgba(255,255,255,0.7); font-size: 14px; text-decoration: none; }
        .footer-links a:hover { color: white; }
        .footer-v2 p { font-size: 14px; opacity: 0.5; }
      `}</style>
        </footer>
    );
};

export default Footer;
