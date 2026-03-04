"use client";
import React, { useState, useEffect } from 'react';
import Footer from '@/components/Footer';
import { ReelsWidget, ChatWidget } from '@/components/Widgets';
import { StarFilled, EnvironmentOutlined } from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

export default function HotelsPage() {
    const [hotelsData, setHotelsData] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchHotels = async () => {
        try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Discovery/Hotels`);
            if (res.ok) {
                setHotelsData(await res.json());
            }
        } catch (e) {
            console.error("Fetch hotels error", e);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchHotels();
    }, []);

    const addToCart = async (hotel: any) => {
        const token = localStorage.getItem('token');
        if (!token) return alert("Vui lòng đăng nhập để đặt phòng");

        try {
            const body = {
                itemName: hotel.name,
                price: hotel.pricePerNight,
                quantity: 1,
                serviceType: 'Hotel',
                serviceId: hotel.hotelId
            };

            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/AddToCart`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(body)
            });

            if (res.ok) {
                alert(`Đã thêm ${hotel.name} vào hành trình của bạn!`);
            } else {
                alert("Lỗi khi thêm vào hành trình");
            }
        } catch (e) {
            alert("Lỗi kết nối");
        }
    };

    return (
        <main>

            <section className="hero-hotels">
                <div className="container">
                    <h1>Khách sạn & Khu nghỉ dưỡng</h1>
                    <p>Tìm kiếm nơi dừng chân hoàn hảo cho chuyến đi của bạn</p>
                </div>
            </section>

            <section className="container section-block">
                {loading ? (
                    <div className="loading" style={{ textAlign: 'center', padding: '50px' }}>Đang tải danh sách khách sạn...</div>
                ) : (
                    <div className="hotels-grid">
                        {hotelsData.map(hotel => (
                            <div key={hotel.hotelId} className="hotel-card">
                                <div className="hotel-img">
                                    <img src={hotel.imageUrl || 'https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80'} alt={hotel.name} />
                                    <div className="hotel-rating">
                                        <StarFilled style={{ color: '#fab005' }} /> {hotel.rating}
                                    </div>
                                </div>
                                <div className="hotel-info">
                                    <h3>{hotel.name}</h3>
                                    <div className="hotel-loc">
                                        <EnvironmentOutlined /> {hotel.location} {hotel.province?.provinceName ? `(${hotel.province.provinceName})` : ''}
                                    </div>
                                    <p className="hotel-desc" style={{ fontSize: '13px', color: '#64748b', marginBottom: '12px', display: '-webkit-box', WebkitLineClamp: '2', WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                                        {hotel.description && !hotel.description.includes(',') ? hotel.description : "Trải nghiệm nghỉ dưỡng đẳng cấp với đầy đủ tiện nghi và dịch vụ chất lượng."}
                                    </p>
                                    <div className="hotel-tags" style={{ display: 'flex', gap: '6px', flexWrap: 'wrap', marginBottom: '20px', minHeight: '26px' }}>
                                        {hotel.description && hotel.description.includes(',') ? (
                                            hotel.description.split(',').map((tag: string) => (
                                                <span key={tag} className="tag" style={{ background: '#f0f9ff', color: '#0ea5e9', padding: '2px 10px', borderRadius: '6px', fontSize: '11px', fontWeight: '700' }}>
                                                    {tag.trim()}
                                                </span>
                                            ))
                                        ) : (
                                            <span className="tag" style={{ background: '#f0f9ff', color: '#0ea5e9', padding: '2px 10px', borderRadius: '6px', fontSize: '11px', fontWeight: '700' }}>⭐ Dịch vụ tốt</span>
                                        )}
                                    </div>
                                    <div className="hotel-footer">
                                        <div className="price">
                                            <strong style={{ color: '#10b981', fontSize: '18px' }}>{money(hotel.pricePerNight)} VND</strong>
                                            <span style={{ fontSize: '12px', color: '#94a3b8' }}>/đêm</span>
                                        </div>
                                        <button className="btn primary" onClick={() => addToCart(hotel)}>Chọn phòng</button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </section>

            <Footer />
            <ReelsWidget />
            <ChatWidget />

            <style jsx>{`
        .hero-hotels { background: linear-gradient(rgba(0,0,0,0.4), rgba(0,0,0,0.4)), url('https://images.unsplash.com/photo-1571896349842-33c89424de2d?auto=format&fit=crop&w=1600&q=80'); background-size: cover; background-position: center; padding: 100px 0; text-align: center; color: white; }
        .hero-hotels h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .hero-hotels p { font-size: 20px; opacity: 0.9; }

        .hotels-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(380px, 1fr)); gap: 32px; margin-top: 40px; }
        @media (max-width: 768px) { .hotels-grid { grid-template-columns: 1fr; } }

        .hotel-card { background: white; border-radius: 24px; overflow: hidden; border: 1px solid var(--line); transition: 0.3s; }
        .hotel-card:hover { transform: translateY(-10px); box-shadow: 0 20px 40px rgba(0,0,0,0.1); border-color: var(--brand); }
        .hotel-img { height: 260px; position: relative; }
        .hotel-img img { width: 100%; height: 100%; object-fit: cover; }
        .hotel-rating { position: absolute; top: 16px; right: 16px; background: white; padding: 4px 12px; border-radius: 20px; font-weight: 700; font-size: 14px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        
        .hotel-info { padding: 24px; }
        .hotel-info h3 { font-size: 20px; font-weight: 800; margin-bottom: 8px; color: #0f172a; }
        .hotel-loc { color: var(--muted); font-size: 14px; margin-bottom: 16px; display: flex; align-items: center; gap: 4px; }
        
        .hotel-tags { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 24px; }
        .tag { background: #f0f9ff; color: var(--brand); padding: 4px 12px; border-radius: 8px; font-size: 12px; font-weight: 600; }

        .hotel-footer { display: flex; justify-content: space-between; align-items: center; border-top: 1px solid var(--line); pt: 20px; padding-top: 20px; }
        .price strong { font-size: 18px; color: #10b981; display: block; }
        .price span { font-size: 12px; color: var(--muted); }
      `}</style>
        </main>
    );
}
