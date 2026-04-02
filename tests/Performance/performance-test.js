import http from 'k6/http';
import { check, fail, sleep, group } from 'k6';
import { Trend, Rate, Counter } from 'k6/metrics';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";

const CONFIG = {
  baseUrl: (__ENV.BASE_URL || 'https://localhost:5001').replace(/\/$/, ''),
  apiPrefix: (__ENV.API_PREFIX || '/api').replace(/\/$/, ''),

  adminEmail: 'systemadmin@gmail.com',
  adminPassword: 'SystemAdmin@123$',

  patientEmail: 'patient@gmail.com',
  patientPassword: 'Patient@123$',

  dashboardVus: Number(20),
  publicSearchVus: Number(30),
  authVus: Number(15),

  warmupDuration: '20s',
  steadyDuration: '2m',
  rampDownDuration: '20s',

  thinkTimeMin: Number(0.5),
  thinkTimeMax: Number(1.5),

  enableSlotRace: ('true').toLowerCase() === 'true',
  slotId: '7fb9fee3-89bd-42e1-96d5-54b03e4c8111',
  slotRaceVus: Number(20),
  reservationMinutes: Number(5),
};

const API_BASE = `${CONFIG.baseUrl}${CONFIG.apiPrefix}`;

const authLoginTime = new Trend('auth_login_time', true);
const dashboardMetricsTime = new Trend('dashboard_metrics_time', true);
const dashboardTrendsTime = new Trend('dashboard_trends_time', true);
const dashboardRecentScreeningsTime = new Trend('dashboard_recent_screenings_time', true);
const publicSearchDoctorsTime = new Trend('public_search_ophthalmologists_time', true);
const publicSearchOrgsTime = new Trend('public_search_organisations_time', true);
const slotReserveTime = new Trend('slot_reserve_time', true);
const slotReleaseTime = new Trend('slot_release_time', true);

const businessErrorRate = new Rate('business_error_rate');
const http5xxRate = new Rate('http_5xx_rate');
const slotConflictRate = new Rate('slot_conflict_rate');

const totalRequests = new Counter('total_requests');
const authFailureCount = new Counter('auth_failure_count');
const unexpectedResponseCount = new Counter('unexpected_response_count');

const scenarios = {
  auth_login: {
    executor: 'ramping-vus',
    exec: 'authLoginScenario',
    startVUs: 0,
    stages: [
      { duration: CONFIG.warmupDuration, target: CONFIG.authVus },
      { duration: CONFIG.steadyDuration, target: CONFIG.authVus },
      { duration: CONFIG.rampDownDuration, target: 0 },
    ],
    gracefulRampDown: '10s',
    tags: { suite: 'auth' },
  },
  dashboard_read: {
    executor: 'ramping-vus',
    exec: 'dashboardScenario',
    startTime: '5s',
    startVUs: 0,
    stages: [
      { duration: CONFIG.warmupDuration, target: CONFIG.dashboardVus },
      { duration: CONFIG.steadyDuration, target: CONFIG.dashboardVus },
      { duration: CONFIG.rampDownDuration, target: 0 },
    ],
    gracefulRampDown: '10s',
    tags: { suite: 'dashboard' },
  },
  public_search: {
    executor: 'ramping-vus',
    exec: 'publicSearchScenario',
    startTime: '10s',
    startVUs: 0,
    stages: [
      { duration: CONFIG.warmupDuration, target: CONFIG.publicSearchVus },
      { duration: CONFIG.steadyDuration, target: CONFIG.publicSearchVus },
      { duration: CONFIG.rampDownDuration, target: 0 },
    ],
    gracefulRampDown: '10s',
    tags: { suite: 'public_search' },
  },
};

if (CONFIG.enableSlotRace) {
  scenarios.slot_reservation_race = {
    executor: 'ramping-vus',
    exec: 'slotReservationRaceScenario',
    startTime: '15s',
    startVUs: 0,
    stages: [
      { duration: CONFIG.warmupDuration, target: CONFIG.slotRaceVus },
      { duration: CONFIG.steadyDuration, target: CONFIG.slotRaceVus },
      { duration: CONFIG.rampDownDuration, target: 0 },
    ],
    gracefulRampDown: '10s',
    tags: { suite: 'slot_race' },
  };
}

export const options = {
  scenarios,

  discardResponseBodies: false,
  noConnectionReuse: false,
  summaryTrendStats: ['avg', 'med', 'p(90)', 'p(95)', 'p(99)', 'min', 'max', 'count'],

  thresholds: {
    http_req_failed: ['rate<0.03'],
    http_5xx_rate: ['rate<0.01'],
    business_error_rate: ['rate<0.05'],

    auth_login_time: ['p(95)<600', 'p(99)<1000'],
    dashboard_metrics_time: ['p(95)<700'],
    dashboard_trends_time: ['p(95)<900'],
    dashboard_recent_screenings_time: ['p(95)<900'],
    public_search_ophthalmologists_time: ['p(95)<550'],
    public_search_organisations_time: ['p(95)<550'],

    'http_req_duration{endpoint:dashboard_metrics}': ['p(95)<700'],
    'http_req_duration{endpoint:dashboard_recent_screenings}': ['p(95)<900'],
    'http_req_duration{endpoint:public_search_ophthalmologists}': ['p(95)<550'],

    'checks{check:status is 200}': ['rate>0.95'],
    'checks{check:api payload indicates success}': ['rate>0.95'],
    'checks{check:access token returned}': ['rate>0.95'],
  },
};

