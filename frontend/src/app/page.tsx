"use client";

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import {
  VideoCameraOutlined,
  CustomerServiceOutlined,
  UserOutlined,
  CloseOutlined,
  SendOutlined,
  SearchOutlined,
  CalendarOutlined,
  DollarOutlined,
  SmileOutlined,
  TeamOutlined,
  StarFilled,
  EditOutlined,
  ArrowRightOutlined,
  FireOutlined,
  CompassOutlined,
  RocketOutlined,
  SafetyCertificateOutlined,
  CheckCircleFilled,
  ClockCircleOutlined,
  HomeOutlined,
  CarOutlined
} from '@ant-design/icons';
import {
  demoVideos,
  danangReels,
  bannerSlides,
  demoTours,
  itineraryTemplates,
  endTemplate,
  Activity,
  Video,
  Tour,
  hotels,
  flights,
  cars
} from '@/dummy/data';

// --- Helpers ---
const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

type Province = { provinceId: number; provinceName: string };

// --- Components ---

const SocialFloatButtons = () => {
  const [showMessengerTooltip, setShowMessengerTooltip] = useState(false);

  return (
    <div className="social-float-container">
      {/* Facebook Icon */}
      <a 
        href="https://www.facebook.com/profile.php?id=61587150455623" 
        target="_blank" 
        rel="noopener noreferrer"
        className="social-float-btn facebook-btn"
        title="Facebook Page"
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
          <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
        </svg>
      </a>

      {/* Messenger Icon with Tooltip */}
      <div 
        className="messenger-wrapper"
        onMouseEnter={() => setShowMessengerTooltip(true)}
        onMouseLeave={() => setShowMessengerTooltip(false)}
      >
        <a 
          href="https://m.me/961137500418471" 
          target="_blank" 
          rel="noopener noreferrer"
          className="social-float-btn messenger-btn"
        >
          <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
            <path d="M12 0C5.373 0 0 4.975 0 11.111c0 3.497 1.745 6.616 4.472 8.652V24l4.086-2.242c1.09.301 2.246.464 3.442.464 6.627 0 12-4.974 12-11.111C24 4.975 18.627 0 12 0zm1.191 14.963l-3.055-3.26-5.963 3.26L10.732 8l3.131 3.259L19.752 8l-6.561 6.963z"/>
          </svg>
        </a>
        {showMessengerTooltip && (
          <div className="messenger-tooltip">
            💬 Chat với chúng tôi
          </div>
        )}
      </div>

      <style jsx>{`
        .social-float-container { position: fixed; right: 24px; bottom: 100px; z-index: 999; display: flex; flex-direction: column; gap: 12px; }
        
        .social-float-btn { width: 56px; height: 56px; border-radius: 50%; display: grid; place-items: center; cursor: pointer; box-shadow: 0 8px 24px rgba(0,0,0,0.2); transition: 0.3s; text-decoration: none; }
        .social-float-btn:hover { transform: translateY(-4px) scale(1.05); box-shadow: 0 12px 32px rgba(0,0,0,0.3); }
        
        .facebook-btn { background: #1877f2; color: white; }
        .messenger-btn { background: linear-gradient(135deg, #00b2ff, #006aff); color: white; }
        
        .messenger-wrapper { position: relative; }
        .messenger-tooltip { position: absolute; right: 70px; top: 50%; transform: translateY(-50%); background: #0f172a; color: white; padding: 10px 16px; border-radius: 12px; font-size: 14px; font-weight: 600; white-space: nowrap; box-shadow: 0 4px 12px rgba(0,0,0,0.2); animation: tooltipSlide 0.3s ease-out; }
        .messenger-tooltip::after { content: ''; position: absolute; right: -6px; top: 50%; transform: translateY(-50%) rotate(45deg); width: 12px; height: 12px; background: #0f172a; }
        
        @keyframes tooltipSlide { from { opacity: 0; transform: translateY(-50%) translateX(-10px); } to { opacity: 1; transform: translateY(-50%) translateX(0); } }
      `}</style>
    </div>
  );
};

const ReelsWidget = () => {
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

const ChatWidget = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<{ text: string, type: 'bot' | 'user' }[]>([{ text: "Xin chào! Mình là trợ lý du lịch AI của GoWhere. Bạn muốn hỏi gì về chuyến đi?", type: 'bot' }]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [conversationId, setConversationId] = useState<string>('');
  const chatEndRef = React.useRef<HTMLDivElement>(null);

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const send = async () => {
    if (!input.trim() || loading) return;

    const userMessage = input.trim();
    setMessages(prev => [...prev, { text: userMessage, type: 'user' }]);
    setInput('');
    setLoading(true);

    try {
      const response = await fetch('http://localhost:5191/api/AIChat/Chat', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          message: userMessage,
          conversationId: conversationId || undefined
        })
      });

      if (response.ok) {
        const data = await response.json();
        setMessages(prev => [...prev, { text: data.response, type: 'bot' }]);
        if (data.conversationId && !conversationId) {
          setConversationId(data.conversationId);
        }
      } else {
        setMessages(prev => [...prev, { text: "Xin lỗi, có lỗi xảy ra. Vui lòng thử lại sau.", type: 'bot' }]);
      }
    } catch (error) {
      console.error('Chat error:', error);
      setMessages(prev => [...prev, { text: "Không thể kết nối đến AI. Vui lòng kiểm tra kết nối.", type: 'bot' }]);
    } finally {
      setLoading(false);
    }
  };


  return (
    <>
      <div className="chat-float" onClick={() => setIsOpen(!isOpen)}>
        {isOpen ? <CloseOutlined style={{ fontSize: '24px' }} /> : <CustomerServiceOutlined style={{ fontSize: '24px' }} />}
      </div>
      {isOpen && (
        <div className="chat-box">
          <div className="chat-header">
            <CustomerServiceOutlined style={{ marginRight: '8px' }} />
            Trợ lý du lịch AI
          </div>
          <div className="chat-content">
            {messages.map((m, i) => (
              <div key={i} className={`msg ${m.type}`}>{m.text}</div>
            ))}
            {loading && (
              <div className="msg bot typing">
                <span></span><span></span><span></span>
              </div>
            )}
            <div ref={chatEndRef} />
          </div>
          <div className="chat-input-bar">
            <input
              value={input}
              onChange={e => setInput(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && send()}
              placeholder="Nhập câu hỏi..."
              disabled={loading}
            />
            <button onClick={send} disabled={loading || !input.trim()}>
              <SendOutlined />
            </button>
          </div>
        </div>
      )}
      <style jsx>{`
        .chat-float { position: fixed; left: 24px; bottom: 24px; width: 60px; height: 60px; border-radius: 50%; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; display: grid; place-items: center; cursor: pointer; box-shadow: 0 10px 30px rgba(102, 126, 234, 0.4); z-index: 1000; transition: 0.3s; }
        .chat-float:hover { transform: scale(1.1); box-shadow: 0 15px 40px rgba(102, 126, 234, 0.5); }
        .chat-box { position: fixed; left: 24px; bottom: 100px; width: 380px; height: 550px; background: white; border-radius: 20px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 20px 60px rgba(0,0,0,0.2); border: 1px solid var(--line); z-index: 1000; }
        .chat-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 18px; font-weight: 700; text-align: center; display: flex; align-items: center; justify-content: center; font-size: 16px; }
        .chat-content { flex: 1; overflow-y: auto; padding: 20px; display: flex; flex-direction: column; gap: 14px; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); }
        .msg { padding: 12px 16px; border-radius: 16px; max-width: 85%; font-size: 14px; line-height: 1.5; animation: slideIn 0.3s ease-out; }
        .msg.bot { background: white; color: var(--ink); align-self: flex-start; border-bottom-left-radius: 4px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); border: 1px solid #e5e7eb; }
        .msg.user { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; align-self: flex-end; border-bottom-right-radius: 4px; box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3); }
        .msg.typing { display: flex; gap: 6px; padding: 16px; }
        .msg.typing span { width: 8px; height: 8px; background: #cbd5e1; border-radius: 50%; animation: bounce 1.4s infinite ease-in-out both; }
        .msg.typing span:nth-child(1) { animation-delay: -0.32s; }
        .msg.typing span:nth-child(2) { animation-delay: -0.16s; }
        .chat-input-bar { padding: 16px; display: flex; gap: 10px; border-top: 1px solid var(--line); background: white; }
        .chat-input-bar input { flex: 1; border: 1px solid var(--line); padding: 10px 14px; border-radius: 12px; outline: none; font-size: 14px; transition: 0.2s; }
        .chat-input-bar input:focus { border-color: #667eea; box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1); }
        .chat-input-bar input:disabled { background: #f3f4f6; cursor: not-allowed; }
        .chat-input-bar button { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; border: none; width: 42px; height: 42px; border-radius: 12px; cursor: pointer; font-size: 18px; display: grid; place-items: center; transition: 0.2s; }
        .chat-input-bar button:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
        .chat-input-bar button:disabled { opacity: 0.5; cursor: not-allowed; }
        @keyframes slideIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes bounce { 0%, 80%, 100% { transform: scale(0); } 40% { transform: scale(1); } }
      `}</style>
    </>
  );
};


