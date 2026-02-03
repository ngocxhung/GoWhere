"use client";
import React from 'react';
import Footer from '@/components/Footer';
import { ReelsWidget, ChatWidget } from '@/components/Widgets';
import { flights } from '@/dummy/data';
import { RocketOutlined, ArrowRightOutlined } from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

export default function FlightsPage() {

    return (
        <main>

            <section className="hero-flights">
                <div className="container">
                    <h1>Vé máy bay giá tốt</h1>
                    <p>Bay muôn nơi với các ưu đãi độc quyền từ GoWhere</p>
                </div>
            </section>

            <section className="container section-block">
                <div className="flights-list">
                    {flights.map(flight => (
                        <div key={flight.id} className="flight-card">
                            <div className="airline-info">
                                <RocketOutlined style={{ fontSize: '24px', color: 'var(--brand)' }} />
                                <strong>{flight.airline}</strong>
                            </div>
                            <div className="route-info">
                                <div className="time-node">
                                    <strong>{flight.departure}</strong>
                                    <span>{flight.from}</span>
                                </div>
                                <div className="route-line">
                                    <div className="dot"></div>
                                    <div className="line"></div>
                                    <ArrowRightOutlined />
                                    <div className="line"></div>
                                    <div className="dot"></div>
                                </div>
                                <div className="time-node">
                                    <strong>{flight.arrival}</strong>
                                    <span>{flight.to}</span>
                                </div>
                            </div>
                            <div className="class-info">
                                <span>Hạng vé</span>
                                <strong>{flight.class}</strong>
                            </div>
                            <div className="price-info">
                                <strong>{money(flight.price)} VND</strong>
                                <button className="btn primary">Chọn chuyến</button>
                            </div>
                        </div>
                    ))}
                </div>
            </section>

            <Footer />
            <ReelsWidget />
            <ChatWidget />

            <style jsx>{`
        .hero-flights { background: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('https://images.unsplash.com/photo-1436491865332-7a61a109cc05?auto=format&fit=crop&w=1600&q=80'); background-size: cover; background-position: center; padding: 100px 0; text-align: center; color: white; }
        .hero-flights h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .hero-flights p { font-size: 20px; opacity: 0.9; }

        .flights-list { display: flex; flex-direction: column; gap: 20px; margin-top: 40px; }

        .flight-card { background: white; border-radius: 20px; padding: 24px; border: 1px solid var(--line); display: flex; align-items: center; justify-content: space-between; transition: 0.3s; }
        .flight-card:hover { border-color: var(--brand); box-shadow: 0 10px 30px rgba(30,161,242,0.1); }

        .airline-info { display: flex; flex-direction: column; align-items: center; gap: 8px; width: 140px; }
        .airline-info strong { font-size: 16px; color: #0f172a; }

        .route-info { flex: 1; display: flex; align-items: center; justify-content: center; gap: 40px; }
        .time-node { text-align: center; }
        .time-node strong { font-size: 24px; display: block; color: #0f172a; }
        .time-node span { color: var(--muted); font-weight: 600; }

        .route-line { display: flex; align-items: center; gap: 8px; color: var(--line); }
        .line { width: 60px; height: 1.5px; background: var(--line); }
        .dot { width: 6px; height: 6px; background: var(--line); border-radius: 50%; }

        .class-info { text-align: center; width: 100px; }
        .class-info span { font-size: 12px; color: var(--muted); display: block; }

        .price-info { text-align: right; display: flex; flex-direction: column; gap: 12px; }
        .price-info strong { font-size: 20px; color: #10b981; }

        @media (max-width: 900px) {
          .flight-card { flex-direction: column; text-align: center; gap: 24px; }
          .route-info { width: 100%; }
          .price-info { text-align: center; }
        }
      `}</style>
        </main>
    );
}
