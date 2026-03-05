"use client";

import React, { useState, useEffect } from 'react';
import { usePathname } from 'next/navigation';
import Header from './Header';
import AuthModal from './AuthModal';

export default function MainLayout({ children }: { children: React.ReactNode }) {
    const [authModal, setAuthModal] = useState<{ open: boolean, view: 'login' | 'signup' }>({
        open: false,
        view: 'login'
    });
    const [showSurvey, setShowSurvey] = useState(false);
    
    const pathname = usePathname();
    const isAdminPage = pathname?.startsWith('/admin');

    useEffect(() => {
        const handleAuthEvent = (e: Event) => {
            const customEvent = e as CustomEvent;
            setAuthModal({ open: true, view: customEvent.detail });
        };

        window.addEventListener('open-auth-modal', handleAuthEvent);
        return () => window.removeEventListener('open-auth-modal', handleAuthEvent);
    }, []);

    useEffect(() => {
        // Hiển thị modal khảo sát cho người dùng, tránh hiển thị ở trang admin
        if (isAdminPage) return;

        if (typeof window !== 'undefined') {
            const dismissed = window.localStorage.getItem('gowhere_survey_dismissed');
            if (!dismissed) {
                setShowSurvey(true);
            }
        }
    }, [isAdminPage]);

    const closeSurvey = () => {
        setShowSurvey(false);
        if (typeof window !== 'undefined') {
            window.localStorage.setItem('gowhere_survey_dismissed', '1');
        }
    };

    const openSurvey = () => {
        if (typeof window !== 'undefined') {
            window.open('https://forms.gle/nfZ6h2JyzKbgShXF6', '_blank', 'noopener,noreferrer');
        }
        closeSurvey();
    };

    return (
        <>
            {!isAdminPage && <Header onAuthClick={(view) => setAuthModal({ open: true, view })} />}

            <main>
                {children}
            </main>

            <AuthModal
                isOpen={authModal.open}
                onClose={() => setAuthModal({ ...authModal, open: false })}
                initialView={authModal.view}
            />

            {showSurvey && !isAdminPage && (
                <div className="survey-overlay" onClick={closeSurvey}>
                    <div className="survey-modal" onClick={(e) => e.stopPropagation()}>
                        <div className="survey-badge">Khảo sát nhanh 🎯</div>
                        <h3>Giúp GoWhere tốt hơn cho bạn?</h3>
                        <p>
                            Bạn có thể dành vài phút để chia sẻ trải nghiệm của mình với GoWhere không?
                            Những góp ý của bạn sẽ giúp chúng tôi cải thiện sản phẩm tốt hơn cho cộng đồng du lịch.
                        </p>
                        <button className="survey-cta" onClick={openSurvey}>
                            Điền khảo sát ngay
                        </button>
                        <button className="survey-skip" onClick={closeSurvey}>
                            Để sau
                        </button>
                    </div>
                </div>
            )}

            <style jsx>{`
                .survey-overlay {
                    position: fixed;
                    inset: 0;
                    background: rgba(15, 23, 42, 0.45);
                    backdrop-filter: blur(6px);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    z-index: 1600;
                }
                .survey-modal {
                    width: 100%;
                    max-width: 420px;
                    background: #ffffff;
                    border-radius: 24px;
                    padding: 28px 24px 24px;
                    box-shadow: 0 24px 60px rgba(15, 23, 42, 0.35);
                    text-align: left;
                    position: relative;
                    transform: translateY(0);
                    animation: survey-pop 0.3s ease-out;
                }
                .survey-badge {
                    display: inline-flex;
                    align-items: center;
                    gap: 6px;
                    padding: 4px 10px;
                    border-radius: 999px;
                    font-size: 11px;
                    font-weight: 700;
                    text-transform: uppercase;
                    letter-spacing: 0.08em;
                    background: rgba(56, 189, 248, 0.12);
                    color: #0369a1;
                    margin-bottom: 10px;
                }
                .survey-modal h3 {
                    font-size: 20px;
                    font-weight: 800;
                    margin: 0 0 10px;
                    color: #0f172a;
                }
                .survey-modal p {
                    font-size: 14px;
                    line-height: 1.6;
                    color: #64748b;
                    margin: 0 0 18px;
                }
                .survey-cta {
                    width: 100%;
                    padding: 12px 16px;
                    border-radius: 999px;
                    border: none;
                    cursor: pointer;
                    font-size: 15px;
                    font-weight: 700;
                    background: linear-gradient(135deg, #1ea1f2, #0ea5e9);
                    color: #ffffff;
                    box-shadow: 0 12px 30px rgba(14, 165, 233, 0.4);
                    transition: transform 0.15s ease, box-shadow 0.15s ease;
                    margin-bottom: 8px;
                }
                .survey-cta:hover {
                    transform: translateY(-1px);
                    box-shadow: 0 16px 40px rgba(14, 165, 233, 0.5);
                }
                .survey-skip {
                    width: 100%;
                    padding: 10px 14px;
                    border-radius: 999px;
                    border: none;
                    cursor: pointer;
                    font-size: 13px;
                    font-weight: 600;
                    background: transparent;
                    color: #64748b;
                }
                @keyframes survey-pop {
                    from {
                        opacity: 0;
                        transform: translateY(10px) scale(0.96);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0) scale(1);
                    }
                }
                @media (max-width: 640px) {
                    .survey-modal {
                        margin: 0 16px;
                    }
                }
            `}</style>
        </>
    );
}
