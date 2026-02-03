"use client";
import React from 'react';
import Footer from '@/components/Footer';
import { ReelsWidget, ChatWidget } from '@/components/Widgets';
import { activities } from '@/dummy/data';
import { CompassOutlined, ArrowRightOutlined } from '@ant-design/icons';

export default function ActivitiesPage() {

    return (
        <main>

            <section className="hero-activities">
                <div className="container">
                    <h1>Hoạt động & Trải nghiệm</h1>
                    <p>Tận hưởng những khoảnh khắc đáng nhớ tại mọi điểm đến</p>
                </div>
            </section>

            <section className="container section-block">
                <div className="activities-grid">
                    {activities.map(act => (
                        <div key={act.id} className="act-card">
                            <div className="act-img">
                                <img src={act.img} alt={act.title} />
                                {act.tag && <div className="act-badge">{act.tag}</div>}
                            </div>
                            <div className="act-info">
                                <h3>{act.title}</h3>
                                <p className="price">Chỉ từ <strong>{act.price}</strong></p>
                                <button className="btn outline full">Khám phá <ArrowRightOutlined /></button>
                            </div>
                        </div>
                    ))}
                </div>
            </section>

            <Footer />
            <ReelsWidget />
            <ChatWidget />

            <style jsx>{`
        .hero-activities { background: linear-gradient(rgba(0,0,0,0.4), rgba(0,0,0,0.4)), url('https://images.unsplash.com/photo-1530789253488-b413832ca17b?auto=format&fit=crop&w=1600&q=80'); background-size: cover; background-position: center; padding: 100px 0; text-align: center; color: white; }
        .hero-activities h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .hero-activities p { font-size: 20px; opacity: 0.9; }

        .activities-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 32px; margin-top: 40px; }

        .act-card { background: white; border-radius: 20px; overflow: hidden; border: 1px solid var(--line); transition: 0.3s; }
        .act-card:hover { transform: translateY(-8px); border-color: var(--brand); box-shadow: 0 15px 30px rgba(0,0,0,0.08); }
        
        .act-img { height: 240px; position: relative; }
        .act-img img { width: 100%; height: 100%; object-fit: cover; }
        .act-badge { position: absolute; top: 16px; left: 16px; background: var(--brand); color: white; padding: 4px 12px; border-radius: 8px; font-size: 12px; font-weight: 700; }

        .act-info { padding: 24px; text-align: center; }
        .act-info h3 { font-size: 18px; font-weight: 800; margin-bottom: 12px; }
        .price { color: var(--muted); font-size: 14px; margin-bottom: 20px; }
        .price strong { color: #10b981; font-size: 16px; }
        
        .btn.outline { border: 2px solid var(--brand); color: var(--brand); font-weight: 700; width: 100%; padding: 12px; border-radius: 12px; }
        .btn.outline:hover { background: var(--brand); color: white; }
      `}</style>
        </main>
    );
}
