export interface Activity {
    t: string;
    name: string;
    price: number;
    description?: string;
    id?: number;
}

export interface Video {
    id: number;
    dst: string;
    tag?: string;
    cost: number;
    img: string;
}

export interface Reel {
    src: string;
    cap: string;
}

export interface Tour {
    id: number;
    title: string;
    img: string;
    price: string;
    link: string;
    tag?: string;
}

export interface Hotel {
    id: number;
    name: string;
    location: string;
    price: number;
    rating: number;
    image: string;
    tags: string[];
}

export interface Flight {
    id: number;
    airline: string;
    from: string;
    to: string;
    departure: string;
    arrival: string;
    price: number;
    class: string;
}

export interface Car {
    id: number;
    model: string;
    type: string;
    seats: number;
    pricePerDay: number;
    rating: number;
    image: string;
}

export interface SpecialOffer {
    id: number;
    title: string;
    description: string;
    discount: string;
    image: string;
    code: string;
}

export const demoVideos: Video[] = [
    { id: 1, dst: "Đà Nẵng", cost: 6200000, img: "https://images.unsplash.com/photo-1559592481-74488ea01937?auto=format&fit=crop&w=800&q=80" },
    { id: 2, dst: "Phú Quốc", cost: 7100000, img: "https://images.unsplash.com/photo-1589394815804-001090173bc3?auto=format&fit=crop&w=800&q=80" },
    { id: 3, dst: "Hà Giang", cost: 8400000, img: "https://images.unsplash.com/photo-1623940177727-463e69f8016f?auto=format&fit=crop&w=800&q=80" },
    { id: 4, dst: "Sapa", cost: 5800000, img: "https://images.unsplash.com/photo-1504457047772-27fb10f41d11?auto=format&fit=crop&w=800&q=80" },
    { id: 5, dst: "Nha Trang", cost: 6500000, img: "https://images.unsplash.com/photo-1563297059-4ad47c0e214c?auto=format&fit=crop&w=800&q=80" },
    { id: 6, dst: "Hội An", cost: 5200000, img: "https://images.unsplash.com/photo-1555931035-7c26356614dc?auto=format&fit=crop&w=800&q=80" },
];

export const danangReels: Reel[] = [
    { src: "/media/tt1.mp4", cap: "Khám phá vẻ đẹp bất tận của Đà Nẵng" },
    { src: "/media/tt2.mp4", cap: "Trải nghiệm cáp treo Bà Nà Hills cực đỉnh" },
    { src: "/media/tt3.mp4", cap: "Hội An lung linh sắc màu về đêm" },
    { src: "/media/tt4.mp4", cap: "Thưởng thức hải sản tươi ngon bên bờ biển" },
    { src: "/media/tt5.mp4", cap: "Check-in Cầu Vàng – biểu tượng du lịch mới" },
    { src: "/media/tt6.mp4", cap: "Bình minh trên biển Mỹ Khê – top bãi biển đẹp nhất hành tinh" },
    { src: "/media/tt7.mp4", cap: "Lặn ngắm san hô tại Cù Lao Chàm trong vắt" },
    { src: "/media/tt8.mp4", cap: "Chinh phục Ngũ Hành Sơn hùng vĩ" },
    { src: "/media/tt9.mp4", cap: "Show diễn Ký ức Hội An đầy cảm xúc" },
    { src: "/media/tt10.mp4", cap: "Cầu Rồng phun lửa – nét độc đáo của thành phố đáng sống" },
    { src: "/media/tt11.mp4", cap: "Dạo chơi phố cổ Hội An yên bình" },
    { src: "/media/tt12.mp4", cap: "Thác nước tuyệt đẹp tại khu du lịch Hòa Phú Thành" },
    { src: "/media/tt13.mp4", cap: "Đường đèo Hải Vân – cung đường ven biển đẹp bậc nhất" },
    { src: "/media/tt14.mp4", cap: "Khám phá bán đảo Sơn Trà và Linh Ứng Tự" },
    { src: "/media/tt15.mp4", cap: "Trải nghiệm cuộc sống người dân địa phương" },
    { src: "/media/tt16.mp4", cap: "Ẩm thực đường phố Đà Nẵng không thể bỏ lỡ" },
    { src: "/media/tt17.mp4", cap: "Những quán cafe view biển cực chill" },
    { src: "/media/tt18.mp4", cap: "Vui chơi tại Công viên Châu Á Asia Park" },
    { src: "/media/tt19.mp4", cap: "Suối khoáng nóng Núi Thần Tài thư giãn" },
    { src: "/media/tt20.mp4", cap: "Cầu quay Sông Hàn – niềm tự hào của người dân Đà Nẵng" },
    { src: "/media/tt21.mp4", cap: "Hoàng hôn buông xuống trên vịnh Đà Nẵng" },
    { src: "/media/tt22.mp4", cap: "Khám phá hang động tại Ngũ Hành Sơn" },
    { src: "/media/tt23.mp4", cap: "Lễ hội pháo hoa quốc tế Đà Nẵng rực rỡ" },
    { src: "/media/tt24.mp4", cap: "Trải nghiệm chèo SUP trên biển Mỹ Khê" },
    { src: "/media/tt25.mp4", cap: "Vẻ đẹp hoang sơ của bãi Rạng" },
    { src: "/media/tt26.mp4", cap: "Thưởng thức Mì Quảng – hương vị quê hương" },
    { src: "/media/tt27.mp4", cap: "Dạo bước trên bãi cát trắng mịn màng" },
    { src: "/media/tt28.mp4", cap: "Không gian xanh mát tại công viên APEC" },
    { src: "/media/tt29.mp4", cap: "Check-in bảo tàng tranh 3D độc lạ" },
    { src: "/media/tt30.mp4", cap: "Kỷ niệm khó quên tại GoWhere - Trợ lý du lịch AI" },
    { src: "/media/short/14.mp4", cap: "Vẻ đẹp ẩn giấu của miền Trung" },
    { src: "/media/short/16.mp4", cap: "Trải nghiệm văn hóa địa phương độc đáo" },
    { src: "/media/short/19.mp4", cap: "Những góc sống ảo cực chất" },
    { src: "/media/short/22.mp4", cap: "Hành trình khám phá không giới hạn" },
    { src: "/media/short/27.mp4", cap: "Vẻ đẹp thiên nhiên hùng vĩ" },
    { src: "/media/short/28.mp4", cap: "Nét đẹp lao động của người dân biển" },
    { src: "/media/short/36.mp4", cap: "Kỳ nghỉ trong mơ tại Đà Nẵng" },
];

