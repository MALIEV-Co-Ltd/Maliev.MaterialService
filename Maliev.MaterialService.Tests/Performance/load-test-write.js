import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 10 }, // Ramp up to 10 users
        { duration: '1m', target: 10 },  // Stay at 10 users
        { duration: '30s', target: 0 },  // Ramp down to 0 users
    ],
};

const BASE_URL = 'http://localhost:5007';
const TOKEN = 'YOUR_JWT_TOKEN_HERE'; // Replace with a valid token

export default function () {
    const payload = JSON.stringify({
        name: `Load Test Material ${__VU}-${__ITER}`,
        code: `LT-${__VU}-${__ITER}-${Date.now()}`,
        description: "Created by load test",
        pricePerUnit: 100,
        stockLevel: 50
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${TOKEN}`,
        },
    };

    const res = http.post(`${BASE_URL}/materials/v1/materials`, payload, params);

    check(res, {
        'status is 201': (r) => r.status === 201,
    });

    sleep(1);
}
