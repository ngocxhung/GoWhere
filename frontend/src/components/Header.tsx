import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { UserOutlined, CalendarOutlined, LogoutOutlined, SettingOutlined } from '@ant-design/icons';

const Header = ({ onAuthClick }: { onAuthClick: (v: 'login' | 'signup') => void }) => {
    const [scrolled, setScrolled] = useState(false);
    const [user, setUser] = useState<string | null>(null);
    const [fullName, setFullName] = useState<string | null>(null);
    const [role, setRole] = useState<string | null>(null);
    const [showDropdown, setShowDropdown] = useState(false);

    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 20);
        window.addEventListener('scroll', handleScroll);

        // Check local storage for user info
        setUser(localStorage.getItem('username'));
        setFullName(localStorage.getItem('fullname'));
        setRole(localStorage.getItem('role'));

        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('fullname');
        setUser(null);
        window.location.reload();
    };

    return (
        <header className={scrolled ? 'scrolled' : ''}>
            <div className="container nav">
                <Link href="/" className="brand-link">
                    <div className="brand">
                        <img src="/media/logo.png" alt="GoWhere Logo" className="logo-img" />
                        <span className="brand-text">Go<span>Where</span></span>
                    </div>
                </Link>
                <nav className="main-nav">
                    <Link href="/ai" style={{ color: '#667eea', fontWeight: '700' }}>Trợ lý</Link>
                    <Link href="/hotels">Khách sạn</Link>
                    <Link href="/flights">Vé máy bay</Link>
                    <Link href="/cars">Cho thuê xe</Link>
                    <Link href="/activities">Hoạt động</Link>
                    <Link href="/offers">Ưu đãi</Link>
                    <a href="#">Hỗ trợ</a>
                </nav>
                <div className="right">
                    {user ? (
                        <div className="user-profile" style={{ position: 'relative' }}>
                            <div
                                className="user-trigger"
                                onClick={() => setShowDropdown(!showDropdown)}
                            >
                                <div className="avatar">
                                    {(fullName || user).charAt(0).toUpperCase()}
                                </div>
                                <span className="welcome-msg">Chào, <strong>{fullName || user}</strong></span>
                                <span style={{ fontSize: '10px', marginLeft: '4px' }}>▼</span>
                            </div>

                            {showDropdown && (
                                <>
                                    <div className="dropdown-overlay" onClick={() => setShowDropdown(false)} />
                                    <div className="user-dropdown">
                                        <div className="dropdown-header">
                                            <strong>{fullName || user}</strong>
                                            <span>@{user}</span>
                                        </div>
                                        <div className="dropdown-divider" />
                                        <Link href="/profile" className="dropdown-item">
                                            <UserOutlined /> Thông tin cá nhân
                                        </Link>
                                        <Link href="/itinerary" className="dropdown-item">
                                            <CalendarOutlined /> Hành trình của tôi
                                        </Link>
                                        <Link href="/history" className="dropdown-item">
                                            <CalendarOutlined /> Lịch sử AI
                                        </Link>

                                        {role === 'Admin' && (
                                            <>
                                                <div className="dropdown-divider" />
                                                <Link href="/admin" className="dropdown-item admin-item">
                                                    <SettingOutlined /> Quản lý thông tin
                                                </Link>
                                            </>
                                        )}

                                        <div className="dropdown-divider" />
                                        <button className="dropdown-item logout-btn" onClick={handleLogout}>
                                            <LogoutOutlined /> Đăng xuất
                                        </button>
                                    </div>
                                </>
                            )}
                        </div>
                    ) : (
                        <>
                            <button className="btn ghost" onClick={() => onAuthClick('login')}>Đăng nhập</button>
                            <button className="btn primary" onClick={() => onAuthClick('signup')}>Bắt đầu ngay</button>
                        </>
                    )}
                </div>
            </div>
            <style jsx>{`
          header { position: sticky; top: 0; z-index: 100; background: rgba(255, 255, 255, 0.92); backdrop-filter: blur(12px); border-bottom: 1px solid var(--line); box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04); transition: all 0.3s; }
          header.scrolled { box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08); background: rgba(255, 255, 255, 0.96); }
          .nav { display: flex; justify-content: space-between; align-items: center; padding: 16px 24px; max-width: 1400px; margin: 0 auto; }
          .brand-link { text-decoration: none; color: inherit; display: block; }
          .brand { display: flex !important; flex-direction: row !important; align-items: center !important; gap: 10px; cursor: pointer; transition: 0.3s; width: max-content; }
          .logo-img { height: 44px; width: auto; object-fit: contain; flex-shrink: 0; }
          .brand-text { font-family: 'Outfit', sans-serif; font-size: 24px; font-weight: 800; letter-spacing: -0.8px; color: var(--brand); display: flex; align-items: center; white-space: nowrap; margin: 0; }
          .brand-text span { color: #0f172a; }
          .main-nav { display: flex; gap: 20px; font-size: 14px; font-weight: 500; }
          .main-nav :global(a) { color: var(--muted); text-decoration: none; transition: 0.2s; }
          .main-nav :global(a:hover) { color: var(--brand); }
          @media (max-width: 1024px) { .main-nav { display: none; } }
          .right { display: flex; gap: 12px; align-items: center; }
          
          .user-trigger { display: flex; align-items: center; gap: 10px; cursor: pointer; padding: 4px 12px; border-radius: 30px; transition: 0.2s; border: 1px solid transparent; }
          .user-trigger:hover { background: rgba(0,0,0,0.03); border-color: var(--line); }
          .avatar { width: 32px; height: 32px; background: var(--brand); color: white; border-radius: 50%; display: grid; place-items: center; font-weight: 700; font-size: 14px; }
          .welcome-msg { font-size: 14px; color: var(--ink); }

          .dropdown-overlay { position: fixed; inset: 0; z-index: 90; }
          .user-dropdown { position: absolute; top: 100%; right: 0; width: 240px; background: white; border-radius: 16px; box-shadow: 0 10px 40px rgba(0,0,0,0.1); border: 1px solid var(--line); padding: 8px; z-index: 100; margin-top: 8px; animation: fadeIn 0.1s ease-out; display: flex; flex-direction: column; }
          .dropdown-header { padding: 12px 16px; display: flex; flex-direction: column; }
          .dropdown-header strong { color: var(--ink); font-size: 15px; }
          .dropdown-header span { color: var(--muted); font-size: 13px; }
          .dropdown-divider { height: 1px; background: var(--line); margin: 4px 0; }
          
          .dropdown-item { display: flex; align-items: center; gap: 10px; padding: 10px 16px; color: var(--ink); font-size: 14px; font-weight: 500; border-radius: 8px; cursor: pointer; border: none; background: none; width: 100%; text-align: left; transition: 0.2s; text-decoration: none; }
          .dropdown-item:hover { background: #f1f5f9; color: var(--brand); }
          .dropdown-item.admin-item { color: #6366f1; font-weight: 700; }
          .dropdown-item.admin-item:hover { background: #eef2ff; }
          .dropdown-item.logout-btn { color: #ef4444; }
          .dropdown-item.logout-btn:hover { background: #fef2f2; }
      `}</style>
        </header>
    );
};

export default Header;
