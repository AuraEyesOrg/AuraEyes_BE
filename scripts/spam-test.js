// Simple Spam Test (Node.js 18+ Built-in Fetch)
// Run with: node scripts/spam-test.js

const BASE_URL = 'http://localhost:5000/api'; // Or your target API URL

const ENDPOINTS = {
    PUBLIC_HEALTH: '/health',         // Expect 429 after 240 req
    LOGIN_BRUTE: '/auth/login',         // Expect 429 after 8 req
};

async function spamEndpoint(endpoint, iterations, label) {
    console.log(`\n🚀 Testing [${label}]: ${endpoint}`);
    let successCount = 0;
    let rateLimitedCount = 0;
    let authErrorCount = 0;

    const requests = Array.from({ length: iterations }).map(async (_, index) => {
        try {
            const res = await fetch(`${BASE_URL}${endpoint}`, {
                method: endpoint.includes('login') ? 'POST' : 'GET',
                headers: { 'Content-Type': 'application/json' },
                body: endpoint.includes('login') ? JSON.stringify({ email: `spam_${index}@test.com`, password: '123' }) : null
            });

            if (res.status === 200 || res.status === 201 || res.status === 204) {
               successCount++;
            } else if (res.status === 429) {
                rateLimitedCount++;
                console.log(`[#${index+1}] 🛑 Rate Limited (429)!`);
            } else if (res.status === 401) {
                authErrorCount++;
            }
        } catch (e) {
            console.error(`[#${index+1}] ❌ Connection Error: ${e.message}`);
        }
    });

    await Promise.all(requests);
    
    console.log(`\n=== [${label}] FINAL ===`);
    console.log(`✅ OK: ${successCount}`);
    console.log(`🛑 Rate Limited (429): ${rateLimitedCount}`);
    console.log(`❌ Auth Denied (401): ${authErrorCount}`);
    console.log(`=======================\n`);
}

async function start() {
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
    
    // Quick test for Sensitive (Auth) endpoint - 8 req/min threshold
    await spamEndpoint(ENDPOINTS.LOGIN_BRUTE, 20, "Sensitive Auth Policy (Brute Force)");

    // Test for Public (Read) endpoint - 240 req/min threshold
    // await spamEndpoint(ENDPOINTS.PUBLIC_HEALTH, 250, "Public Read Policy (Scraper)");
}

start();
