'use client';

import React, { useState, useEffect } from 'react';
import { createPortal } from 'react-dom';
import { SearchOutlined, EnvironmentOutlined, DollarOutlined, FilterOutlined, CloseOutlined, ClockCircleOutlined, StarFilled, HeartOutlined, ShareAltOutlined, CameraOutlined } from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

const SpotDetailModal = ({ spot, onClose, getImageUrl }: { spot: any, onClose: () => void, getImageUrl: (s: any) => string }) => {
    const [activeImg, setActiveImg] = useState(0);
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
        document.body.style.overflow = 'hidden';
        return () => { document.body.style.overflow = ''; };
    }, []);

    if (!spot || !mounted) return null;

    const allImages = [
        getImageUrl(spot),
        ...(spot.medias?.map((m: any) => m.mediaUrl ? `${process.env.NEXT_PUBLIC_API_URL}${m.mediaUrl}` : null).filter(Boolean) || [])
    ].slice(0, 5);

    const costBreakdown = [
        { label: 'Vé vào cổng', value: spot.estimatedVisitPrice || 0 },
        { label: 'Ăn uống (ước tính)', value: 100000 },
        { label: 'Di chuyển (ước tính)', value: 80000 },
    ];
    const totalEstimate = costBreakdown.reduce((s, c) => s + c.value, 0);

    const modalContent = (
        <>
            <style>{`
                .sdm-backdrop { position: fixed; inset: 0; background: rgba(0,0,0,0.78); backdrop-filter: blur(6px); z-index: 99999; display: flex; align-items: center; justify-content: center; padding: 16px; overflow-y: auto; animation: sdmFadeIn 0.25s ease; }
                @keyframes sdmFadeIn { from { opacity: 0; } to { opacity: 1; } }
                .sdm-panel { background: white; border-radius: 24px; width: 100%; max-width: 780px; max-height: 90vh; overflow-y: auto; position: relative; animation: sdmSlideUp 0.3s cubic-bezier(0.16,1,0.3,1); box-shadow: 0 40px 80px rgba(0,0,0,0.45); flex-shrink: 0; }
                @keyframes sdmSlideUp { from { opacity: 0; transform: scale(0.94) translateY(24px); } to { opacity: 1; transform: scale(1) translateY(0); } }
                .sdm-panel::-webkit-scrollbar { width: 6px; }
                .sdm-panel::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 3px; }
                .sdm-close { position: sticky; top: 0; float: right; z-index: 20; width: 40px; height: 40px; background: rgba(0,0,0,0.55); border: none; border-radius: 50%; color: white; font-size: 16px; cursor: pointer; display: flex; align-items: center; justify-content: center; transition: 0.2s; margin: 16px 16px 0 0; }
                .sdm-close:hover { background: rgba(0,0,0,0.85); transform: rotate(90deg); }
                .sdm-gallery { padding: 0 20px; margin-top: -44px; clear: both; }
                .sdm-main-img-wrap { position: relative; height: 360px; border-radius: 20px; overflow: hidden; }
                .sdm-main-img { width: 100%; height: 100%; object-fit: cover; }
                .sdm-img-grad { position: absolute; inset: 0; background: linear-gradient(to top, rgba(0,0,0,0.55) 0%, transparent 60%); pointer-events: none; }
                .sdm-img-actions { position: absolute; top: 14px; right: 14px; display: flex; gap: 10px; }
                .sdm-img-btn { width: 38px; height: 38px; background: rgba(255,255,255,0.9); border: none; border-radius: 50%; cursor: pointer; display: flex; align-items: center; justify-content: center; font-size: 16px; color: #0f172a; transition: 0.2s; }
                .sdm-img-btn:hover { background: white; transform: scale(1.1); }
                .sdm-prov-badge { position: absolute; bottom: 14px; left: 14px; background: rgba(255,255,255,0.95); padding: 7px 16px; border-radius: 20px; font-size: 13px; font-weight: 700; color: #1ea1f2; display: flex; align-items: center; gap: 6px; }
                .sdm-thumbs { display: flex; gap: 10px; margin-top: 12px; overflow-x: auto; padding-bottom: 4px; }
                .sdm-thumb { width: 76px; height: 56px; border-radius: 10px; overflow: hidden; flex-shrink: 0; cursor: pointer; border: 3px solid transparent; transition: 0.2s; }
                .sdm-thumb.active, .sdm-thumb:hover { border-color: #1ea1f2; }
                .sdm-thumb img { width: 100%; height: 100%; object-fit: cover; }
                .sdm-body { padding: 24px 28px 32px; }
                .sdm-stars { display: flex; align-items: center; gap: 4px; margin-bottom: 10px; }
                .sdm-rating-txt { font-size: 13px; color: #64748b; font-weight: 600; margin-left: 6px; }
                .sdm-title { font-size: 30px; font-weight: 800; color: #0f172a; line-height: 1.2; margin: 0 0 24px; }
                .sdm-sec-title { font-size: 17px; font-weight: 800; color: #0f172a; margin: 24px 0 14px; }
                .sdm-desc { color: #374151; font-size: 15px; line-height: 1.8; margin: 0; }
                .sdm-tips { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
                .sdm-tip { display: flex; align-items: flex-start; gap: 12px; padding: 14px; background: #f8fafc; border-radius: 12px; border: 1px solid #f1f5f9; }
                .sdm-tip-icon { font-size: 22px; flex-shrink: 0; }
                .sdm-tip-label { font-size: 11px; color: #64748b; font-weight: 700; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 4px; }
                .sdm-tip-val { font-size: 13px; color: #0f172a; font-weight: 700; }
                .sdm-cost-box { background: #f8fafc; border-radius: 16px; padding: 20px; border: 1px solid #f1f5f9; }
                .sdm-cost-row { display: flex; justify-content: space-between; align-items: center; padding: 10px 0; border-bottom: 1px solid #e5e7eb; font-size: 15px; color: #374151; }
                .sdm-cost-total { display: flex; justify-content: space-between; align-items: center; padding: 14px 0 0; font-size: 18px; font-weight: 800; color: #1ea1f2; border-top: 2px solid #e5e7eb; margin-top: 8px; }
                .sdm-free { color: #10b981; font-weight: 700; }
                .sdm-cost-val { color: #0f172a; font-weight: 700; }
                .sdm-disclaimer { font-size: 12px; color: #94a3b8; margin-top: 10px; font-style: italic; line-height: 1.5; }
                .sdm-cost-note { font-size: 13px; font-weight: 500; color: #64748b; }
                .sdm-cta { margin-top: 24px; display: block; text-align: center; padding: 16px; background: linear-gradient(135deg, #1ea1f2, #6366f1); color: white; border-radius: 16px; font-weight: 800; font-size: 16px; text-decoration: none; transition: 0.3s; }
                .sdm-cta:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(30,161,242,0.4); }
                @media (max-width: 600px) {
                    .sdm-main-img-wrap { height: 220px; }
                    .sdm-title { font-size: 22px; }
                    .sdm-body { padding: 20px 16px 24px; }
                    .sdm-tips { grid-template-columns: 1fr; }
                }
            `}</style>
            <div className="sdm-backdrop" onClick={onClose}>
                <div className="sdm-panel" onClick={e => e.stopPropagation()}>
                    <button className="sdm-close" onClick={onClose}><CloseOutlined /></button>

                    <div className="sdm-gallery">
                        <div className="sdm-main-img-wrap">
                            <img className="sdm-main-img" src={allImages[activeImg]} alt={spot.scenicSpotName}
                                onError={(e) => { (e.target as HTMLImageElement).src = 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1200&q=80'; }} />
                            <div className="sdm-img-grad" />
                            <div className="sdm-img-actions">
                                <button className="sdm-img-btn"><HeartOutlined /></button>
                                <button className="sdm-img-btn"><ShareAltOutlined /></button>
                            </div>
                            {spot.province && (
                                <div className="sdm-prov-badge"><EnvironmentOutlined /> {spot.province.provinceName}</div>
                            )}
                        </div>
                        {allImages.length > 1 && (
                            <div className="sdm-thumbs">
                                {allImages.map((img: string, i: number) => (
                                    <div key={i} className={`sdm-thumb ${activeImg === i ? 'active' : ''}`} onClick={() => setActiveImg(i)}>
                                        <img src={img} alt="" onError={(e) => { (e.target as HTMLImageElement).src = 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=200&q=60'; }} />
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    <div className="sdm-body">
                        <div className="sdm-stars">
                            {[1,2,3,4,5].map(s => <StarFilled key={s} style={{ color: '#f59e0b', fontSize: 15 }} />)}
                            <span className="sdm-rating-txt">4.8 / 5 • Điểm đến nổi bật</span>
                        </div>
                        <h2 className="sdm-title">{spot.scenicSpotName}</h2>

                        <div className="sdm-sec-title">📍 Giới thiệu</div>
                        <p className="sdm-desc">
                            {spot.description || `${spot.scenicSpotName} là một trong những điểm đến hấp dẫn nhất tại ${spot.province?.provinceName || 'Việt Nam'}. Nơi đây mang đến cho du khách những trải nghiệm tuyệt vời và khó quên với vẻ đẹp thiên nhiên hoặc văn hóa đặc sắc.`}
                        </p>

                        <div className="sdm-sec-title">💰 Chi phí ước tính <span className="sdm-cost-note">(tham khảo / người)</span></div>
                        <div className="sdm-cost-box">
                            {costBreakdown.map((item, i) => (
                                <div key={i} className="sdm-cost-row">
                                    <span>{item.label}</span>
                                    <span className={item.value === 0 ? 'sdm-free' : 'sdm-cost-val'}>
                                        {item.value === 0 ? 'Miễn phí' : `~${money(item.value)}đ`}
                                    </span>
                                </div>
                            ))}
                            <div className="sdm-cost-total">
                                <span>Tổng ước tính</span>
                                <span>{money(totalEstimate)}đ</span>
                            </div>
                            <p className="sdm-disclaimer">* Chi phí mang tính chất tham khảo, có thể thay đổi tuỳ thời điểm và số người.</p>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );

    return createPortal(modalContent, document.body);
};

export default function DestinationsPage() {
    const [allSpots, setAllSpots] = useState<any[]>([]);
    const [filteredSpots, setFilteredSpots] = useState<any[]>([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [selectedProvince, setSelectedProvince] = useState('all');
    const [priceFilter, setPriceFilter] = useState('all');
    const [loading, setLoading] = useState(true);
    const [provinces, setProvinces] = useState<any[]>([]);
    const [selectedSpot, setSelectedSpot] = useState<any>(null);

    const ITEMS_PER_PAGE = 9;

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [spotsRes, provincesRes] = await Promise.all([
                    fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/ScenicSpot/GetAllScenicSpots`),
                    fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Province/GetAllProvinces`)
                ]);
                if (spotsRes.ok) {
                    const spotsData = await spotsRes.json();
                    setAllSpots(spotsData);
                    setFilteredSpots(spotsData);
                }
                if (provincesRes.ok) {
                    const provincesData = await provincesRes.json();
                    setProvinces(provincesData);
                }
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    useEffect(() => {
        let filtered = [...allSpots];
        if (searchTerm.trim()) {
            filtered = filtered.filter(spot =>
                spot.scenicSpotName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                spot.description?.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }
        if (selectedProvince !== 'all') {
            filtered = filtered.filter(spot => spot.provinceId === parseInt(selectedProvince));
        }
        if (priceFilter !== 'all') {
            switch (priceFilter) {
                case 'free': filtered = filtered.filter(s => s.estimatedVisitPrice === 0); break;
                case 'under100': filtered = filtered.filter(s => s.estimatedVisitPrice > 0 && s.estimatedVisitPrice < 100000); break;
                case 'under500': filtered = filtered.filter(s => s.estimatedVisitPrice >= 100000 && s.estimatedVisitPrice < 500000); break;
                case 'above500': filtered = filtered.filter(s => s.estimatedVisitPrice >= 500000); break;
            }
        }
        setFilteredSpots(filtered);
        setCurrentPage(1);
    }, [searchTerm, selectedProvince, priceFilter, allSpots]);

    const totalPages = Math.ceil(filteredSpots.length / ITEMS_PER_PAGE);
    const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
    const currentSpots = filteredSpots.slice(startIndex, startIndex + ITEMS_PER_PAGE);

    const getImageUrl = (spot: any) => {
        if (spot.imageUrl) return spot.imageUrl;
        if (spot.mediaUrls && spot.mediaUrls.length > 0) return `${process.env.NEXT_PUBLIC_API_URL}${spot.mediaUrls[0]}`;
        return 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=800&q=80';
    };

    return (
        <main className="destinations-page">
            {/* Hero */}
            <section className="destinations-hero">
                <div className="hero-overlay">
                    <div className="container hero-content">
                        <p className="hero-tag">🌏 Khám phá Việt Nam</p>
                        <h1>Tìm điểm đến <br />hoàn hảo của bạn</h1>
                        <p className="hero-sub">Hơn <strong>{allSpots.length}+</strong> địa điểm tuyệt vời trên khắp 63 tỉnh thành</p>
                    </div>
                </div>
            </section>

            {/* Search & Filter */}
            <section className="search-filter-section">
                <div className="container">
                    <div className="search-filter-card">
                        <div className="search-bar">
                            <SearchOutlined className="search-icon" />
                            <input
                                type="text"
                                placeholder="Tìm kiếm địa điểm..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                        <div className="filters">
                            <div className="filter-group">
                                <FilterOutlined className="filter-icon" />
                                <select value={selectedProvince} onChange={(e) => setSelectedProvince(e.target.value)} className="filter-select">
                                    <option value="all">Tất cả tỉnh thành</option>
                                    {provinces.map(p => (
                                        <option key={p.provinceId} value={p.provinceId}>{p.provinceName}</option>
                                    ))}
                                </select>
                            </div>
                            <div className="filter-group">
                                <DollarOutlined className="filter-icon" />
                                <select value={priceFilter} onChange={(e) => setPriceFilter(e.target.value)} className="filter-select">
                                    <option value="all">Tất cả mức giá</option>
                                    <option value="free">Miễn phí</option>
                                    <option value="under100">Dưới 100.000đ</option>
                                    <option value="under500">100.000đ – 500.000đ</option>
                                    <option value="above500">Trên 500.000đ</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div className="results-info">
                        <p>Tìm thấy <strong>{filteredSpots.length}</strong> địa điểm</p>
                    </div>
                </div>
            </section>

            {/* Grid */}
            <section className="destinations-grid-section">
                <div className="container">
                    {loading ? (
                        <div className="loading-state">
                            <div className="loader"></div>
                            <p>Đang tải địa điểm...</p>
                        </div>
                    ) : currentSpots.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-icon">🔍</div>
                            <h3>Không tìm thấy địa điểm</h3>
                            <p>Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm</p>
                        </div>
                    ) : (
                        <>
                            <div className="destinations-grid">
                                {currentSpots.map((spot) => (
                                    <div key={spot.id || spot.scenicSpotId} className="destination-card" onClick={() => setSelectedSpot(spot)}>
                                        <div className="card-image">
                                            <img
                                                src={getImageUrl(spot)}
                                                alt={spot.scenicSpotName}
                                                onError={(e) => {
                                                    (e.target as HTMLImageElement).src = 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=800&q=80';
                                                }}
                                            />
                                            <div className="card-overlay">
                                                <button className="view-detail-btn">🔍 Xem chi tiết</button>
                                            </div>
                                            <div className="card-price-badge">
                                                {spot.estimatedVisitPrice === 0
                                                    ? <span className="free-badge">Miễn phí</span>
                                                    : <span className="price-badge">{money(spot.estimatedVisitPrice)}đ</span>
                                                }
                                            </div>
                                        </div>
                                        <div className="card-content">
                                            <div className="card-top">
                                                {spot.province && (
                                                    <span className="location-badge">
                                                        <EnvironmentOutlined /> {spot.province.provinceName}
                                                    </span>
                                                )}
                                                <div className="card-rating">
                                                    <StarFilled style={{ color: '#f59e0b', fontSize: 12 }} /> 4.8
                                                </div>
                                            </div>
                                            <h3>{spot.scenicSpotName}</h3>
                                            <p className="card-description">
                                                {spot.description?.substring(0, 90) || 'Điểm đến du lịch hấp dẫn tại Việt Nam'}
                                                {(spot.description?.length || 0) > 90 ? '...' : ''}
                                            </p>
                                            <div className="card-footer">
                                                <span className="card-visit-time">
                                                    <ClockCircleOutlined /> 2 – 4 giờ
                                                </span>
                                                <span className="card-detail-link">Xem chi tiết →</span>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>

                            {totalPages > 1 && (
                                <div className="pagination">
                                    <button className="page-btn" disabled={currentPage === 1} onClick={() => { setCurrentPage(p => p - 1); window.scrollTo({ top: 0, behavior: 'smooth' }); }}>← Trước</button>
                                    <div className="page-numbers">
                                        {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
                                            <button key={page} className={`page-num ${currentPage === page ? 'active' : ''}`} onClick={() => { setCurrentPage(page); window.scrollTo({ top: 0, behavior: 'smooth' }); }}>{page}</button>
                                        ))}
                                    </div>
                                    <button className="page-btn" disabled={currentPage === totalPages} onClick={() => { setCurrentPage(p => p + 1); window.scrollTo({ top: 0, behavior: 'smooth' }); }}>Sau →</button>
                                </div>
                            )}
                        </>
                    )}
                </div>
            </section>

            {/* Detail Modal */}
            {selectedSpot && (
                <SpotDetailModal
                    spot={selectedSpot}
                    onClose={() => setSelectedSpot(null)}
                    getImageUrl={getImageUrl}
                />
            )}

            <style jsx>{`
                .destinations-page { min-height: 100vh; background: #f8f9ff; }

                /* Hero */
                .destinations-hero {
                    height: 460px;
                    background: linear-gradient(135deg, rgba(15,23,42,0.85) 0%, rgba(30,161,242,0.75) 100%),
                                url('https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=1920&q=80');
                    background-size: cover;
                    background-position: center;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }
                .hero-overlay { width: 100%; }
                .hero-content { text-align: center; color: white; }
                .hero-tag { font-size: 16px; font-weight: 700; letter-spacing: 1px; opacity: 0.9; margin-bottom: 16px; }
                .hero-content h1 { font-size: 56px; font-weight: 800; margin-bottom: 20px; text-shadow: 0 4px 24px rgba(0,0,0,0.4); line-height: 1.1; }
                .hero-sub { font-size: 18px; opacity: 0.9; }
                .hero-sub strong { color: #7bd3ff; }
                @media (max-width: 600px) { .hero-content h1 { font-size: 36px; } .destinations-hero { height: 340px; } }

                /* Search */
                .search-filter-section { margin-top: -56px; margin-bottom: 60px; position: relative; z-index: 10; }
                .search-filter-card { background: white; border-radius: 24px; padding: 28px 32px; box-shadow: 0 20px 60px rgba(0,0,0,0.15); border: 1px solid #e5e7eb; }
                .search-bar { position: relative; margin-bottom: 20px; }
                .search-icon { position: absolute; left: 20px; top: 50%; transform: translateY(-50%); font-size: 20px; color: #94a3b8; }
                .search-bar input { width: 100%; padding: 16px 20px 16px 56px; border: 2px solid #e5e7eb; border-radius: 16px; font-size: 16px; outline: none; transition: 0.3s; box-sizing: border-box; }
                .search-bar input:focus { border-color: #1ea1f2; box-shadow: 0 0 0 4px rgba(30,161,242,0.1); }
                .filters { display: flex; gap: 16px; flex-wrap: wrap; }
                .filter-group { flex: 1; min-width: 200px; display: flex; align-items: center; gap: 12px; }
                .filter-icon { font-size: 18px; color: #64748b; }
                .filter-select { flex: 1; padding: 12px 16px; border: 2px solid #e5e7eb; border-radius: 12px; font-size: 14px; font-weight: 600; outline: none; background: white; cursor: pointer; transition: 0.3s; }
                .filter-select:focus { border-color: #1ea1f2; }
                .results-info { margin-top: 20px; text-align: center; }
                .results-info p { color: #64748b; font-size: 16px; }
                .results-info strong { color: #1ea1f2; font-weight: 800; }

                /* Grid */
                .destinations-grid-section { padding-bottom: 100px; }
                .destinations-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 28px; }
                @media (max-width: 1024px) { .destinations-grid { grid-template-columns: repeat(2, 1fr); } }
                @media (max-width: 640px) { .destinations-grid { grid-template-columns: 1fr; } }

                /* Card */
                .destination-card { background: white; border-radius: 20px; overflow: hidden; box-shadow: 0 4px 16px rgba(0,0,0,0.07); border: 1px solid #f1f5f9; transition: 0.3s; cursor: pointer; }
                .destination-card:hover { transform: translateY(-8px); box-shadow: 0 20px 40px rgba(0,0,0,0.14); border-color: #1ea1f2; }

                .card-image { position: relative; height: 220px; overflow: hidden; }
                .card-image img { width: 100%; height: 100%; object-fit: cover; transition: 0.5s; }
                .destination-card:hover .card-image img { transform: scale(1.08); }

                .card-overlay { position: absolute; inset: 0; background: linear-gradient(to top, rgba(0,0,0,0.6), transparent 50%); display: flex; align-items: flex-end; justify-content: center; padding: 20px; opacity: 0; transition: 0.3s; }
                .destination-card:hover .card-overlay { opacity: 1; }
                .view-detail-btn { background: white; color: #1ea1f2; border: none; padding: 11px 24px; border-radius: 12px; font-weight: 700; font-size: 14px; cursor: pointer; transition: 0.2s; }
                .view-detail-btn:hover { transform: scale(1.05); }

                .card-price-badge { position: absolute; top: 14px; right: 14px; }
                .free-badge { background: #10b981; color: white; padding: 5px 12px; border-radius: 20px; font-size: 12px; font-weight: 700; }
                .price-badge { background: rgba(15,23,42,0.85); color: white; padding: 5px 12px; border-radius: 20px; font-size: 12px; font-weight: 700; }

                .card-content { padding: 20px; }
                .card-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
                .location-badge { display: inline-flex; align-items: center; gap: 4px; background: #f0f9ff; color: #1ea1f2; padding: 4px 10px; border-radius: 20px; font-size: 11px; font-weight: 700; }
                .card-rating { font-size: 13px; font-weight: 700; color: #0f172a; display: flex; align-items: center; gap: 4px; }

                .card-content h3 { font-size: 18px; font-weight: 800; color: #0f172a; margin-bottom: 8px; line-height: 1.3; }
                .card-description { color: #64748b; font-size: 13px; line-height: 1.6; margin-bottom: 14px; }
                .card-footer { display: flex; justify-content: space-between; align-items: center; padding-top: 14px; border-top: 1px solid #f1f5f9; }
                .card-visit-time { font-size: 12px; color: #94a3b8; display: flex; align-items: center; gap: 4px; }
                .card-detail-link { font-size: 13px; font-weight: 700; color: #1ea1f2; }

                /* Pagination */
                .pagination { display: flex; justify-content: center; align-items: center; gap: 12px; margin-top: 60px; flex-wrap: wrap; }
                .page-btn { padding: 12px 24px; background: white; border: 2px solid #e5e7eb; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
                .page-btn:hover:not(:disabled) { border-color: #1ea1f2; color: #1ea1f2; }
                .page-btn:disabled { opacity: 0.4; cursor: not-allowed; }
                .page-numbers { display: flex; gap: 8px; flex-wrap: wrap; justify-content: center; }
                .page-num { width: 44px; height: 44px; background: white; border: 2px solid #e5e7eb; border-radius: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
                .page-num:hover { border-color: #1ea1f2; color: #1ea1f2; }
                .page-num.active { background: #1ea1f2; color: white; border-color: #1ea1f2; }

                /* States */
                .loading-state, .empty-state { text-align: center; padding: 80px 20px; }
                .loader { width: 48px; height: 48px; border: 5px solid #f3f3f3; border-top: 5px solid #1ea1f2; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 20px; }
                @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
                .empty-icon { font-size: 64px; margin-bottom: 16px; }
                .empty-state h3 { font-size: 24px; font-weight: 800; color: #0f172a; margin-bottom: 8px; }
                .empty-state p { color: #64748b; }
            `}</style>
        </main>
    );
}