// --- Restored Components ---

const Banner = () => {
  const [current, setCurrent] = useState(0);
  useEffect(() => {
    const timer = setInterval(() => setCurrent(p => (p + 1) % bannerSlides.length), 8000);
    return () => clearInterval(timer);
  }, []);
  return (
    <div className="full-width-banner">
      <div className="slider">
        <div className="slides" style={{ transform: `translateX(-${current * 100}%)` }}>
          {bannerSlides.map((slide, i) => (
            <div key={i} className="slide">
              <video
                src={slide.video}
                autoPlay
                loop
                muted
                playsInline
                className="banner-video"
              />
              <div className="banner-content">
                <div className="banner-text">
                  <h2>{slide.title}</h2>
                  <p>{slide.sub}</p>
                  <button className="btn white">{slide.btnText}</button>
                </div>
              </div>
            </div>
          ))}
        </div>
        <div className="nav-arrows">
          <button className="arrow left" onClick={() => setCurrent((current - 1 + bannerSlides.length) % bannerSlides.length)}>❮</button>
          <button className="arrow right" onClick={() => setCurrent((current + 1) % bannerSlides.length)}>❯</button>
        </div>
        <div className="dots">
          {bannerSlides.map((_, i) => <div key={i} className={`dot ${current === i ? 'active' : ''}`} onClick={() => setCurrent(i)} />)}
        </div>
      </div>
      <style jsx>{`
          .full-width-banner { width: 100%; overflow: hidden; position: relative; }
          .slider { position: relative; width: 100%; height: 600px; background: black; }
          .slides { display: flex; width: 100%; height: 100%; transition: transform 0.8s cubic-bezier(0.4, 0, 0.2, 1); }
          .slide { min-width: 100%; height: 100%; position: relative; }
          .slide::before { content:''; position:absolute; inset:0; background: linear-gradient(to right, rgba(0,0,0,0.6) 0%, transparent 60%); z-index:1; }
          .banner-video { width: 100%; height: 100%; object-fit: cover; }
          .banner-content { position: absolute; inset: 0; display: flex; align-items: center; padding: 0 10%; z-index: 2; }
          .banner-text { color: white; max-width: 600px; animation: fadeInUp 0.8s ease-out; }
          .banner-text h2 { font-size: 56px; font-weight: 800; margin-bottom: 20px; text-shadow: 0 4px 30px rgba(0,0,0,0.5); }
          .banner-text p { font-size: 20px; margin-bottom: 32px; opacity: 0.9; line-height: 1.6; text-shadow: 0 2px 20px rgba(0,0,0,0.5); }
          .btn.white { background: white; color: var(--brand); font-weight: 700; padding: 14px 32px; border-radius: 12px; margin-top: 10px; cursor: pointer; transition: 0.3s; }
          .btn.white:hover { transform: translateY(-3px); box-shadow: 0 10px 20px rgba(0,0,0,0.2); }
          .nav-arrows { position: absolute; top: 50%; width: 100%; display: flex; justify-content: space-between; padding: 0 40px; transform: translateY(-50%); z-index: 10; }
          .arrow { background: rgba(255,255,255,0.2); border: none; color: white; width: 50px; height: 50px; border-radius: 50%; display: grid; place-items: center; cursor: pointer; backdrop-filter: blur(4px); transition: 0.3s; }
          .arrow:hover { background: white; color: var(--brand); }
          .dots { position: absolute; bottom: 30px; width: 100%; display: flex; justify-content: center; gap: 12px; z-index: 10; }
          .dot { width: 10px; height: 10px; background: rgba(255,255,255,0.4); border-radius: 50%; cursor: pointer; transition: 0.3s; }
          .dot.active { width: 36px; border-radius: 5px; background: white; }
          @media (max-width: 768px) {
            .slider { height: 450px; }
            .banner-text h2 { font-size: 36px; }
            .banner-text p { font-size: 16px; }
          }
        `}</style>
    </div>
  );
};