export const bannerSlides = [
    {
        title: "Kỳ quan thiên nhiên",
        sub: "Những điểm đến không thể bỏ qua tại dải đất hình chữ S.",
        video: "/media/vd2.mp4",
        btnText: "Khám phá ngay"
    },
    {
        title: "Trải nghiệm văn hóa",
        sub: "Hòa mình vào nhịp sống và nét đẹp truyền thống địa phương.",
        video: "/media/vd3.mp4",
        btnText: "Xem lịch trình"
    },
    {
        title: "Thiên đường nghỉ dưỡng",
        sub: "Tận hưởng không gian thư giãn tuyệt vời bên bờ biển xanh.",
        video: "/media/vd4.mp4",
        btnText: "Đặt chỗ ngay"
    },
    {
        title: "Hành trình bất tận",
        sub: "Cùng trợ lý AI lên kế hoạch cho chuyến đi mơ ước của bạn.",
        video: "/media/vd5.mp4",
        btnText: "Bắt đầu hành trình"
    },
];

export const demoTours: Tour[] = [
    {
        id: 1,
        title: "Tour Đà Nẵng – Hội An 3N2Đ",
        img: "https://cdn-media.sforum.vn/storage/app/media/ctvseo_MH/%E1%BA%A3nh%20%C4%91%E1%BA%B9p%20%C4%91%C3%A0%20n%E1%BA%B5ng/anh-dep-da-nang-thumb.jpg",
        price: "Giảm 42% – Chỉ còn 2.890.000đ",
        link: "https://www.vntrip.vn/",
    },
    {
        id: 2,
        title: "Tour Hà Giang 4N3Đ",
        img: "https://media-cdn-v2.laodong.vn/storage/newsportal/2023/8/26/1233704/2-69.jpg",
        price: "Giảm 35% – Chỉ còn 3.450.000đ",
        link: "https://www.klook.com/vi/",
    },
    {
        id: 3,
        title: "Tour Phú Quốc 3N2Đ",
        img: "https://homepage.momocdn.net/blogscontents/momo-upload-api-220915135020-637988466208505792.png",
        price: "Flash Sale – Chỉ 2.790.000đ",
        link: "https://www.vntrip.vn/",
    }
];

export const hotels: Hotel[] = [
    {
        id: 1,
        name: "InterContinental Danang Sun Peninsula Resort",
        location: "Bán đảo Sơn Trà, Đà Nẵng",
        price: 12500000,
        rating: 5,
        image: "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80",
        tags: ["Bể bơi vô cực", "Spa", "Nhà hàng 5 sao"]
    },
    {
        id: 2,
        name: "JW Marriott Phu Quoc Emerald Bay Resort",
        location: "Khem Beach, Phú Quốc",
        price: 8900000,
        rating: 5,
        image: "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=800&q=80",
        tags: ["Kiến trúc độc bản", "Biển riêng", "Kids Club"]
    },
    {
        id: 3,
        name: "Sofitel Legend Metropole Hanoi",
        location: "Hoàn Kiếm, Hà Nội",
        price: 6500000,
        rating: 5,
        image: "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=800&q=80",
        tags: ["Lịch sử", "Trung tâm", "Pháp cổ"]
    }
];

