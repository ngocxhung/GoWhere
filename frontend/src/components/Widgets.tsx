"use client";
import React, { useState, useEffect } from 'react';
import { VideoCameraOutlined, CloseOutlined, CustomerServiceOutlined, SendOutlined } from '@ant-design/icons';
import { danangReels } from '@/dummy/data';

export const ReelsWidget = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [index, setIndex] = useState(0);
    const widgetRef = React.useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (widgetRef.current && !widgetRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };
        if (isOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isOpen]);

    return (
        <>
            <div className="reel-float" onClick={() => setIsOpen(true)}>
                <div className="pulse-ring"></div>
                <VideoCameraOutlined style={{ fontSize: '24px', position: 'relative', zIndex: 1 }} />
            </div>
            {isOpen && (
                <div className="reel-overlay-bg">
                    <div className="reel-box" ref={widgetRef}>
                        <div className="reel-header">
                            <div className="header-info">
                                <VideoCameraOutlined className="spinning-icon" />
                                <span>GoWhere Reels</span>
                            </div>
                            <button className="close-btn" onClick={() => setIsOpen(false)}><CloseOutlined /></button>
                        </div>
                        <div className="reel-video-container">
                            <video
                                key={danangReels[index].src}
                                src={danangReels[index].src}
                                autoPlay
                                onEnded={() => setIndex((index + 1) % danangReels.length)}
                                className="reel-video"
                            />
                            <div className="reel-gradient-top"></div>
                            <div className="reel-overlay">
                                <p>{danangReels[index].cap}</p>
                                <div className="reel-actions">
                                    <button className="next-btn" onClick={() => setIndex((index + 1) % danangReels.length)}>
                                        Video tiếp theo ↓
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            )}
            <style jsx>{`
          .reel-overlay-bg { position: fixed; inset: 0; background: rgba(0,0,0,0.7); backdrop-filter: blur(8px); z-index: 2000; display: flex; align-items: center; justify-content: center; pointer-events: auto; }
          
          .reel-float { position: fixed; right: 30px; bottom: 30px; width: 65px; height: 65px; border-radius: 50%; background: linear-gradient(135deg, #1ea1f2, #0d87d4); color: white; display: grid; place-items: center; cursor: pointer; box-shadow: 0 10px 30px rgba(30,161,242,0.4); z-index: 1000; transition: all 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275); }
          .reel-float:hover { transform: scale(1.15) rotate(5deg); box-shadow: 0 15px 40px rgba(30,161,242,0.6); }
          
          .pulse-ring { position: absolute; width: 100%; height: 100%; border-radius: 50%; background: #1ea1f2; opacity: 0; animation: pulse 2s infinite; }
          @keyframes pulse { 0% { transform: scale(1); opacity: 0.5; } 100% { transform: scale(1.6); opacity: 0; } }

          .reel-box { position: relative; width: 380px; height: 85vh; max-height: 750px; background: #000; border-radius: 24px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 30px 60px rgba(0,0,0,0.8); border: 1px solid rgba(255,255,255,0.15); animation: slideUp 0.6s cubic-bezier(0.23, 1, 0.32, 1); }
          @keyframes slideUp { from { transform: translateY(50px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }

          .reel-header { height: 60px; background: rgba(0,0,0,0.8); backdrop-filter: blur(10px); color: white; display: flex; justify-content: space-between; align-items: center; padding: 0 20px; font-weight: 700; border-bottom: 1px solid rgba(255,255,255,0.1); position: absolute; top: 0; left: 0; right: 0; z-index: 10; }
          .header-info { display: flex; align-items: center; gap: 10px; }
          .spinning-icon { color: #1ea1f2; font-size: 20px; }
          .close-btn { background: rgba(255,255,255,0.1); border: none; color: white; width: 32px; height: 32px; border-radius: 50%; cursor: pointer; transition: 0.3s; display: grid; place-items: center; }
          .close-btn:hover { background: rgba(255,255,255,0.25); transform: rotate(90deg); }

          .reel-video-container { flex: 1; position: relative; width: 100%; height: 100%; }
          .reel-video { width: 100%; height: 100%; object-fit: cover; }
          
          .reel-gradient-top { position: absolute; top: 0; width: 100%; height: 120px; background: linear-gradient(to bottom, rgba(0,0,0,0.7), transparent); }
          
          .reel-overlay { position: absolute; bottom: 0; left: 0; right: 0; padding: 60px 24px 40px; background: linear-gradient(to top, rgba(0,0,0,0.95) 20%, rgba(0,0,0,0.5) 60%, transparent); color: white; display: flex; flex-direction: column; gap: 20px; }
          .reel-overlay p { font-size: 18px; font-weight: 600; line-height: 1.5; text-shadow: 0 2px 10px rgba(0,0,0,0.5); margin: 0; animation: fadeIn 0.8s ease; }
          @keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

          .next-btn { background: white; color: #1ea1f2; font-weight: 800; width: 100%; padding: 16px; border: none; border-radius: 16px; cursor: pointer; transition: all 0.3s; box-shadow: 0 10px 20px rgba(0,0,0,0.3); font-size: 15px; text-transform: uppercase; letter-spacing: 1px; }
          .next-btn:hover { background: #f0f9ff; transform: translateY(-3px); box-shadow: 0 15px 25px rgba(0,0,0,0.4); }
          .next-btn:active { transform: translateY(-1px); }
          
          @media (max-width: 480px) {
            .reel-box { width: 100vw; height: 100vh; max-height: none; border-radius: 0; }
            .reel-float { right: 15px; bottom: 15px; }
          }
        `}</style>
        </>
    );
};