const CustomSelect = ({ label, value, options, onChange }: { label: string, value: any, options: { label: string, value: any }[], onChange: (v: any) => void }) => {
  const [isOpen, setIsOpen] = useState(false);
  const ref = React.useRef<HTMLDivElement>(null);

  useEffect(() => {
    const click = (e: MouseEvent) => { if (ref.current && !ref.current.contains(e.target as Node)) setIsOpen(false); };
    document.addEventListener('mousedown', click);
    return () => document.removeEventListener('mousedown', click);
  }, []);

  const selectedLabel = options.find(o => o.value === value)?.label || value;

  return (
    <div className={`field custom-select-container ${isOpen ? 'is-open' : ''}`} ref={ref} onClick={() => setIsOpen(!isOpen)}>
      <span className="label-text">{label}</span>
      <div className="select-trigger">
        <span className="current-value">{selectedLabel}</span>
        <ArrowRightOutlined style={{ transform: `rotate(${isOpen ? -90 : 90}deg)`, fontSize: '10px', transition: '0.3s', color: 'var(--brand)' }} />
      </div>
      {isOpen && (
        <div className="options-dropdown">
          {options.map(o => (
            <div key={o.value} className={`option-item ${value === o.value ? 'active' : ''}`} onClick={(e) => { e.stopPropagation(); onChange(o.value); setIsOpen(false); }}>
              {o.label}
            </div>
          ))}
        </div>
      )}
      <style jsx>{`
        .custom-select-container { position: relative; display: flex; flex-direction: column; text-align: left; border: 1px solid var(--line); padding: 12px 16px; border-radius: 14px; background: #ffffff; transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); cursor: pointer; height: 100%; z-index: 1; }
        .custom-select-container.is-open { z-index: 100; border-color: var(--brand); box-shadow: 0 4px 12px rgba(30, 161, 242, 0.08); }
        .custom-select-container:hover { border-color: var(--brand); transform: translateY(-2px); box-shadow: 0 4px 12px rgba(30, 161, 242, 0.08); }
        .label-text { font-size: 10px; color: var(--muted); margin-bottom: 6px; font-weight: 800; text-transform: uppercase; letter-spacing: 0.5px; }
        .select-trigger { display: flex; justify-content: space-between; align-items: center; width: 100%; flex: 1; }
        .current-value { font-size: 15px; color: var(--ink); font-weight: 700; line-height: 1; }
        .options-dropdown { position: absolute; top: calc(100% + 8px); left: 0; right: 0; background: white; border-radius: 16px; box-shadow: 0 10px 40px rgba(0,0,0,0.15); border: 1px solid var(--line); z-index: 1100; padding: 6px; animation: slideIn 0.2s ease-out; max-height: 320px; overflow-y: auto; overflow-x: hidden; }
        .options-dropdown::-webkit-scrollbar { width: 8px; }
        .options-dropdown::-webkit-scrollbar-track { background: #f1f5f9; border-radius: 10px; }
        .options-dropdown::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 10px; }
        .options-dropdown::-webkit-scrollbar-thumb:hover { background: #94a3b8; }
        .option-item { padding: 12px 16px; font-size: 14px; font-weight: 600; color: var(--ink); border-radius: 10px; cursor: pointer; transition: 0.2s; }
        .option-item:hover { background: #f0f9ff; color: var(--brand); }
        .option-item.active { background: var(--brand); color: white; }
        @keyframes slideIn { from { opacity: 0; transform: translateY(-10px); } to { opacity: 1; transform: translateY(0); } }
      `}</style>
    </div>
  );
};

const SearchSection = ({
  onSearch,
  isGenerating,
  provinces
}: {
  onSearch: (data: any) => void;
  isGenerating: boolean;
  provinces: Province[];
}) => {
  const [form, setForm] = useState({ budget: '5000000', days: '4', style: 'dynamic', group: 'friends', hint: '', pax: 2 });
  const [locating, setLocating] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.hint?.trim()) return alert("Vui lòng chọn điểm đến!");
    
    const budgetNum = parseInt(form.budget);
    if (!budgetNum || budgetNum < 500000) {
      return alert("Vui lòng nhập ngân sách tối thiểu 500,000 VND!");
    }

    setLocating(true);
    let userLoc = ""; // Để trống nếu không lấy được

    // Yêu cầu quyền truy cập vị trí
    if (navigator.geolocation) {
      try {
        const pos = await new Promise<GeolocationPosition>((res, rej) => {
          navigator.geolocation.getCurrentPosition(res, rej, {
            enableHighAccuracy: true,
            timeout: 5000,
            maximumAge: 0
          });
        });
        // Gửi tọa độ GPS
        userLoc = `${pos.coords.latitude}, ${pos.coords.longitude}`;
        console.log('Vị trí hiện tại:', userLoc);
      } catch (e) {
        console.log("Người dùng từ chối hoặc không thể lấy vị trí. Để trống userLocation.");
        // Để trống, AI sẽ tự xử lý
        userLoc = "";
      }
    }
    setLocating(false);
    onSearch({ ...form, userLocation: userLoc });
  };

  return (
    <section className="search-section">
      <div className="container">
        <div className="hero-text">
          <h1>Tạo lịch trình du lịch cá nhân hoá trong 60 giây</h1>
          <p>Mô tả sở thích của bạn và để trí tuệ nhân tạo của GoWhere lo liệu phần còn lại.</p>
        </div>

        <form className="search-card" onSubmit={handleSubmit}>
          <div className="search-grid">
            <CustomSelect
              label="Bạn muốn đi đâu?"
              value={form.hint}
              options={provinces.length > 0 
                ? provinces.map(p => ({ label: p.provinceName, value: p.provinceName }))
                : [{ label: 'Đang tải...', value: '' }]
              }
              onChange={(v) => setForm({ ...form, hint: v })}
            />

            <CustomSelect
              label="Số ngày"
              value={form.days}
              options={[1, 2, 3, 4, 5, 7, 10].map(d => ({ label: `${d} ngày`, value: String(d) }))}
              onChange={(v) => setForm({ ...form, days: v })}
            />

            <div className="field">
              <span className="label-text">Ngân sách (VND)</span>
              <input 
                className="input" 
                type="number" 
                min="500000" 
                step="100000"
                placeholder="5,000,000"
                value={form.budget}
                onChange={(e) => setForm({ ...form, budget: e.target.value })}
              />
            </div>

            <CustomSelect
              label="Phong cách"
              value={form.style}
              options={[
                { label: 'Nghỉ dưỡng', value: 'lazy' },
                { label: 'Năng động', value: 'dynamic' },
                { label: 'Ẩm thực', value: 'foodie' }
              ]}
              onChange={(v) => setForm({ ...form, style: v })}
            />

            <CustomSelect
              label="Số người"
              value={form.group}
              options={[
                { label: 'Cá nhân (1)', value: 'single' },
                { label: 'Cặp đôi (2)', value: 'couple' },
                { label: 'Bạn bè (>2)', value: 'friends' }
              ]}
              onChange={(v) => {
                let paxNum = 2;
                if (v === 'single') paxNum = 1;
                if (v === 'couple') paxNum = 2;
                if (v === 'friends') paxNum = 4;
                setForm({ ...form, group: v, pax: paxNum });
              }}
            />
          </div>
          <button type="submit" className="btn primary search-submit" disabled={isGenerating || locating}>
            {isGenerating ? "AI ĐANG SUY NGHĨ..." : locating ? "ĐANG LẤY VỊ TRÍ..." : "TẠO LỊCH TRÌNH AI"}
          </button>
        </form >
      </div >
      <style jsx>{`
          .search-section { padding-top: 40px; padding-bottom: 80px; text-align: center; }
          .hero-text { margin-bottom: 40px; }
          .hero-text h1 { font-size: 40px; font-weight: 800; color: #1e40af; margin-bottom: 12px; }
          .hero-text p { color: var(--muted); font-size: 18px; }
          .search-card { width: 100%; max-width: 1100px; background: white; padding: 24px; border-radius: 24px; box-shadow: 0 20px 60px rgba(30, 161, 242, 0.15); display: grid; gap: 20px; margin: 0 auto; border: 1px solid var(--line); }
          .search-grid { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 1fr; gap: 12px; }
          @media (max-width: 900px) { .search-grid { grid-template-columns: 1fr 1fr; } .hero-text h1 { font-size: 30px; } }
          .field { display: flex; flex-direction: column; text-align: left; border: 1px solid var(--line); padding: 12px 16px; border-radius: 14px; background: #ffffff; transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); position: relative; }
          .field:hover { border-color: var(--brand); transform: translateY(-2px); box-shadow: 0 4px 12px rgba(30, 161, 242, 0.08); }
          .field:focus-within { border-color: var(--brand); background: white; box-shadow: 0 0 0 4px rgba(30, 161, 242, 0.15); }
          .label-text { font-size: 10px; color: var(--muted); margin-bottom: 6px; font-weight: 800; text-transform: uppercase; letter-spacing: 0.5px; line-height: 1; }
          .input { border: none; outline: none; font-size: 15px; background: transparent; color: var(--ink); font-weight: 700; width: 100%; height: 24px; line-height: 24px; padding: 0; }
          .input::placeholder { font-weight: 500; color: #cbd5e1; }
          .select { display: none; }
          .select-trigger { display: flex; justify-content: space-between; align-items: center; width: 100%; height: 24px; }
          .search-submit { 
            grid-column: 1 / -1; 
            padding: 20px; 
            font-weight: 800; 
            font-size: 18px; 
            border-radius: 18px; 
            font-family: 'Montserrat', sans-serif; 
            letter-spacing: 1px;
            background: linear-gradient(135deg, #1ea1f2, #0ea5e9);
            color: white;
            border: none;
            cursor: pointer;
            transition: 0.3s;
            box-shadow: 0 10px 25px rgba(30, 161, 242, 0.3);
          }
          .search-submit:hover { transform: translateY(-2px); box-shadow: 0 15px 30px rgba(30, 161, 242, 0.4); filter: brightness(1.1); }
        `}</style>
    </section >
  );
};

