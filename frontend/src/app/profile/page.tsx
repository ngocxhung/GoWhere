"use client";

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { ArrowLeftOutlined, UserOutlined, MailOutlined, PhoneOutlined, SafetyCertificateOutlined } from '@ant-design/icons';

export default function ProfilePage() {
    const [user, setUser] = useState<{ username: string, fullName: string, email: string, phoneNumber: string } | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProfile = async () => {
            const token = localStorage.getItem('token');
            if (!token) {
                setLoading(false);
                return;
            }

            try {
                const res = await fetch('http://localhost:5191/api/Auth/Me', {
                    headers: { 'Authorization': `Bearer ${token}` }
                });
                if (res.ok) {
                    const data = await res.json();
                    setUser(data);
                }
            } catch (error) {
                console.error("Failed to fetch profile", error);
            } finally {
                setLoading(false);
            }
        };
        fetchProfile();
    }, []);

    if (loading) return <div className="container" style={{ padding: '40px', textAlign: 'center' }}>Đang tải thông tin...</div>;

    if (!user) {
        return (
            <div className="container" style={{ padding: '40px', textAlign: 'center' }}>
                <h2>Vui lòng đăng nhập để xem thông tin</h2>
                <Link href="/" className="btn primary">Quay về trang chủ</Link>
            </div>
        );
    }

    return (
        <div className="page-container">
            <div className="container">
                <div className="header-simple">
                    <Link href="/" className="back-link"><ArrowLeftOutlined /> Quay lại</Link>
                    <h1>Hồ sơ cá nhân</h1>
                </div>

                <div className="profile-card">
                    <div className="avatar-section">
                        <div className="avatar-large">
                            {(user.fullName || user.username).charAt(0).toUpperCase()}
                        </div>
                        <h2>{user.fullName || user.username}</h2>
                        <p>@{user.username}</p>
                        <span className="badge">Thành viên thân thiết</span>
                    </div>

                    <div className="info-grid">
                        <div className="info-item">
                            <label><UserOutlined /> Tên đăng nhập</label>
                            <div className="val">{user.username}</div>
                        </div>
                        <div className="info-item">
                            <label><MailOutlined /> Email</label>
                            <div className="val">{user.email || 'Chưa cập nhật'}</div>
                        </div>
                        <div className="info-item">
                            <label><PhoneOutlined /> Số điện thoại</label>
                            <div className="val">{user.phoneNumber || 'Chưa cập nhật'}</div>
                        </div>
                        <div className="info-item">
                            <label><SafetyCertificateOutlined /> Trạng thái</label>
                            <div className="val" style={{ color: '#10b981' }}>Đã xác thực</div>
                        </div>
                    </div>

                    <div className="actions">
                        <button className="btn outline">Chỉnh sửa thông tin</button>
                        <button className="btn outline">Đổi mật khẩu</button>
                    </div>
                </div>
            </div>

            <style jsx>{`
        .page-container { background: #f8fafc; min-height: 100vh; padding-bottom: 80px; }
        .container { max-width: 800px; margin: 0 auto; padding: 40px 20px; }
        .header-simple { display: flex; align-items: center; gap: 20px; margin-bottom: 40px; }
        .back-link { color: var(--muted); font-weight: 600; display: flex; align-items: center; gap: 8px; transition: 0.2s; }
        .back-link:hover { color: var(--brand); }
        h1 { margin: 0; font-size: 28px; }

        .profile-card { background: white; border-radius: 24px; padding: 40px; box-shadow: 0 10px 40px rgba(0,0,0,0.05); text-align: center; }
        .avatar-large { width: 100px; height: 100px; background: var(--brand); color: white; border-radius: 50%; display: grid; place-items: center; font-size: 40px; font-weight: 800; margin: 0 auto 20px; box-shadow: 0 10px 20px rgba(30, 161, 242, 0.3); }
        .avatar-section h2 { margin-bottom: 4px; font-size: 24px; }
        .avatar-section p { color: var(--muted); margin-bottom: 12px; }
        .badge { background: #e0f2fe; color: var(--brand); padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 700; text-transform: uppercase; }

        .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; margin: 40px 0; text-align: left; }
        @media (max-width: 600px) { .info-grid { grid-template-columns: 1fr; } }
        
        .info-item label { color: var(--muted); font-size: 13px; font-weight: 600; display: block; margin-bottom: 8px; display: flex; align-items: center; gap: 6px; }
        .val { font-size: 16px; font-weight: 600; color: #0f172a; border: 1px solid var(--line); padding: 12px; border-radius: 12px; background: #f8fbff; }

        .actions { display: flex; justify-content: center; gap: 16px; border-top: 1px solid var(--line); padding-top: 30px; }
        .btn { padding: 12px 24px; border-radius: 12px; font-weight: 600; cursor: pointer; transition: 0.2s; }
        .btn.primary { background: var(--brand); color: white; border: none; }
        .btn.outline { background: white; border: 1px solid var(--line); color: var(--ink); }
        .btn.outline:hover { border-color: var(--brand); color: var(--brand); }
      `}</style>
        </div>
    );
}
