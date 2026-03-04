'use client';

import React, { useState, useEffect } from 'react';
// import Header from '@/components/Header';
// import Footer from '@/components/Footer';
import {
    UserOutlined,
    EnvironmentOutlined,
    HomeOutlined,
    CarOutlined,
    CreditCardOutlined,
    DashboardOutlined,
    PlusOutlined,
    EditOutlined,
    DeleteOutlined,
    CheckCircleOutlined,
    SyncOutlined,
    EyeOutlined
} from '@ant-design/icons';

const AdminPage = () => {
    const [activeTab, setActiveTab] = useState('dashboard');
    const [loading, setLoading] = useState(false);
    const [data, setData] = useState<any[]>([]);
    const [stats, setStats] = useState({
        usersCount: 0,
        spotsCount: 0,
        hotelsCount: 0,
        ordersCount: 0,
        revenue: 0
    });
    const [autoRefreshing, setAutoRefreshing] = useState(false);
    const [nextRefresh, setNextRefresh] = useState(10);
    const [fullname, setFullname] = useState('Admin');
    const [provinces, setProvinces] = useState<any[]>([]);

    // Scenic Spot Modal State
    const [isSpotModalOpen, setIsSpotModalOpen] = useState(false);
    const [editingSpot, setEditingSpot] = useState<any>(null);
    const [spotForm, setSpotForm] = useState({
        scenicSpotName: '',
        description: '',
        provinceId: 0,
        estimatedVisitPrice: 0,
        imageUrl: '',
        isViewed: true,
        mediaUrls: '' // Comma separated for simplicity in UI
    });

    useEffect(() => {
        // Check role và get fullname từ localStorage (chỉ chạy ở client)
        if (typeof window !== 'undefined') {
            const role = localStorage.getItem('role');
            if (role !== 'Admin') {
                window.location.href = '/';
            }
            setFullname(localStorage.getItem('fullname') || 'Admin');
        }
        fetchStats();
        fetchProvinces();
    }, []);

    const fetchProvinces = async () => {
        try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Province/GetAllProvinces`);
            if (res.ok) setProvinces(await res.json());
        } catch (e) { console.error('Fetch provinces failed'); }
    };

    useEffect(() => {
        if (activeTab !== 'dashboard') {
            fetchData();
        }
    }, [activeTab]);

    // Auto-refresh cho tab payments
    useEffect(() => {
        if (activeTab === 'payments') {
            // Countdown timer
            const countdownInterval = setInterval(() => {
                setNextRefresh(prev => prev > 0 ? prev - 1 : 10);
            }, 1000);

            // Auto-refresh mỗi 10 giây (silent mode)
            const refreshInterval = setInterval(async () => {
                setAutoRefreshing(true);
                await fetchData(true);  // ✅ Silent = true, không show loading spinner
                setAutoRefreshing(false);
                setNextRefresh(10);
            }, 10000);

            return () => {
                clearInterval(countdownInterval);
                clearInterval(refreshInterval);
            };
        }
    }, [activeTab]);

    const fetchStats = async () => {
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Admin/DashboardStats`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                const s = await res.json();
                setStats(s);
            }
        } catch (e) {
            console.error('Fetch stats failed');
        }
    };

    const fetchData = async (silent = false) => {
        if (!silent) setLoading(true);
        try {
            const token = localStorage.getItem('token');
            let endpoint = '';
            switch (activeTab) {
                case 'users': endpoint = 'Users'; break;
                case 'spots': endpoint = 'ScenicSpots'; break;
                case 'hotels': endpoint = 'Hotels'; break;
                case 'cars': endpoint = 'CarRentals'; break;
                case 'payments': endpoint = 'Orders'; break;
            }

            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Admin/${endpoint}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                const json = await res.json();
                setData(json);
            }
        } catch (e) {
            console.error('Fetch data failed');
        } finally {
            if (!silent) setLoading(false);
        }
    };

    const confirmOrder = async (id: number) => {
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Admin/Orders/${id}/Confirm`, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                alert('Đã xác nhận thanh toán!');
                fetchData(true);
                fetchStats(); // Update revenue
            }
        } catch (e) {
            alert('Lỗi khi xác nhận');
        }
    };

    const handleAddSpot = () => {
        setEditingSpot(null);
        setSpotForm({
            scenicSpotName: '',
            description: '',
            provinceId: provinces[0]?.provinceId || 0,
            estimatedVisitPrice: 0,
            imageUrl: '',
            isViewed: true,
            mediaUrls: ''
        });
        setIsSpotModalOpen(true);
    };

    const handleEditSpot = (spot: any) => {
        setEditingSpot(spot);
        setSpotForm({
            scenicSpotName: spot.scenicSpotName,
            description: spot.description,
            provinceId: spot.provinceId,
            estimatedVisitPrice: spot.estimatedVisitPrice,
            imageUrl: spot.imageUrl || '',
            isViewed: spot.isViewed,
            mediaUrls: spot.medias?.map((m: any) => m.mediaUrl).join(', ') || ''
        });
        setIsSpotModalOpen(true);
    };

    const handleSaveSpot = async () => {
        try {
            const token = localStorage.getItem('token');
            const url = editingSpot
                ? `${process.env.NEXT_PUBLIC_API_URL}/api/Admin/ScenicSpots/${editingSpot.scenicSpotId}`
                : `${process.env.NEXT_PUBLIC_API_URL}/api/Admin/ScenicSpots`;

            const method = editingSpot ? 'PUT' : 'POST';

            // Map comma separated URLs back to Medias array if backend expects it
            // For now, let's just send the basic fields and see
            const body = {
                ...spotForm,
                scenicSpotId: editingSpot?.scenicSpotId || 0,
                // Converting back to Medias collection if needed
                medias: spotForm.mediaUrls.split(',').filter(u => u.trim()).map(u => ({ mediaUrl: u.trim() }))
            };

            const res = await fetch(url, {
                method,
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(body)
            });

            if (res.ok) {
                alert(editingSpot ? 'Đã cập nhật!' : 'Đã thêm mới!');
                setIsSpotModalOpen(false);
                fetchData();
            } else {
                const err = await res.json();
                alert('Lỗi: ' + (err.message || 'Không thể lưu'));
            }
        } catch (e) {
            alert('Lỗi khi lưu địa điểm');
        }
    };

    const updateOrderStatus = async (orderId: number, newStatus: string) => {
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AdminOrder/UpdateOrderStatus/${orderId}`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(newStatus)
            });
            if (res.ok) {
                alert('Đã cập nhật trạng thái!');
                fetchData();
            } else {
                const err = await res.json();
                alert(err.Message || 'Không thể cập nhật');
            }
        } catch (e) {
            alert('Lỗi khi cập nhật');
        }
    };

    const deleteItem = async (id: number) => {
        if (!confirm('Bạn có chắc chắn muốn xóa?')) return;
        try {
            const token = localStorage.getItem('token');
            let endpoint = '';
            switch (activeTab) {
                case 'users': endpoint = 'Users'; break;
                case 'spots': endpoint = 'ScenicSpots'; break;
                case 'hotels': endpoint = 'Hotels'; break;
            }
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Admin/${endpoint}/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                fetchData();
            }
        } catch (e) {
            alert('Lỗi khi xóa');
        }
    };

    const sidebarItems = [
        { id: 'dashboard', label: 'Dashboard', icon: <DashboardOutlined /> },
        { id: 'users', label: 'Người dùng', icon: <UserOutlined /> },
        { id: 'spots', label: 'Địa điểm', icon: <EnvironmentOutlined /> },
        { id: 'hotels', label: 'Khách sạn', icon: <HomeOutlined /> },
        { id: 'payments', label: 'Đơn hàng', icon: <CreditCardOutlined /> },
    ];

    return (
        <div className="admin-layout">
            <div className="admin-header">
                <div className="admin-header-inner">
                    <div className="brand-section">
                        <div className="logo-admin">🏝️</div>
                        <div>
                            <h1>GoWhere Admin</h1>
                            <p>Quản trị hệ thống</p>
                        </div>
                    </div>
                    <div className="user-section">
                        <span>{fullname}</span>
                        <button className="btn-logout" onClick={() => {
                            if (typeof window !== 'undefined') {
                                localStorage.clear();
                                window.location.href = '/';
                            }
                        }}>
                            Đăng xuất
                        </button>
                    </div>
                </div>
            </div>

            <main className="admin-main">
                <aside className="admin-sidebar">
                    <div className="sidebar-header">
                        <h3>Quản trị hệ thống</h3>
                    </div>
                    <nav className="sidebar-nav">
                        {sidebarItems.map(item => (
                            <button
                                key={item.id}
                                className={`sidebar-item ${activeTab === item.id ? 'active' : ''}`}
                                onClick={() => setActiveTab(item.id)}
                            >
                                {item.icon}
                                <span>{item.label}</span>
                            </button>
                        ))}
                    </nav>
                </aside>

                <section className="admin-content">
                    <header className="content-header">
                        <h2>{sidebarItems.find(i => i.id === activeTab)?.label}</h2>
                        <div className="header-actions">
                            {activeTab === 'payments' && (
                                <div className="refresh-indicator">
                                    {autoRefreshing ? (
                                        <span className="refreshing"><SyncOutlined spin /> Đang làm mới...</span>
                                    ) : (
                                        <span className="countdown">🔄 Làm mới sau {nextRefresh}s</span>
                                    )}
                                </div>
                            )}
                            {activeTab !== 'dashboard' && activeTab !== 'payments' && activeTab !== 'users' && (
                                <button className="btn primary btn-add" onClick={activeTab === 'spots' ? handleAddSpot : () => { }}>
                                    <PlusOutlined /> Thêm mới
                                </button>
                            )}
                        </div>
                    </header>

                    <div className="content-body">
                        {loading ? (
                            <div className="admin-loading"><SyncOutlined spin /> Đang tải dữ liệu...</div>
                        ) : (
                            <>
                                {activeTab === 'dashboard' && <DashboardView stats={stats} />}
                                {activeTab === 'users' && <UserManagement data={data} onDelete={deleteItem} />}
                                {activeTab === 'spots' && <ScenicSpotManagement data={data} onEdit={handleEditSpot} onDelete={deleteItem} />}
                                {activeTab === 'hotels' && <HotelManagement data={data} />}
                                {activeTab === 'payments' && <PaymentManagement data={data} onConfirm={confirmOrder} onUpdateStatus={updateOrderStatus} />}
                            </>
                        )}
                    </div>

                    {isSpotModalOpen && (
                        <div className="modal-overlay-admin">
                            <div className="modal-admin">
                                <div className="modal-header-admin">
                                    <h3>{editingSpot ? 'Chỉnh sửa địa điểm' : 'Thêm địa điểm mới'}</h3>
                                    <button className="close-btn" onClick={() => setIsSpotModalOpen(false)}>×</button>
                                </div>
                                <div className="modal-body-admin">
                                    <div className="form-group-admin">
                                        <label>Tên địa điểm</label>
                                        <input
                                            value={spotForm.scenicSpotName}
                                            onChange={e => setSpotForm({ ...spotForm, scenicSpotName: e.target.value })}
                                            placeholder="VD: Vịnh Hạ Long"
                                        />
                                    </div>
                                    <div className="form-group-admin">
                                        <label>Tỉnh thành</label>
                                        <select
                                            value={spotForm.provinceId}
                                            onChange={e => setSpotForm({ ...spotForm, provinceId: parseInt(e.target.value) })}
                                        >
                                            {provinces.map(p => (
                                                <option key={p.provinceId} value={p.provinceId}>{p.provinceName}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <div className="form-group-row">
                                        <div className="form-group-admin">
                                            <label>Giá vé (VND)</label>
                                            <input
                                                type="number"
                                                value={spotForm.estimatedVisitPrice}
                                                onChange={e => setSpotForm({ ...spotForm, estimatedVisitPrice: parseFloat(e.target.value) })}
                                            />
                                        </div>
                                        <div className="form-group-admin">
                                            <label>Trạng thái</label>
                                            <select
                                                value={spotForm.isViewed ? 'true' : 'false'}
                                                onChange={e => setSpotForm({ ...spotForm, isViewed: e.target.value === 'true' })}
                                            >
                                                <option value="true">Hiển thị</option>
                                                <option value="false">Ẩn</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div className="form-group-admin">
                                        <label>Ảnh đại diện (URL)</label>
                                        <input
                                            value={spotForm.imageUrl}
                                            onChange={e => setSpotForm({ ...spotForm, imageUrl: e.target.value })}
                                            placeholder="https://example.com/image.jpg"
                                        />
                                        {spotForm.imageUrl && <img src={spotForm.imageUrl} className="preview-img" alt="" />}
                                    </div>
                                    <div className="form-group-admin">
                                        <label>Danh sách Media (Cách nhau bởi dấu phẩy)</label>
                                        <textarea
                                            rows={3}
                                            value={spotForm.mediaUrls}
                                            onChange={e => setSpotForm({ ...spotForm, mediaUrls: e.target.value })}
                                            placeholder="URL ảnh 1, URL ảnh 2, URL video..."
                                        />
                                        <p className="hint">Dùng để hiển thị các ảnh/video khác cho khách du lịch</p>
                                    </div>
                                    <div className="form-group-admin">
                                        <label>Mô tả chi tiết</label>
                                        <textarea
                                            rows={5}
                                            value={spotForm.description}
                                            onChange={e => setSpotForm({ ...spotForm, description: e.target.value })}
                                        />
                                    </div>
                                </div>
                                <div className="modal-footer-admin">
                                    <button className="btn outline" onClick={() => setIsSpotModalOpen(false)}>Hủy</button>
                                    <button className="btn primary" onClick={handleSaveSpot}>Lưu lại</button>
                                </div>
                            </div>
                        </div>
                    )}
                </section>
            </main>

            <style jsx>{`
                .admin-layout { min-height: 100vh; background: #f1f5f9; }
                
                .admin-header { position: sticky; top: 0; z-index: 100; background: white; border-bottom: 1px solid #e2e8f0; box-shadow: 0 1px 3px rgba(0,0,0,0.05); }
                .admin-header-inner { max-width: 1600px; margin: 0 auto; padding: 16px 40px; display: flex; justify-content: space-between; align-items: center; }
                .brand-section { display: flex; align-items: center; gap: 16px; }
                .logo-admin { width: 48px; height: 48px; background: linear-gradient(135deg, #1ea1f2, #7bd3ff); border-radius: 12px; display: grid; place-items: center; font-size: 24px; }
                .brand-section h1 { font-size: 20px; font-weight: 800; color: #0f172a; margin: 0; }
                .brand-section p { font-size: 12px; color: #64748b; margin: 0; }
                .user-section { display: flex; align-items: center; gap: 16px; }
                .user-section span { font-weight: 600; color: #0f172a; }
                .btn-logout { padding: 8px 16px; background: #fee2e2; color: #ef4444; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; transition: 0.2s; }
                .btn-logout:hover { background: #fca5a5; }
                
                .admin-main { display: flex; max-width: 1600px; margin: 0 auto; gap: 0; min-height: calc(100vh - 80px); }
                .admin-loading { text-align: center; padding: 100px; color: #6366f1; font-weight: 700; font-size: 18px; display: flex; align-items: center; justify-content: center; gap: 12px; }
                
                .admin-sidebar { width: 280px; background: white; border-right: 1px solid #e2e8f0; padding: 32px 16px; display: flex; flex-direction: column; position: sticky; top: 80px; height: calc(100vh - 80px); overflow-y: auto; }
                .sidebar-header { padding: 0 16px 24px; border-bottom: 1px solid #f1f5f9; margin-bottom: 24px; }
                .sidebar-header h3 { font-size: 18px; font-weight: 800; color: #0f172a; }
                
                .sidebar-nav { display: flex; flex-direction: column; gap: 8px; }
                .sidebar-item { display: flex; align-items: center; gap: 12px; padding: 14px 16px; border: none; background: none; border-radius: 12px; color: #64748b; font-weight: 600; cursor: pointer; transition: 0.2s; text-align: left; font-size: 15px; }
                .sidebar-item:hover { background: #f1f5f9; color: #0f172a; }
                .sidebar-item.active { background: #eef2ff; color: #6366f1; }
                
                .admin-content { flex: 1; padding: 40px; }
                .content-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 32px; }
                .content-header h2 { font-size: 28px; font-weight: 800; color: #0f172a; }
                
                .refresh-indicator { display: flex; align-items: center; gap: 10px; }
                .refreshing { color: #1ea1f2; font-weight: 600; font-size: 14px; display: flex; align-items: center; gap: 8px; }
                .countdown { background: #f0f9ff; color: #0284c7; padding: 8px 14px; border-radius: 10px; font-size: 13px; font-weight: 600; border: 1px solid #bae6fd; }
                
                .btn-add { display: flex; align-items: center; gap: 8px; font-size: 14px; padding: 10px 20px; border-radius: 12px; }
                
                .admin-table-container { background: white; border-radius: 20px; border: 1px solid #e2e8f0; overflow: hidden; }
                table { width: 100%; border-collapse: collapse; text-align: left; }
                th { background: #f8fafc; padding: 16px 24px; font-size: 13px; font-weight: 700; color: #64748b; border-bottom: 1px solid #e2e8f0; }
                td { padding: 16px 24px; font-size: 14px; color: #0f172a; border-bottom: 1px solid #f1f5f9; }
                .user-cell { display: flex; align-items: center; gap: 12px; }
                .avatar-sm { width: 32px; height: 32px; border-radius: 50%; background: #6366f1; color: white; display: grid; place-items: center; font-weight: 700; font-size: 12px; }
                .badge { padding: 4px 12px; border-radius: 20px; background: #f1f5f9; font-size: 12px; font-weight: 600; color: #64748b; }
                .badge.success { background: #ecfdf5; color: #10b981; }
                .badge.warning { background: #fffbeb; color: #f59e0b; }
                .actions { display: flex; gap: 8px; }
                .btn-icon { width: 32px; height: 32px; border-radius: 8px; border: 1px solid #e2e8f0; background: white; cursor: pointer; display: grid; place-items: center; color: #64748b; transition: 0.2s; }
                .btn-icon:hover { border-color: #6366f1; color: #6366f1; }
                .btn-icon.delete:hover { border-color: #ef4444; color: #ef4444; }
                .btn-icon.success:hover { border-color: #10b981; color: #10b981; }

                .dashboard-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 24px; }
                .stat-card { background: white; padding: 24px; border-radius: 20px; border: 1px solid #e2e8f0; display: flex; align-items: center; gap: 20px; transition: 0.3s; }
                .stat-card:hover { transform: translateY(-4px); box-shadow: 0 12px 24px rgba(0,0,0,0.05); }
                .stat-icon { width: 56px; height: 56px; border-radius: 16px; display: grid; place-items: center; font-size: 24px; }
                .stat-info h4 { font-size: 14px; color: #64748b; margin-bottom: 4px; }
                .stat-info strong { font-size: 24px; color: #0f172a; }

                /* Admin Modals */
                .modal-overlay-admin { position: fixed; inset: 0; background: rgba(0,0,0,0.6); backdrop-filter: blur(4px); z-index: 2000; display: grid; place-items: center; padding: 40px; }
                .modal-admin { background: white; width: 100%; max-width: 600px; border-radius: 20px; overflow: hidden; box-shadow: 0 25px 50px -12px rgba(0,0,0,0.25); animation: zoomIn 0.3s ease; display: flex; flex-direction: column; max-height: 90vh; }
                .modal-header-admin { padding: 20px 24px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; background: #f8fafc; }
                .modal-header-admin h3 { margin: 0; font-size: 18px; font-weight: 800; color: #0f172a; }
                .close-btn { background: none; border: none; font-size: 24px; color: #94a3b8; cursor: pointer; }
                
                .modal-body-admin { padding: 24px; overflow-y: auto; display: flex; flex-direction: column; gap: 20px; }
                .form-group-admin { display: flex; flex-direction: column; gap: 8px; }
                .form-group-admin label { font-size: 13px; font-weight: 700; color: #475569; }
                .form-group-admin input, .form-group-admin select, .form-group-admin textarea { padding: 10px 14px; border-radius: 10px; border: 1.5px solid #e2e8f0; font-size: 14px; outline: none; transition: 0.2s; }
                .form-group-admin input:focus { border-color: #6366f1; box-shadow: 0 0 0 4px rgba(99, 102, 241, 0.1); }
                .form-group-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
                .preview-img { width: 100%; height: 120px; object-fit: cover; border-radius: 8px; margin-top: 8px; border: 1px solid #e2e8f0; }
                .hint { font-size: 11px; color: #94a3b8; margin: 0; }

                .modal-footer-admin { padding: 20px 24px; border-top: 1px solid #f1f5f9; display: flex; justify-content: flex-end; gap: 12px; background: #f8fafc; }
                
                @keyframes zoomIn { from { opacity: 0; transform: scale(0.95); } to { opacity: 1; transform: scale(1); } }
            `}</style>
        </div>
    );
};

const DashboardView = ({ stats }: any) => {
    const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

    return (
        <div>
            <div className="welcome-banner">
                <div className="welcome-content">
                    <h2>Chào mừng trở lại, Admin!</h2>
                    <p>Tổng quan hệ thống GoWhere</p>
                </div>
                <div className="welcome-emoji">📊</div>
            </div>

            <div className="stats-grid-dashboard">
                <div className="stat-card-dash stat-blue">
                    <div className="stat-icon-dash"><UserOutlined /></div>
                    <div className="stat-info-dash">
                        <div className="stat-num">{stats.usersCount || 0}</div>
                        <div className="stat-lbl">Người dùng</div>
                    </div>
                </div>

                <div className="stat-card-dash stat-green">
                    <div className="stat-icon-dash"><EnvironmentOutlined /></div>
                    <div className="stat-info-dash">
                        <div className="stat-num">{stats.spotsCount || 0}</div>
                        <div className="stat-lbl">Địa điểm</div>
                    </div>
                </div>

                <div className="stat-card-dash stat-orange">
                    <div className="stat-icon-dash"><HomeOutlined /></div>
                    <div className="stat-info-dash">
                        <div className="stat-num">{stats.hotelsCount || 0}</div>
                        <div className="stat-lbl">Khách sạn</div>
                    </div>
                </div>

                <div className="stat-card-dash stat-cyan">
                    <div className="stat-icon-dash"><CreditCardOutlined /></div>
                    <div className="stat-info-dash">
                        <div className="stat-num">{money(stats.revenue || 0)}</div>
                        <div className="stat-lbl">Doanh thu</div>
                    </div>
                </div>
            </div>

            <div className="chart-section">
                <div className="chart-card-full">
                    <h3>Hoạt động hệ thống</h3>
                    <div className="chart-placeholder">
                        <DashboardOutlined style={{ fontSize: '64px', color: '#cbd5e1' }} />
                        <p>Biểu đồ thống kê sẽ hiển thị tại đây</p>
                    </div>
                </div>
            </div>

            <style jsx>{`
                .welcome-banner { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 20px; padding: 36px 40px; margin-bottom: 32px; display: flex; justify-content: space-between; align-items: center; color: white; box-shadow: 0 10px 30px rgba(102, 126, 234, 0.25); }
                .welcome-content h2 { font-size: 32px; font-weight: 800; margin: 0 0 8px 0; }
                .welcome-content p { font-size: 16px; opacity: 0.95; margin: 0; }
                .welcome-emoji { font-size: 72px; opacity: 0.9; }

                .stats-grid-dashboard { display: grid; grid-template-columns: repeat(4, 1fr); gap: 24px; margin-bottom: 32px; }
                @media (max-width: 1200px) { .stats-grid-dashboard { grid-template-columns: repeat(2, 1fr); } }
                
                .stat-card-dash { background: white; padding: 28px; border-radius: 18px; border: 1px solid #e2e8f0; display: flex; align-items: center; gap: 20px; transition: 0.3s; position: relative; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.05); }
                .stat-card-dash::before { content: ''; position: absolute; top: 0; left: 0; right: 0; height: 4px; }
                .stat-card-dash:hover { transform: translateY(-6px); box-shadow: 0 12px 28px rgba(0,0,0,0.1); }
                
                .stat-blue::before { background: linear-gradient(90deg, #3b82f6, #60a5fa); }
                .stat-green::before { background: linear-gradient(90deg, #10b981, #34d399); }
                .stat-orange::before { background: linear-gradient(90deg, #f59e0b, #fbbf24); }
                .stat-cyan::before { background: linear-gradient(90deg, #1ea1f2, #0ea5e9); }
                
                .stat-icon-dash { width: 68px; height: 68px; border-radius: 16px; display: grid; place-items: center; font-size: 32px; flex-shrink: 0; }
                .stat-blue .stat-icon-dash { background: #eff6ff; color: #3b82f6; }
                .stat-green .stat-icon-dash { background: #ecfdf5; color: #10b981; }
                .stat-orange .stat-icon-dash { background: #fffbeb; color: #f59e0b; }
                .stat-cyan .stat-icon-dash { background: #f0f9ff; color: #1ea1f2; }
                
                .stat-info-dash { flex: 1; }
                .stat-num { font-size: 36px; font-weight: 800; color: #0f172a; line-height: 1; margin-bottom: 8px; }
                .stat-lbl { font-size: 14px; color: #64748b; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; }

                .chart-section { display: grid; gap: 24px; }
                .chart-card-full { background: white; padding: 36px; border-radius: 20px; border: 1px solid #e2e8f0; box-shadow: 0 1px 3px rgba(0,0,0,0.05); }
                .chart-card-full h3 { font-size: 20px; font-weight: 800; color: #0f172a; margin-bottom: 28px; }
                .chart-placeholder { text-align: center; padding: 100px 40px; color: #94a3b8; }
                .chart-placeholder p { margin-top: 20px; font-size: 15px; }
            `}</style>
        </div>
    );
};

const UserManagement = ({ data, onDelete }: any) => (
    <div className="table-wrapper-modern">
        <table className="modern-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Người dùng</th>
                    <th>Liên hệ</th>
                    <th>Vai trò</th>
                    <th>Thao tác</th>
                </tr>
            </thead>
            <tbody>
                {data.map((u: any) => (
                    <tr key={u.userId}>
                        <td><span className="id-badge">#{u.userId}</span></td>
                        <td>
                            <div className="user-cell-modern">
                                <div className="avatar-modern">{u.username?.charAt(0).toUpperCase()}</div>
                                <div className="user-info-col">
                                    <strong>{u.fullName || u.username}</strong>
                                    <span className="sub-text">@{u.username}</span>
                                </div>
                            </div>
                        </td>
                        <td>
                            <div className="contact-info">
                                <span>{u.email}</span>
                                <span className="sub-text">{u.phoneNumber}</span>
                            </div>
                        </td>
                        <td>
                            <span className={`role-badge ${u.roleId === 2 ? 'badge-admin' : 'badge-user'}`}>
                                {u.roleId === 2 ? '👑 Admin' : '👤 User'}
                            </span>
                        </td>
                        <td>
                            <div className="action-buttons">
                                <button className="action-btn edit-btn" title="Sửa"><EditOutlined /></button>
                                <button
                                    className="action-btn delete-btn"
                                    onClick={() => onDelete(u.userId)}
                                    disabled={u.roleId === 2}
                                    title="Xóa"
                                    style={{ opacity: u.roleId === 2 ? 0.3 : 1, cursor: u.roleId === 2 ? 'not-allowed' : 'pointer' }}
                                >
                                    <DeleteOutlined />
                                </button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
        <style jsx>{`
            .table-wrapper-modern { background: white; border-radius: 16px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 1px 3px rgba(0,0,0,0.05); }
            .modern-table { width: 100%; border-collapse: collapse; }
            .modern-table thead { background: linear-gradient(to right, #f8fafc, #f1f5f9); }
            .modern-table th { padding: 16px 20px; font-size: 11px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid #e2e8f0; }
            .modern-table td { padding: 14px 20px; font-size: 14px; border-bottom: 1px solid #f1f5f9; }
            .modern-table tbody tr { transition: 0.15s; }
            .modern-table tbody tr:hover { background: #f8fafc; }
            .id-badge { background: #eef2ff; color: #6366f1; padding: 4px 10px; border-radius: 6px; font-weight: 700; font-size: 12px; }
            .user-cell-modern { display: flex; align-items: center; gap: 12px; }
            .avatar-modern { width: 40px; height: 40px; border-radius: 12px; background: linear-gradient(135deg, #6366f1, #8b5cf6); color: white; display: grid; place-items: center; font-weight: 800; font-size: 16px; }
            .user-info-col { display: flex; flex-direction: column; gap: 2px; }
            .user-info-col strong { font-size: 14px; font-weight: 700; }
            .contact-info { display: flex; flex-direction: column; gap: 2px; }
            .contact-info span { font-size: 13px; color: #475569; }
            .sub-text { font-size: 12px; color: #94a3b8; }
            .role-badge { padding: 6px 12px; border-radius: 20px; font-size: 12px; font-weight: 700; display: inline-block; }
            .badge-admin { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #92400e; }
            .badge-user { background: #eff6ff; color: #3b82f6; }
            .action-buttons { display: flex; gap: 8px; justify-content: center; }
            .action-btn { width: 34px; height: 34px; border-radius: 8px; border: 1px solid #e2e8f0; background: white; cursor: pointer; display: grid; place-items: center; font-size: 14px; transition: 0.2s; }
            .edit-btn { color: #6366f1; }
            .edit-btn:hover { background: #eef2ff; border-color: #6366f1; }
            .delete-btn { color: #ef4444; }
            .delete-btn:hover:not(:disabled) { background: #fef2f2; border-color: #ef4444; }
        `}</style>
    </div>
);

const ScenicSpotManagement = ({ data, onEdit, onDelete }: any) => (
    <div className="admin-table-container">
        <table>
            <thead>
                <tr>
                    <th>Ảnh</th>
                    <th>Tên địa điểm</th>
                    <th>Tỉnh thành</th>
                    <th>Giá vé</th>
                    <th>Trạng thái</th>
                    <th>Thao tác</th>
                </tr>
            </thead>
            <tbody>
                {data.map((s: any) => (
                    <tr key={s.scenicSpotId}>
                        <td><img src={s.imageUrl} style={{ width: 80, height: 50, objectFit: 'cover', borderRadius: 8 }} alt="" /></td>
                        <td><strong>{s.scenicSpotName}</strong></td>
                        <td>{s.province?.provinceName}</td>
                        <td>{s.estimatedVisitPrice === 0 ? 'Miễn phí' : `${s.estimatedVisitPrice} VND`}</td>
                        <td><span className={`badge ${s.isViewed ? 'success' : 'warning'}`}>{s.isViewed ? 'Hiển thị' : 'Ẩn'}</span></td>
                        <td>
                            <div className="action-buttons-cell">
                                <button className="btn-icon edit" onClick={() => onEdit(s)}><EditOutlined /></button>
                                <button className="btn-icon delete" onClick={() => onDelete(s.scenicSpotId)}><DeleteOutlined /></button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
        <style jsx>{`
            .action-buttons-cell { display: flex; gap: 8px; }
            .btn-icon { width: 32px; height: 32px; border-radius: 8px; border: 1px solid #e2e8f0; background: white; cursor: pointer; display: grid; place-items: center; color: #64748b; transition: 0.2s; }
            .btn-icon:hover { border-color: #6366f1; color: #6366f1; }
            .btn-icon.delete:hover { border-color: #ef4444; color: #ef4444; }
        `}</style>
    </div>
);

const HotelManagement = ({ data }: any) => (
    <div className="admin-table-container">
        <table>
            <thead>
                <tr>
                    <th>Ảnh</th>
                    <th>Khách sạn</th>
                    <th>Vị trí</th>
                    <th>Giá/Đêm</th>
                    <th>Đánh giá</th>
                    <th>Thao tác</th>
                </tr>
            </thead>
            <tbody>
                {data.map((h: any) => (
                    <tr key={h.hotelId}>
                        <td><img src={h.imageUrl} style={{ width: 80, height: 50, objectFit: 'cover', borderRadius: 8 }} alt="" /></td>
                        <td><strong>{h.name}</strong></td>
                        <td>{h.location}</td>
                        <td>{h.pricePerNight} VND</td>
                        <td>⭐ {h.rating}</td>
                        <td>
                            <div className="actions">
                                <button className="btn-icon"><EditOutlined /></button>
                                <button className="btn-icon delete"><DeleteOutlined /></button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    </div>
);

const CarManagement = ({ data }: any) => (
    <div className="admin-table-container">
        <table>
            <thead>
                <tr>
                    <th>Mẫu xe</th>
                    <th>Loại</th>
                    <th>Chỗ ngồi</th>
                    <th>Giá/Ngày</th>
                    <th>Thao tác</th>
                </tr>
            </thead>
            <tbody>
                {data.map((c: any) => (
                    <tr key={c.carRentalId}>
                        <td><strong>{c.model}</strong></td>
                        <td>{c.type}</td>
                        <td>{c.seats} chỗ</td>
                        <td>{c.pricePerDay.toLocaleString()} VND</td>
                        <td>
                            <div className="actions">
                                <button className="btn-icon"><EditOutlined /></button>
                                <button className="btn-icon delete"><DeleteOutlined /></button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    </div>
);

const PaymentManagement = ({ data, onConfirm, onUpdateStatus }: any) => {
    const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

    // Filter out null/undefined items
    const validData = (data || []).filter((o: any) => o && o.orderId);

    return (
        <div className="orders-container">
            <div className="orders-stats-mini">
                <div className="mini-stat">
                    <span className="mini-label">Tổng đơn</span>
                    <strong>{validData.length}</strong>
                </div>
                <div className="mini-stat">
                    <span className="mini-label">Chờ thanh toán</span>
                    <strong>{validData.filter((o: any) => o.status === 'WaitPayment').length}</strong>
                </div>
                <div className="mini-stat">
                    <span className="mini-label">Đã thanh toán</span>
                    <strong>{validData.filter((o: any) => o.status === 'Payed').length}</strong>
                </div>
            </div>

            <div className="table-wrapper-payment">
                <table className="payment-table">
                    <thead>
                        <tr>
                            <th>Mã đơn</th>
                            <th>Khách hàng</th>
                            <th>Số tiền</th>
                            <th>Trạng thái</th>
                            <th>Ngày tạo</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {validData.map((o: any) => {
                            const isPayed = o.status === 'Payed';
                            const orderId = o.orderId || o.OrderId || 0;

                            return (
                                <tr key={orderId} className={isPayed ? 'row-payed' : ''}>
                                    <td>
                                        <span className="order-id">#ORD-{orderId.toString().padStart(4, '0')}</span>
                                    </td>
                                    <td>
                                        <div className="customer-cell">
                                            <div className="customer-avatar">{(o.userName || o.user?.username || 'U').charAt(0).toUpperCase()}</div>
                                            <div className="customer-info">
                                                <strong>{o.userName || o.user?.username || 'N/A'}</strong>
                                                <span>{o.userEmail || o.user?.email || ''}</span>
                                            </div>
                                        </div>
                                    </td>
                                    <td><span className="price-cell">{money(o.totalPrice || 0)} đ</span></td>
                                    <td>
                                        <select
                                            className={`status-dropdown status-${(o.status || 'pending').toLowerCase()}`}
                                            value={o.status || 'Pending'}
                                            disabled={isPayed}
                                            onChange={(e) => onUpdateStatus(orderId, e.target.value)}
                                        >
                                            <option value="Pending">🔵 Đang xử lý</option>
                                            <option value="WaitPayment">🟡 Chờ thanh toán</option>
                                            <option value="Payed">✅ Đã thanh toán</option>
                                            <option value="Cancelled">❌ Đã hủy</option>
                                        </select>
                                    </td>
                                    <td><span className="date-text">{o.orderDate ? new Date(o.orderDate).toLocaleDateString('vi-VN') : 'N/A'}</span></td>
                                    <td>
                                        <div className="action-cell">
                                            {o.status === 'WaitPayment' && (
                                                <button className="btn-confirm" onClick={() => onConfirm(orderId)}>
                                                    <CheckCircleOutlined /> Duyệt
                                                </button>
                                            )}
                                            <button className="btn-view-only" title="Xem chi tiết">
                                                <EyeOutlined />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>

            <style jsx>{`
                .orders-container { display: flex; flex-direction: column; gap: 24px; }
                .orders-stats-mini { display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px; }
                .mini-stat { background: white; padding: 16px 20px; border-radius: 12px; border: 1px solid #e2e8f0; text-align: center; }
                .mini-label { display: block; font-size: 12px; color: #64748b; margin-bottom: 6px; font-weight: 600; }
                .mini-stat strong { font-size: 24px; color: #1ea1f2; font-weight: 800; }

                .table-wrapper-payment { background: white; border-radius: 16px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 1px 3px rgba(0,0,0,0.05); }
                .payment-table { width: 100%; border-collapse: collapse; }
                .payment-table thead { background: linear-gradient(to right, #f8fafc, #f1f5f9); }
                .payment-table th { padding: 16px 20px; font-size: 11px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid #e2e8f0; }
                .payment-table td { padding: 14px 20px; font-size: 14px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
                .payment-table tbody tr { transition: 0.15s; }
                .payment-table tbody tr:hover { background: #f8fafc; }
                .row-payed { background: #f0fdf4; }
                
                .order-id { background: #eef2ff; color: #6366f1; padding: 5px 12px; border-radius: 8px; font-weight: 700; font-size: 13px; }
                .customer-cell { display: flex; align-items: center; gap: 12px; }
                .customer-avatar { width: 36px; height: 36px; border-radius: 10px; background: linear-gradient(135deg, #1ea1f2, #0ea5e9); color: white; display: grid; place-items: center; font-weight: 800; font-size: 14px; }
                .customer-info { display: flex; flex-direction: column; gap: 2px; }
                .customer-info strong { font-size: 14px; color: #0f172a; }
                .customer-info span { font-size: 12px; color: #94a3b8; }
                .price-cell { font-size: 16px; font-weight: 700; color: #1ea1f2; }
                .date-text { color: #64748b; font-size: 13px; }
                
                .status-dropdown { padding: 7px 14px; border-radius: 20px; border: 1.5px solid; font-size: 12px; font-weight: 700; background: white; transition: 0.2s; outline: none; cursor: pointer; }
                .status-dropdown:disabled { cursor: not-allowed; opacity: 0.7; }
                .status-pending { background: #eff6ff; color: #3b82f6; border-color: #93c5fd; }
                .status-waitpayment { background: #fffbeb; color: #f59e0b; border-color: #fcd34d; }
                .status-payed { background: #ecfdf5; color: #10b981; border-color: #86efac; }
                .status-cancelled { background: #fef2f2; color: #ef4444; border-color: #fca5a5; }
                
                .action-cell { display: flex; gap: 8px; align-items: center; }
                .btn-confirm { background: #10b981; color: white; border: none; padding: 6px 12px; border-radius: 8px; font-size: 12px; font-weight: 700; cursor: pointer; display: flex; align-items: center; gap: 4px; transition: 0.2s; white-space: nowrap; }
                .btn-confirm:hover { background: #059669; transform: translateY(-1px); }
                .btn-view-only { width: 32px; height: 32px; background: #f1f5f9; border: 1px solid #e2e8f0; color: #64748b; border-radius: 8px; display: grid; place-items: center; cursor: pointer; border: none; }
                .btn-view-only:hover { background: #e2e8f0; color: #0f172a; }
            `}</style>
        </div>
    );
};

const CloseOutlined = () => (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
        <line x1="18" y1="6" x2="6" y2="18"></line>
        <line x1="6" y1="6" x2="18" y2="18"></line>
    </svg>
);

export default AdminPage;
