import http from 'k6/http';
import { check, sleep } from 'k6';

// Cấu hình các giai đoạn test
export const options = {
  scenarios: {
    // 1. Giả lập Brute Force Login (Endpoint nhạy cảm - Giới hạn 8 req/60s)
    brute_force_login: {
      executor: 'constant-arrival-rate',
      rate: 2, // 2 requests mỗi giây => 120 req/phút (Vượt xa ngưỡng 8/60s)
      timeUnit: '1s',
      duration: '30s',
      preAllocatedVUs: 10,
      maxVUs: 20,
      exec: 'testLogin',
    },
    // 2. Giả lập Public Scraper (Endpoint Read - Giới hạn 120-240 req/60s)
    public_scraper: {
      executor: 'per-vu-iterations',
      vus: 5,
      iterations: 50, // Mỗi user ảo quét 50 trang liên tục
      startTime: '5s',
      exec: 'testPublicRead',
    },
    // 3. Giả lập Spam Write (Endpoint Write - Giới hạn 60-80 req/60s)
    feedback_spam: {
      executor: 'shared-iterations',
      vus: 10,
      iterations: 100, // Tổng 100 feedback gửi trong thời gian ngắn
      startTime: '10s',
      exec: 'testFeedbackSpam',
    },
  },
  thresholds: {
    // Chúng ta kỳ vọng sẽ thấy rất nhiều lỗi 429 (Rate Limited)
    'http_req_failed{scenario:brute_force_login}': ['rate > 0.8'], // >80% login phải thất bại
  },
};

const BASE_URL = 'https://localhost:5001/api'; // Thay đổi local port nếu cần

export function testLogin() {
  const payload = JSON.stringify({
      email: `spam_user_${__ITER}@auraeyes.com`,
      password: 'wrong_password_123'
  });
  const params = { headers: { 'Content-Type': 'application/json' } };
  
  const res = http.post(`${BASE_URL}/auth/login`, payload, params);
  
  check(res, {
    'is rate limited (429)': (r) => r.status === 429,
    'login failed (401)': (r) => r.status === 401,
  });
}

export function testPublicRead() {
  const res = http.get(`${BASE_URL}/patient/search/ophthalmologists`);
  
  check(res, {
    'is status 200': (r) => r.status === 200,
    'is rate limited (429)': (r) => r.status === 429,
  });
}

export function testFeedbackSpam() {
  const payload = JSON.stringify({
      content: "Spam feedback content",
      rating: 5
  });
  const params = { headers: { 'Content-Type': 'application/json' } };
  
  const res = http.post(`${BASE_URL}/feedback/website`, payload, params);
  
  check(res, {
    'write success (200/201)': (r) => r.status === 200 || r.status === 201,
    'write rate limited (429)': (r) => r.status === 429,
  });
}
