import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '2m', target: 50 }, // Ramp up to 50 users
        { duration: '3h56m', target: 50 }, // Stay at 50 users for ~4 hours
        { duration: '2m', target: 0 }, // Ramp down to 0 users
    ],
};

const BASE_URL = 'http://localhost:5007';

export default function () {
    const res = http.get(`${BASE_URL}/materials/v1/materials`);

    check(res, {
        'status is 200': (r) => r.status === 200,
    });

    sleep(1);
}
