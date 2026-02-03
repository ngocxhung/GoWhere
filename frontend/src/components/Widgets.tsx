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
                <VideoCameraOutlined style={{ fontSize: '24px' }} />
            </div>
            {isOpen && (
                <div className="reel-overlay-bg">
                    <div className="reel-box" ref={widgetRef}>
                        <div className="reel-header">
                            <span><VideoCameraOutlined /> Gợi ý Video</span>
                            <button className="close-btn" onClick={() => setIsOpen(false)}><CloseOutlined /></button>
                        </div>
                        <div className="reel-video-container">
                            <video
                                key={danangReels[index].src}
                                src={danangReels[index].src}
                                autoPlay
                                loop
                                muted
                                className="reel-video"
                            />
                            <div className="reel-overlay">
                                <p>{danangReels[index].cap}</p>
                                <button className="btn white" onClick={() => setIndex((index + 1) % danangReels.length)}>Video tiếp theo ↓</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
            <style jsx>{`
          .reel-overlay-bg { position: fixed; inset: 0; background: rgba(0,0,0,0.4); z-index: 1000; display: flex; align-items: flex-end; justify-content: flex-end; padding: 24px; pointer-events: auto; }
          .reel-float { position: fixed; right: 24px; bottom: 24px; width: 60px; height: 60px; border-radius: 50%; background: var(--brand); color: white; display: grid; place-items: center; cursor: pointer; box-shadow: 0 10px 30px rgba(30,161,242,0.4); z-index: 1000; transition: 0.3s; }
          .reel-float:hover { transform: scale(1.1); }
          .reel-box { position: relative; width: 360px; height: 600px; background: black; border-radius: 20px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 20px 50px rgba(0,0,0,0.5); border: 2px solid rgba(255,255,255,0.1); }
          .reel-header { height: 50px; background: #1ea1f2; color: white; display: flex; justify-content: space-between; align-items: center; padding: 0 16px; font-weight: 700; }
          .close-btn { background: none; border: none; color: white; font-size: 18px; cursor: pointer; }
          .reel-video-container { flex: 1; position: relative; display: flex; flex-direction: column; }
          .reel-video { width: 100%; height: 100%; object-fit: cover; }
          .reel-overlay { position: absolute; bottom: 0; left: 0; right: 0; padding: 40px 20px 80px; background: linear-gradient(transparent, rgba(0,0,0,0.9)); color: white; display: flex; flex-direction: column; gap: 25px; }
          .reel-overlay p { font-size: 16px; font-weight: 600; line-height: 1.4; text-shadow: 0 2px 10px rgba(0,0,0,0.5); margin: 0; }
          .btn.white { background: white; color: var(--brand); font-weight: 700; width: 100%; padding: 14px; border: none; border-radius: 14px; cursor: pointer; position: relative; z-index: 10; box-shadow: 0 4px 15px rgba(0,0,0,0.2); }
          .btn.white:active { transform: scale(0.98); }
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