function buildUrl(path) {
  if (path.startsWith('/')) {
    return `${API_BASE}${path}`;
  }

  return `${API_BASE}/${path}`;
}

function authHeaders(token) {
  return {
    Authorization: `Bearer ${token}`,
    'Content-Type': 'application/json',
  };
}

function randomThink() {
  const min = CONFIG.thinkTimeMin;
  const max = CONFIG.thinkTimeMax;
  sleep(min + Math.random() * Math.max(0, max - min));
}

function safeJson(response) {
  try {
    return response.json();
  } catch (_) {
    return null;
  }
}

function markTechnicalError(res) {
  const is5xx = res.status >= 500;
  http5xxRate.add(is5xx);
  if (is5xx) {
    unexpectedResponseCount.add(1);
  }
}

function markBusinessError(isError) {
  businessErrorRate.add(isError);
  if (isError) {
    unexpectedResponseCount.add(1);
  }
}

function evaluateApiResponse(res, body, expectSuccess = true) {
  const statusOk = check(res, {
    'status is 200': (r) => r.status === 200,
  });

  const payloadOk = check(body, {
    'api payload indicates success': (b) => b && b.success === true,
  });

  markTechnicalError(res);
  markBusinessError(expectSuccess && !(statusOk && payloadOk));

  return statusOk && payloadOk;
}

function login(email, password, endpointTag = 'auth_login') {
  const payload = JSON.stringify({ email, password });

  const res = http.post(buildUrl('/auth/login'), payload, {
    headers: { 'Content-Type': 'application/json' },
    tags: { endpoint: endpointTag, api: 'auth' },
  });

  totalRequests.add(1);
  authLoginTime.add(res.timings.duration);
  markTechnicalError(res);

  const body = safeJson(res);
  const statusOk = check(res, {
    'login status is 200': (r) => r.status === 200,
  });

  const token = body && body.data && body.data.accessToken ? body.data.accessToken : '';
  const hasToken = check(body, {
    'access token returned': (b) => Boolean(b && b.data && b.data.accessToken),
  });

  const no2fa = check(body, {
    'test user does not require 2fa': (b) => !(b && b.data && b.data.requiresTwoFactor === true),
  });

  const isBusinessError = !(statusOk && hasToken && no2fa);
  markBusinessError(isBusinessError);

  if (isBusinessError) {
    authFailureCount.add(1);
  }

  return { token, response: res, body, success: !isBusinessError };
}

export function setup() {
  console.log(`[setup] Target API base: ${API_BASE}`);
  console.log(`[setup] Auth user: ${CONFIG.adminEmail}`);

  const adminLogin = login(CONFIG.adminEmail, CONFIG.adminPassword, 'auth_login_setup');
  if (!adminLogin.success || !adminLogin.token) {
    fail('Setup failed: admin login failed. Check ADMIN_EMAIL / ADMIN_PASSWORD or 2FA settings.');
  }

  const setupData = {
    startTime: new Date().toISOString(),
    adminToken: adminLogin.token,
    patientToken: '',
  };

  if (CONFIG.enableSlotRace) {
    if (!CONFIG.slotId) {
      fail('Setup failed: ENABLE_SLOT_RACE=true requires SLOT_ID.');
    }

    if (!CONFIG.patientEmail || !CONFIG.patientPassword) {
      fail('Setup failed: ENABLE_SLOT_RACE=true requires PATIENT_EMAIL and PATIENT_PASSWORD.');
    }

    const patientLogin = login(CONFIG.patientEmail, CONFIG.patientPassword, 'auth_patient_setup');
    if (!patientLogin.success || !patientLogin.token) {
      fail('Setup failed: patient login failed for slot race scenario.');
    }

    setupData.patientToken = patientLogin.token;
  }

  return setupData;
}

export function authLoginScenario() {
  group('Auth - login throughput', function () {
    login(CONFIG.adminEmail, CONFIG.adminPassword, 'auth_login');
  });

  randomThink();
}

