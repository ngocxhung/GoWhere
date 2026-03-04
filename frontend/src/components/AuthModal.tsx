import React, { useState, useEffect, useRef } from 'react';
import { CloseOutlined } from '@ant-design/icons';

const AuthModal = ({ isOpen, onClose, initialView = 'login' }: { isOpen: boolean, onClose: () => void, initialView?: 'login' | 'signup' }) => {
    const [view, setView] = useState(initialView);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
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
        setFieldErrors({});
        setFormData({
            username: '',
            password: '',
            email: '',
            fullName: ''
        });
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
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
        // Clear field error when user starts typing
        if (fieldErrors[name]) {
            setFieldErrors({ ...fieldErrors, [name]: '' });
        }
    };

    // Validation functions
    const validateLogin = (): boolean => {
        const errors: Record<string, string> = {};

        if (!formData.username.trim()) {
            errors.username = 'Vui lòng nhập tên đăng nhập';
        }

        if (!formData.password) {
            errors.password = 'Vui lòng nhập mật khẩu';
        } else if (formData.password.length < 6) {
            errors.password = 'Mật khẩu phải có ít nhất 6 ký tự';
        }

        setFieldErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const validateRegister = (): boolean => {
        const errors: Record<string, string> = {};

        if (!formData.username.trim()) {
            errors.username = 'Vui lòng nhập tên đăng nhập';
        } else if (formData.username.length > 50) {
            errors.username = 'Tên đăng nhập không được quá 50 ký tự';
        }

        if (!formData.password) {
            errors.password = 'Vui lòng nhập mật khẩu';
        } else if (formData.password.length < 6) {
            errors.password = 'Mật khẩu phải có ít nhất 6 ký tự';
        }

        if (!formData.email.trim()) {
            errors.email = 'Vui lòng nhập email';
        } else {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(formData.email)) {
                errors.email = 'Email không hợp lệ';
            } else if (formData.email.length > 100) {
                errors.email = 'Email không được quá 100 ký tự';
            }
        }

        if (!formData.fullName.trim()) {
            errors.fullName = 'Vui lòng nhập họ và tên';
        } else if (formData.fullName.length > 100) {
            errors.fullName = 'Họ và tên không được quá 100 ký tự';
        }

        setFieldErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const parseBackendErrors = (errorData: any): { general: string | null, fields: Record<string, string> } => {
        const result = {
            general: null as string | null,
            fields: {} as Record<string, string>
        };

        // Handle ModelState errors from backend
        if (errorData.errors) {
            const modelErrors = errorData.errors;

            Object.keys(modelErrors).forEach(key => {
                const fieldErrors = modelErrors[key];
                if (Array.isArray(fieldErrors) && fieldErrors.length > 0) {
                    // Map field names (Backend DTO -> Frontend state name)
                    // Frontend uses lowercase: username, password, email, fullName
                    let fieldName = '';
                    if (key.toLowerCase() === 'username') fieldName = 'username';
                    else if (key.toLowerCase() === 'password') fieldName = 'password';
                    else if (key.toLowerCase() === 'email') fieldName = 'email';
                    else if (key.toLowerCase() === 'fullname') fieldName = 'fullName';
                    else if (key.toLowerCase() === 'phonenumber') fieldName = 'phoneNumber';

                    if (fieldName) {
                        const err = fieldErrors[0]; // Take the first error
                        // Translate common validation messages
                        let translatedErr = err;
                        if (err.includes('required')) {
                            translatedErr = 'Thông tin này là bắt buộc';
                        } else if (err.includes('invalid') || err.includes('Invalid')) {
                            translatedErr = 'Không hợp lệ';
                        } else if (err.includes('longer than') || err.includes('cannot be longer')) {
                            translatedErr = err.replace(/cannot be longer than (\d+) characters?/i, 'không được quá $1 ký tự');
                        }
                        result.fields[fieldName] = translatedErr;
                    }
                }
            });

            // If we found field errors, we might not need a general error.
            // But if ONLY general errors exist in "errors" (unlikely for ModelState), handle them.
        }

        // Handle simple message errors or exceptions
        if (errorData.message) {
            const message = errorData.message.toLowerCase();

            // Map specific business logic errors to fields
            if (message.includes('username already exists') || message.includes('tên đăng nhập đã tồn tại')) {
                result.fields['username'] = 'Tên đăng nhập đã được sử dụng';
            }
            else if (message.includes('invalid username or password') || message.includes('sai tên đăng nhập hoặc mật khẩu')) {
                // For security, usually keep vague, but user wants field errors.
                // We can put it in password or keep as general. 
                // Let's keep it general as it involves both.
                result.general = 'Tên đăng nhập hoặc mật khẩu không chính xác';
            }
            else if (message.includes('login failed')) {
                if (!result.general) result.general = 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.';
            }
            else if (message.includes('registration failed')) {
                if (!result.general) result.general = 'Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.';
            }
            else if (message.includes('an error occurred')) {
                if (!result.general) result.general = 'Đã xảy ra lỗi. Vui lòng thử lại sau.';
            }
            else {
                // Unknown message, set as general
                if (!result.general && Object.keys(result.fields).length === 0) {
                    result.general = errorData.message;
                }
            }
        }

        // Handle Details field (from 500 errors)
        if (errorData.details) {
            const details = errorData.details.toLowerCase();
            if (details.includes('username already exists')) {
                result.fields['username'] = 'Tên đăng nhập đã được sử dụng';
            }
        }

        // If no errors parsed but we know there was an error
        if (!result.general && Object.keys(result.fields).length === 0) {
            result.general = 'Đã xảy ra lỗi. Vui lòng thử lại.';
        }

        return result;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setFieldErrors({});

        // Frontend validation first
        if (view === 'login') {
            if (!validateLogin()) {
                return;
            }
        } else {
            if (!validateRegister()) {
                return;
            }
        }

        setLoading(true);

        try {
            if (view === 'login') {
                const form = new FormData();
                form.append('Username', formData.username.trim());
                form.append('Password', formData.password);

                const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Auth/Login`, {
                    method: 'POST',
                    body: form
                });

                let data: any = {};
                try {
                    const contentType = response.headers.get('content-type');
                    if (contentType && contentType.includes('application/json')) {
                        data = await response.json();
                    }
                } catch (parseError) {
                    console.error('Error parsing response:', parseError);
                }

                if (response.ok) {
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('username', formData.username);
                    localStorage.setItem('fullname', data.fullName);
                    localStorage.setItem('role', data.role);

                    if (data.role === 'Admin') {
                        window.location.href = '/admin';
                    } else {
                        window.location.reload();
                    }
                } else {
                    // Connected to server but received error response
                    const parsed = parseBackendErrors(data);

                    // Priority handling for Login errors:
                    // If backend returns 401 (Unauthorized) OR general failure without specific field errors
                    if (response.status === 401 || (!parsed.general && Object.keys(parsed.fields).length === 0)) {
                        parsed.general = 'Tên đăng nhập hoặc mật khẩu không hợp lệ';
                    } else if (response.status === 500) {
                        // For 500 errors, if it looks like an auth error, say so. Otherwise generic server error.
                        if (parsed.general && (parsed.general.includes('không chính xác') || parsed.general.includes('thất bại'))) {
                            parsed.general = 'Tên đăng nhập hoặc mật khẩu không hợp lệ';
                        } else if (!parsed.general) {
                            parsed.general = 'Đã xảy ra lỗi từ phía máy chủ.';
                        }
                    }

                    if (parsed.general) setError(parsed.general);
                    if (Object.keys(parsed.fields).length > 0) setFieldErrors(parsed.fields);
                }
            } else {
                // ... (Register logic remains similar) ...
                const form = new FormData();
                form.append('Username', formData.username.trim());
                form.append('Password', formData.password);
                form.append('Email', formData.email.trim());
                form.append('FullName', formData.fullName.trim());
                form.append('PhoneNumber', '0000000000');
                form.append('RoleId', '1');

                const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Auth/Register`, {
                    method: 'POST',
                    body: form
                });

                let data: any = {};
                try {
                    const contentType = response.headers.get('content-type');
                    if (contentType && contentType.includes('application/json')) {
                        data = await response.json();
                    }
                } catch (parseError) {
                    console.error('Error parsing response:', parseError);
                }

                if (response.ok) {
                    setView('login');
                    setError(null);
                    setFieldErrors({});
                    alert('Đăng ký thành công! Vui lòng đăng nhập.');
                } else {
                    const parsed = parseBackendErrors(data);
                    if (parsed.general) setError(parsed.general);
                    if (Object.keys(parsed.fields).length > 0) setFieldErrors(parsed.fields);
                }
            }
        } catch (err) {
            // Network error / No connection to server
            console.error('Request error:', err);
            setError('Đã xảy ra lỗi kết nối Server. Vui lòng thử lại sau.');
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
                        <input
                            name="username"
                            type="text"
                            placeholder="Tên đăng nhập"
                            value={formData.username}
                            onChange={handleInputChange}
                            className={fieldErrors.username ? 'error' : ''}
                        />
                        {fieldErrors.username && <span className="field-error">{fieldErrors.username}</span>}
                    </div>

                    {view === 'signup' && (
                        <>
                            <div className="auth-field">
                                <label>Họ và tên</label>
                                <input
                                    name="fullName"
                                    type="text"
                                    placeholder="Nguyễn Văn A"
                                    value={formData.fullName}
                                    onChange={handleInputChange}
                                    className={fieldErrors.fullName ? 'error' : ''}
                                />
                                {fieldErrors.fullName && <span className="field-error">{fieldErrors.fullName}</span>}
                            </div>
                            <div className="auth-field">
                                <label>Email</label>
                                <input
                                    name="email"
                                    type="email"
                                    placeholder="example@gmail.com"
                                    value={formData.email}
                                    onChange={handleInputChange}
                                    className={fieldErrors.email ? 'error' : ''}
                                />
                                {fieldErrors.email && <span className="field-error">{fieldErrors.email}</span>}
                            </div>
                        </>
                    )}

                    <div className="auth-field">
                        <label>Mật khẩu</label>
                        <input
                            name="password"
                            type="password"
                            placeholder="••••••••"
                            value={formData.password}
                            onChange={handleInputChange}
                            className={fieldErrors.password ? 'error' : ''}
                        />
                        {fieldErrors.password && <span className="field-error">{fieldErrors.password}</span>}
                    </div>

                    <button className={`btn primary auth-submit ${loading ? 'loading' : ''}`} disabled={loading}>
                        {loading ? 'Đang xử lý...' : (view === 'login' ? 'Đăng nhập' : 'Tạo tài khoản')}
                    </button>
                </form>

                {view === 'login' && (
                    <div className="auth-forgot">
                        <span>Quên mật khẩu?</span>
                    </div>
                )}

                <div className="auth-footer">
                    {view === 'login' ? (
                        <p>Chưa có tài khoản? <span onClick={() => {
                            setView('signup');
                            setError(null);
                            setFieldErrors({});
                            setFormData({ username: '', password: '', email: '', fullName: '' });
                        }}>Đăng ký ngay</span></p>
                    ) : (
                        <p>Đã có tài khoản? <span onClick={() => {
                            setView('login');
                            setError(null);
                            setFieldErrors({});
                            setFormData({ username: '', password: '', email: '', fullName: '' });
                        }}>Đăng nhập</span></p>
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
        .auth-field input.error { border-color: #dc2626; }
        .auth-field input.error:focus { border-color: #dc2626; box-shadow: 0 0 0 4px rgba(220, 38, 38, 0.15); }
        .field-error { color: #dc2626; font-size: 12px; font-weight: 500; margin-top: -2px; }
        
        .auth-submit { padding: 14px; font-size: 16px; font-weight: 700; border-radius: 12px; margin-top: 10px; border: none; width: 100%; cursor: pointer; background: var(--brand); color: white; transition: 0.3s; }
        .auth-submit:hover:not(:disabled) { filter: brightness(1.1); transform: translateY(-2px); }
        .auth-submit:disabled { opacity: 0.7; cursor: not-allowed; }
        
        .auth-forgot { text-align: center; margin: 16px 0 8px; font-size: 14px; }
        .auth-forgot span { color: var(--brand); font-weight: 600; cursor: pointer; }
        .auth-forgot span:hover { text-decoration: underline; }

        .auth-footer { text-align: center; font-size: 14px; color: var(--muted); margin-top: 16px; }
        .auth-footer span { color: var(--brand); font-weight: 700; cursor: pointer; }
        .auth-footer span:hover { text-decoration: underline; }

        @keyframes fadeIn { from { opacity: 0; } }
        @keyframes cardSlide { from { transform: translateY(40px) scale(0.95); opacity: 0; } }
      `}</style>
        </div>
    );
};

export default AuthModal;
