"use client";

import React, { useState, useEffect } from 'react';
import Header from './Header';
import AuthModal from './AuthModal';

export default function MainLayout({ children }: { children: React.ReactNode }) {
    const [authModal, setAuthModal] = useState<{ open: boolean, view: 'login' | 'signup' }>({
        open: false,
        view: 'login'
    });

    useEffect(() => {
        const handleAuthEvent = (e: Event) => {
            const customEvent = e as CustomEvent;
            setAuthModal({ open: true, view: customEvent.detail });
        };

        window.addEventListener('open-auth-modal', handleAuthEvent);
        return () => window.removeEventListener('open-auth-modal', handleAuthEvent);
    }, []);

    return (
        <>
            <Header onAuthClick={(view) => setAuthModal({ open: true, view })} />

            <main>
                {children}
            </main>

            <AuthModal
                isOpen={authModal.open}
                onClose={() => setAuthModal({ ...authModal, open: false })}
                initialView={authModal.view}
            />
        </>
    );
}