export const flights: Flight[] = [
    {
        id: 1,
        airline: "Vietnam Airlines",
        from: "HAN",
        to: "DAD",
        departure: "08:00",
        arrival: "09:20",
        price: 1200000,
        class: "Economy"
    },
    {
        id: 2,
        airline: "Bamboo Airways",
        from: "SGN",
        to: "PQC",
        departure: "10:30",
        arrival: "11:30",
        price: 950000,
        class: "Economy"
    },
    {
        id: 3,
        airline: "Vietjet Air",
        from: "HAN",
        to: "SGN",
        departure: "14:00",
        arrival: "16:15",
        price: 780000,
        class: "Economy"
    }
];

export const cars: Car[] = [
    {
        id: 1,
        model: "Toyota Fortuner 2023",
        type: "SUV",
        seats: 7,
        pricePerDay: 1500000,
        rating: 4.8,
        image: "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=800&q=80"
    },
    {
        id: 2,
        model: "VinFast Lux A2.0",
        type: "Sedan",
        seats: 5,
        pricePerDay: 1200000,
        rating: 4.9,
        image: "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=800&q=80"
    },
    {
        id: 3,
        model: "Ford Transit",
        type: "Van",
        seats: 16,
        pricePerDay: 2500000,
        rating: 4.7,
        image: "https://images.unsplash.com/photo-1532581291347-9c39cf10a73c?auto=format&fit=crop&w=800&q=80"
    }
];

export const activities: Tour[] = [
    {
        id: 101,
        title: "Dạo thuyền trên sông Hoài",
        img: "https://images.unsplash.com/photo-1605658639202-0e8666049e29?auto=format&fit=crop&w=800&q=80",
        price: "150.000đ",
        link: "#",
        tag: "Văn hóa"
    },
    {
        id: 102,
        title: "Lặn ngắm san hô Cù Lao Chàm",
        img: "https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=800&q=80",
        price: "650.000đ",
        link: "#",
        tag: "Mạo hiểm"
    },
    {
        id: 103,
        title: "Show múa rối nước Thăng Long",
        img: "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?auto=format&fit=crop&w=800&q=80",
        price: "200.000đ",
        link: "#",
        tag: "Nghệ thuật"
    }
];

export const specialOffers: SpecialOffer[] = [
    {
        id: 1,
        title: "Giảm 30% khi đặt sớm",
        description: "Áp dụng cho các booking khách sạn trước 30 ngày khởi hành.",
        discount: "30%",
        image: "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=800&q=80",
        code: "EARLYBIRD"
    },
    {
        id: 2,
        title: "Tặng voucher 500k",
        description: "Khi thanh toán qua ví điện tử liên kết.",
        discount: "500.000đ",
        image: "https://images.unsplash.com/photo-1559136555-9303baea8ebd?auto=format&fit=crop&w=800&q=80",
        code: "GWCHAT"
    }
];

export const itineraryTemplates: Record<number, Activity[]> = {
    1: [
        { t: "08:00", name: "Bữa sáng địa phương", price: 150000 },
        { t: "10:00", name: `Di chuyển & check-in`, price: 250000 },
        { t: "12:30", name: "Bữa trưa đặc sản", price: 220000 },
        { t: "15:00", name: "Tham quan điểm nổi bật", price: 0 },
        { t: "18:30", name: "Ăn tối & dạo phố", price: 300000 }
    ],
    2: [
        { t: "07:30", name: "Buffet sáng", price: 180000 },
        { t: "09:00", name: `Tham quan điểm đến`, price: 400000 },
        { t: "12:00", name: "Thưởng thức ẩm thực", price: 200000 },
        { t: "14:00", name: "Hoạt động giải trí", price: 350000 },
        { t: "19:00", name: "Tiệc tối đặc sản", price: 300000 }
    ],
    3: [
        { t: "08:00", name: "Sáng & Cafe", price: 150000 },
        { t: "10:00", name: "Khám phá trung tâm", price: 250000 },
        { t: "13:00", name: "Bữa trưa", price: 200000 },
        { t: "15:30", name: "Dạo phố đi bộ", price: 0 },
        { t: "19:00", name: "Trải nghiệm tối", price: 450000 }
    ],
};

export const endTemplate: Activity[] = [
    { t: "07:00", name: "Ăn sáng cuối", price: 150000 },
    { t: "09:00", name: "Check-out & Hành lý", price: 0 },
    { t: "11:30", name: "Bữa trưa nhẹ", price: 200000 },
    { t: "13:30", name: "Di chuyển ra ga/xe", price: 250000 },
    { t: "15:00", name: "Kết thúc hành trình", price: 0 }
];