const TourDetailModal = ({ tour, isOpen, onClose, onRequireAuth }: { tour: any, isOpen: boolean, onClose: () => void, onRequireAuth: () => void }) => {
  const [calculating, setCalculating] = useState(false);
  const [routeOpened, setRouteOpened] = useState(false);
  const [addingToCart, setAddingToCart] = useState(false);

  useEffect(() => {
    setRouteOpened(false);
    setCalculating(false);
    setAddingToCart(false);
  }, [tour]);

  useEffect(() => {
    if (isOpen) document.body.style.overflow = 'hidden';
    return () => { document.body.style.overflow = 'unset'; };
  }, [isOpen]);

  const handleCalculateRoute = () => {
    setCalculating(true);
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const { latitude, longitude } = position.coords;
          setTimeout(() => {
            setCalculating(false);
            setRouteOpened(true);
            const destination = encodeURIComponent(tour.title);
            const url = `https://www.google.com/maps/dir/?api=1&origin=${latitude},${longitude}&destination=${destination}`;
            window.open(url, '_blank');
          }, 1000);
        },
        (error) => {
          alert("Không thể lấy vị trí hiện tại. Vui lòng kiểm tra quyền truy cập.");
          setCalculating(false);
        }
      );
    } else {
      alert("Trình duyệt không hỗ trợ Geolocation.");
      setCalculating(false);
    }
  };

  const handleAddToCart = async (item: any, type: string = 'Tour') => {
    const token = localStorage.getItem('token');
    if (!token) {
      onRequireAuth();
      return;
    }

    setAddingToCart(true);
    try {
      const body: any = {
        itemName: item.title || item.name || item.model,
        price: 0,
        quantity: 1,
        serviceType: type,
      };

      if (typeof item.price === 'string') {
        body.price = parseFloat(item.price.replace(/[^0-9]/g, ''));
      } else {
        body.price = item.price || item.pricePerDay || 0;
      }

      if (type === 'Tour') body.scenicSpotId = item.id;
      else body.serviceId = item.id;

      // Special case for flight display name
      if (type === 'Flight') {
        body.itemName = `Vé máy bay ${item.airline} (${item.from} - ${item.to})`;
      } else if (type === 'Car') {
        body.itemName = `Thuê xe ${item.model}`;
      } else if (type === 'Hotel') {
        body.itemName = `Khách sạn ${item.name}`;
      }

      const response = await fetch('http://localhost:5191/api/Order/AddToCart', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(body)
      });

      if (response.ok) {
        alert(`Đã thêm ${body.itemName} vào hành trình thành công!`);
      } else {
        const data = await response.json();
        alert(data.message || 'Có lỗi xảy ra khi thêm vào hành trình.');
      }
    } catch (error) {
      console.error(error);
      alert('Lỗi kết nối đến server.');
    } finally {
      setAddingToCart(false);
    }
  };

  if (!isOpen || !tour) return null;

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}><CloseOutlined /></button>
        <div className="modal-body-grid">
          <div className="modal-img-col">
            <img src={tour.img} alt={tour.title} />
          </div>
          <div className="modal-info-col">
            <h2>{tour.title}</h2>
            <div className="price-tag">{money(tour.price)} VND</div>
            <p className="desc">Khám phá vẻ đẹp tuyệt vời của {tour.title}. Trải nghiệm văn hóa, ẩm thực và thiên nhiên độc đáo tại điểm đến này.</p>

            <div className="modal-actions">
              <button
                className={`btn primary ${addingToCart ? 'loading' : ''}`}
                onClick={() => handleAddToCart(tour, 'Tour')}
                disabled={addingToCart}
                style={{ marginBottom: '16px', width: '100%' }}
              >
                <CalendarOutlined /> {addingToCart ? 'Đang thêm...' : 'Thêm vào hành trình'}
              </button>
            </div>

            <div className="route-section">
              <button
                className={`btn outline full ${calculating ? 'loading' : ''}`}
                onClick={handleCalculateRoute}
                disabled={calculating}
              >
                {calculating ? 'Đang lấy vị trí...' : 'Tính lộ trình & Xem bản đồ'}
              </button>

              {routeOpened && (
                <div className="route-result">
                  <div className="note success">
                    Quãng đường và thời gian dự kiến đã được hiển thị trên Google Maps
                  </div>
                </div>
              )}
            </div>

            <div className="features">
              <div className="feat"><CheckCircleFilled style={{ color: '#10b981' }} /> Hỗ trợ xe đưa đón</div>
              <div className="feat"><CheckCircleFilled style={{ color: '#10b981' }} /> Hướng dẫn viên</div>
              <div className="feat"><CheckCircleFilled style={{ color: '#10b981' }} /> Bảo hiểm du lịch</div>
            </div>
          </div>
        </div>
      </div>
      <style jsx>{`
        .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.7); backdrop-filter: blur(5px); z-index: 3000; display: grid; place-items: center; padding: 20px; animation: fadeIn 0.3s; }
        .modal-content { background: white; width: 100%; max-width: 900px; border-radius: 24px; overflow: hidden; position: relative; animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1); box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25); max-height: 90vh; overflow-y: auto; }
        .modal-close { position: absolute; top: 20px; right: 20px; z-index: 10; background: rgba(255,255,255,0.8); border: none; width: 40px; height: 40px; border-radius: 50%; cursor: pointer; font-size: 20px; transition: 0.2s; }
        .modal-close:hover { background: white; transform: rotate(90deg); }
        
        .modal-body-grid { display: grid; grid-template-columns: 1.2fr 1fr; }
        @media (max-width: 768px) { .modal-body-grid { grid-template-columns: 1fr; } }
        
        .modal-img-col { height: 100%; min-height: 300px; background: #f1f5f9; }
        .modal-img-col img { width: 100%; height: 100%; object-fit: cover; }
        
        .modal-info-col { padding: 40px; display: flex; flex-direction: column; }
        .modal-info-col h2 { font-size: 32px; font-weight: 800; margin-bottom: 8px; line-height: 1.1; }
        .price-tag { font-size: 24px; font-weight: 700; color: var(--brand); margin-bottom: 20px; }
        .desc { color: var(--muted); line-height: 1.6; margin-bottom: 30px; font-size: 16px; }
        
        .route-section { background: #f8fafc; padding: 24px; border-radius: 16px; border: 1px solid var(--line); margin-bottom: 20px; }
        .route-result { margin-top: 16px; animation: fadeIn 0.3s; }
        .note { font-size: 12px; color: var(--brand); margin-top: 8px; font-weight: 600; text-align: center; }
        
        .suggestions-section { margin-bottom: 30px; border-top: 1px dashed var(--line); padding-top: 20px; }
        .suggestions-section h4 { font-size: 16px; font-weight: 700; margin-bottom: 16px; display: flex; align-items: center; gap: 8px; color: #0f172a; }
        .suggestions-list { display: flex; flex-direction: column; gap: 12px; }
        .suggestion-item { display: flex; align-items: center; gap: 12px; padding: 12px; border: 1px solid var(--line); border-radius: 12px; transition: 0.2s; }
        .suggestion-item:hover { border-color: var(--brand); background: #f0f9ff; }
        .s-icon { width: 32px; height: 32px; background: #e0f2fe; color: var(--brand); border-radius: 8px; display: grid; place-items: center; font-size: 16px; }
        .s-info { flex: 1; }
        .s-name { font-weight: 700; font-size: 14px; color: #0f172a; }
        .s-sub { font-size: 12px; color: var(--muted); }
        .btn-sm { padding: 6px 12px; border-radius: 8px; font-size: 12px; font-weight: 700; background: white; border: 1px solid var(--line); cursor: pointer; color: var(--brand); transition: 0.2s; }
        .btn-sm:hover { background: var(--brand); color: white; border-color: var(--brand); }

        .features { display: grid; gap: 12px; }
        .feat { display: flex; gap: 10px; align-items: center; font-weight: 600; color: var(--ink); }
        
        @keyframes fadeIn { from { opacity: 0; } }
        @keyframes slideUp { from { transform: translateY(40px); opacity: 0; } }
      `}</style>
    </div>
  );
};