export const ChatWidget = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [messages, setMessages] = useState<{ text: string, type: 'bot' | 'user' }[]>([{ text: "Xin chào! Mình có thể giúp gì cho bạn về chuyến đi này?", type: 'bot' }]);
    const [input, setInput] = useState('');

    const send = () => {
        if (!input.trim()) return;
        setMessages([...messages, { text: input, type: 'user' }]);
        setInput('');
        setTimeout(() => {
            setMessages(prev => [...prev, { text: "Cảm ơn bạn! Mình đang tìm hiểu thêm thông tin để hỗ trợ bạn tốt nhất.", type: 'bot' }]);
        }, 1000);
    };

    return (
        <>
            <div className="chat-float" onClick={() => setIsOpen(!isOpen)}>
                {isOpen ? <CloseOutlined style={{ fontSize: '24px' }} /> : <CustomerServiceOutlined style={{ fontSize: '24px' }} />}
            </div>
            {isOpen && (
                <div className="chat-box">
                    <div className="chat-header">Trợ lý du lịch AI</div>
                    <div className="chat-content">
                        {messages.map((m, i) => (
                            <div key={i} className={`msg ${m.type}`}>{m.text}</div>
                        ))}
                    </div>
                    <div className="chat-input-bar">
                        <input value={input} onChange={e => setInput(e.target.value)} onKeyPress={(e) => e.key === 'Enter' && send()} placeholder="Nhập câu hỏi..." />
                        <button onClick={send}><SendOutlined /></button>
                    </div>
                </div>
            )}
            <style jsx>{`
          .chat-float { position: fixed; left: 24px; bottom: 24px; width: 60px; height: 60px; border-radius: 50%; background: #0f172a; color: white; display: grid; place-items: center; cursor: pointer; box-shadow: 0 10px 30px rgba(0,0,0,0.2); z-index: 1000; transition: 0.3s; }
          .chat-float:hover { transform: scale(1.1); }
          .chat-box { position: fixed; left: 24px; bottom: 100px; width: 340px; height: 480px; background: white; border-radius: 20px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 20px 40px rgba(0,0,0,0.15); border: 1px solid var(--line); z-index: 1000; }
          .chat-header { background: #0f172a; color: white; padding: 16px; font-weight: 700; text-align: center; }
          .chat-content { flex: 1; overflow-y: auto; padding: 16px; display: flex; flex-direction: column; gap: 12px; background: #f8fbff; }
          .msg { padding: 10px 14px; border-radius: 14px; max-width: 80%; font-size: 14px; line-height: 1.4; }
          .msg.bot { background: white; color: var(--ink); align-self: flex-start; border-bottom-left-radius: 4px; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
          .msg.user { background: var(--brand); color: white; align-self: flex-end; border-bottom-right-radius: 4px; }
          .chat-input-bar { padding: 12px; display: flex; gap: 8px; border-top: 1px solid var(--line); }
          .chat-input-bar input { flex: 1; border: 1px solid var(--line); padding: 8px 12px; border-radius: 10px; outline: none; }
          .chat-input-bar button { background: var(--brand); color: white; border: none; width: 36px; height: 36px; border-radius: 10px; cursor: pointer; font-size: 18px; display: grid; place-items: center; }
        `}</style>
        </>
    );
};