export function dashboardScenario(data) {
  let token = data.adminToken;

  group('SystemAdmin Dashboard - read heavy', function () {
    const reqTags = { api: 'system_admin' };

    let metricsRes = http.get(buildUrl('/system-admin/dashboard/metrics'), {
      headers: authHeaders(token),
      tags: { ...reqTags, endpoint: 'dashboard_metrics' },
    });
    totalRequests.add(1);
    dashboardMetricsTime.add(metricsRes.timings.duration);

    if (metricsRes.status === 401) {
      const relogin = login(CONFIG.adminEmail, CONFIG.adminPassword, 'auth_relogin');
      if (relogin.success && relogin.token) {
        token = relogin.token;
        metricsRes = http.get(buildUrl('/system-admin/dashboard/metrics'), {
          headers: authHeaders(token),
          tags: { ...reqTags, endpoint: 'dashboard_metrics' },
        });
        totalRequests.add(1);
        dashboardMetricsTime.add(metricsRes.timings.duration);
      }
    }

    const metricsBody = safeJson(metricsRes);
    evaluateApiResponse(metricsRes, metricsBody, true);

    const trendsRes = http.get(buildUrl('/system-admin/dashboard/screening-trends?timeRange=monthly&periods=12'), {
      headers: authHeaders(token),
      tags: { ...reqTags, endpoint: 'dashboard_screening_trends' },
    });
    totalRequests.add(1);
    dashboardTrendsTime.add(trendsRes.timings.duration);
    const trendsBody = safeJson(trendsRes);
    evaluateApiResponse(trendsRes, trendsBody, true);

    const recentRes = http.get(buildUrl('/system-admin/dashboard/recent-screenings?pageNumber=1&pageSize=20'), {
      headers: authHeaders(token),
      tags: { ...reqTags, endpoint: 'dashboard_recent_screenings' },
    });
    totalRequests.add(1);
    dashboardRecentScreeningsTime.add(recentRes.timings.duration);
    const recentBody = safeJson(recentRes);
    evaluateApiResponse(recentRes, recentBody, true);
  });

  randomThink();
}

export function publicSearchScenario() {
  group('Public Search - anonymous browsing', function () {
    const doctorsRes = http.get(buildUrl('/patient/search/ophthalmologists?pageNumber=1&pageSize=10'), {
      headers: { 'Content-Type': 'application/json' },
      tags: { endpoint: 'public_search_ophthalmologists', api: 'patient_search' },
    });
    totalRequests.add(1);
    publicSearchDoctorsTime.add(doctorsRes.timings.duration);
    const doctorsBody = safeJson(doctorsRes);
    evaluateApiResponse(doctorsRes, doctorsBody, true);

    const orgsRes = http.get(buildUrl('/patient/search/organisations?pageNumber=1&pageSize=10'), {
      headers: { 'Content-Type': 'application/json' },
      tags: { endpoint: 'public_search_organisations', api: 'patient_search' },
    });
    totalRequests.add(1);
    publicSearchOrgsTime.add(orgsRes.timings.duration);
    const orgsBody = safeJson(orgsRes);
    evaluateApiResponse(orgsRes, orgsBody, true);
  });

  randomThink();
}

export function slotReservationRaceScenario(data) {
  if (!CONFIG.slotId || !data.patientToken) {
    fail('Slot race scenario requires SLOT_ID and a valid patient token from setup.');
  }

  group('Appointment Slot - reservation race condition', function () {
    const reserveRes = http.post(
      buildUrl('/appointment-slots/reserve'),
      JSON.stringify({
        slotId: CONFIG.slotId,
        reservationMinutes: CONFIG.reservationMinutes,
      }),
      {
        headers: authHeaders(data.patientToken),
        tags: { endpoint: 'slot_reserve', api: 'appointment_slots' },
      },
    );

    totalRequests.add(1);
    slotReserveTime.add(reserveRes.timings.duration);
    markTechnicalError(reserveRes);

    const reserveBody = safeJson(reserveRes);
    const statusExpected = check(reserveRes, {
      'slot reserve returns 200 or 409': (r) => r.status === 200 || r.status === 409,
    });

    const isConflict = reserveRes.status === 409;
    slotConflictRate.add(isConflict);

    if (isConflict) {
      markBusinessError(false);
      return;
    }

    const reserveOk = statusExpected && check(reserveBody, {
      'reserve payload indicates success': (b) => b && b.success === true,
    });

    markBusinessError(!reserveOk);
    if (!reserveOk) {
      return;
    }

    const releaseRes = http.post(
      buildUrl('/appointment-slots/release'),
      JSON.stringify({ slotId: CONFIG.slotId }),
      {
        headers: authHeaders(data.patientToken),
        tags: { endpoint: 'slot_release', api: 'appointment_slots' },
      },
    );

    totalRequests.add(1);
    slotReleaseTime.add(releaseRes.timings.duration);

    const releaseBody = safeJson(releaseRes);
    evaluateApiResponse(releaseRes, releaseBody, true);
  });

  randomThink();
}

export function handleSummary(data) {
  return {
    'report-performance.html': htmlReport(data),
    'report-performance.json': JSON.stringify(data, null, 2),
    stdout: textSummary(data, { indent: " ", enableColors: true }),
  };
}