const ToursSection = ({ tours, onSelect }: { tours: any[], onSelect: (t: any) => void }) => (
  <section className="container section-block">
    <div className="section-head center">
      <h2><FireOutlined style={{ color: '#ff4d4f' }} /> Địa điểm nổi bật</h2>
      <div className="sub">Khám phá những điểm đến tuyệt vời được cộng đồng yêu thích</div>
    </div>
    <div className="tours-grid">
      {tours.map(tour => (
        <div key={tour.id} className="tour-card">
          <div className="tour-img">
            <img src={tour.img} alt={tour.title} />
          </div>
          <div className="tour-info">
            <h3>{tour.title}</h3>
            <p className="tour-price">{money(tour.price)} VND</p>
            <button onClick={() => onSelect(tour)} className="btn outline full">Khám phá ngay <ArrowRightOutlined /></button>
          </div>
        </div>
      ))}
    </div>
    <style jsx>{`
      .tours-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 30px; margin-top: 40px; }
      @media (max-width: 960px) { .tours-grid { grid-template-columns: 1fr; } }
      .tour-card { background: white; border-radius: 20px; overflow: hidden; border: 1px solid var(--line); box-shadow: 0 10px 30px rgba(0,0,0,0.05); transition: 0.3s; }
      .tour-card:hover { transform: translateY(-8px); box-shadow: 0 20px 40px rgba(0,0,0,0.1); border-color: var(--brand); }
      .tour-img { height: 240px; position: relative; }
      .tour-img img { width: 100%; height: 100%; object-fit: cover; }
      .tour-info { padding: 24px; text-align: center; }
      .tour-info h3 { font-size: 18px; font-weight: 700; margin-bottom: 12px; }
      .tour-price { color: #10b981; font-weight: 600; font-size: 15px; margin-bottom: 20px; }
      .btn.outline { border: 2px solid #1ea1f2; color: #1ea1f2; }
      .btn.outline:hover { background: #1ea1f2; color: white; }
      .btn.full { width: 100%; padding: 12px; cursor: pointer; }
    `}</style>
  </section>
);

const VideoRecommendations = ({ videos, onSelect }: { videos: Video[], onSelect: (v: Video) => void }) => (
  <section id="video" className="container section-block">
    <div className="section-head center">
      <h2><CompassOutlined style={{ color: 'var(--brand)' }} /> Gợi ý dành riêng cho bạn</h2>
      <div className="sub">Được cá nhân hóa hoàn toàn dựa trên tiêu chí của bạn</div>
    </div>
    <div className="video-grid">
      {videos.map((v) => (
        <div key={v.id} className="vcard" onClick={() => onSelect(v)}>
          <div className="vthumb">
            <img src={v.img} alt={v.dst} />
            <div className="vmeta"><StarFilled style={{ color: '#fab005' }} /> 4.9 • {money(v.cost)} VND</div>
          </div>
          <div className="vbody">
            <strong>{v.dst}</strong>
            <button className="btn primary full">Khám phá lộ trình</button>
          </div>
        </div>
      ))}
    </div>
    <style jsx>{`
        .video-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 28px; }
        @media (max-width: 960px) { .video-grid { grid-template-columns: 1fr; } }
        .vcard { background: white; border: 2px solid var(--line); border-radius: 20px; overflow: hidden; transition: 0.4s; cursor: pointer; }
        .vcard:hover { transform: translateY(-10px); box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1); border-color: var(--brand); }
        .vthumb { height: 260px; position: relative; }
        .vthumb img { width: 100%; height: 100%; object-fit: cover; }
        .vmeta { position: absolute; bottom: 16px; left: 16px; color: white; font-weight: 700; text-shadow: 0 2px 10px rgba(0,0,0,0.8); }
        .vbody { padding: 20px; }
        .vbody strong { font-size: 19px; display: block; margin-bottom: 16px; }
        .btn.full { width: 100%; padding: 12px; font-weight: 700; }
      `}</style>
  </section>
);

