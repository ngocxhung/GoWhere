"use client";
import React, { useState, useEffect } from 'react';
import Footer from '@/components/Footer';
import { ReelsWidget, ChatWidget } from '@/components/Widgets';
import { TeamOutlined, CarOutlined, StarFilled } from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

export default function CarsPage() {
    const [carsData, setCarsData] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchCars = async () => {
        try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Discovery/Cars`);
            if (res.ok) {
                setCarsData(await res.json());
            }
        } catch (e) {
            console.error("Fetch cars error", e);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCars();
    }, []);

    const addToCart = async (car: any) => {
        const token = localStorage.getItem('token');
        if (!token) return alert("Vui lòng đăng nhập để thuê xe");

        try {
            const body = {
                itemName: car.model,
                price: car.pricePerDay,
                quantity: 1,
                serviceType: 'Car',
                serviceId: car.carRentalId
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
                alert(`Đã thêm ${car.model} vào hành trình của bạn!`);
            } else {
                alert("Lỗi khi thêm vào hành trình");
            }
        } catch (e) {
            alert("Lỗi kết nối");
        }
    };

    return (
        <main>

            <section className="hero-cars">
                <div className="container">
                    <h1>Cho thuê xe tự lái</h1>
                    <p>Tự do khám phá mọi cung đường với dàn xe đời mới nhất</p>
                </div>
            </section>

            <section className="container section-block">
                {loading ? (
                    <div className="loading" style={{ textAlign: 'center', padding: '50px' }}>Đang tải danh sách xe...</div>
                ) : (
                    <div className="cars-grid">
                        {carsData.map(car => (
                            <div key={car.carRentalId} className="car-card">
                                <div className="car-img">
                                    <img src={car.imageUrl || 'https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=800&q=80'} alt={car.model} />
                                </div>
                                <div className="car-info">
                                    <div className="car-header">
                                        <h3>{car.model}</h3>
                                        <div className="rating"><StarFilled style={{ color: '#fab005' }} /> {car.rating}</div>
                                    </div>
                                    <div className="car-specs">
                                        <span><CarOutlined /> {car.type}</span>
                                        <span><TeamOutlined /> {car.seats} chỗ</span>
                                    </div>
                                    <div className="car-footer">
                                        <div className="price">
                                            <strong>{money(car.pricePerDay)} VND</strong>
                                            <span>/ngày</span>
                                        </div>
                                        <button className="btn primary" onClick={() => addToCart(car)}>Thuê ngay</button>
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
        .hero-cars { background: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('https://images.unsplash.com/photo-1449965408869-eaa3f722e40d?auto=format&fit=crop&w=1600&q=80'); background-size: cover; background-position: center; padding: 100px 0; text-align: center; color: white; }
        .hero-cars h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .hero-cars p { font-size: 20px; opacity: 0.9; }

        .cars-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 32px; margin-top: 40px; }

        .car-card { background: white; border-radius: 20px; overflow: hidden; border: 1px solid var(--line); transition: 0.3s; }
        .car-card:hover { transform: translateY(-8px); border-color: var(--brand); box-shadow: 0 15px 30px rgba(0,0,0,0.08); }
        
        .car-img { height: 200px; }
        .car-img img { width: 100%; height: 100%; object-fit: cover; }
        
        .car-info { padding: 24px; }
        .car-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
        .car-header h3 { font-size: 18px; font-weight: 800; }
        .rating { font-weight: 700; font-size: 14px; }

        .car-specs { display: flex; gap: 16px; margin-bottom: 24px; color: var(--muted); font-size: 14px; font-weight: 500; }
        .car-specs span { display: flex; align-items: center; gap: 6px; }

        .car-footer { display: flex; justify-content: space-between; align-items: center; padding-top: 20px; border-top: 1px solid var(--line); }
        .price strong { font-size: 18px; color: #1ea1f2; display: block; }
        .price span { font-size: 12px; color: var(--muted); }
      `}</style>
        </main>
    );
}
