"use client";
import React from 'react';
import Footer from '@/components/Footer';
import { ReelsWidget, ChatWidget } from '@/components/Widgets';
import { specialOffers } from '@/dummy/data';
import { FireOutlined, CopyOutlined } from '@ant-design/icons';

export default function OffersPage() {

    return (
        <main>

            <section className="hero-offers">
                <div className="container">
                    <h1>Đặc quyền ưu đãi</h1>
                    <p>Tiết kiệm hơn, đi nhiều hơn cùng GoWhere</p>
                </div>
            </section>

            <section className="container section-block">
                <div className="offers-grid">
                    {specialOffers.map(offer => (
                        <div key={offer.id} className="offer-card">
                            <div className="offer-img">
                                <img src={offer.image} alt={offer.title} />
                                <div className="discount-tag">-{offer.discount}</div>
                            </div>
                            <div className="offer-info">
                                <h3><FireOutlined style={{ color: '#ff4d4f' }} /> {offer.title}</h3>
                                <p>{offer.description}</p>
                                <div className="coupon-box">
                                    <code>{offer.code}</code>
                                    <button className="copy-btn"><CopyOutlined /> Sao chép</button>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </section>

            <Footer />
            <ReelsWidget />
            <ChatWidget />

            <style jsx>{`
        .hero-offers { background: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?auto=format&fit=crop&w=1600&q=80'); background-size: cover; background-position: center; padding: 100px 0; text-align: center; color: white; }
        .hero-offers h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .hero-offers p { font-size: 20px; opacity: 0.9; }

        .offers-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(400px, 1fr)); gap: 32px; margin-top: 40px; }
        @media (max-width: 768px) { .offers-grid { grid-template-columns: 1fr; } }

        .offer-card { background: white; border-radius: 24px; overflow: hidden; border: 1px dashed var(--brand); display: flex; transition: 0.3s; }
        .offer-card:hover { transform: scale(1.02); box-shadow: 0 20px 40px rgba(30,161,242,0.1); }
        
        .offer-img { width: 160px; position: relative; }
        .offer-img img { width: 100%; height: 100%; object-fit: cover; }
        .discount-tag { position: absolute; inset: 0; background: rgba(30,161,242,0.8); display: grid; place-items: center; color: white; font-size: 24px; font-weight: 900; }

        .offer-info { padding: 24px; flex: 1; display: flex; flex-direction: column; gap: 12px; }
        .offer-info h3 { font-size: 18px; font-weight: 800; color: #0f172a; margin: 0; }
        .offer-info p { color: var(--muted); font-size: 14px; margin: 0; line-height: 1.5; }

        .coupon-box { margin-top: auto; display: flex; gap: 8px; }
        code { background: #f0f9ff; border: 1px solid var(--line); padding: 8px 16px; border-radius: 8px; font-weight: 700; color: var(--brand); flex: 1; text-align: center; }
        .copy-btn { border: none; background: #0f172a; color: white; padding: 8px 12px; border-radius: 8px; font-size: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .copy-btn:hover { background: var(--brand); }
      `}</style>
        </main>
    );
}
