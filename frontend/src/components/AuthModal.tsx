import React, { useState, useEffect, useRef } from 'react';
import { CloseOutlined } from '@ant-design/icons';

const AuthModal = ({ isOpen, onClose, initialView = 'login' }: { isOpen: boolean, onClose: () => void, initialView?: 'login' | 'signup' }) => {
    const [view, setView] = useState(initialView);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        email: '',
        fullName: ''
    });

    const modalRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        setView(initialView);
        setError(null);
    }, [initialView, isOpen]);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (modalRef.current && !modalRef.current.contains(event.target as Node)) onClose();
        };
        if (isOpen) {
            document.addEventListener('mousedown', handleClickOutside);
            document.body.style.overflow = 'hidden';
        }
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
            document.body.style.overflow = 'unset';
        };
    }, [isOpen, onClose]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            if (view === 'login') {
                const form = new FormData();
                form.append('Username', formData.username);
                form.append('Password', formData.password);

                const response = await fetch('http://localhost:5191/api/Auth/Login', {
                    method: 'POST',
                    body: form
                });

                const data = await response.json();

                if (response.ok) {
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('username', formData.username);
                    localStorage.setItem('fullname', data.fullName);
                    localStorage.setItem('role', data.role);
                    
                    // Redirect to admin page if role is Admin
                    if (data.role === 'Admin') {
                        window.location.href = '/admin';
                    } else {
                        window.location.reload();
                    }
                } else {
                    setError(data.message || 'Đăng nhập không thành công');
                }
            } else {
                const form = new FormData();
                form.append('Username', formData.username);
                form.append('Password', formData.password);
                form.append('Email', formData.email);
                form.append('FullName', formData.fullName);
                form.append('PhoneNumber', '0000000000');
                form.append('RoleId', '1');

                const response = await fetch('http://localhost:5191/api/Auth/Register', {
                    method: 'POST',
                    body: form
                });

                if (response.ok) {
                    setView('login');
                    setError(null);
                    alert('Đăng ký thành công! Vui lòng đăng nhập.');
                } else {
                    const data = await response.json();
                    setError(data.message || 'Đăng ký không thành công');
                }
            }
        } catch (err) {
            setError('Lỗi kết nối Server. Vui lòng thử lại sau.');
        } finally {
            setLoading(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="auth-overlay">
            <div className="auth-card" ref={modalRef}>
                <button className="auth-close" onClick={onClose}><CloseOutlined /></button>
                <div className="auth-header">
                    <img src="/media/logo.png" alt="GoWhere" className="auth-logo-img" />
                    <h2>{view === 'login' ? 'Chào mừng trở lại!' : 'Bắt đầu hành trình'}</h2>
                    <p>{view === 'login' ? 'Đăng nhập để quản lý lịch trình của bạn' : 'Tham gia cùng 10,000+ người du lịch thông minh'}</p>
                </div>

                <form className="auth-form" onSubmit={handleSubmit}>
                    {error && <div className="auth-error-msg">{error}</div>}

                    <div className="auth-field">
                        <label>Tên đăng nhập</label>
                        <input name="username" type="text" placeholder="Tên đăng nhập" required onChange={handleInputChange} />
                    </div>

                    {view === 'signup' && (
                        <>
                            <div className="auth-field">
                                <label>Họ và tên</label>
                                <input name="fullName" type="text" placeholder="Nguyễn Văn A" required onChange={handleInputChange} />
                            </div>
                            <div className="auth-field">
                                <label>Email</label>
                                <input name="email" type="email" placeholder="example@gmail.com" required onChange={handleInputChange} />
                            </div>
                        </>
                    )}

                    <div className="auth-field">
                        <label>Mật khẩu</label>
                        <input name="password" type="password" placeholder="••••••••" required onChange={handleInputChange} />
                    </div>

                    <button className={`btn primary auth-submit ${loading ? 'loading' : ''}`} disabled={loading}>
                        {loading ? 'Đang xử lý...' : (view === 'login' ? 'Đăng nhập' : 'Tạo tài khoản')}
                    </button>
                </form>

                <div className="auth-divider"><span>HOẶC</span></div>

                <div className="auth-social">
                    <button className="social-btn google">Tiếp tục với Google</button>
                </div>

                <div className="auth-footer">
                    {view === 'login' ? (
                        <p>Chưa có tài khoản? <span onClick={() => setView('signup')}>Đăng ký ngay</span></p>
                    ) : (
                        <p>Đã có tài khoản? <span onClick={() => setView('login')}>Đăng nhập</span></p>
                    )}
                </div>
            </div>
            <style jsx>{`
        .auth-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.6); backdrop-filter: blur(8px); z-index: 2000; display: grid; place-items: center; padding: 20px; animation: fadeIn 0.3s ease; }
        .auth-card { background: white; width: 100%; max-width: 440px; border-radius: 24px; padding: 40px; position: relative; box-shadow: 0 30px 60px rgba(0,0,0,0.2); animation: cardSlide 0.4s cubic-bezier(0.34, 1.56, 0.64, 1); }
        .auth-close { position: absolute; top: 20px; right: 24px; background: none; border: none; font-size: 20px; color: var(--muted); cursor: pointer; transition: 0.2s; }
        .auth-close:hover { color: var(--brand); transform: rotate(90deg); }
        
        .auth-header { text-align: center; margin-bottom: 32px; }
        .auth-logo-img { height: 48px; width: auto; object-fit: contain; margin: 0 auto 16px; display: block; }
        .auth-header h2 { font-size: 26px; font-weight: 800; color: #0f172a; margin-bottom: 8px; }
        .auth-header p { color: var(--muted); font-size: 15px; }

        .auth-form { display: flex; flex-direction: column; gap: 20px; }
        .auth-error-msg { background: #fee2e2; color: #dc2626; padding: 12px; border-radius: 12px; font-size: 14px; font-weight: 600; text-align: center; }
        .auth-field { display: flex; flex-direction: column; gap: 6px; }
        .auth-field label { font-size: 13px; font-weight: 700; color: #1e293b; }
        .auth-field input { padding: 12px 16px; border-radius: 12px; border: 1.5px solid var(--line); outline: none; transition: 0.3s; font-size: 15px; }
        .auth-field input:focus { border-color: var(--brand); box-shadow: 0 0 0 4px rgba(30, 161, 242, 0.15); }
        
        .auth-submit { padding: 14px; font-size: 16px; font-weight: 700; border-radius: 12px; margin-top: 10px; border: none; width: 100%; cursor: pointer; background: var(--brand); color: white; transition: 0.3s; }
        .auth-submit:hover:not(:disabled) { filter: brightness(1.1); transform: translateY(-2px); }
        .auth-submit:disabled { opacity: 0.7; cursor: not-allowed; }
        
        .auth-divider { position: relative; text-align: center; margin: 24px 0; }
        .auth-divider::before { content: ""; position: absolute; left: 0; top: 50%; width: 100%; height: 1px; background: var(--line); }
        .auth-divider span { position: relative; background: white; padding: 0 16px; font-size: 12px; font-weight: 700; color: var(--muted); }

        .auth-social { margin-bottom: 24px; }
        .social-btn { width: 100%; padding: 12px; border-radius: 12px; border: 1.5px solid var(--line); background: white; font-weight: 600; cursor: pointer; transition: 0.3s; display: flex; align-items: center; justify-content: center; gap: 10px; }
        .social-btn:hover { background: #f8fafc; border-color: var(--brand); }

        .auth-footer { text-align: center; font-size: 14px; color: var(--muted); }
        .auth-footer span { color: var(--brand); font-weight: 700; cursor: pointer; }
        .auth-footer span:hover { text-decoration: underline; }

        @keyframes fadeIn { from { opacity: 0; } }
        @keyframes cardSlide { from { transform: translateY(40px) scale(0.95); opacity: 0; } }
      `}</style>
        </div>
    );
};

export default AuthModal;
