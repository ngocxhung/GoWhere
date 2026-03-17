'use client';

import React, { useState, useEffect } from 'react';
import { createPortal } from 'react-dom';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { UserOutlined, CalendarOutlined, LogoutOutlined, SettingOutlined, MenuOutlined, CloseOutlined } from '@ant-design/icons';

const Header = ({ onAuthClick }: { onAuthClick: (v: 'login' | 'signup') => void }) => {
    const [scrolled, setScrolled] = useState(false);
    const [user, setUser] = useState<string | null>(null);
    const [fullName, setFullName] = useState<string | null>(null);
    const [role, setRole] = useState<string | null>(null);
    const [showDropdown, setShowDropdown] = useState(false);
    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
    const [mounted, setMounted] = useState(false);
    const pathname = usePathname();

    useEffect(() => {
        setMounted(true);
        const handleScroll = () => setScrolled(window.scrollY > 20);
        window.addEventListener('scroll', handleScroll);

        setUser(localStorage.getItem('username'));
        setFullName(localStorage.getItem('fullname'));
        setRole(localStorage.getItem('role'));

        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    // Khoá scroll body khi mở menu
    useEffect(() => {
        if (mobileMenuOpen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = '';
        }
        return () => { document.body.style.overflow = ''; };
    }, [mobileMenuOpen]);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('fullname');
        setUser(null);
        window.location.reload();
    };

    const mobileMenuJSX = mounted && mobileMenuOpen ? createPortal(
        <>
            {/* Overlay */}
            <div
                onClick={() => setMobileMenuOpen(false)}
                style={{
                    position: 'fixed',
                    inset: 0,
                    background: 'rgba(0,0,0,0.75)',
                    backdropFilter: 'blur(6px)',
                    WebkitBackdropFilter: 'blur(6px)',
                    zIndex: 2147483646,
                    animation: 'mobileOverlayIn 0.3s ease',
                }}
            />
            {/* Panel */}
            <div
                style={{
                    position: 'fixed',
                    top: 0,
                    right: 0,
                    bottom: 0,
                    width: '300px',
                    maxWidth: '85vw',
                    background: 'white',
                    zIndex: 2147483647,
                    display: 'flex',
                    flexDirection: 'column',
                    boxShadow: '-8px 0 32px rgba(0,0,0,0.3)',
                    animation: 'mobilePanelIn 0.3s cubic-bezier(0.16, 1, 0.3, 1)',
                    overflowY: 'auto',
                }}
            >
                {/* Header */}
                <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    padding: '20px 24px',
                    background: 'linear-gradient(135deg, #1ea1f2, #6366f1)',
                    color: 'white',
                    flexShrink: 0,
                }}>
                    <span style={{ fontWeight: 800, fontSize: '20px', letterSpacing: '-0.5px' }}>
                        Go<span style={{ color: 'rgba(255,255,255,0.8)' }}>Where</span>
                    </span>
                    <button
                        onClick={() => setMobileMenuOpen(false)}
                        style={{
                            background: 'rgba(255,255,255,0.2)',
                            border: 'none',
                            color: 'white',
                            width: '36px',
                            height: '36px',
                            borderRadius: '10px',
                            cursor: 'pointer',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            fontSize: '18px',
                        }}
                    >
                        <CloseOutlined />
                    </button>
                </div>

                {/* Nav Links */}
                <nav style={{ padding: '16px 12px', display: 'flex', flexDirection: 'column', gap: '4px' }}>
                    {[
                        { href: '/', label: '🏠 Trang chủ' },
                        { href: '/ai', label: '🤖 Trợ lý AI' },
                        { href: '/destinations', label: '🗺️ Khám phá' },
                        { href: '/support', label: '💬 Hỗ trợ' },
                    ].map(({ href, label }) => (
                        <Link
                            key={href}
                            href={href}
                            onClick={() => setMobileMenuOpen(false)}
                            style={{
                                display: 'block',
                                padding: '14px 16px',
                                borderRadius: '12px',
                                fontSize: '16px',
                                fontWeight: pathname === href ? 700 : 600,
                                color: pathname === href ? '#1ea1f2' : '#0f172a',
                                background: pathname === href ? '#f0f9ff' : 'transparent',
                                textDecoration: 'none',
                                transition: '0.2s',
                                borderLeft: pathname === href ? '4px solid #1ea1f2' : '4px solid transparent',
                            }}
                        >
                            {label}
                        </Link>
                    ))}
                </nav>

                <div style={{ height: '1px', background: '#f1f5f9', margin: '0 16px' }} />

                {/* User section */}
                {user ? (
                    <div style={{ padding: '16px 12px', display: 'flex', flexDirection: 'column', gap: '4px' }}>
                        {/* User info card */}
                        <div style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '12px',
                            padding: '16px',
                            background: '#f8fafc',
                            borderRadius: '12px',
                            marginBottom: '8px',
                        }}>
                            <div style={{
                                width: '44px',
                                height: '44px',
                                background: 'linear-gradient(135deg, #1ea1f2, #6366f1)',
                                color: 'white',
                                borderRadius: '50%',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontWeight: 800,
                                fontSize: '18px',
                                flexShrink: 0,
                            }}>
                                {(fullName || user).charAt(0).toUpperCase()}
                            </div>
                            <div>
                                <div style={{ fontWeight: 700, color: '#0f172a', fontSize: '15px' }}>{fullName || user}</div>
                                <div style={{ color: '#64748b', fontSize: '13px' }}>@{user}</div>
                            </div>
                        </div>

                        {[
                            { href: '/profile', icon: <UserOutlined />, label: 'Thông tin cá nhân' },
                            { href: '/itinerary', icon: <CalendarOutlined />, label: 'Hành trình của tôi' },
                            { href: '/history', icon: <CalendarOutlined />, label: 'Lịch sử AI' },
                            ...(role === 'Admin' ? [{ href: '/admin', icon: <SettingOutlined />, label: 'Quản lý thông tin', admin: true }] : []),
                        ].map((item: any) => (
                            <Link
                                key={item.href}
                                href={item.href}
                                onClick={() => setMobileMenuOpen(false)}
                                style={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '10px',
                                    padding: '12px 16px',
                                    borderRadius: '10px',
                                    fontSize: '14px',
                                    fontWeight: 600,
                                    color: item.admin ? '#6366f1' : '#0f172a',
                                    textDecoration: 'none',
                                    background: 'transparent',
                                    transition: '0.2s',
                                }}
                            >
                                {item.icon} {item.label}
                            </Link>
                        ))}

                        <div style={{ height: '1px', background: '#f1f5f9', margin: '4px 4px' }} />

                        <button
                            onClick={() => { handleLogout(); setMobileMenuOpen(false); }}
                            style={{
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px',
                                padding: '12px 16px',
                                borderRadius: '10px',
                                fontSize: '14px',
                                fontWeight: 600,
                                color: '#ef4444',
                                background: 'transparent',
                                border: 'none',
                                cursor: 'pointer',
                                width: '100%',
                                textAlign: 'left',
                            }}
                        >
                            <LogoutOutlined /> Đăng xuất
                        </button>
                    </div>
                ) : (
                    <div style={{ padding: '16px', display: 'flex', flexDirection: 'column', gap: '12px' }}>
                        <button
                            onClick={() => { onAuthClick('login'); setMobileMenuOpen(false); }}
                            style={{
                                padding: '14px',
                                border: '2px solid #1ea1f2',
                                borderRadius: '12px',
                                background: 'white',
                                color: '#1ea1f2',
                                fontWeight: 700,
                                fontSize: '15px',
                                cursor: 'pointer',
                                width: '100%',
                            }}
                        >
                            Đăng nhập
                        </button>
                        <button
                            onClick={() => { onAuthClick('signup'); setMobileMenuOpen(false); }}
                            style={{
                                padding: '14px',
                                border: 'none',
                                borderRadius: '12px',
                                background: 'linear-gradient(135deg, #1ea1f2, #6366f1)',
                                color: 'white',
                                fontWeight: 700,
                                fontSize: '15px',
                                cursor: 'pointer',
                                width: '100%',
                            }}
                        >
                            Bắt đầu ngay
                        </button>
                    </div>
                )}
            </div>

            {/* Keyframes qua style tag toàn cục */}
            <style>{`
                @keyframes mobileOverlayIn {
                    from { opacity: 0; }
                    to { opacity: 1; }
                }
                @keyframes mobilePanelIn {
                    from { transform: translateX(100%); }
                    to { transform: translateX(0); }
                }
            `}</style>
        </>,
        document.body
    ) : null;

    return (
        <>
            <header className={scrolled ? 'scrolled' : ''}>
                <div className="container nav">
                    <Link href="/" className="brand-link">
                        <div className="brand">
                            <img src="/media/logo.png" alt="GoWhere Logo" className="logo-img" />
                            <span className="brand-text">Go<span>Where</span></span>
                        </div>
                    </Link>

                    <nav className="main-nav">
                        <Link href="/" className={pathname === '/' ? 'active' : ''}>Trang chủ</Link>
                        <Link href="/ai" className={pathname === '/ai' ? 'active' : ''}>Trợ lý AI</Link>
                        <Link href="/destinations" className={pathname === '/destinations' ? 'active' : ''}>Khám phá</Link>
                        <Link href="/support" className={pathname === '/support' ? 'active' : ''}>Hỗ trợ</Link>
                    </nav>

                    {/* Hamburger button */}
                    <button className="mobile-menu-btn" onClick={() => setMobileMenuOpen(!mobileMenuOpen)}>
                        {mobileMenuOpen ? <CloseOutlined /> : <MenuOutlined />}
                    </button>

                    <div className="right">
                        {user ? (
                            <div className="user-profile" style={{ position: 'relative' }}>
                                <div className="user-trigger" onClick={() => setShowDropdown(!showDropdown)}>
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
                                            <Link href="/profile" className="dropdown-item"><UserOutlined /> Thông tin cá nhân</Link>
                                            <Link href="/itinerary" className="dropdown-item"><CalendarOutlined /> Hành trình của tôi</Link>
                                            <Link href="/history" className="dropdown-item"><CalendarOutlined /> Lịch sử AI</Link>
                                            {role === 'Admin' && (
                                                <>
                                                    <div className="dropdown-divider" />
                                                    <Link href="/admin" className="dropdown-item admin-item"><SettingOutlined /> Quản lý thông tin</Link>
                                                </>
                                            )}
                                            <div className="dropdown-divider" />
                                            <button className="dropdown-item logout-btn" onClick={handleLogout}><LogoutOutlined /> Đăng xuất</button>
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
                    header { position: sticky; top: 0; z-index: 1000; background: rgba(255,255,255,0.92); backdrop-filter: blur(12px); border-bottom: 1px solid var(--line); box-shadow: 0 2px 8px rgba(0,0,0,0.04); transition: all 0.3s; }
                    header.scrolled { box-shadow: 0 4px 20px rgba(0,0,0,0.08); background: rgba(255,255,255,0.96); }
                    .nav { display: flex; justify-content: space-between; align-items: center; padding: 16px 24px; max-width: 1400px; margin: 0 auto; }
                    .brand-link { text-decoration: none; color: inherit; display: block; }
                    .brand { display: flex !important; flex-direction: row !important; align-items: center !important; gap: 10px; cursor: pointer; transition: 0.3s; width: max-content; }
                    .logo-img { height: 44px; width: auto; object-fit: contain; flex-shrink: 0; }
                    .brand-text { font-family: 'Outfit', sans-serif; font-size: 24px; font-weight: 800; letter-spacing: -0.8px; color: var(--brand); display: flex; align-items: center; white-space: nowrap; margin: 0; }
                    .brand-text span { color: #0f172a; }

                    .main-nav { display: flex; gap: 20px; font-size: 14px; font-weight: 500; }
                    .main-nav :global(a) { color: var(--muted); text-decoration: none; transition: 0.2s; padding: 8px 16px; border-radius: 8px; position: relative; }
                    .main-nav :global(a:hover) { color: var(--brand); background: rgba(30,161,242,0.08); }
                    .main-nav :global(a.active) { color: var(--brand); font-weight: 700; background: rgba(30,161,242,0.1); }
                    .main-nav :global(a.active::after) { content: ''; position: absolute; bottom: 0; left: 16px; right: 16px; height: 3px; background: var(--brand); border-radius: 3px 3px 0 0; }

                    @media (max-width: 1024px) { .main-nav { display: none; } .right { display: none; } }

                    .right { display: flex; gap: 12px; align-items: center; }

                    /* Hamburger */
                    .mobile-menu-btn { display: none; width: 44px; height: 44px; background: white; border: 2px solid #e5e7eb; border-radius: 12px; cursor: pointer; align-items: center; justify-content: center; font-size: 20px; color: var(--brand); transition: 0.2s; }
                    .mobile-menu-btn:hover { background: #f8fafc; border-color: var(--brand); }
                    .mobile-menu-btn:active { transform: scale(0.95); }
                    @media (max-width: 1024px) { .mobile-menu-btn { display: flex; } }

                    /* Desktop dropdown */
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
                    @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
                `}</style>
            </header>

            {/* Portal: render mobile menu thẳng vào body */}
            {mobileMenuJSX}
        </>
    );
};

export default Header;