const Itinerary = ({ acts, onDaySelect, currentDay, totalCost, aiResult }: { acts: Record<number, Activity[]>, onDaySelect: (d: number) => void, currentDay: number, totalCost: number, aiResult?: any }) => {
  const days = Object.keys(acts).map(Number);
  return (
    <section id="itinerary" className="section-block-compact">
      <div className="section-head-compact">
        <h3><CalendarOutlined /> Lịch trình chi tiết</h3>
        {aiResult?.summary && <div className="ai-summary-compact">{aiResult.summary}</div>}
      </div>
      <div className="itinerary-layout">
        <div className="days-sidebar">
          {days.map(d => (
            <div key={d} className={`day-btn ${currentDay === d ? 'active' : ''}`} onClick={() => onDaySelect(d)}>
              <strong>Ngày {d}</strong><span>{acts[d].length} hoạt động</span>
            </div>
          ))}
        </div>
        <div className="acts-panel">
          <div className="acts-list">
            {(acts[currentDay] || []).map((a, i) => (
              <div key={i} className="act-card">
                <div className="act-time">{a.t}</div>
                <div className="act-info">
                  <strong>{a.name}</strong>
                  {a.description && <p className="act-desc">{a.description}</p>}
                  <span className="act-price">{a.price > 0 ? money(a.price) + ' VND' : 'Miễn phí'}</span>
                </div>
                <button className="btn-icon"><EditOutlined /></button>
              </div>
            ))}
          </div>
          <div className="it-footer">
            <div className="total"><DollarOutlined /> Tổng: <strong>{money(totalCost)} VND</strong></div>
            <button className="btn primary">Đặt chuyến <ArrowRightOutlined /></button>
          </div>
        </div>
      </div>
      <style jsx>{`
          .itinerary-layout { display: grid; grid-template-columns: 180px 1fr; gap: 20px; }
          @media (max-width: 900px) { .itinerary-layout { grid-template-columns: 1fr; } }
          .day-btn { padding: 12px 14px; border: 2px solid var(--line); border-radius: 12px; margin-bottom: 8px; cursor: pointer; transition: 0.3s; background: white; }
          .day-btn:hover { border-color: var(--brand); background: #f8fafc; }
          .day-btn.active { border-color: var(--brand); background: #f0f9ff; }
          .day-btn strong { display: block; margin-bottom: 2px; font-size: 15px; }
          .day-btn span { font-size: 11px; color: var(--muted); }
          .act-card { background: white; border: 1px solid var(--line); border-radius: 12px; padding: 12px 14px; display: grid; grid-template-columns: 60px 1fr auto; gap: 12px; align-items: start; margin-bottom: 10px; transition: 0.2s; }
          .act-card:hover { border-color: var(--brand); box-shadow: 0 2px 8px rgba(30,161,242,0.1); }
          .act-time { background: #f0f9ff; color: var(--brand); font-weight: 800; padding: 6px 8px; border-radius: 8px; text-align: center; font-size: 13px; }
          .act-info { min-width: 0; }
          .act-info strong { display: block; margin-bottom: 4px; font-size: 14px; color: #0f172a; }
          .act-desc { font-size: 12px; color: #64748b; line-height: 1.4; margin-bottom: 4px; }
          .act-price { font-size: 13px; color: #10b981; font-weight: 600; }
          .it-footer { margin-top: 20px; padding: 16px 20px; background: linear-gradient(135deg, #1ea1f2, #0ea5e9); border-radius: 16px; color: white; display: flex; justify-content: space-between; align-items: center; }
          .total { font-size: 14px; }
          .total strong { font-size: 18px; margin-left: 6px; }
          .btn-icon { background: none; border: none; font-size: 16px; color: #94a3b8; cursor: pointer; padding: 4px; transition: 0.2s; }
          .btn-icon:hover { color: var(--brand); }
          
          .section-block-compact { padding: 0; margin: 0; }
          .section-head-compact { margin-bottom: 20px; }
          .section-head-compact h3 { font-size: 22px; font-weight: 800; color: #0f172a; display: flex; align-items: center; gap: 10px; margin-bottom: 8px; }
          .ai-summary-compact { background: #f0f9ff; border: 1px dashed #1ea1f2; border-radius: 12px; padding: 12px 16px; font-size: 13px; color: #0369a1; font-style: italic; line-height: 1.5; margin-top: 12px; }
        `}</style>
    </section>
  );
};

const Stats = () => (
  <section className="container section-block">
    <div className="stats-grid">
      <div className="stat-card"><strong><RocketOutlined /> 10k+</strong><span>Hành trình đã tạo</span></div>
      <div className="stat-card"><strong><CompassOutlined /> 50+</strong><span>Điểm đến phổ biến</span></div>
      <div className="stat-card"><strong><SafetyCertificateOutlined /> 4.9/5</strong><span>Đánh giá người dùng</span></div>
      <div className="stat-card"><strong><SmileOutlined /> AI</strong><span>Tối ưu chi phí</span></div>
    </div>
    <style jsx>{`
        .stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 24px; }
        @media (max-width: 768px) { .stats-grid { grid-template-columns: 1fr 1fr; } }
        .stat-card { background: white; padding: 32px; border-radius: 20px; text-align: center; border: 1px solid var(--line); }
        .stat-card strong { font-size: 32px; color: var(--brand); display: block; margin-bottom: 8px; }
        .stat-card span { font-weight: 600; color: var(--muted); }
      `}</style>
  </section>
);

