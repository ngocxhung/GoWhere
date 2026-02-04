'use client';

import React, { useState, useEffect } from 'react';
import Header from '@/components/Header';
import Footer from '@/components/Footer';
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
    SyncOutlined
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

    useEffect(() => {
        const role = localStorage.getItem('role');
        if (role !== '2') {
            window.location.href = '/';
        }
        fetchStats();
    }, []);

    useEffect(() => {
        if (activeTab !== 'dashboard') {
            fetchData();
        }
    }, [activeTab]);

    const fetchStats = async () => {
        try {
            const token = localStorage.getItem('token');
            const res = await fetch('http://localhost:5191/api/Admin/DashboardStats', {
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

    const fetchData = async () => {
        setLoading(true);
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

            const res = await fetch(`http://localhost:5191/api/Admin/${endpoint}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                const json = await res.json();
                setData(json);
            }
        } catch (e) {
            console.error('Fetch data failed');
        } finally {
            setLoading(false);
        }
    };

    const confirmOrder = async (id: number) => {
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`http://localhost:5191/api/Admin/Orders/${id}/Confirm`, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                alert('Đã xác nhận thanh toán!');
                fetchData();
            }
        } catch (e) {
            alert('Lỗi khi xác nhận');
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
            }
            const res = await fetch(`http://localhost:5191/api/Admin/${endpoint}/${id}`, {
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
        { id: 'cars', label: 'Cho thuê xe', icon: <CarOutlined /> },
        { id: 'payments', label: 'Thanh toán', icon: <CreditCardOutlined /> },
    ];

    return (
        <div className="admin-layout">
            <Header onAuthClick={() => { }} />

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
                            {activeTab !== 'dashboard' && activeTab !== 'payments' && (
                                <button className="btn primary btn-add">
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
                                {activeTab === 'spots' && <ScenicSpotManagement data={data} onDelete={deleteItem} />}
                                {activeTab === 'hotels' && <HotelManagement data={data} />}
                                {activeTab === 'cars' && <CarManagement data={data} />}
                                {activeTab === 'payments' && <PaymentManagement data={data} onConfirm={confirmOrder} />}
                            </>
                        )}
                    </div>
                </section>
            </main>

            <style jsx>{`
                .admin-layout { min-height: 100vh; background: #f8fafc; }
                .admin-main { display: flex; max-width: 1600px; margin: 0 auto; gap: 0; min-height: calc(100vh - 80px); }
                .admin-loading { text-align: center; padding: 100px; color: #6366f1; font-weight: 700; font-size: 18px; display: flex; align-items: center; justify-content: center; gap: 12px; }
                
                .admin-sidebar { width: 280px; background: white; border-right: 1px solid #e2e8f0; padding: 32px 16px; display: flex; flex-direction: column; position: sticky; top: 80px; height: calc(100vh - 80px); }
                .sidebar-header { padding: 0 16px 24px; border-bottom: 1px solid #f1f5f9; margin-bottom: 24px; }
                .sidebar-header h3 { font-size: 18px; font-weight: 800; color: #0f172a; }
                
                .sidebar-nav { display: flex; flex-direction: column; gap: 8px; }
                .sidebar-item { display: flex; align-items: center; gap: 12px; padding: 14px 16px; border: none; background: none; border-radius: 12px; color: #64748b; font-weight: 600; cursor: pointer; transition: 0.2s; text-align: left; font-size: 15px; }
                .sidebar-item:hover { background: #f1f5f9; color: #0f172a; }
                .sidebar-item.active { background: #eef2ff; color: #6366f1; }
                
                .admin-content { flex: 1; padding: 40px; }
                .content-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 32px; }
                .content-header h2 { font-size: 28px; font-weight: 800; color: #0f172a; }
                
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
            `}</style>
        </div>
    );
};

const DashboardView = ({ stats }: any) => (
    <div className="dashboard-grid">
        {[
            { label: 'Người dùng', value: stats.usersCount, icon: <UserOutlined />, color: '#6366f1' },
            { label: 'Địa điểm', value: stats.spotsCount, icon: <EnvironmentOutlined />, color: '#10b981' },
            { label: 'Doanh thu (VND)', value: (stats.revenue || 0).toLocaleString(), icon: <CreditCardOutlined />, color: '#f59e0b' },
            { label: 'Đơn hàng', value: stats.ordersCount, icon: <SyncOutlined />, color: '#ec4899' },
        ].map((stat, i) => (
            <div key={i} className="stat-card">
                <div className="stat-icon" style={{ background: `${stat.color}15`, color: stat.color }}>{stat.icon}</div>
                <div className="stat-info">
                    <h4>{stat.label}</h4>
                    <strong>{stat.value}</strong>
                </div>
            </div>
        ))}
    </div>
);

const UserManagement = ({ data, onDelete }: any) => (
    <div className="admin-table-container">
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Người dùng</th>
                    <th>Email</th>
                    <th>Số điện thoại</th>
                    <th>Quyền</th>
                    <th>Thao tác</th>
                </tr>
            </thead>
            <tbody>
                {data.map((u: any) => (
                    <tr key={u.userId}>
                        <td>{u.userId}</td>
                        <td>
                            <div className="user-cell">
                                <div className="avatar-sm">{u.username.charAt(0).toUpperCase()}</div>
                                <span>{u.fullName || u.username}</span>
                            </div>
                        </td>
                        <td>{u.email}</td>
                        <td>{u.phoneNumber}</td>
                        <td><span className={`badge ${u.roleId === 2 ? 'admin' : ''}`}>{u.roleId === 2 ? 'Admin' : 'User'}</span></td>
                        <td>
                            <div className="actions">
                                <button className="btn-icon"><EditOutlined /></button>
                                <button className="btn-icon delete" onClick={() => onDelete(u.userId)} disabled={u.userId === 2}><DeleteOutlined /></button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
        <style jsx>{`
            .badge.admin { background: #eef2ff; color: #6366f1; }
        `}</style>
    </div>
);

const ScenicSpotManagement = ({ data, onDelete }: any) => (
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
                        <td>{s.estimatedVisitPrice === 0 ? 'Miễn phí' : `${s.estimatedVisitPrice.toLocaleString()} VND`}</td>
                        <td><span className={`badge ${s.isViewed ? 'success' : 'warning'}`}>{s.isViewed ? 'Đã duyệt' : 'Chưa duyệt'}</span></td>
                        <td>
                            <div className="actions">
                                <button className="btn-icon"><EditOutlined /></button>
                                <button className="btn-icon delete" onClick={() => onDelete(s.scenicSpotId)}><DeleteOutlined /></button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
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
                        <td>{h.pricePerNight.toLocaleString()} VND</td>
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

const PaymentManagement = ({ data, onConfirm }: any) => (
    <div className="admin-table-container">
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>User</th>
                    <th>Trạng thái</th>
                    <th>Số tiền</th>
                    <th>Ngày</th>
                    <th>Thao tác</th>
                </tr>
            </thead>
            <tbody>
                {data.map((o: any) => (
                    <tr key={o.orderId}>
                        <td>#ORD-{o.orderId.toString().padStart(4, '0')}</td>
                        <td><strong>{o.user?.fullName || o.user?.username}</strong></td>
                        <td>
                            <span className={`badge ${o.status === 'Paid' || o.status === 'Payed' ? 'success' : 'warning'}`}>
                                {o.status === 'Paid' || o.status === 'Payed' ? 'Đã thanh toán' : 'Chờ duyệt'}
                            </span>
                        </td>
                        <td>{o.totalPrice.toLocaleString()} VND</td>
                        <td>{new Date(o.orderDate).toLocaleDateString()}</td>
                        <td>
                            <div className="actions">
                                {(o.status !== 'Paid' && o.status !== 'Payed') && (
                                    <button className="btn-icon success" onClick={() => onConfirm(o.orderId)}><CheckCircleOutlined /></button>
                                )}
                                <button className="btn-icon delete"><DeleteOutlined /></button>
                            </div>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    </div>
);

const CloseOutlined = () => (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
        <line x1="18" y1="6" x2="6" y2="18"></line>
        <line x1="6" y1="6" x2="18" y2="18"></line>
    </svg>
);

export default AdminPage;