export default function Home() {
  const [searchData, setSearchData] = useState<any>(null);
  const [itinerary, setItinerary] = useState<Record<number, Activity[]> | null>(null);
  const [currentDay, setCurrentDay] = useState(1);
  const [totalCost, setTotalCost] = useState(0);
  const [tours, setTours] = useState<any[]>([]);
  const [selectedTour, setSelectedTour] = useState<any>(null);
  const [generating, setGenerating] = useState(false);
  const [aiResult, setAiResult] = useState<any>(null);
  const [scheduleProvinceId, setScheduleProvinceId] = useState<number | null>(null);
  const [provinces, setProvinces] = useState<Province[]>([]);
  const [paymentModal, setPaymentModal] = useState<{ show: boolean; message: string; amount: number; qrUrl?: string; orderId?: number }>({ 
    show: false, 
    message: '', 
    amount: 0 
  });
  const [currentVideoIndex, setCurrentVideoIndex] = useState(0);

  // Videos theo tỉnh
  const provinceVideos: Record<number, string[]> = {
    16: [ // Hà Nội
      '/media/short/16.mp4',
      '/media/short/shutterstock_3725692797.mp4',
      '/media/short/shutterstock_3775625057.mp4',
      '/media/short/shutterstock_3857698879.mp4'
    ],
    33: [ // Quảng Nam
      '/media/short/33.mp4'
    ]
  };

  const currentVideos = scheduleProvinceId ? (provinceVideos[scheduleProvinceId] || []) : [];
  const provinceName = scheduleProvinceId === 16 ? 'Hà Nội' : scheduleProvinceId === 33 ? 'Quảng Nam' : 'Điểm đến';

  useEffect(() => {
    const fetchTours = async () => {
      try {
        const response = await fetch('http://localhost:5191/api/ScenicSpot/GetAllScenicSpots');
        if (response.ok) {
          const data = await response.json();
          const mappedTours = data.map((spot: any) => ({
            id: spot.id,
            title: spot.scenicSpotName,
            price: spot.estimatedVisitPrice,
            img: spot.imageUrl
              ? spot.imageUrl
              : (spot.mediaUrls && spot.mediaUrls.length > 0
                ? `http://localhost:5191${spot.mediaUrls[0]}`
                : 'https://images.unsplash.com/photo-1502602898657-3e91760cbb34?auto=format&fit=crop&w=800&q=80'),
            link: "#"
          }));
          setTours(mappedTours);
        } else {
          setTours(demoTours);
        }
      } catch (error) {
        setTours(demoTours);
      }
    };
    fetchTours();
  }, []);

  useEffect(() => {
    const fetchProvinces = async () => {
      try {
        const res = await fetch("http://localhost:5191/api/Province/GetAllProvinces");
        if (!res.ok) return;
        const data = await res.json();
        if (Array.isArray(data)) setProvinces(data);
      } catch (error) {
        console.error('Error fetching provinces:', error);
      }
    };
    fetchProvinces();
  }, []);

  const handleSearch = async (data: any) => {
    const token = localStorage.getItem('token');
    if (!token) return window.dispatchEvent(new CustomEvent('open-auth-modal', { detail: 'login' }));

    // Find provinceId from selected province name
    const province = provinces.find(p => p.provinceName === data.hint);
    const provinceId = province?.provinceId || 0;
    
    if (!provinceId) {
      return alert("Vui lòng chọn điểm đến từ danh sách!");
    }

    setGenerating(true);
    setItinerary(null);
    setAiResult(null);

    try {
      const res = await fetch('http://localhost:5191/api/AIChat/GenerateSchedule', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          provinceId,
          userLocation: data.userLocation,
          destination: data.hint,
          pax: data.pax,
          budget: parseInt(data.budget) || 5000000,
          numberOfDays: parseInt(data.days),
          travelStyle: data.style,
          companionType: data.group
        })
      });

      if (res.ok) {
        const result = await res.json();
        setAiResult(result);
        setScheduleProvinceId(result.provinceId || null);

        // Transform AI result to UI format
        const newItinerary: Record<number, Activity[]> = {};
        result.dailyItinerary.forEach((day: any) => {
          newItinerary[day.day] = day.activities.map((act: any) => ({
            t: act.time,
            name: act.scenicSpot?.name || act.activity,
            price: act.estimatedCost,
            id: act.scenicSpot?.scenicSpotId || 0,
            description: act.description
          }));
        });

        setItinerary(newItinerary);
        setTotalCost(result.estimatedTotalCost);
        setCurrentDay(1);
        setCurrentVideoIndex(0); // Reset video carousel
        
        // Scroll to video section first
        setTimeout(() => {
          document.getElementById('video-preview')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }, 100);
      } else if (res.status === 402) {
        // Payment required
        const err = await res.json();
        setPaymentModal({
          show: true,
          message: err.Message || err.message || 'Bạn đã hết lượt miễn phí',
          amount: err.PaymentAmount || 50000
        });
      } else {
        const err = await res.json();
        alert(err.message || "Lỗi khi tạo lịch trình AI");
      }
    } catch (e) {
      alert("Lỗi kết nối server AI");
    } finally {
      setGenerating(false);
    }
  };

  const openAuth = (view: 'login' | 'signup') => {
    window.dispatchEvent(new CustomEvent('open-auth-modal', { detail: view }));
  };

  const handleAIPayment = async () => {
    const token = localStorage.getItem('token');
    if (!token) return;

    try {
      // Create payment order
      const res = await fetch('http://localhost:5191/api/AIChat/CreateAIPaymentOrder', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        }
      });

      if (res.ok) {
        const data = await res.json();
        const orderId = data.OrderId;
        const amount = data.Amount || 50000;

        // Get payment info
        const payInfoRes = await fetch('http://localhost:5191/api/Order/GetPaymentInfo');
        const payInfo = await payInfoRes.json();

        // Generate VietQR
        const qrUrl = `https://img.vietqr.io/image/${payInfo.bankBin}-${payInfo.accountNo}-${payInfo.template}.png?amount=${amount}&addInfo=${encodeURIComponent("GOWHERE AI " + orderId)}&accountName=${encodeURIComponent(payInfo.accountName)}`;

        setPaymentModal(prev => ({
          ...prev,
          qrUrl,
          orderId
        }));
      } else {
        alert('Không thể tạo đơn thanh toán. Vui lòng thử lại.');
      }
    } catch (error) {
      alert('Lỗi kết nối server');
    }
  };

  return (
    <main>
      <Banner />
      <SearchSection onSearch={handleSearch} isGenerating={generating} provinces={provinces} />
      <Stats />

      {generating && (
        <div className="ai-loading-state">
          <div className="loader"></div>
          <p>GoWhere AI đang tính toán phương án tốt nhất...</p>
        </div>
      )}

      <ToursSection tours={tours} onSelect={(tour) => setSelectedTour(tour)} />

      {/* Split Layout: Itinerary (2/3) + Video (1/3) */}
      {itinerary && (
        <div className="container split-layout-container" id="video-preview">
          <div className="itinerary-column">
            <Itinerary acts={itinerary} currentDay={currentDay} onDaySelect={setCurrentDay} totalCost={totalCost} aiResult={aiResult} />
          </div>

          {currentVideos.length > 0 && (
            <div className="video-column">
              <div className="video-sticky-wrapper">
                <div className="video-carousel-wrapper">
                  <video
                    autoPlay
                    loop
                    muted
                    playsInline
                    className="carousel-video"
                    key={currentVideoIndex}
                    src={currentVideos[currentVideoIndex]}
                  />
                  
                  <div className="video-overlay-compact">
                    <h4>{provinceName}</h4>
                    <span>{currentVideoIndex + 1}/{currentVideos.length}</span>
                  </div>
                  
                  {currentVideos.length > 1 && (
                    <>
                      <button 
                        className="video-nav-compact prev"
                        onClick={() => setCurrentVideoIndex(prev => (prev - 1 + currentVideos.length) % currentVideos.length)}
                      >
                        ❮
                      </button>
                      
                      <button 
                        className="video-nav-compact next"
                        onClick={() => setCurrentVideoIndex(prev => (prev + 1) % currentVideos.length)}
                      >
                        ❯
                      </button>
                      
                      <div className="video-dots-compact">
                        {currentVideos.map((_, idx) => (
                          <div 
                            key={idx}
                            className={`dot-compact ${idx === currentVideoIndex ? 'active' : ''}`}
                            onClick={() => setCurrentVideoIndex(idx)}
                          />
                        ))}
                      </div>
                    </>
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
      )}

      <SocialFloatButtons />
      <ReelsWidget />
      <ChatWidget />

      <TourDetailModal
        tour={selectedTour}
        isOpen={!!selectedTour}
        onClose={() => setSelectedTour(null)}
        onRequireAuth={() => openAuth('login')}
      />

      {/* AI Payment Modal */}
      {paymentModal.show && (
        <div className="payment-modal-overlay" onClick={() => setPaymentModal({ show: false, message: '', amount: 0 })}>
          <div className="payment-modal-box" onClick={(e) => e.stopPropagation()}>
            <button className="modal-close-btn" onClick={() => setPaymentModal({ show: false, message: '', amount: 0 })}>✕</button>
            
            {!paymentModal.qrUrl ? (
              <div className="payment-content">
                <div className="pay-icon">💳</div>
                <h3>AI Unlimited</h3>
                <p className="pay-msg">{paymentModal.message}</p>
                <div className="pay-price">{money(paymentModal.amount)} VND</div>
                <div className="pay-features">
                  <span>✅ Không giới hạn</span>
                  <span>✅ Hỗ trợ 24/7</span>
                </div>
                <button className="btn-pay-now" onClick={handleAIPayment}>
                  Thanh toán ngay
                </button>
              </div>
            ) : (
              <div className="payment-content">
                <h3>Quét mã thanh toán</h3>
                <img src={paymentModal.qrUrl} alt="QR" className="qr-img" />
                <div className="pay-info-compact">
                  <span>Đơn #{paymentModal.orderId}</span>
                  <span className="highlight">{money(paymentModal.amount)} VND</span>
                </div>
                <button className="btn-done" onClick={() => setPaymentModal({ show: false, message: '', amount: 0 })}>
                  ✓ Đã thanh toán
                </button>
              </div>
            )}
          </div>
        </div>
      )}

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
      </footer>

      <style jsx global>{`
        .ai-loading-state { padding: 80px 20px; text-align: center; }
        .loader { width: 48px; height: 48px; border: 5px solid #f3f3f3; border-top: 5px solid var(--brand); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 20px; }
        @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
        
        .ai-summary-box { max-width: 600px; margin: 20px auto 0; padding: 16px; background: #f0f9ff; border: 1px dashed var(--brand); border-radius: 12px; font-size: 14px; color: #0369a1; font-style: italic; }

        .section-head.center { text-align: center; margin-bottom: 40px; }
        .section-head h2 { font-size: 32px; font-weight: 800; color: #0f172a; margin-bottom: 12px; }
        .section-head .sub { color: var(--muted); font-size: 16px; }
        .footer-v2 { padding: 80px 0; background: #0f172a; color: white; text-align: center; margin-top: 100px; }
        .footer-content { display: flex; flex-direction: column; align-items: center; gap: 32px; }
        .footer-brand { display: flex; align-items: center; justify-content: center; gap: 12px; margin-bottom: 12px; }
        .logo-img-small { height: 32px; width: auto; object-fit: contain; }
        .brand-text-small { font-family: 'Outfit', sans-serif; font-size: 20px; font-weight: 800; letter-spacing: -0.5px; color: var(--brand); }
        .brand-text-small span { color: white; }
        .footer-links { display: flex; gap: 30px; }
        .footer-links a { color: rgba(255,255,255,0.7); font-size: 14px; }
        .footer-links a:hover { color: white; }
        .footer-v2 p { font-size: 14px; opacity: 0.5; }

        /* AI Payment Modal - Compact */
        .payment-modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); backdrop-filter: blur(3px); z-index: 9999; display: grid; place-items: center; padding: 20px; animation: fadeIn 0.2s; }
        .payment-modal-box { background: white; border-radius: 20px; max-width: 380px; width: 100%; overflow: hidden; box-shadow: 0 20px 40px rgba(0,0,0,0.25); position: relative; animation: slideUp 0.3s ease-out; }
        .modal-close-btn { position: absolute; top: 12px; right: 12px; z-index: 10; background: rgba(0,0,0,0.05); border: none; width: 32px; height: 32px; border-radius: 50%; cursor: pointer; font-size: 16px; color: #64748b; transition: 0.2s; }
        .modal-close-btn:hover { background: rgba(0,0,0,0.1); transform: rotate(90deg); }
        .payment-content { padding: 32px 24px; text-align: center; }
        .pay-icon { font-size: 48px; margin-bottom: 12px; }
        .payment-content h3 { font-size: 20px; font-weight: 800; margin-bottom: 8px; color: #0f172a; }
        .pay-msg { font-size: 14px; color: #64748b; margin-bottom: 16px; }
        .pay-price { font-size: 32px; font-weight: 800; color: #1ea1f2; margin-bottom: 16px; }
        .pay-features { display: flex; gap: 12px; justify-content: center; margin-bottom: 24px; font-size: 13px; color: #166534; }
        .btn-pay-now { width: 100%; padding: 14px; background: linear-gradient(135deg, #1ea1f2, #0ea5e9); color: white; border: none; border-radius: 12px; font-weight: 700; font-size: 15px; cursor: pointer; transition: 0.3s; }
        .btn-pay-now:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(30,161,242,0.4); }
        .qr-img { width: 200px; height: 200px; border-radius: 12px; margin: 16px 0; border: 1px solid #e5e7eb; }
        .pay-info-compact { display: flex; justify-content: space-between; padding: 12px 16px; background: #f8fafc; border-radius: 10px; margin-bottom: 16px; font-size: 13px; font-weight: 600; }
        .pay-info-compact .highlight { color: #1ea1f2; }
        .btn-done { width: 100%; padding: 12px; background: #10b981; color: white; border: none; border-radius: 10px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .btn-done:hover { background: #059669; }

        /* Split Layout: 2/3 Itinerary + 1/3 Video */
        .split-layout-container { display: grid; grid-template-columns: 2fr 1fr; gap: 32px; margin-top: 60px; align-items: start; }
        @media (max-width: 1024px) { .split-layout-container { grid-template-columns: 1fr; } }
        .itinerary-column { min-width: 0; }
        .video-column { position: relative; }
        .video-sticky-wrapper { position: sticky; top: 100px; }
        
        /* Video Carousel Vertical */
        .video-carousel-wrapper { position: relative; border-radius: 20px; overflow: hidden; background: #000; box-shadow: 0 12px 32px rgba(0,0,0,0.2); aspect-ratio: 9/16; max-height: 700px; }
        .carousel-video { width: 100%; height: 100%; object-fit: cover; display: block; }
        .video-overlay-compact { position: absolute; bottom: 60px; left: 16px; right: 16px; z-index: 10; color: white; text-align: center; }
        .video-overlay-compact h4 { font-size: 20px; font-weight: 800; margin-bottom: 4px; text-shadow: 0 2px 10px rgba(0,0,0,0.8); }
        .video-overlay-compact span { font-size: 13px; opacity: 0.9; text-shadow: 0 2px 8px rgba(0,0,0,0.6); }
        .video-nav-compact { position: absolute; top: 50%; transform: translateY(-50%); background: rgba(255,255,255,0.85); border: none; width: 40px; height: 40px; border-radius: 50%; cursor: pointer; font-size: 16px; color: #1ea1f2; transition: 0.2s; z-index: 10; display: grid; place-items: center; }
        .video-nav-compact:hover { background: white; transform: translateY(-50%) scale(1.1); }
        .video-nav-compact.prev { left: 12px; }
        .video-nav-compact.next { right: 12px; }
        .video-dots-compact { position: absolute; bottom: 24px; left: 50%; transform: translateX(-50%); display: flex; gap: 6px; z-index: 10; }
        .dot-compact { width: 8px; height: 8px; background: rgba(255,255,255,0.5); border-radius: 50%; cursor: pointer; transition: 0.3s; }
        .dot-compact:hover { background: rgba(255,255,255,0.8); }
        .dot-compact.active { width: 24px; border-radius: 4px; background: white; }

        @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(40px); } to { opacity: 1; transform: translateY(0); } }
      `}</style>
    </main>
  );
}
